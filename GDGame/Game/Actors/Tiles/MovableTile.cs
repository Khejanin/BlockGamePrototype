using System;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     The Movable Tile is a Tile which can move.
    /// </summary>
    public class MovableTile : Tile
    {
        #region Constructors

        public MovableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking, ETileType tileType) :
            base(id, actorType, statusType, transform, effectParameters, model, isBlocking, tileType)
        {
            IsMoving = false;
        }

        #endregion

        #region Properties, Indexers

        public bool IsMoving { get; set; }

        #endregion

        #region Override Method

        //Override of what this tile will do when it dies.
        protected override void Die(Action callbackAfterDeath)
        {
            this.ScaleTo(new AnimationEventData
            {
                isRelative = false, destination = Vector3.Zero,
                maxTime = 1000,
                smoothing = Smoother.SmoothingMethod.Accelerate, loopMethod = LoopMethod.PlayOnce,
                resetAferDone = true
            });

            this.RotateTo(new AnimationEventData
            {
                isRelative = true, destination = Vector3.Up * 360, maxTime = 1000, resetAferDone = true,
                callback = callbackAfterDeath
            });
        }

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
            return hashCode.ToHashCode();
        }

        #endregion

        #region Public Method

        public new object Clone()
        {
            MovableTile movableTile = new MovableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking,
                TileType);
            movableTile.ControllerList.AddRange(GetControllerListClone());
            return movableTile;
        }

        #endregion

        #region Private Method

        protected bool Equals(MovableTile other)
        {
            return base.Equals(other) && IsMoving == other.IsMoving;
        }

        #endregion
    }
}