using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    class SpikeTile : BasicTile
    {
        public SpikeTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public new object Clone()
        {
            SpikeTile spikeTile = new SpikeTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    spikeTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return spikeTile;
        }
    }
}
