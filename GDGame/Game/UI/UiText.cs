using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class UiText : UiElement
    {
        #region 05. Private variables

        private Color color;
        private bool isDirty = true;
        private Vector2 origin;
        private bool originAtCenter;
        private Vector2 position;

        private SpriteFont spriteFont;
        private string text;

        #endregion

        #region 06. Constructors

        public UiText(StatusType statusType, string text, SpriteFont spriteFont, Vector2 position, Color color,
            bool originAtCenter = true) :
            base(statusType)
        {
            this.text = text;
            this.spriteFont = spriteFont;
            this.position = position;
            this.color = color;
            this.originAtCenter = originAtCenter;
            if (originAtCenter)
                origin = spriteFont.MeasureString(text) / 2;
            else
                origin = Vector2.Zero;
        }

        #endregion

        #region 07. Properties, Indexers

        public string Text
        {
            set
            {
                text = value;
                isDirty = true;
            }
        }

        #endregion

        #region 09. Override Methode

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (originAtCenter && isDirty) origin = spriteFont.MeasureString(text) / 2;

            spriteBatch.DrawString(spriteFont, text, position, color, 0, origin, Vector2.One, SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
        }

        #endregion
    }
}