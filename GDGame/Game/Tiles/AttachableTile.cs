using System;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class AttachableTile : GridTile, ICloneable
    {
        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 offset;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;

        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform,
            effectParameters, model)
        {
        }

        public void Move(Vector3 direction, Vector3 rotatePoint)
        {
            //Get the end position and end rotation to apply
            QueryMove(direction, rotatePoint, out Vector3 translation, out Quaternion rotation);

            //Start and End Rotation --> Will be lerped between
            startRotQ = Transform3D.Rotation;
            endRotQ = rotation * startRotQ;

            //Start and End Position --> Will be lerped between
            startPos = Transform3D.Translation;
            endPos = Transform3D.Translation + translation;

            //Set animation time and movement flag
            currentMovementTime = movementTime;
            isMoving = true;
        }

        public void QueryMove(Vector3 direction, Vector3 rotatePoint, out Vector3 endPos, out Quaternion rotation)
        {
            Vector3 offset = Transform3D.Translation - rotatePoint; //offset between the player and the point to rotate around
            rotation =
                Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up),
                    MathHelper.ToRadians(-90)); //The rotation to apply
            endPos = Vector3.Transform(offset, rotation); //Rotate around the offset point
            endPos -= offset;
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

                Quaternion rot = Quaternion.Lerp(startRotQ, endRotQ, 1 - currentMovementTime / movementTime);
                Vector3 trans = Vector3.Lerp(startPos, endPos, 1 - currentMovementTime / movementTime);

                Transform3D.Rotation = rot;
                Transform3D.Translation = trans;
                currentMovementTime -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public new object Clone()
        {
            AttachableTile attachableTile = new AttachableTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    attachableTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return attachableTile;
        }
    }
}