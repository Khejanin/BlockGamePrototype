using System;
using System.Collections.Generic;
using GDGame.Game.Enums;
using GDGame.Game.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        struct PlayerSurroundCheck
        {
            public EDirection dir;
            public List<Raycaster.HitResult> hit;
        }

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            leftRotatePoint = rightRotatePoint = forwardRotatePoint = backwardRotatePoint = transform.Translation;
        }

        public bool IsMoving => isMoving;

        public override void Initialize()
        {
            base.Initialize();
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
                //transform3D.parent.Translation += direction;
                if (direction == Vector3.UnitX)
                    offset = rightRotatePoint - Transform3D.Translation;
                else if (direction == -Vector3.UnitX)
                    offset = leftRotatePoint - Transform3D.Translation;
                else if (direction == -Vector3.UnitZ)
                    offset = forwardRotatePoint - Transform3D.Translation;
                else if (direction == Vector3.UnitZ)
                    offset = backwardRotatePoint - Transform3D.Translation;
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
                }

                Quaternion rot = Quaternion.Lerp(this.startRotQ, this.endRotQ, 1 - currentMovementTime / movementTime);
                Vector3 trans = Vector3.Lerp(this.startPos, this.endPos, 1 - currentMovementTime / movementTime);

                Transform3D.Rotation = rot;
                SetPosition(trans);
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void SetPosition(Vector3 position)
        {
            Vector3 difference = position - Transform3D.Translation;
            leftRotatePoint += difference;
            rightRotatePoint += difference;
            forwardRotatePoint += difference;
            backwardRotatePoint += difference;
            base.SetPosition(position);
        }

        private void UpdateRotatePoints()
        {
            foreach (PlayerSurroundCheck check in CheckSurroundings())
            {
                foreach (Raycaster.HitResult hit in check.hit)
                {
                    
                }

                switch(check.dir)
                {
                    case EDirection.Right:
                        break;
                    case EDirection.Left:
                        break;
                    case EDirection.Forward:
                        break;
                    case EDirection.Back:
                        break;
                }
            }
        }

        private List<PlayerSurroundCheck> CheckSurroundings()
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.RaycastAll(Transform3D.Translation, Vector3.Right, true);
            surroundCheck.dir = EDirection.Right;
            result.Add(surroundCheck);

            surroundCheck.hit = this.RaycastAll(Transform3D.Translation, -Vector3.Right, true);
            surroundCheck.dir = EDirection.Left;
            result.Add(surroundCheck);

            surroundCheck.hit = this.RaycastAll(Transform3D.Translation, Vector3.Forward, true);
            surroundCheck.dir = EDirection.Forward;
            result.Add(surroundCheck);

            surroundCheck.hit = this.RaycastAll(Transform3D.Translation, -Vector3.Forward, true);
            surroundCheck.dir = EDirection.Back;
            result.Add(surroundCheck);

            return result;
        }
    }
}
