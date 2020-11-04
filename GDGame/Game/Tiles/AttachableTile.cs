﻿using System;
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
            Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up),
                    MathHelper.ToRadians(-90));

            //Start and End Rotation --> Will be lerped between
            startRotQ = Transform3D.Rotation;
            endRotQ = rotation * startRotQ;

            //Start and End Position --> Will be lerped between
            startPos = Transform3D.Translation;
            endPos = CalculateTargetPosition(rotatePoint, rotation);

            //Set animation time and movement flag
            currentMovementTime = movementTime;
            isMoving = true;
        }

        public Vector3 CalculateTargetPosition(Vector3 rotatePoint, Quaternion rotationToApply)
        {
            Vector3 offset = Transform3D.Translation - rotatePoint; //offset between the player and the point to rotate around
            Vector3 endPos = Vector3.Transform(offset, rotationToApply); //Rotate around the offset point
            endPos += Transform3D.Translation - offset;
            return endPos;
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