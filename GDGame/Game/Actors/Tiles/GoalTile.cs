﻿using GDGame.Game.Parameters.Effect;
﻿using GDGame.Enums;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
 using GDLibrary.Parameters;
 using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class GoalTile : BasicTile
    {
        #region Constructors

        public GoalTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform,
            effectParameters, model, tileType)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            GoalTile goalTile = new GoalTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, TileType);
            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    goalTile.ControllerList.Add(controller.Clone() as IController);

            return goalTile;
        }

        #endregion
    }
}