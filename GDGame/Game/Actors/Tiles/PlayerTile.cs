using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static GDGame.Utilities.Raycaster;

namespace GDGame.Actors
{
    /// <summary>
    /// This Tile represents the player
    /// </summary>
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
            IsAlive = true;
        }

        #endregion

        #region Properties, Indexers

        public List<Shape> AttachCandidates { get; }

        public List<AttachableTile> AttachedTiles { get; }
        public bool IsAlive { get; private set; }

        #endregion

        #region Initialization

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            EventManager.FireEvent(new SoundEventInfo
                {soundEventType = SoundEventType.SetListener, listenerTransform = Transform3D});
            lastCheckpoint = Transform3D.Translation;
            base.InitializeTile();
        }

        #endregion

        #region Override Methode

        public override void Respawn()
        {
            IsAlive = false;
            Die(RespawnAtLastCheckpoint);
        }

        public override void OnMoveEnd()
        {
            CheckAndProcessSurroundings(GetSurroundings(Transform3D.Translation));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to attach to the Attachables around the Player.
        /// </summary>
        public void Attach()
        {
            if (AttachCandidates.Count == 0 || IsMoving) return;

            AttachedTiles.Clear();
            foreach (AttachableTile tile in AttachCandidates.SelectMany(shape => shape.AttachableTiles))
            { 
                AttachedTiles.Add(tile);
                tile.IsAttached = true;
            }

            IsAttached = true;
            EventManager.FireEvent(new SoundEventInfo
                {soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.PlayerAttach, listenerTransform = Transform3D});
        }

        /// <summary>
        /// Method that performs raycasts to check what's around the player.
        /// </summary>
        /// <param name="surroundings"></param>
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

        public new object Clone()
        {
            PlayerTile playerTile = new PlayerTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model, TileType);

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

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck
                {hit = RaycastManager.Instance.Raycast(this, translation, Vector3.Right, true, 1f)};
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

            return result;
        }

        private void RespawnAtLastCheckpoint()
        {
            SetTranslation(lastCheckpoint);
            Transform3D.RotationInDegrees = Vector3.Zero;
            IsAlive = true;
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

        private void UpdateAttachCandidates(IEnumerable<MovableTile> detectedAttachableTiles)
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

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            switch (info.type)
            {
                case PlayerEventType.SetCheckpoint:
                    SetCheckpoint(info.position);
                    break;
                case PlayerEventType.MovableTileDie:
                    AttachedTiles.Remove(info.movableTile);
                    info.movableTile.Respawn();
                    break;
                case PlayerEventType.PickupMug:

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