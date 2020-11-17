using System;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public abstract class UiElement : IDisposable
    {
        public StatusType StatusType { get; set; }


        protected UiElement(StatusType statusType)
        {
            StatusType = statusType;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);

        public void Dispose()
        {
        }
    }
}