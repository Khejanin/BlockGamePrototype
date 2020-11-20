using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDGame.Actors
{
    public class ButtonTile : BasicTile
    {
        private List<BasicTile> targets;
        public List<BasicTile> Targets { get => targets; set => targets = value; }

        public ButtonTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
            Targets = new List<BasicTile>();
        }

        public new object Clone()
        {
            ButtonTile buttonTile = new ButtonTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

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
