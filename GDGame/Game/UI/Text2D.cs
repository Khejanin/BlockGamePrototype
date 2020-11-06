using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.UI
{
    public class Text2D
    {
        private string text;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Vector2 position;
        private Color color;

        public Text2D(string text, SpriteFont spriteFont, Vector2 position,Color color)
        {
            this.text = text;
            this.spriteFont = spriteFont;
            this.position = position;
            this.color = color;
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            spriteBatch ??= new SpriteBatch(graphicsDevice);
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, text, position, color);
            spriteBatch.End();
            
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}