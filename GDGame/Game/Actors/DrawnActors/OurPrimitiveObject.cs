using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Actors;

namespace GDGame.Actors
{
    /// <summary>
    /// Base class for all drawn 3D draw primtive objects used in the engine. This class adds an IVertexData field.
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject"/>
    public class OurPrimitiveObject : OurDrawnActor3D
    {
        #region Fields

        private IVertexData vertexData;

        #endregion Fields

        #region Properties

        public IVertexData IVertexData
        {
            get { return vertexData; }
        }

        #endregion Properties

        #region Constructors

        public OurPrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
           OurEffectParameters effectParameters, IVertexData vertexData)
            : base(id, actorType, statusType, transform3D, effectParameters)
        {
            this.vertexData = vertexData;
        }

        #endregion Constructors

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            EffectParameters.DrawPrimitive(Transform3D.World, camera,gameTime);
            //This draw method doesnt use effect
            IVertexData.Draw(gameTime, null, graphicsDevice);
        }

        public new object Clone()
        {
            return new OurPrimitiveObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, vertexData.Clone()
                    as IVertexData);
        }

        public override bool Equals(object obj)
        {
            return obj is OurPrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<IVertexData>.Default.Equals(vertexData, @object.vertexData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), vertexData);
        }

        public BoundingBox GetDrawnBoundingBox()
        {
            List<Vector3> transformedPositions = new List<Vector3>();

            foreach (var primitivePosition in IVertexData.GetPrimitivePositions())
            {
                transformedPositions.Add(Vector3.Transform(primitivePosition, Transform3D.World));
            }

            return BoundingBox.CreateFromPoints(transformedPositions);
        }
    }
}