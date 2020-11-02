﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private Vector3 rotPoint;
        private Vector3 offset;

        private Vector3 leftRotatePoint;
        private Vector3 rightRotatePoint;
        private Vector3 forwardRotatePoint;
        private Vector3 backwardRotatePoint;

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            leftRotatePoint = rightRotatePoint = forwardRotatePoint = backwardRotatePoint = transform.Translation;
            rightRotatePoint = transform.Translation /*+ Vector3.UnitX*/;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Move(Vector3 direction)
        {
            if (!isMoving)
            {
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

                transform3D.Rotation = rot;
                SetPosition(trans);
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void SetPosition(Vector3 position)
        {
            Vector3 difference = position - transform3D.Translation;
            leftRotatePoint += difference;
            rightRotatePoint += difference;
            forwardRotatePoint += difference;
            backwardRotatePoint += difference;
            base.SetPosition(position);
        }
    }
}
