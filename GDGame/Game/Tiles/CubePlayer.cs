using GDGame.Game.Enums;
using GDGame.Game.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDGame.Game.Tiles
{
    public class CubePlayer : GridTile
    {
        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;
        private bool isAttached;

        private Vector3 leftRotatePoint;
        private Vector3 rightRotatePoint;
        private Vector3 forwardRotatePoint;
        private Vector3 backwardRotatePoint;

        List<AttachableTile> attachedTiles;
        List<Shape> attachCandidates;

        struct PlayerSurroundCheck
        {
            public EDirection dir;
            public Raycaster.HitResult hit;
        }

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            attachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
            leftRotatePoint = rightRotatePoint = forwardRotatePoint = backwardRotatePoint = transform.Translation;
        }

        public bool IsMoving => isMoving;
        public bool IsAttached { get => isAttached; set => isAttached = value; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Attach()
        {
            if (attachCandidates.Count == 0) return;

            foreach (Shape shape in attachCandidates)
                foreach (AttachableTile tile in shape.AttachableTiles)
                    attachedTiles.Add(tile);

            isAttached = true;
        }

        public void Detach()
        {
            attachedTiles.Clear();
            isAttached = false;
        }

        /// <summary>
        /// This method will change the Player's state to moving if he's not already moving and calculates how the player will move.
        /// </summary>
        /// <param name="direction"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Move(Vector3 direction)
        {
            if (!isMoving)
            {
                UpdateRotatePoints();

                Vector3 rotatePoint;

                if (direction == Vector3.UnitX)
                    rotatePoint = rightRotatePoint;
                else if (direction == -Vector3.UnitX)
                    rotatePoint = leftRotatePoint;
                else if (direction == -Vector3.UnitZ)
                    rotatePoint = forwardRotatePoint;
                else if (direction == Vector3.UnitZ)
                    rotatePoint = backwardRotatePoint;
                else
                    throw new System.ArgumentException("Invalid direction!");

                //Calculate movement for each attached tile
                foreach(AttachableTile tile in attachedTiles)
                    tile.Move(direction, rotatePoint);

                Vector3 offset = Transform3D.Translation - rotatePoint; //offset between the player and the point to rotate around
                Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));   //The rotation to apply
                Vector3 translation = Vector3.Transform(offset, rot);   //Rotate around the offset point

                //Start and End Rotation --> Will be lerped between
                startRotQ = Transform3D.Rotation;
                endRotQ = rot * startRotQ;

                //Start and End Position --> Will be lerped between
                startPos = Transform3D.Translation;
                endPos = Transform3D.Translation + translation - offset;

                //Set animation time and movement flag
                currentMovementTime = movementTime;
                isMoving = true;
            }
        }

        public void OnMove()
        {
            UpdateAttachCandidates();
        }

        /// <summary>
        /// Is in charge of the Animation for when the Player Moves
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isMoving)
            {
                if (currentMovementTime <= 0)
                {
                    isMoving = false;
                    currentMovementTime = 0;
                    UpdateAttachCandidates(); //remove this later
                }

                Quaternion rot = Quaternion.Lerp(startRotQ, endRotQ, 1 - currentMovementTime / movementTime);
                Vector3 trans = Vector3.Lerp(startPos, endPos, 1 - currentMovementTime / movementTime);
                Transform3D.Rotation = rot;
                Transform3D.Translation = trans;

                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void UpdateRotatePoints()
        {
            //Set all rotation points to the edges of the player cube
            rightRotatePoint = Transform3D.Translation + new Vector3(.5f, -.5f, 0);
            leftRotatePoint = Transform3D.Translation + new Vector3(-.5f, -.5f, 0);
            forwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, -.5f);
            backwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, .5f);

            //Loops through attached tiles to update the rotation points
            foreach (AttachableTile tile in attachedTiles)
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

        public void UpdateAttachCandidates()
        {
            attachCandidates.Clear();

            foreach (PlayerSurroundCheck check in CheckSurroundings())
            {
                if (check.hit == null) continue;
                if (check.hit.actor is AttachableTile)
                    attachCandidates.Add((check.hit.actor as AttachableTile).Shape);
            }
        }

        private List<PlayerSurroundCheck> CheckSurroundings()
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Right, true);
            surroundCheck.dir = EDirection.Right;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Right, true);
            surroundCheck.dir = EDirection.Left;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Forward, true);
            surroundCheck.dir = EDirection.Forward;
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Forward, true);
            surroundCheck.dir = EDirection.Back;
            result.Add(surroundCheck);

            return result;
        }
    }
}
