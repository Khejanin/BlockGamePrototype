using GDGame.Actors;
using GDGame.Component;
using GDGame.EventSystem;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDGame.Game.Actors.Tiles
{
    class EnemyTile : BasicTile
    {
        private MovementComponent movementComponent;

        public List<Vector3> path;
        public int currentPositionIndex;

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
            path = new List<Vector3>();
            //EventSystem.EventSystem.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            if(info.type == Enums.PlayerEventType.Move)
            {
                if(movementComponent == null) 
                    movementComponent = (MovementComponent)ControllerList.Find(controller => controller.GetType() == typeof(MovementComponent));

                movementComponent.Move(Vector3.Normalize(NextPathPoint() - Transform3D.Translation));
            }
        }

        private Vector3 NextPathPoint()
        {
            if (++currentPositionIndex == path.Count)
                currentPositionIndex = 0;

            return path[currentPositionIndex];
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
