using System;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public abstract class UiElement : IDisposable
    {
        #region 06. Constructors

        protected UiElement(StatusType statusType)
        {
            StatusType = statusType;
        }

        #endregion

        #region 07. Properties, Indexers

        public StatusType StatusType { get; set; }

        #endregion

        #region 11. Methods

        public void Dispose()
        {
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);

        #endregion
    }
}