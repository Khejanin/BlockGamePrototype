using GDGame.Enums;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    internal class CheckpointTile : BasicTile
    {
        #region 06. Constructors

        public CheckpointTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) :
            base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
        }

        #endregion

        #region 08. Initialization

        public override void InitializeTile()
        {
            base.InitializeTile();
        }

        #endregion

        #region 11. Methods

        public new object Clone()
        {
            CheckpointTile checkpointTile = new CheckpointTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    checkpointTile.ControllerList.Add(controller.Clone() as IController);

            return checkpointTile;
        }

        #endregion
    }
}