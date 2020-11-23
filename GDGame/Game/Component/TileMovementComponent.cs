using System;
using System.Linq;
using GDGame.Actors;
using GDGame.Controllers;
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
        private MovableTile parent;

        private int movementTime;
        private int currentMovementTime;
        private bool useFlipMovement;
        private Curve1D curve1D;
        private Vector3 diff;
        private Quaternion rotationQuaternion;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotation;
        private Action endMoveCallback;
        private Action<Raycaster.HitResult> onCollideCallback;

        public TileMovementComponent(int movementTime, Curve1D curve1D, bool useFlipMovement = false)
        {
            this.movementTime = movementTime;
            this.curve1D = curve1D;
            this.useFlipMovement = useFlipMovement;
            this.curve1D.Add(1, 0);
            this.curve1D.Add(0, movementTime);
            startRotation = rotationQuaternion = Quaternion.Identity;
        }

        public void MoveInDirection(Vector3 direction, Action onMoveEndCallback = null, Action<Raycaster.HitResult> onCollideCallback = null)
        {
            if (parent != null && !parent.IsMoving)
            {
                this.endMoveCallback = onMoveEndCallback;
                this.onCollideCallback = onCollideCallback;

                startPos = parent.Transform3D.Translation;

                if (useFlipMovement)
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

                if (useFlipMovement)
                {
                    float t = 1 - (float)currentMovementTime / movementTime;
                    Quaternion quaternion = Quaternion.Slerp(startRotation, rotationQuaternion, t);
                    parent.Transform3D.RotationInDegrees = MathHelperFunctions.QuaternionToEulerAngles(quaternion);
                }

                float currentStep = curve1D.Evaluate(currentMovementTime, 5);
                Vector3 trans = startPos + diff * currentStep;

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
            return new TileMovementComponent(movementTime, new Curve1D(curve1D.CurveLookType), useFlipMovement);
        }

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }
    }
}