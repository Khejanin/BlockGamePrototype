using System;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class MovableTile : BasicTile, ICloneable
    {
        
        public bool IsMoving { get; set; }
        public Vector3 RotatePoint { get; set; }
        public int CurrentMovementTime { get; set; }
        
        public MovableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform,
            effectParameters, model)
        {
        }

        public Vector3 CalculateTargetPosition(Vector3 rotatePoint, Quaternion rotationToApply)
        {
            //offset between the player and the point to rotate around
            Vector3 offset = Transform3D.Translation - rotatePoint;
            Vector3 targetPosition = Vector3.Transform(offset, rotationToApply); //Rotate around the offset point
            targetPosition += Transform3D.Translation - offset;
            return targetPosition;
        }

        public new object Clone()
        {
            MovableTile movableTile = new MovableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    movableTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return movableTile;
        }
    }
}