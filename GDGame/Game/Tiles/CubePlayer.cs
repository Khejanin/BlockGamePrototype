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
            AttachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
            LeftRotatePoint = RightRotatePoint = ForwardRotatePoint = BackwardRotatePoint = transform.Translation;
        }

        public bool IsMoving => isMoving;
        public List<AttachableTile> AttachedTiles { get => attachedTiles; private set => attachedTiles = value; }
        public Vector3 LeftRotatePoint { get => leftRotatePoint; private set => leftRotatePoint = value; }
        public Vector3 RightRotatePoint { get => rightRotatePoint; private set => rightRotatePoint = value; }
        public Vector3 ForwardRotatePoint { get => forwardRotatePoint; private set => forwardRotatePoint = value; }
        public Vector3 BackwardRotatePoint { get => backwardRotatePoint; private set => backwardRotatePoint = value; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Attach()
        {
            foreach (Shape shape in attachCandidates)
                foreach (AttachableTile tile in shape.AttachableTiles)
                    AttachedTiles.Add(tile);

        }

        public void Detach()
        {
            AttachedTiles.Clear();
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
                    offset = RightRotatePoint - Transform3D.Translation;
                else if (direction == -Vector3.UnitX)
                    offset = LeftRotatePoint - Transform3D.Translation;
                else if (direction == -Vector3.UnitZ)
                    offset = ForwardRotatePoint - Transform3D.Translation;
                else if (direction == Vector3.UnitZ)
                    offset = BackwardRotatePoint - Transform3D.Translation;
                else
                    throw new System.ArgumentException("Invalid direction!");

                startRotQ = Transform3D.Rotation;
                endRotQ = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, -Vector3.Up), MathHelper.ToRadians(90)) * startRotQ;

                offset = Vector3.Transform(-offset, endRotQ * Quaternion.Inverse(startRotQ));
                startPos = Transform3D.Translation;
                endPos = Transform3D.Translation + direction + offset;

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
                SetPosition(trans);
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void SetPosition(Vector3 position)
        {
            base.SetPosition(position);
        }

        public void UpdateRotatePoints()
        {
            LeftRotatePoint = RightRotatePoint = ForwardRotatePoint = BackwardRotatePoint = Transform3D.Translation;

            foreach (AttachableTile tile in AttachedTiles)
            {
                Vector3 playerPos = Transform3D.Translation;
                Vector3 tilePos = tile.Transform3D.Translation;

                //Update right rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X > RightRotatePoint.X || tilePos.Y < RightRotatePoint.Y)
                    RightRotatePoint = tilePos;

                //Update left rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X < LeftRotatePoint.X || tilePos.Y < LeftRotatePoint.Y)
                    LeftRotatePoint = tilePos;

                //Update forward rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z < ForwardRotatePoint.Z || tilePos.Y < ForwardRotatePoint.Y)
                    ForwardRotatePoint = tilePos;

                //Update back rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z > ForwardRotatePoint.Z || tilePos.Y < BackwardRotatePoint.Y)
                    BackwardRotatePoint = tilePos;
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
