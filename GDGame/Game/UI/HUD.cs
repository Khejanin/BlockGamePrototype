using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class Hud : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture2D;

        private SpriteFont spriteFont;

        public Hud(Microsoft.Xna.Framework.Game game, Texture2D texture2D, SpriteFont spriteFont,
            SpriteBatch spriteBatch) : base(game)
        {
            this.texture2D = texture2D;
            this.spriteFont = spriteFont;
            this.spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            int height = 50;
            Point location = new Point(0, GameConstants.screenHeight - height);
            Point size = new Point(GameConstants.screenWidth, height);

            DrawTexture(location, size);

            height = 90;
            int width = 100;
            int screenWidth = (GameConstants.screenWidth - width) / 2;
            location = new Point(screenWidth, GameConstants.screenHeight - height);
            size = new Point(width, height);

            DrawTexture(location, size);

            float halfWidth = GraphicsDevice.Viewport.Width / 2f;
            float screenHeight = GraphicsDevice.Viewport.Height;

            float heightFromBottom = 45;

            string text = "moves";
            Vector2 position = new Vector2(halfWidth - (text.Length - 1) * 12, screenHeight - heightFromBottom * 2);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "5";
            position = new Vector2(halfWidth - text.Length * 12, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "Current Level";
            position = new Vector2(x: halfWidth / 4f, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "Time : 00:00:00";
            position = new Vector2(x: halfWidth + halfWidth / 4f, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }

        private void DrawTexture(Point location, Point size)
        {
            Rectangle rectangle = new Rectangle(location, size);
            spriteBatch.Draw(texture2D, rectangle, Color.White);
        }
    }
}