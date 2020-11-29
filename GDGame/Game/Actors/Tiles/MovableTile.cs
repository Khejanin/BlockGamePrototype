using System;
using GDGame.Enums;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class MovableTile : BasicTile, ICloneable
    {
        #region 06. Constructors

        protected MovableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform,
            effectParameters, model, tileType)
        {
            StartRotation = RotationQuaternion = Quaternion.Identity;
        }

        #endregion

        #region 07. Properties, Indexers

        public int CurrentMovementTime { get; set; }
        public Vector3 Diff { get; set; }
        public Action EndMoveCallback { get; set; }

        public Vector3 EndPos { get; set; }

        public bool IsMoving { get; set; }
        public Action<Raycaster.HitResult> OnCollideCallback { get; set; }
        public Vector3 RotatePoint { get; set; }
        public Quaternion RotationQuaternion { get; set; }

        public Vector3 StartPos { get; set; }
        public Quaternion StartRotation { get; set; }

        #endregion

        #region 09. Override Methode

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MovableTile) obj);
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(IsMoving);
            hashCode.Add(RotatePoint);
            hashCode.Add(StartPos);
            hashCode.Add(EndPos);
            hashCode.Add(CurrentMovementTime);
            hashCode.Add(RotationQuaternion);
            hashCode.Add(Diff);
            hashCode.Add(EndMoveCallback);
            hashCode.Add(OnCollideCallback);
            hashCode.Add(StartRotation);
            return hashCode.ToHashCode();
        }

        #endregion

        #region 11. Methods

        public Vector3 CalculateTargetPosition(Vector3 rotatePoint, Quaternion rotationToApply)
        {
            //offset between the player and the point to rotate around
            Vector3 offset = Transform3D.Translation - rotatePoint;
            Vector3 targetPosition = Vector3.Transform(offset, rotationToApply); //Rotate around the offset point
            targetPosition += Transform3D.Translation - offset;
            return targetPosition;
        }

        public new object Clone()
        {
            MovableTile movableTile = new MovableTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as EffectParameters,
                Model,
                TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    movableTile.ControllerList.Add(controller.Clone() as IController);

            return movableTile;
        }

        private bool Equals(MovableTile other)
        {
            return base.Equals(other) && IsMoving == other.IsMoving && RotatePoint.Equals(other.RotatePoint) && StartPos.Equals(other.StartPos) && EndPos.Equals(other.EndPos) &&
                   CurrentMovementTime == other.CurrentMovementTime && RotationQuaternion.Equals(other.RotationQuaternion) && Diff.Equals(other.Diff) &&
                   Equals(EndMoveCallback, other.EndMoveCallback) && Equals(OnCollideCallback, other.OnCollideCallback) && StartRotation.Equals(other.StartRotation);
        }

        #endregion
    }
}