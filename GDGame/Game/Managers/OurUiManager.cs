using GDLibrary.Enums;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    public class OurUiManager : UIManager
    {
        #region Constructors

        public OurUiManager(Game game, StatusType statusType, SpriteBatch spriteBatch, int initialDrawSize) : base(game, statusType, spriteBatch, initialDrawSize)
        {
        }

        #endregion

        #region Override Methode

        protected override void ApplyDraw(GameTime gameTime)
        {
            base.ApplyDraw(gameTime);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        #endregion
    }
}