using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class AttachableTile : GridTile
    {
        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;

        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public void Move(Vector3 direction, Vector3 rotatePoint/*, Vector3 playerPos*/)
        {
            //Vector3 trans = direction * (Transform3D.Translation - rotatePoint).Length();
            rotatePoint -= Transform3D.Translation;
            startRotQ = Transform3D.Rotation;
            endRotQ = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, -Vector3.Up), MathHelper.ToRadians(90)) * startRotQ;

            rotatePoint = Vector3.Transform(-rotatePoint, endRotQ * Quaternion.Inverse(startRotQ));
            startPos = Transform3D.Translation;
            endPos = Transform3D.Translation /*+ trans*/ + direction + rotatePoint;

            currentMovementTime = movementTime;
            isMoving = true;
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
                    //UpdateAttachCandidates(); //remove this later
                }

                Quaternion rot = Quaternion.Lerp(startRotQ, endRotQ, 1 - currentMovementTime / movementTime);
                Vector3 trans = Vector3.Lerp(startPos, endPos, 1 - currentMovementTime / movementTime);

                Transform3D.Rotation = rot;
                SetPosition(trans);
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
