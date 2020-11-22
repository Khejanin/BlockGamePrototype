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
        private List<Shape> attachCandidates;
        public List<Shape> AttachCandidates => attachCandidates;
        public List<AttachableTile> AttachedTiles { get; }
        public bool IsAttached { get; private set; }

        private struct PlayerSurroundCheck
        {
            public HitResult hit;
        }

        public PlayerTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            AttachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            switch(info.type)
            {
                case PlayerEventType.Die:
                    System.Diagnostics.Debug.WriteLine("Player ded");
                    EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Lost));
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
                tile.EffectParameters.DiffuseColor = Color.Green;
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

        private bool CheckWinCondition()
        {
            HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, 0.5f,false);
            return hit?.actor is GoalTile;
        }

        private bool CheckLoseCondition()
        {
            HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Down, true, 0.5f,false);
            return hit?.actor is SpikeTile;
        }

        public void OnMoveEnd()
        {
            UpdateAttachCandidates();
            if(IsAttached) Attach();

            if (CheckWinCondition()) 
                EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Won));
            else if(CheckLoseCondition())
                EventManager.FireEvent(new PlayerEventInfo { type = PlayerEventType.Die });
        }

        public void UpdateAttachCandidates()
        {
            attachCandidates.Clear();

            foreach (PlayerSurroundCheck check in CheckSurroundings(Transform3D.Translation))
            {
                if (check.hit?.actor is AttachableTile tile)
                    attachCandidates.Add(tile.Shape);
            }
        }

        private List<PlayerSurroundCheck> CheckSurroundings(Vector3 translation)
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(translation, Vector3.Right, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Left, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Backward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Up, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(translation, Vector3.Down, true, 1f);
            result.Add(surroundCheck);

            return result;
        }

        public new object Clone()
        {
            PlayerTile playerTile = new PlayerTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
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