using GDGame.Actors;
using GDGame.Component;
using GDGame.EventSystem;
using GDGame.Utilities;
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
        private MovementComponent movementComponent;
        private int pathDir = 1;

        public List<Vector3> path;
        public int currentPositionIndex;

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
            path = new List<Vector3>();
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            movementComponent = (MovementComponent)ControllerList.Find(controller => controller.GetType() == typeof(MovementComponent));
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            if(info.type == Enums.PlayerEventType.Move)
            {
                Vector3 moveDir = GetDirection();
                if (moveDir != Vector3.Zero)
                {
                    movementComponent.Move(moveDir, MoveEnd);
                    currentPositionIndex += pathDir;
                }
            }
        }

        private Vector3 GetDirection()
        {
            Vector3 origin = path[currentPositionIndex];
            Vector3 destination = NextPathPoint();
            Vector3 direction = Vector3.Normalize(destination - origin);

            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, direction, true, 1f);
            if (hit == null || hit.actor is PlayerTile)
                return direction;

            pathDir *= -1;
            Vector3 dest2 = NextPathPoint();
            Vector3 dir2 = Vector3.Normalize(dest2 - origin);

            hit = this.Raycast(Transform3D.Translation, dir2, true, 1f);
            if (destination != dest2 && (hit == null || hit.actor is PlayerTile))
                return dir2;

           return Vector3.Zero;
        }

        private Vector3 NextPathPoint()
        {
            if (currentPositionIndex + pathDir == path.Count || currentPositionIndex + pathDir == -1)
                pathDir *= -1;

            return path[currentPositionIndex + pathDir];
        }

        private void MoveEnd()
        {
            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, .5f);

            if (hit != null && hit.actor is PlayerTile)
                EventManager.FireEvent(new PlayerEventInfo() { type = Enums.PlayerEventType.Die });
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
