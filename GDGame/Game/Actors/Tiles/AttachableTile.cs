using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     The attachable tile is a tile which can be attached to the player
    /// </summary>
    public class AttachableTile : MovableTile
    {
        #region Private variables

        protected Vector3 backwardRotatePoint;
        protected Vector3 forwardRotatePoint;
        protected Vector3 leftRotatePoint;
        protected Vector3 rightRotatePoint;

        #endregion

        #region Constructors

        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, ETileType tileType) :
            base(id, actorType, statusType, transform, effectParameters, model, true, tileType)
        {
            IsAttached = false;
            RotatePoint = Vector3.Zero;
        }

        #endregion

        #region Properties, Indexers

        public bool IsAttached { get; set; }
        public Vector3 RotatePoint { get; set; }

        #endregion

        #region Public Method

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
            AttachableTile attachableTile = new AttachableTile(ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters,
                Model, TileType);
            attachableTile.ControllerList.AddRange(GetControllerListClone());
            return attachableTile;
        }

        #endregion

        #region Events

        public virtual void OnMoveEnd()
        {
        }

        #endregion
    }
}