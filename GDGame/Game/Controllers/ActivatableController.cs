using System;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Actors;
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
    /// An ActivatableController is a Controller who's functionality can be Activated in different ways.
    /// </summary>
    public abstract class ActivatableController : Controller, IActivatable, ICloneable
    {
        protected Tile parent;
        private bool getParent = false;
        protected ActivationType activationType;
        protected bool active;

        public ActivatableController(string id, ControllerType controllerType,ActivationType activationType) : base(id,controllerType)
        {
            this.activationType = activationType;
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

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (getParent && parent == null)
                parent = (Tile) actor;
        }

        private void OnActivatorEvent(ActivatorEventInfo obj)
        {
            if (obj.id == parent.activatorId)
            {
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
        }

        public void Activate()
        {
            active = true;
            OnActivated();
        }

        protected abstract void OnActivated();

        public void Deactivate()
        {
            active = false;
            OnDeactivated();
        }
        
        protected abstract void OnDeactivated();

        public void ToggleActivation()
        {
            if(active) Deactivate();
            else Activate();
        }

    }
}