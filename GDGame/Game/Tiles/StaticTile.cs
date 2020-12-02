using System;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class StaticTile : GridTile, ICloneable
    {
        public StaticTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {

        }

        public new object Clone()
        {
            StaticTile staticTile = new StaticTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    staticTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return staticTile;
        }
    }
}