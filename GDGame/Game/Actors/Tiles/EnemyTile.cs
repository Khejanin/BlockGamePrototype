using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    class EnemyTile : PathMoveTile
    {
        private float movementCoolDown;
        private float currentMovementCoolDown;
        private bool canMove;

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType,
            float movementCoolDown = 0.5f) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            this.movementCoolDown = movementCoolDown;
        }

        public override void InitializeTile()
        {
            currentMovementCoolDown = movementCoolDown;
            canMove = true;
            base.InitializeTile();
        }

        protected override void MoveToNextPoint()
        {
            Vector3 moveDir = GetDirection();
            if (moveDir != Vector3.Zero)
            {
                EventManager.FireEvent(new MovementEvent {type = MovementType.OnEnemyMove, tile = this, direction = moveDir, onMoveEnd = OnMoveEnd, onCollideCallback = OnCollide});
            }
                
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
            canMove = true;
            currentMovementCoolDown = movementCoolDown;
        }

        private void OnCollide(Raycaster.HitResult hitInfo)
        {
            if (hitInfo?.actor is PlayerTile)
                EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.Die});
            else if (hitInfo?.actor is AttachableTile tile)
                EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.AttachedTileDie, attachedTile = tile});
        }

        private Vector3 GetDirection()
        {
            Vector3 origin = path[currentPositionIndex];
            Vector3 destination = NextPathPoint();
            Vector3 direction = Vector3.Normalize(destination - origin);

            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, direction, true, 1f);
            if (hit == null || hit.actor is PlayerTile || hit.actor is AttachableTile)
                return direction;

            pathDir *= -1;
            Vector3 dest2 = NextPathPoint();
            Vector3 dir2 = Vector3.Normalize(dest2 - origin);

            hit = this.Raycast(Transform3D.Translation, dir2, true, 1f);
            if (destination != dest2 && (hit == null || hit.actor is PlayerTile))
                return dir2;

            return Vector3.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (canMove && currentMovementCoolDown <= 0)
            {
                canMove = false;
                MoveToNextPoint();
                return;
            }

            currentMovementCoolDown -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public new object Clone()
        {
            EnemyTile enemyTile = new EnemyTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
            {
                foreach (Controller controller in ControllerList)
                {
                    enemyTile.ControllerList.Add(controller.Clone() as Controller);
                }
            }

            return enemyTile;
        }
    }
}