using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static GDGame.Utilities.Raycaster;

namespace GDGame.Actors
{
    public class PlayerTile : MovableTile, ICloneable
    {
        private Vector3 lastCheckpoint;
        private List<Shape> attachCandidates;
        public List<Shape> AttachCandidates => attachCandidates;
        public List<AttachableTile> AttachedTiles { get; }
        public bool IsAttached { get; private set; }

        private struct PlayerSurroundCheck
        {
            public HitResult hit;
            public Direction direction;
        }

        public PlayerTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, TileType tileType)
            : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            AttachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            lastCheckpoint = Transform3D.Translation;
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            switch(info.type)
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

        public void Attach()
        {
            if (attachCandidates.Count == 0 || IsMoving) return;

            AttachedTiles.Clear();
            foreach (AttachableTile tile in attachCandidates.SelectMany(shape =>  shape.AttachableTiles))
            {
                AttachedTiles.Add(tile);
                tile.EffectParameters.DiffuseColor = Color.DarkGray;
                if(!IsAttached)
                    EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.PlayerAttach });
            }

            IsAttached = true;
        }

        public void Detach()
        {
            foreach (AttachableTile tile in AttachedTiles)
            {
                tile.EffectParameters.DiffuseColor = Color.White;
            }

            AttachedTiles.Clear();
            IsAttached = false;
        }

        //replace with proper collision detection
        private void CheckCollision(HitResult hit)
        {
            if (hit?.actor == null) return;

            switch (hit.actor)
            {
                case GoalTile t:
                    EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Won));
                    break;
                case SpikeTile t:
                    EventManager.FireEvent(new PlayerEventInfo { type = PlayerEventType.Die });
                    break;
                case CheckpointTile t:
                    EventManager.FireEvent(new PlayerEventInfo { type = PlayerEventType.SetCheckpoint, position = t.Transform3D.Translation });
                    break;
                case ButtonTile t:
                    t.Activate();
                    break;
            }
        }

        public void OnMoveEnd()
        {
            CheckAndProcessSurroundings(GetSurroundings(Transform3D.Translation));
            if(IsAttached) Attach();

            CheckCollision(this.Raycast(Transform3D.Translation, Vector3.Down, true, 0.5f,false));
        }

        private void CheckAndProcessSurroundings(List<PlayerSurroundCheck> surroundings)
        {
            List<AttachableTile> detectedAttachableTiles = new List<AttachableTile>();

            foreach (PlayerSurroundCheck check in surroundings)
            {
                if (check.hit == null) 
                    continue;

                switch (check.hit.actor)
                {
                    case AttachableTile t:
                        detectedAttachableTiles.Add(t);
                        break;
                }
            }

            UpdateAttachCandidates(detectedAttachableTiles);
        }

        private void UpdateAttachCandidates(List<AttachableTile> detectedAttachableTiles)
        {
            attachCandidates.Clear();

            foreach (AttachableTile tile in detectedAttachableTiles)
                attachCandidates.Add(tile.Shape);
        }

        private List<PlayerSurroundCheck> GetSurroundings(Vector3 translation)
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(translation, Vector3.Right, true, 1f);
            surroundCheck.direction = Direction.Right;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Left, true, 1f);
            surroundCheck.direction = Direction.Left;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Forward, true, 1f);
            surroundCheck.direction = Direction.Forward;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Backward, true, 1f);
            surroundCheck.direction = Direction.Backward;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Up, true, 1f);
            surroundCheck.direction = Direction.Up;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Down, true, 1f);
            surroundCheck.direction = Direction.Down;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Down, true, 0.5f);
            surroundCheck.direction = Direction.None;
            result.Add(surroundCheck);

            return result;
        }

        private void SetCheckpoint(Vector3? position)
        {
            if (position != null)
                lastCheckpoint = (Vector3)position;
        }

        private void RespawnAtLastCheckpoint()
        {
            EventManager.FireEvent(new TileEventInfo { type = TileEventType.Reset, targetedTileType = Enums.TileType.Attachable });
            Transform3D.Translation = lastCheckpoint;
        }

        public new object Clone()
        {
            PlayerTile playerTile = new PlayerTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    playerTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return playerTile;
        }
    }
}