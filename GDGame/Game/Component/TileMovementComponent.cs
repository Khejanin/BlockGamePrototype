using System;
using System.Linq;
using GDGame.Actors;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class TileMovementComponent : IController
    {
        //TODO: Refactor this class. Use a State pattern for the different movement types.

        private MovableTile parent;

        private int movementTime;
        private int currentMovementTime;
        private MovementType movementType;
        private Curve1D moveCurve1D;
        private Curve1D jumpCurve1D;
        private Vector3 diff;
        private Quaternion rotationQuaternion;
        private Vector3 startPos;
        private Vector3 endPos;
        private float jumpHeight;
        private Quaternion startRotation;
        private Action endMoveCallback;
        private Action<Raycaster.HitResult> onCollideCallback;

        public TileMovementComponent(int movementTime, Curve1D moveCurve1D, MovementType movementType = MovementType.Slide)
        {
            this.movementTime = movementTime;
            this.moveCurve1D = moveCurve1D;
            this.movementType = movementType;
            this.moveCurve1D.Add(1, 0);
            this.moveCurve1D.Add(0, movementTime);

            startRotation = rotationQuaternion = Quaternion.Identity;

            if (movementType == MovementType.Jump)
            {
                jumpHeight = .5f;
                jumpCurve1D = new Curve1D(CurveLoopType.Cycle);
                jumpCurve1D.Add(0, 0);
                jumpCurve1D.Add(1, movementTime / 2);
                jumpCurve1D.Add(0, movementTime);
            }
        }

        public void MoveInDirection(Vector3 direction, Action onMoveEndCallback = null, Action<Raycaster.HitResult> onCollideCallback = null)
        {
            if (parent != null && !parent.IsMoving)
            {
                this.endMoveCallback = onMoveEndCallback;
                this.onCollideCallback = onCollideCallback;

                startPos = parent.Transform3D.Translation;

                if (movementType == MovementType.Flip)
                {
                    RotationComponent rotationComponent =
                        (RotationComponent)parent.ControllerList.Find(controller =>
                           controller.GetType() == typeof(RotationComponent));

                    rotationComponent?.SetRotatePoint(direction);

                    //offset between the parent and the point to rotate around
                    Vector3 offset = parent.Transform3D.Translation - parent.RotatePoint;
                    //The rotation to apply
                    var rot = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));
                    //Rotate around the offset point
                    Vector3 translation = Vector3.Transform(offset, rot);

                    //startRotation = MathHelperFunctions.EulerAnglesToQuaternion(parent.Transform3D.RotationInDegrees);
                    rotationQuaternion = rot * startRotation;
                    endPos = parent.Transform3D.Translation + translation - offset;

                    if (parent.ActorType == ActorType.Player)
                    {
                        PlayerController playerController =
                            (PlayerController)parent.ControllerList.Find(controller =>
                               controller.GetType() == typeof(PlayerController));
                        if (playerController != null &&
                            playerController.IsMoveValid(rot, parent.RotatePoint, endPos, offset))
                        {
                            EventManager.FireEvent(new PlayerEventInfo { type = Enums.PlayerEventType.Move });
                            EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.PlayerMove });
                            //Calculate movement for each attached tile
                            if (parent is PlayerTile player)
                                foreach (AttachableTile tile in player.AttachedTiles)
                                    (tile.ControllerList.Find(c => c is TileMovementComponent) as TileMovementComponent)?.MoveInDirection(direction, tile.OnMoveEnd);

                            //Set animation time and movement flag
                            currentMovementTime = movementTime;
                            parent.IsMoving = true;
                        }
                        else
                            return;
                    }
                }
                else
                {
                    endPos = parent.Transform3D.Translation + direction;
                }

                diff = endPos - startPos;
                currentMovementTime = movementTime;
                parent.IsMoving = true;
            }
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as MovableTile;

            if (parent != null && parent.IsMoving)
            {
                if (currentMovementTime <= 0)
                {
                    parent.IsMoving = false;
                    currentMovementTime = 0;
                    startRotation = rotationQuaternion;
                    endMoveCallback?.Invoke();
                }

                if (movementType == MovementType.Flip)
                {
                    float t = 1 - (float)currentMovementTime / movementTime;
                    Quaternion quaternion = Quaternion.Slerp(startRotation, rotationQuaternion, t);
                    parent.Transform3D.RotationInDegrees = MathHelperFunctions.QuaternionToEulerAngles(quaternion);
                }

                Vector3 trans = startPos + diff * moveCurve1D.Evaluate(currentMovementTime, 5);
                if(jumpHeight != 0)
                    trans.Y += jumpHeight * jumpCurve1D.Evaluate(currentMovementTime, 5);

                if (onCollideCallback != null)
                {
                    Raycaster.HitResult hit = parent.Raycast(trans, Vector3.Up, true, 1f, false);

                    if (hit != null)
                    {
                        onCollideCallback?.Invoke(hit);
                        onCollideCallback = null;
                    }
                }

                parent.Transform3D.Translation = trans;
                currentMovementTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public object Clone()
        {
            return new TileMovementComponent(movementTime, new Curve1D(moveCurve1D.CurveLookType), movementType);
        }

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }
    }
}