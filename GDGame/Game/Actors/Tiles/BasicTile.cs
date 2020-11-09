using System;
using System.Collections.Generic;
using GDGame.Tiles;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class BasicTile : ModelObject, ICloneable
    {
        public Shape Shape { get; set; }
        public bool CanMoveInto { get; set; }

        public List<Vector3> GetBoundsPoints()
        {
            List<Vector3> result = new List<Vector3>
            {
                Transform3D.Translation + Transform3D.Scale, Transform3D.Translation - Transform3D.Scale
            };
            return result;
        }
        
        public List<Vector3> GetBoundsPointsWithRotation()
        {
            List<Vector3> result = new List<Vector3>
            {
                Transform3D.Translation + Vector3.Transform(Transform3D.Scale, Transform3D.Rotation),
                Transform3D.Translation - Vector3.Transform(Transform3D.Scale, Transform3D.Rotation)
            };
            return result;
        } 

        public BasicTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public virtual void SetPosition(Vector3 position)
        {
            Transform3D.Translation = position;
        }

        public new object Clone()
        {
            BasicTile basicTile = new BasicTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    basicTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return basicTile;
        }
    }
}