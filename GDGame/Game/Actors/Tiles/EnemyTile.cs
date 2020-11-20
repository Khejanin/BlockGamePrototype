﻿using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    class EnemyTile : PathMoveTile
    {

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            base.InitializeTile();
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            if(info.type == Enums.PlayerEventType.Move)
            {
                MoveToNextPoint();
            }
        }

        protected override void MoveToNextPoint()
        {
            Vector3 moveDir = GetDirection();
            if (moveDir != Vector3.Zero)
                movementComponent.MoveInDirection(moveDir, OnMoveEnd);
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();

            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, .5f);

            if (hit != null && hit.actor is PlayerTile)
                EventManager.FireEvent(new PlayerEventInfo() { type = Enums.PlayerEventType.Die });
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
