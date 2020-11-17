using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GDGame.Game.UI
{
    class UiQuickOptions : UiElement
    {
        private Texture2D background;
        private SpriteFont font;
        private Color textColour;
        Vector2 position;
        string text;

        public UiQuickOptions(StatusType statusType, Vector2 position, string text, Texture2D background, SpriteFont font) : base(statusType)
        {
            this.background = background;
            this.font = font;
            textColour = Color.White;
            this.position = position;
            this.text = text;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            spriteBatch.Draw(background, Box, colour);

            if (!string.IsNullOrEmpty(text))
            {
                var x = Box.X + (Box.Width / 2) - (font.MeasureString(text).X / 2);
                var y = Box.Y + (Box.Height / 2) - (font.MeasureString(text).Y / 2);

                spriteBatch.DrawString(font, text, new Vector2(x, y), textColour);
            }
        }

        public override void Update(GameTime gameTime) { }

        public Rectangle Box
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, background.Width, background.Height);
            }
        }
    }
}
