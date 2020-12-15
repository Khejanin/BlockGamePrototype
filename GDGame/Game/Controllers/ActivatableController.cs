using System;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public enum ActivationType
    {
        AlwaysOn,
        Activated
    }

    /// <summary>
    ///     An ActivatableController is a Controller who's functionality can be Activated in different ways.
    /// </summary>
    public abstract class ActivatableController : Controller, IActivatable, IDisposable
    {
        #region Private variables

        protected ActivationType activationType;
        protected bool active;
        private bool getParent;
        protected Tile parent;

        #endregion

        #region Constructors

        protected ActivatableController(string id, ControllerType controllerType, ActivationType activationType) : base(
            id, controllerType)
        {
            this.activationType = activationType;
        }

        #endregion

        #region Override Method

        public override void Dispose()
        {
            EventManager.UnregisterListener<ActivatorEventInfo>(OnActivatorEvent);
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (getParent && parent == null)
                parent = (Tile) actor;
        }

        #endregion

        #region Public Method

        public void Activate()
        {
            active = true;
            OnActivated();
        }

        public void Deactivate()
        {
            active = false;
            OnDeactivated();
        }

        public void ToggleActivation()
        {
            if (active) Deactivate();
            else Activate();
        }

        #endregion

        #region Events

        protected abstract void OnActivated();

        private void OnActivatorEvent(ActivatorEventInfo obj)
        {
            if (obj.id == parent.activatorId)
                switch (obj.type)
                {
                    case ActivatorEventType.Activate:
                        Activate();
                        break;
                    case ActivatorEventType.Deactivate:
                        Deactivate();
                        break;
                }
        }

        protected virtual void OnClone()
        {
            switch (activationType)
            {
                case ActivationType.AlwaysOn:
                    active = true;
                    break;
                case ActivationType.Activated:
                    active = false;
                    EventManager.RegisterListener<ActivatorEventInfo>(OnActivatorEvent);
                    getParent = true;
                    break;
            }
        }

        protected abstract void OnDeactivated();

        #endregion
    }
}