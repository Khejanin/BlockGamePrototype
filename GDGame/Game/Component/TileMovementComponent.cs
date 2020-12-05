using System;
using GDGame.Actors;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class TileMovementComponent : Controller, ICloneable
    {
        #region Private variables

        private int currentMovementTime;

        private Curve1D curve1D;
        private Vector3 diff;
        private Action endMoveCallback;
        private Vector3 endPos;
        private int movementTime;
        private Action<Raycaster.HitResult> onCollideCallback;
        private Quaternion rotationQuaternion;
        private Vector3 startPos;
        private Quaternion startRotation;

        #endregion

        #region Constructors

        public TileMovementComponent(string id, ControllerType controllerType, int movementTime, Curve1D curve1D) :
            base(id, controllerType)
        {
            this.movementTime = movementTime;
            this.curve1D = curve1D;

            startRotation = rotationQuaternion = Quaternion.Identity;

            this.curve1D.Add(1, 0);
            this.curve1D.Add(0, movementTime);
        }

        #endregion

        #region Properties, Indexers

        private AttachableTile Tile { get; set; }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            Tile ??= actor as AttachableTile;

            if (Tile != null && Tile.IsMoving)
            {
                endMoveCallback ??= Tile.OnMoveEnd;
                if (currentMovementTime <= 0)
                {
                    Tile.IsMoving = false;
                    currentMovementTime = 0;
                    startRotation = rotationQuaternion;
                    endMoveCallback?.Invoke();
                }

                float t = 1 - (float) currentMovementTime / movementTime;
                Quaternion quaternion = Quaternion.Slerp(startRotation, rotationQuaternion, t);
                Tile.Transform3D.RotationInDegrees = MathHelperFunctions.QuaternionToEulerAngles(quaternion);


                float currentStep = curve1D.Evaluate(currentMovementTime, 5);
                Vector3 trans = startPos + diff * currentStep;

                if (onCollideCallback != null)
                {
                    Raycaster.HitResult hit = RaycastManager.Instance.Raycast(Tile, trans, Vector3.Up, true, 1f);

                    if (hit != null)
                    {
                        onCollideCallback?.Invoke(hit);
                        onCollideCallback = null;
                    }
                }

                if (currentMovementTime <= 0)
                {
                    trans = new Vector3((float) Math.Round(trans.X) , (float) Math.Round(trans.Y), (float) Math.Round(trans.Z));
                }

                Tile.Transform3D.Translation = trans;
                Tile.Body.MoveTo(trans, Matrix.Identity);
                currentMovementTime -= (int) gameTime.ElapsedGameTime.TotalMilliseconds;
                Tile.Body.ApplyGravity = false;
                Tile.Body.Immovable = true;
                Tile.Body.SetInactive();
            }

            if (Tile != null && !Tile.IsMoving && !Tile.IsAttached && !Tile.Body.ApplyGravity)
            {
                Tile.Body.ApplyGravity = true;
                Tile.Body.Immovable = false;
            }

            if (Tile != null && !Tile.IsMoving)
            {
                Tile.Body.Velocity = Vector3.UnitY * Tile.Body.Velocity.Y;
                Tile.Body.Torque = Vector3.UnitY * Tile.Body.Torque.Y;
                Tile.Body.AngularVelocity = Vector3.UnitY * Tile.Body.AngularVelocity.Y;
            }
        }

        #endregion

        #region Methods

        public void CalculateEndPos(Vector3 direction, out Vector3 endPos, out Quaternion rot, out Vector3 offset)
        {
            startPos = Tile.Transform3D.Translation;
            //offset between the parent and the point to rotate around
            offset = Tile.Transform3D.Translation - Tile.RotatePoint;
            //The rotation to apply
            rot = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));
            //Rotate around the offset point
            Vector3 translation = Vector3.Transform(offset, rot);
            //startRotation = MathHelperFunctions.EulerAnglesToQuaternion(parent.Transform3D.RotationInDegrees);
            rotationQuaternion = rot * startRotation;
            endPos = Tile.Transform3D.Translation + translation - offset;
            this.endPos = endPos;
        }

        public new object Clone()
        {
            TileMovementComponent tileMovementComponent =
                new TileMovementComponent(ID, ControllerType, movementTime, new Curve1D(curve1D.CurveLookType));
            return tileMovementComponent;
        }

        public void MoveTile()
        {
            diff = endPos - startPos;
            currentMovementTime = movementTime;
            Tile.IsMoving = true;
        }

        #endregion
    }
}