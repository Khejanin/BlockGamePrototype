using System;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
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

        #region Override Methode

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

        #region Methods

        public new object Clone()
        {
            MovableTile movableTile = new MovableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking,
                TileType);
            movableTile.ControllerList.AddRange(GetControllerListClone());
            return movableTile;
        }

        protected bool Equals(MovableTile other)
        {
            return base.Equals(other) && IsMoving == other.IsMoving;
        }

        #endregion
    }
}