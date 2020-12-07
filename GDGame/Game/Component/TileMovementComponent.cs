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
    /// <summary>
    /// Class That makes the Player and his Attachable Tiles move.
    /// As this is not just a one-shot unimportant animation we handle this manually in this code instead of in the TransformAnimationManager.
    /// </summary>
    public class TileMovementComponent : Controller, ICloneable
    {
        #region Private variables

        private int currentMovementTime;

        private Vector3 diff;
        private Action endMoveCallback;
        private Vector3 endPos;
        private bool isDirty;
        private int movementTime;
        private Action<Raycaster.HitResult> onCollideCallback;
        private Quaternion rotationQuaternion;
        private Vector3 startPos;
        private Quaternion startRotation;

        #endregion

        #region Constructors

        public TileMovementComponent(string id, ControllerType controllerType, int movementTime) :
            base(id, controllerType)
        {
            this.movementTime = movementTime;

            startRotation = rotationQuaternion = Quaternion.Identity;
        }

        #endregion

        #region Properties, Indexers

        private AttachableTile Tile { get; set; }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            Tile ??= actor as AttachableTile;
            if (Tile == null) return;
            if (Tile.IsMoving)
            {
                endMoveCallback ??= Tile.OnMoveEnd;
                //Check if done moving.
                if (currentMovementTime <= 0)
                {
                    Tile.IsMoving = false;
                    currentMovementTime = 0;
                    startRotation = rotationQuaternion;
                    endMoveCallback?.Invoke();
                    isDirty = true;
                }

                //The Smoother Replaces Curves as it's easier to have slopes.
                //We count down so it's inverted.
                float currentStep = 1-Smoother.SmoothValue(Smoother.SmoothingMethod.Smooth, (float) currentMovementTime / movementTime);
                Vector3 trans = startPos + diff * currentStep;
                
                //Invert Lerp percent and get Rotation
                Quaternion quaternion = Quaternion.Slerp(startRotation, rotationQuaternion, currentStep);
                Tile.Transform3D.RotationInDegrees = MathHelperFunctions.QuaternionToEulerAngles(quaternion);

                //Check if we have a callback that we must invoke if we collide in the move
                if (onCollideCallback != null)
                {
                    //Raycast to check if there's anything
                    Raycaster.HitResult hit = RaycastManager.Instance.Raycast(Tile, trans, Vector3.Up, true, 1f);

                    //Invoke callback if we hit something
                    if (hit != null)
                    {
                        onCollideCallback?.Invoke(hit);
                        onCollideCallback = null;
                    }
                }
                
                //Dont go anywhere we don't want the tile to go
                if (currentMovementTime <= 0)
                    trans = new Vector3((float) Math.Round(trans.X), (float) Math.Round(trans.Y),
                        (float) Math.Round(trans.Z));

                Tile.SetTranslation(trans);
                currentMovementTime -= (int) gameTime.ElapsedGameTime.TotalMilliseconds;
                if (currentMovementTime <= 0) Tile.IsDirty = false;
            }

            //Here we handle whether we activate physics or not.
            if (isDirty && !Tile.IsMoving && !Tile.IsAttached && !Tile.Body.ApplyGravity)
            {
                Tile.Body.ApplyGravity = true;
                Tile.Body.Immovable = false;
                isDirty = false;
            }
            else if (!Tile.IsMoving && !Tile.IsDirty)
            {
                Tile.Body.Velocity = Vector3.UnitY * Tile.Body.Velocity.Y;
                Tile.Body.Torque = Vector3.UnitY * Tile.Body.Torque.Y;
                Tile.Body.AngularVelocity = Vector3.UnitY * Tile.Body.AngularVelocity.Y;

                Tile.Transform3D.Translation = Tile.Body.Position;
            }
            else if (!Tile.IsMoving)
            {
                Tile.SetTranslation(Tile.Transform3D.Translation);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method calculates the end position for this tile given the parameters.
        /// Offset is very important when rotating like the player does.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="endPos"></param>
        /// <param name="rot"></param>
        /// <param name="offset"></param>
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
                new TileMovementComponent(ID, ControllerType, movementTime);
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