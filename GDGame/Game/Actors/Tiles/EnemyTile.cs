using GDGame.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDGame.Game.Actors.Tiles
{
    class EnemyTile : MovableTile
    {
        public List<Vector3> path;
        public Vector3 currentPos;

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
            path = new List<Vector3>();
        }

        public new object Clone()
        {
            EnemyTile enemyTile = new EnemyTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    enemyTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return enemyTile;
        }
    }
}
