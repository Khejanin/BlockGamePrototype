using System;
using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     Base class for all drawn 3D draw primtive objects used in the engine. This class adds an IVertexData field.
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject" />
    public class OurPrimitiveObject : OurDrawnActor3D
    {
        #region Private variables

        #endregion

        #region Constructors

        public OurPrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            OurEffectParameters effectParameters, IVertexData vertexData)
            : base(id, actorType, statusType, transform3D, effectParameters)
        {
            this.IVertexData = vertexData;
        }

        #endregion

        #region Properties, Indexers

        public IVertexData IVertexData { get; }

        #endregion

        #region Override Methode

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            EffectParameters.DrawPrimitive(Transform3D.World, camera, gameTime);
            //This draw method doesnt use effect
            IVertexData.Draw(gameTime, null, graphicsDevice);
        }

        public override bool Equals(object obj)
        {
            return obj is OurPrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<IVertexData>.Default.Equals(IVertexData, @object.IVertexData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), IVertexData);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new OurPrimitiveObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, IVertexData.Clone()
                    as IVertexData);
        }

        public BoundingBox GetDrawnBoundingBox()
        {
            List<Vector3> transformedPositions = new List<Vector3>();

            foreach (Vector3 primitivePosition in IVertexData.GetPrimitivePositions()) transformedPositions.Add(Vector3.Transform(primitivePosition, Transform3D.World));

            return BoundingBox.CreateFromPoints(transformedPositions);
        }

        #endregion
    }
}