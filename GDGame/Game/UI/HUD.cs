using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class HUD : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture2D;
        private SpriteFont spriteFont;
        private BlendState blendState;

        public HUD(Microsoft.Xna.Framework.Game game, Texture2D texture2D, SpriteFont spriteFont,
            SpriteBatch spriteBatch) : base(game)
        {
            this.texture2D = texture2D;
            this.spriteFont = spriteFont;
            this.spriteBatch = spriteBatch;

            blendState = new BlendState();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, blendState);

            int height = 40;
            Point location = new Point(0, GameConstants.screenHeight - height);
            Point size = new Point(GameConstants.screenWidth, height);

            DrawTexture(location, size);

            height = 90;
            int width = 100;
            int screenWidth = (GameConstants.screenWidth - width) / 2;
            location = new Point(screenWidth, GameConstants.screenHeight - height);
            size = new Point(width, height);

            DrawTexture(location, size);

            //spriteBatch.DrawString(spriteFont, "Test Level", Vector2.Zero, Color.Black);

            StringBuilder stringBuilder = new StringBuilder("Moves");
            Vector2 position = new Vector2(screenWidth, GameConstants.screenHeight - height);
            spriteBatch.DrawString(spriteFont, stringBuilder, position, Color.Red);

            stringBuilder = new StringBuilder("5");
            position = new Vector2(screenWidth + 45, GameConstants.screenHeight - height + 26);
            spriteBatch.DrawString(spriteFont, stringBuilder, position, Color.Red);
            
            stringBuilder = new StringBuilder("Current Level");
            position = new Vector2(x: screenWidth / 4f, GameConstants.screenHeight - height / 2);
            spriteBatch.DrawString(spriteFont, stringBuilder, position, Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTexture(Point location, Point size)
        {
            Rectangle rectangle = new Rectangle(location, size);
            spriteBatch.Draw(texture2D, rectangle, Color.White);
        }
    }
}