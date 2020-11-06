using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class Text2D : UiElement
    {
        private string text;
        private SpriteFont spriteFont;
        private Vector2 position;
        private Color color;

        public Text2D(StatusType statusType, string text, SpriteFont spriteFont, Vector2 position, Color color) : base(statusType)
        {
            this.text = text;
            this.spriteFont = spriteFont;
            this.position = position;
            this.color = color;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, text, position, color);
        }
    }
}