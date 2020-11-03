using GD_Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;

namespace GDLibrary
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
        private Vector3 offset;

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

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Attach()
        {
            foreach(Shape shape in attachCandidates)
                foreach(AttachableTile tile in shape.AttachableTiles)
                    attachedTiles.Add(tile);

        }
        
        public void Detach()
        {
            attachedTiles.Clear();
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

                //transform3D.parent.Translation += direction;
                if (direction == Vector3.UnitX)
                    offset = rightRotatePoint - transform3D.Translation;
                else if (direction == -Vector3.UnitX)
                    offset = leftRotatePoint - transform3D.Translation;
                else if (direction == -Vector3.UnitZ)
                    offset = forwardRotatePoint - transform3D.Translation;
                else if (direction == Vector3.UnitZ)
                    offset = backwardRotatePoint - transform3D.Translation;
                else
                    throw new System.ArgumentException("Invalid direction!");

                startRotQ = transform3D.Rotation;
                endRotQ = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, -Vector3.Up), MathHelper.ToRadians(90)) * startRotQ;

                offset = Vector3.Transform(-offset, endRotQ * Quaternion.Inverse(startRotQ));
                startPos = transform3D.Translation;
                endPos = transform3D.Translation + direction + offset;

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

                Quaternion rot = Quaternion.Lerp(this.startRotQ, this.endRotQ, 1 - currentMovementTime / movementTime);
                Vector3 trans = Vector3.Lerp(this.startPos, this.endPos, 1 - currentMovementTime / movementTime);

                transform3D.Rotation = rot;
                SetPosition(trans);
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void SetPosition(Vector3 position)
        {
            base.SetPosition(position);
        }

        private void UpdateRotatePoints()
        {
            this.leftRotatePoint = this.rightRotatePoint = this.forwardRotatePoint = this.backwardRotatePoint = transform3D.Translation;

            foreach(AttachableTile tile in attachedTiles)
            {
                Vector3 playerPos = transform3D.Translation;
                Vector3 tilePos = tile.Transform3D.Translation;

                //Update right rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X > rightRotatePoint.X || tilePos.Y < rightRotatePoint.Y)
                    rightRotatePoint = tilePos;

                //Update left rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X < leftRotatePoint.X || tilePos.Y < leftRotatePoint.Y)
                    leftRotatePoint = tilePos;

                //Update forward rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z < forwardRotatePoint.Z || tilePos.Y < forwardRotatePoint.Y)
                    forwardRotatePoint = tilePos;

                //Update back rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z > forwardRotatePoint.Z || tilePos.Y < backwardRotatePoint.Y)
                    backwardRotatePoint = tilePos;
            }
        }

        private void UpdateAttachCandidates()
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
