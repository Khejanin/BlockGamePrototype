using GDGame.Enums;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    internal class PickupTile : BasicTile
    {
        #region Constructors

        public PickupTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform,
            effectParameters, model, tileType)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            PickupTile pickupTile = new PickupTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    pickupTile.ControllerList.Add(controller.Clone() as IController);

            return pickupTile;
        }

        #endregion
    }
}