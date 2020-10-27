using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public interface IActor : ICloneable
    {
        void Initialize();
        void Update(GameTime gameTime);

     //   string GetID();
     //   float GetAlpha();
    }
}
