using System.Collections.Generic;
using System.Linq;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static GDGame.Utilities.Raycaster;

namespace GDGame.Actors
{
    public class PlayerTile : AttachableTile
    {
        #region Private variables

        private Vector3 lastCheckpoint;

        #endregion

        #region Constructors

        public PlayerTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, OurEffectParameters effectParameters, Model model, ETileType tileType)
            : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            AttachedTiles = new List<AttachableTile>();
            AttachCandidates = new List<Shape>();
        }

        #endregion

        #region Properties, Indexers

        public List<Shape> AttachCandidates { get; }

        public List<AttachableTile> AttachedTiles { get; }

        #endregion

        #region Initialization

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            EventManager.RegisterListener<MovementEvent>(HandleMovementEvent);
            this.Body.CollisionSkin.callbackFn += HandleCollision;
            lastCheckpoint = Transform3D.Translation;
        }

        #endregion

        #region Override Methode

        public override void OnMoveEnd()
        {
            CheckAndProcessSurroundings(GetSurroundings(Transform3D.Translation));
            if (IsAttached) Attach();

            CheckCollision(RaycastManager.Instance.Raycast(this, Transform3D.Translation, Vector3.Up, true, 0.5f,false));
        }

        #endregion

        #region Methods

        public void Attach()
        {
            if (AttachCandidates.Count == 0 || IsMoving) return;

            AttachedTiles.Clear();
            foreach (AttachableTile tile in AttachCandidates.SelectMany(shape => shape.AttachableTiles))
            {
                AttachedTiles.Add(tile);
                ((BasicEffectParameters) tile.EffectParameters).Color = Color.DarkGray;
                tile.IsAttached = true;
            }

            IsAttached = true;
        }

        private void CheckAndProcessSurroundings(IEnumerable<PlayerSurroundCheck> surroundings)
        {
            List<MovableTile> detectedAttachableTiles = new List<MovableTile>();

            foreach (PlayerSurroundCheck check in surroundings.Where(check => check.hit != null))
                switch (check.hit.actor)
                {
                    case MovableTile t:
                        detectedAttachableTiles.Add(t);
                        break;
                }

            UpdateAttachCandidates(detectedAttachableTiles);
        }

        //replace with proper collision detection
        private void CheckCollision(HitResult hit)
        {
            if (hit?.actor == null) return;

            Actor3D actor3D = hit.actor;
            Tile tile = actor3D as Tile;
            if (tile == null) return;

            switch (tile.TileType)
            {
                case ETileType.Win:
                    EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Won));
                    break;
                case ETileType.Spike:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.Die});
                    break;
                case ETileType.Checkpoint:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.SetCheckpoint, position = tile.Transform3D.Translation});
                    break;
                case ETileType.Button:
                    ActivatableTile b = tile as ActivatableTile;
                    b?.Activate();
                    break;
                case ETileType.Star:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.PickupMug, tile = tile });
                    break;
            }
        }

        public new object Clone()
        {
            PlayerTile playerTile = new PlayerTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters,
                Model,
                TileType);

            playerTile.ControllerList.AddRange(GetControllerListClone());

            return playerTile;
        }

        public void Detach()
        {
            foreach (AttachableTile tile in AttachedTiles)
            {
                ((BasicEffectParameters) tile.EffectParameters).Color = Color.White;
                tile.IsAttached = false;
            }

            IsAttached = false;
            AttachedTiles.Clear();
        }

        private IEnumerable<PlayerSurroundCheck> GetSurroundings(Vector3 translation)
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck {hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Right, true, 1f)};
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Left, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Backward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Up, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Down, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Down, true, 0.5f);
            result.Add(surroundCheck);

            return result;
        }

        private void RespawnAtLastCheckpoint()
        {
            EventManager.FireEvent(new TileEventInfo {type = TileEventType.Reset, targetedTileType = ETileType.Attachable});
            Transform3D.Translation = lastCheckpoint;
        }

        private void SetCheckpoint(Vector3? position)
        {
            if (position != null)
                lastCheckpoint = (Vector3) position;
        }

        public void SetRotatePoint(Vector3 direction)
        {
            UpdateRotatePoints();

            Vector3 rotatePoint = Vector3.Zero;

            if (direction == Vector3.UnitX)
                rotatePoint = rightRotatePoint;
            else if (direction == -Vector3.UnitX)
                rotatePoint = leftRotatePoint;
            else if (direction == -Vector3.UnitZ)
                rotatePoint = forwardRotatePoint;
            else if (direction == Vector3.UnitZ)
                rotatePoint = backwardRotatePoint;

            RotatePoint = rotatePoint;
            foreach (AttachableTile tile in AttachedTiles) tile.RotatePoint = RotatePoint;
        }

        private void UpdateAttachCandidates(List<MovableTile> detectedAttachableTiles)
        {
            AttachCandidates.Clear();

            foreach (MovableTile tile in detectedAttachableTiles)
                AttachCandidates.Add(tile.Shape);
        }

        private void UpdateRotatePoints()
        {
            rightRotatePoint = Transform3D.Translation + new Vector3(.5f, -.5f, 0);
            leftRotatePoint = Transform3D.Translation + new Vector3(-.5f, -.5f, 0);
            forwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, -.5f);
            backwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, .5f);
            //Loops through attached tiles to update the rotation points
            foreach (AttachableTile tile in AttachedTiles)
            {
                Vector3 playerPos = Transform3D.Translation;
                Vector3 tilePos = tile.Transform3D.Translation;

                //Update right rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X > rightRotatePoint.X || tilePos.Y < rightRotatePoint.Y)
                    rightRotatePoint = tilePos + new Vector3(.5f, -.5f, 0);

                //Update left rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X < leftRotatePoint.X || tilePos.Y < leftRotatePoint.Y)
                    leftRotatePoint = tilePos + new Vector3(-.5f, -.5f, 0);

                //Update forward rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z < forwardRotatePoint.Z || tilePos.Y < forwardRotatePoint.Y)
                    forwardRotatePoint = tilePos + new Vector3(0, -.5f, -.5f);

                //Update back rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z > backwardRotatePoint.Z || tilePos.Y < backwardRotatePoint.Y)
                    backwardRotatePoint = tilePos + new Vector3(0, -.5f, .5f);
            }
        }

        #endregion

        #region Events

        private bool HandleCollision(CollisionSkin collider, CollisionSkin collidee)
        {
            //System.Diagnostics.Debug.WriteLine(collidee.Owner.ExternalData.GetType());
            return false;
        }

        private void HandleMovementEvent(MovementEvent movementEvent)
        {
            if (movementEvent.type == MovementType.OnPlayerMoved)
                foreach (AttachableTile attachable in AttachedTiles)
                    EventManager.FireEvent(new MovementEvent {type = MovementType.OnMove, direction = movementEvent.direction, onMoveEnd = OnMoveEnd, tile = attachable});
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            switch (info.type)
            {
                case PlayerEventType.Die:
                    RespawnAtLastCheckpoint();
                    break;
                case PlayerEventType.SetCheckpoint:
                    SetCheckpoint(info.position);
                    break;
                case PlayerEventType.MovableTileDie:
                    AttachedTiles.Remove(info.movableTile);
                    info.movableTile.Respawn();
                    break;
                case PlayerEventType.PickupMug:
                    //info.tile.MoveTo(true, Vector3.Up, 300, Smoother.SmoothingMethod.Smooth, LoopMethod.PingPongOnce);
                    //info.tile.ScaleTo(false, Vector3.One * 1.5f, 1000, Smoother.SmoothingMethod.Smooth, LoopMethod.PingPongOnce);
                    //info.tile.RotateTo(true, Vector3.Up * 720, 1000, Smoother.SmoothingMethod.Smooth);
                    break;
            }
        }

        #endregion

        #region Nested Types

        private struct PlayerSurroundCheck
        {
            public HitResult hit;
        }

        #endregion
    }
}