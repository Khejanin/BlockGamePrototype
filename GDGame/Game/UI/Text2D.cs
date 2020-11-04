using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class Text2D
    {
        private string text;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public Text2D(string text, SpriteFont spriteFont)
        {
            this.text = text;
            this.spriteFont = spriteFont;
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            spriteBatch ??= new SpriteBatch(graphicsDevice);
            spriteBatch.Begin();
            Vector2 position = new Vector2(0, 0);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);
            spriteBatch.End();
            
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}