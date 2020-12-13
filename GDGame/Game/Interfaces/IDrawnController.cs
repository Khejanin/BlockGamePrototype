using GDLibrary.Actors;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Interfaces
{
    public interface IDrawnController : IController
    {
        #region Public Method

        public void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);

        #endregion
    }
}