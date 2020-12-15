using System;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Interfaces;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     The Activatable Tile is a BasicTile which implements IActivatable giving it the possibility to have Activation
    ///     functionality like a Button.
    /// </summary>
    public class ActivatableTile : Tile, IActivatable
    {
        #region Private variables

        private bool isActivated;

        #endregion

        #region Constructors

        //public List<IActivatable> Targets { get; set; }

        public ActivatableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking,
            ETileType tileType) : base(
            id, actorType, statusType, transform, effectParameters, model, isBlocking, tileType)
        {
        }

        #endregion

        #region Override Method

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ActivatableTile) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), isActivated);
        }

        #endregion

        #region Public Method

        public void Activate()
        {
            EventManager.FireEvent(new ActivatorEventInfo {type = ActivatorEventType.Activate, id = activatorId});
            isActivated = true;
        }

        public new object Clone()
        {
            ActivatableTile activatableTile = new ActivatableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    activatableTile.ControllerList.Add(controller.Clone() as IController);

            return activatableTile;
        }

        public void Deactivate()
        {
            EventManager.FireEvent(new ActivatorEventInfo {type = ActivatorEventType.Deactivate, id = activatorId});
            isActivated = false;
        }

        public void ToggleActivation()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }

        #endregion

        #region Private Method

        private bool Equals(ActivatableTile other)
        {
            return base.Equals(other) && isActivated == other.isActivated;
        }

        #endregion
    }
}