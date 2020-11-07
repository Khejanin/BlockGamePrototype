using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class UiText : UiElement
    {
        private string text;
        private SpriteFont spriteFont;
        private Vector2 position;
        private Color color;
        private Vector2 origin;

        public UiText(StatusType statusType, string text, SpriteFont spriteFont, Vector2 position, Color color,
            bool originAtCenter = true) :
            base(statusType)
        {
            this.text = text;
            this.spriteFont = spriteFont;
            this.position = position;
            this.color = color;
            if (originAtCenter)
            {
                origin = spriteFont.MeasureString(text) / 2;
            }
            else
            {
                origin = Vector2.Zero;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, text, position, color, 0, origin, Vector2.One, SpriteEffects.None, 0);
        }
    }
}