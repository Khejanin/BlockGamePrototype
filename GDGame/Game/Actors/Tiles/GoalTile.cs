using GDGame.Enums;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class GoalTile : BasicTile
    {
        #region 06. Constructors

        public GoalTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform,
            effectParameters, model, tileType)
        {
        }

        #endregion

        #region 11. Methods

        public new object Clone()
        {
            GoalTile goalTile = new GoalTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);
            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    goalTile.ControllerList.Add(controller.Clone() as IController);

            return goalTile;
        }

        #endregion
    }
}