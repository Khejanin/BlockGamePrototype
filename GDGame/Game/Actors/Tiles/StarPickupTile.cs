using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    class StarPickupTile : BasicTile
    {
        public StarPickupTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform,
            effectParameters, model)
        {
        }

        public new object Clone()
        {
            StarPickupTile starPickupTile = new StarPickupTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    starPickupTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return starPickupTile;
        }
    }
}
