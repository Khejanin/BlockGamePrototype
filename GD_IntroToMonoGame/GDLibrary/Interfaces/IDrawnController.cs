using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public interface IDrawnController : IController
    {
        public void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }
}