using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Controllers;

namespace GDGame.Actors
{
    public class ButtonTile : BasicTile, IActivatable
    {
        private bool isActivated;
        //public List<IActivatable> Targets { get; set; }

        public ButtonTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            //Targets = new List<IActivatable>();
        }

        public void Activate()
        {
            EventManager.FireEvent(new ActivatorEventInfo { type = ActivatorEventType.Activate, id = activatorId });
            //foreach (IActivatable target in Targets)
            //    target.Activate();

            isActivated = true;
        }

        public void Deactivate()
        {
            EventManager.FireEvent(new ActivatorEventInfo { type = ActivatorEventType.Deactivate, id = activatorId });
            //foreach (IActivatable target in Targets)
            //    target.Activate();

            isActivated = false;
        }

        public void ToggleActivation()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }

        public new object Clone()
        {
            ButtonTile buttonTile = new ButtonTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    buttonTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return buttonTile;
        }
    }
}
