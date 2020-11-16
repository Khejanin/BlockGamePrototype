using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Tiles;
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

        public List<MovableTile> AttachedTiles { get; }

        public bool IsAttached { get; private set; }

        private struct PlayerSurroundCheck
        {
            public HitResult hit;
        }

        public PlayerTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            AttachedTiles = new List<MovableTile>();
            attachCandidates = new List<Shape>();
        }

        public void Attach()
        {
            if (attachCandidates.Count == 0) return;

            foreach (MovableTile tile in attachCandidates.SelectMany(shape =>  shape.AttachableTiles))
            {
                AttachedTiles.Add(tile);
                tile.EffectParameters.DiffuseColor = Color.Green;
            }

            IsAttached = true;
        }

        public void Detach()
        {
            foreach (MovableTile tile in AttachedTiles)
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

        /// <summary>
        /// Is in charge of the Animation for when the Player Moves
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

             if (IsMoving)
             {
                 if (CurrentMovementTime <= 0)
                 {
                    UpdateAttachCandidates(); //remove this later
                    bool won = CheckWinCondition(); //remove this later
                    if (won) EventSystem.EventManager.FireEvent(new GameStateMessageEventInfo(GameState.Won));
                 }
             }
        }


        public void UpdateAttachCandidates()
        {
            attachCandidates.Clear();

            foreach (PlayerSurroundCheck check in CheckSurroundings())
                if (check.hit?.actor is MovableTile tile)
                    attachCandidates.Add(tile.Shape);
        }

        private List<PlayerSurroundCheck> CheckSurroundings()
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Right, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Right, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Up, true, 1f);
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