using GDGame.Enums;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;
using Transform3D = GDLibrary.Parameters.Transform3D;

namespace GDGame.Actors
{
    class CheckpointTile : BasicTile
    {
        public CheckpointTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
        }

        public override void InitializeTile()
        {
            base.InitializeTile();
        }

        public new object Clone()
        {
            CheckpointTile checkpointTile = new CheckpointTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
            {
                foreach (Controller controller in ControllerList)
                {
                    checkpointTile.ControllerList.Add(controller.Clone() as Controller);
                }
            }

            return checkpointTile;
        }
    }
}
