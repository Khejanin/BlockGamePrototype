using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class GoalTile : BasicTile
    {
        public GoalTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform,
            effectParameters, model)
        {
        }

        public new object Clone()
        {
            GoalTile goalTile = new GoalTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    goalTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return goalTile;
        }
    }
}