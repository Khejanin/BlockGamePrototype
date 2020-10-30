using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class CubePlayer : GridTile
    {
        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 startPos;
        private Vector3 endPos;
        private Vector3 startRot;
        private Vector3 endRot;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {

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
                //transform3D.RotateBy(direction * 90, false);
                startPos = transform3D.Translation;
                endPos = transform3D.Translation + direction;
                startRot = transform3D.RotationInDegrees;
                endRot = /*startRot +*/ Vector3.Cross(direction, -Vector3.Up) * 90;
                startRotQ = transform3D.Rotation;
                endRotQ = startRotQ * Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, -Vector3.Up), MathHelper.ToRadians(90));

                transform3D.Rotate(endRot);
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

                //transform3D.RotationInDegrees = Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime);
                //transform3D.Rotate()
                //transform3D.Rotation = Quaternion.Lerp(this.startRotQ, this.endRotQ, 1 - currentMovementTime / movementTime);
                //transform3D.RotationAsMatrix = Matrix.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime);
                //transform3D.RotateBy(Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime));
                transform3D.Translation = Vector3.Lerp(this.startPos, this.endPos, 1 - currentMovementTime / movementTime);
                //transform3D.RotateBy(Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime));
                //Vector3 rot = endRot * (1f - currentMovementTime / movementTime);
                //transform3D.Rotate(Matrix.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z));
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
