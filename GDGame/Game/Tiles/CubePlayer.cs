using System;
using GDGame.Game.Enums;
using GDGame.Game.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Interfaces;
using static GDGame.Game.Utilities.Raycaster;

namespace GDGame.Game.Tiles
{
    public class CubePlayer : GridTile, ICloneable
    {
        private SpriteFont font;

        private List<Shape> attachCandidates;

        public List<Shape> AttachCandidates => attachCandidates;

        public List<AttachableTile> AttachedTiles { get; }

        public bool IsAttached { get; private set; }

        private struct PlayerSurroundCheck
        {
            public EDirection dir;
            public HitResult hit;
        }

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, SpriteFont font)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            this.font = font;
            AttachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
        }

        public void Attach()
        {
            if (attachCandidates.Count == 0) return;

            foreach (AttachableTile tile in attachCandidates.SelectMany(shape =>  shape.AttachableTiles))
            {
                AttachedTiles.Add(tile);
                //tile.EffectParameters.DiffuseColor = Color.Green;
            }

            IsAttached = true;
        }

        public void Detach()
        {
            foreach (AttachableTile tile in AttachedTiles)
            {
                //tile.EffectParameters.DiffuseColor = Color.White;
            }

            AttachedTiles.Clear();
            IsAttached = false;
        }

        public bool CheckWinCondition()
        {
            Raycaster.HitResult hit = Raycaster.Raycast(this, Transform3D.Translation, Vector3.Up, true, 0.5f);
            System.Diagnostics.Debug.WriteLine("YOU WIN!!!");
            return hit != null && hit.actor is GoalTile;
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
                    CheckWinCondition(); //remove this later
                 }
             }
        }


        public void UpdateAttachCandidates()
        {
            attachCandidates.Clear();

            foreach (PlayerSurroundCheck check in CheckSurroundings())
                if (check.hit?.actor is AttachableTile tile)
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
            CubePlayer cubePlayer = new CubePlayer("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, font);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    cubePlayer.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return cubePlayer;
        }
    }
}