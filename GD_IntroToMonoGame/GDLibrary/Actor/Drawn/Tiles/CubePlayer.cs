using System;
using System.Xml;
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
        private float oldPitch;
        private float totalPitch;
        private float endPitch;
        private float oldRoll;
        private float totalRoll;
        private float endRoll;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private Transform3D drawTransform;
        private Transform3D localTransform;
        private bool isMoving;

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            drawTransform = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One,Vector3.Forward, Vector3.Up);
            localTransform = new Transform3D(transform.Translation,transform.Look,transform.Up);
            transform.SetParent(ref localTransform);
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
                
                if (direction.X != 0.0f)
                {
                    endPitch = totalPitch + direction.X * 90f;
                    //endPitch = Math.Clamp(endPitch, 0f,360f);
                }
                else if (direction.Z != 0.0f)
                {
                    endRoll = totalRoll + direction.Z * 90f;
                    //endRoll = Math.Clamp(endRoll, 0f,360f);
                }

                currentMovementTime = movementTime;
                isMoving = true;
            }
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            base.Draw(gameTime, camera, graphicsDevice);

            BasicEffect custom = new BasicEffect(graphicsDevice);
            custom.TextureEnabled = false;
            custom.Alpha = 1;
            custom.VertexColorEnabled = true;

            EffectParameters effectParameters = new EffectParameters(custom,
                null, Color.White, 1);

            effectParameters.Draw(drawTransform.World,camera);

            VertexPositionColor[] vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(transform3D.Translation,Color.Green);
            vertices[1] = new VertexPositionColor(transform3D.Translation + transform3D.World.Up,Color.Green);
            vertices[2] = new VertexPositionColor(transform3D.Translation,Color.Red);
            vertices[3] = new VertexPositionColor(transform3D.Translation + transform3D.World.Right,Color.Red);
            vertices[4] = new VertexPositionColor(transform3D.Translation,Color.Blue);
            vertices[5] = new VertexPositionColor(transform3D.Translation - transform3D.World.Forward,Color.Blue);
            
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,vertices,0,3);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isMoving)
            {
                if (currentMovementTime <= 0)
                {
                    oldRoll = totalRoll;
                    oldPitch = totalPitch;
                    isMoving = false;
                    currentMovementTime = 0;
                }

                //transform3D.RotationInDegrees = Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime);
                //transform3D.Rotate()
                totalPitch = MathHelper.ToRadians(MathHelper.Lerp(oldPitch, endPitch, 1 - currentMovementTime / movementTime));
                totalRoll = MathHelper.ToRadians(MathHelper.Lerp(oldRoll, endRoll, 1 - currentMovementTime / movementTime));
                localTransform.Rotation = Quaternion.CreateFromYawPitchRoll(0,totalPitch,0);
                transform3D.Rotation = Quaternion.CreateFromYawPitchRoll(0,0,totalRoll);
                //transform3D.RotationAsMatrix = Matrix.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime);
                //transform3D.RotateBy(Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime));
                //transform3D.Translation = Vector3.Lerp(this.startPos, this.endPos, 1 - currentMovementTime / movementTime);
                //transform3D.RotateBy(Vector3.Lerp(this.startRot, this.endRot, 1 - currentMovementTime / movementTime));
                //Vector3 rot = endRot * (1f - currentMovementTime / movementTime);
                //transform3D.Rotate(Matrix.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z));
                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
    
}
