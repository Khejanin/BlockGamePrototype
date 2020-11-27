using System;
using GDGame.Actors;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class TileMovementComponent : Controller
    {
        private int movementTime;
        private bool useFlipMovement;
        private Curve1D curve1D;


        public TileMovementComponent(string id, ControllerType controllerType, int movementTime, Curve1D curve1D, bool useFlipMovement = false, MovableTile movableTile = null) : base(id, controllerType)
        {
            this.movementTime = movementTime;
            this.useFlipMovement = useFlipMovement;
            this.curve1D = curve1D;
            Tile = movableTile;

            this.curve1D.Add(1, 0);
            this.curve1D.Add(0, movementTime);
        }

        public MovableTile Tile { get; set; }

        private void HandleMovementEvent(MovementEvent movementEventInfo)
        {
            switch (movementEventInfo.type)
            {
                case MovementType.OnEnemyMove:
                    MoveInDirection(movementEventInfo.tile, movementEventInfo.direction, movementEventInfo.onMoveEnd, movementEventInfo.onCollideCallback);
                    break;
                case MovementType.OnMove:
                    MoveInDirection(movementEventInfo.tile, movementEventInfo.direction, movementEventInfo.onMoveEnd);
                    break;
            }
        }

        private void MoveInDirection(MovableTile movableTile, Vector3 direction, Action onMoveEndCallback = null, Action<Raycaster.HitResult> onCollideCallback = null)
        {
            if (Equals(Tile, movableTile) && !Tile.IsMoving)
            {
                Tile.EndMoveCallback = onMoveEndCallback;
                Tile.OnCollideCallback = onCollideCallback;

                Tile.StartPos = Tile.Transform3D.Translation;

                if (useFlipMovement)
                {
                    RotationComponent rotationComponent = (RotationComponent) Tile.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Rotation);
                    rotationComponent?.SetRotatePoint(direction, Tile);
                    //offset between the parent and the point to rotate around
                    Vector3 offset = Tile.Transform3D.Translation - Tile.RotatePoint;
                    //The rotation to apply
                    Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));
                    //Rotate around the offset point
                    Vector3 translation = Vector3.Transform(offset, rot);
                    //startRotation = MathHelperFunctions.EulerAnglesToQuaternion(parent.Transform3D.RotationInDegrees);
                    Tile.RotationQuaternion = rot * Tile.StartRotation;
                    Tile.EndPos = Tile.Transform3D.Translation + translation - offset;

                    if (movableTile.ActorType == ActorType.Player)
                    {
                        PlayerController playerController = (PlayerController) Tile.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Player);
                        if (playerController != null && playerController.IsMoveValid(Tile as PlayerTile, rot, Tile.RotatePoint, Tile.EndPos, offset))
                        {
                            EventManager.FireEvent(new MovementEvent {type = MovementType.OnPlayerMoved, direction = direction});

                            //Calculate movement for each attached tile
                            Tile.CurrentMovementTime = movementTime;
                            Tile.IsMoving = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    Tile.EndPos = Tile.Transform3D.Translation + direction;
                }

                Tile.Diff = Tile.EndPos - Tile.StartPos;
                Tile.CurrentMovementTime = movementTime;
                Tile.IsMoving = true;
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (Tile != null && Tile.IsMoving)
            {
                if (Tile.CurrentMovementTime <= 0)
                {
                    Tile.IsMoving = false;
                    Tile.CurrentMovementTime = 0;
                    Tile.StartRotation = Tile.RotationQuaternion;
                    Tile.EndMoveCallback?.Invoke();
                }

                if (useFlipMovement)
                {
                    float t = 1 - (float) Tile.CurrentMovementTime / movementTime;
                    Quaternion quaternion = Quaternion.Slerp(Tile.StartRotation, Tile.RotationQuaternion, t);
                    Tile.Transform3D.RotationInDegrees = MathHelperFunctions.QuaternionToEulerAngles(quaternion);
                }

                float currentStep = curve1D.Evaluate(Tile.CurrentMovementTime, 5);
                Vector3 trans = Tile.StartPos + Tile.Diff * currentStep;

                if (Tile.OnCollideCallback != null)
                {
                    Raycaster.HitResult hit = Tile.Raycast(trans, Vector3.Up, true, 1f, false);

                    if (hit != null)
                    {
                        Tile.OnCollideCallback?.Invoke(hit);
                        Tile.OnCollideCallback = null;
                    }
                }

                Tile.Transform3D.Translation = trans;
                Tile.CurrentMovementTime -= (int) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public override object Clone()
        {
            TileMovementComponent tileMovementComponent = new TileMovementComponent(Id, ControllerType, movementTime, new Curve1D(curve1D.CurveLookType), useFlipMovement, Tile);
            tileMovementComponent.Init();
            return tileMovementComponent;
        }

        private void Init()
        {
            EventManager.RegisterListener<MovementEvent>(HandleMovementEvent);
        }
    }
}