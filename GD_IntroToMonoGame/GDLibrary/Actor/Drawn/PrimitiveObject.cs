using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using SharpDX;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GDLibrary
{
    public class PrimitiveObject : DrawnActor3D
    {
        #region Fields
        private IVertexData vertexData;
        #endregion

        #region Properties
        public IVertexData IVertexData
        {
            get
            {
                return this.vertexData;
            }
        }
        #endregion

        #region Constructors
        public PrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D, 
            EffectParameters effectParameters, IVertexData vertexData, RasterizerState rasterizerState = null) 
                        : base(id, actorType, statusType, transform3D, effectParameters,rasterizerState)
        {
            this.vertexData = vertexData;
        }
        #endregion

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            base.Draw(gameTime,camera,graphicsDevice);
            this.EffectParameters.Draw(this.Transform3D.World, camera);
            this.IVertexData.Draw(gameTime, this.EffectParameters.Effect, graphicsDevice);
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

        public new object Clone()
        {
            return new PrimitiveObject(this.ID, this.ActorType, this.StatusType, this.Transform3D.Clone() as Transform3D,
                this.EffectParameters.Clone() as EffectParameters, this.vertexData.Clone()
                as IVertexData);
        }

        public override bool Equals(object obj)
        {
            return obj is PrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<IVertexData>.Default.Equals(vertexData, @object.vertexData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), vertexData);
        }
    }
}
