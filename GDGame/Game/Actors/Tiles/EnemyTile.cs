using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    internal class EnemyTile : PathMoveTile
    {
        #region Private variables

        private bool canMove;

        #endregion

        #region Constructors

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, OurEffectParameters effectParameters, Model model, bool isBlocking ,ETileType tileType) : base(id, actorType, statusType, transform, effectParameters, model, isBlocking,tileType)
        {
            EventManager.RegisterListener<MovingTilesEventInfo>(OnMovingTileEvent);
        }

        #endregion

        #region Initialization

        public override void InitializeTile()
        {
            canMove = true;
            base.InitializeTile();
        }

        #endregion

        #region Override Methode

        protected override void MoveToNextPoint()
        {
            Vector3 moveDir = GetDirection();
            if (moveDir != Vector3.Zero)
                EventManager.FireEvent(new MovementEvent {type = MovementType.OnEnemyMove, tile = this, direction = moveDir, onMoveEnd = OnMoveEnd, onCollideCallback = OnCollide});
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
            canMove = true;
        }

        private void OnMovingTileEvent(MovingTilesEventInfo info)
        {
            if (canMove)
            {
                canMove = false;
                MoveToNextPoint();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            EnemyTile enemyTile = new EnemyTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model,
               isBlocking, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    enemyTile.ControllerList.Add(controller.Clone() as IController);

            return enemyTile;
        }

        private Vector3 GetDirection()
        {
            Vector3 origin = path[currentPositionIndex];
            Vector3 destination = NextPathPoint();
            Vector3 direction = Vector3.Normalize(destination - origin);

            Raycaster.HitResult hit = RaycastManager.Instance.Raycast(this, Transform3D.Translation, direction, true, 1f);
            if (hit == null || hit.actor is PlayerTile || hit.actor is MovableTile)
                return direction;

            pathDir *= -1;
            Vector3 dest2 = NextPathPoint();
            Vector3 dir2 = Vector3.Normalize(dest2 - origin);

            hit = RaycastManager.Instance.Raycast(this, Transform3D.Translation, dir2, true, 1f);
            if (destination != dest2 && (hit == null || hit.actor is PlayerTile))
                return dir2;

            return Vector3.Zero;
        }

        #endregion

        #region Events

        private void OnCollide(Raycaster.HitResult hitInfo)
        {
            switch (hitInfo?.actor)
            {
                case PlayerTile _:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.Die});
                    break;
                case AttachableTile tile:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.MovableTileDie, movableTile = tile});
                    break;
            }
        }

        #endregion
    }
}