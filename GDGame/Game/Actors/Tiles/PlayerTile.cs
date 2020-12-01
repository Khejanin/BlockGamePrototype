using System.Collections.Generic;
using System.Linq;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Managers;
using GDGame.Tiles;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static GDGame.Utilities.Raycaster;

namespace GDGame.Actors
{
    public class PlayerTile : MovableTile
    {
        #region Private variables

        private Vector3 lastCheckpoint;

        #endregion

        #region Constructors

        public PlayerTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType)
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
            lastCheckpoint = Transform3D.Translation;
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
                tile.EffectParameters.DiffuseColor = Color.DarkGray;
                tile.IsAttached = true;
            }

            IsAttached = true;
        }

        private void CheckAndProcessSurroundings(IEnumerable<PlayerSurroundCheck> surroundings)
        {
            List<AttachableTile> detectedAttachableTiles = new List<AttachableTile>();

            foreach (PlayerSurroundCheck check in surroundings.Where(check => check.hit != null))
                switch (check.hit.actor)
                {
                    case AttachableTile t:
                        detectedAttachableTiles.Add(t);
                        break;
                }

            UpdateAttachCandidates(detectedAttachableTiles);
        }

        //replace with proper collision detection
        private void CheckCollision(HitResult hit)
        {
            if (hit?.actor == null) return;

            switch (hit.actor)
            {
                case GoalTile _:
                    EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Won));
                    break;
                case SpikeTile _:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.Die});
                    break;
                case CheckpointTile t:
                    EventManager.FireEvent(new PlayerEventInfo {type = PlayerEventType.SetCheckpoint, position = t.Transform3D.Translation});
                    break;
                case ButtonTile t:
                    t.Activate();
                    break;
            }
        }

        public new object Clone()
        {
            PlayerTile playerTile = new PlayerTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as EffectParameters, Model,
                TileType);

            playerTile.ControllerList.AddRange(GetControllerListClone());

            return playerTile;
        }

        public void Detach()
        {
            foreach (AttachableTile tile in AttachedTiles)
            {
                tile.EffectParameters.DiffuseColor = Color.White;
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

        private void UpdateAttachCandidates(List<AttachableTile> detectedAttachableTiles)
        {
            AttachCandidates.Clear();

            foreach (AttachableTile tile in detectedAttachableTiles)
                AttachCandidates.Add(tile.Shape);
        }

        #endregion

        #region Events

        private void HandleMovementEvent(MovementEvent movementEvent)
        {
            if (movementEvent.type == MovementType.OnPlayerMoved)
                foreach (AttachableTile attachedTile in AttachedTiles)
                    EventManager.FireEvent(new MovementEvent {type = MovementType.OnMove, direction = movementEvent.direction, onMoveEnd = OnMoveEnd, tile = attachedTile});
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
                case PlayerEventType.AttachedTileDie:
                    AttachedTiles.Remove(info.attachedTile);
                    info.attachedTile.Respawn();
                    break;
            }
        }

        public void OnMoveEnd()
        {
            CheckAndProcessSurroundings(GetSurroundings(Transform3D.Translation));
            if (IsAttached) Attach();

            CheckCollision(RaycastManager.Instance.Raycast(this, Transform3D.Translation, Vector3.Down, true, 0.5f));
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