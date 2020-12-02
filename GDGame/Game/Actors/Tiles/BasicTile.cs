using System;
using GDGame.Game.Parameters.Effect;
using GDGame.Tiles;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class BasicTile : ModelObject, ICloneable
    {
        public Shape Shape { get; set; }
        public bool CanMoveInto { get; set; }

        public BasicTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }
        
        public virtual void InitializeTile() { }

        public new object Clone()
        {
            BasicTile basicTile = new BasicTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    basicTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return basicTile;
        }
    }
}