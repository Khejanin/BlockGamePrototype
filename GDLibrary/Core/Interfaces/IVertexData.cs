using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Parent interface for all vertex data objects
    /// </summary>
    /// <see cref="GDLibrary.Actors.PrimitiveObject"/>
    public interface IVertexData : ICloneable
    {
        void Draw(GameTime gameTime, BasicEffect effect, GraphicsDevice graphicsDevice);

        //getters
        PrimitiveType GetPrimitiveType();

        int GetPrimitiveCount();

        //setters
        void SetPrimitiveType(PrimitiveType primitiveType);

        void SetPrimitiveCount(int primitiveCount);
        Vector3[] GetPrimitivePositions();
    }
}