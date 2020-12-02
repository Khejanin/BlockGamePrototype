using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    internal class SpikeTile : BasicTile
    {
        #region Constructors

        public SpikeTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, OurEffectParameters effectParameters, Model model, ETileType tileType) : base(
            id, actorType, statusType, transform, effectParameters, model, tileType)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            SpikeTile spikeTile = new SpikeTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    spikeTile.ControllerList.Add(controller.Clone() as IController);

            return spikeTile;
        }

        #endregion
    }
}