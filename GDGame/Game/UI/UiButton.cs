using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.UI
{
    public delegate void EventHandler();

    public class UiButton : UiElement
    {
        #region 05. Private variables

        private Texture2D background;
        private SpriteFont font;
        private bool hover;
        private MouseState mouseCurrent;
        private MouseState mousePrevious;
        private Vector2 position;
        private string text;
        private Color textColour;

        #endregion

        #region 06. Constructors

        public UiButton(StatusType statusType, Vector2 position, string text, Texture2D background, SpriteFont font) : base(statusType)
        {
            this.background = background;
            this.font = font;
            textColour = Color.White;
            this.position = position;
            this.text = text;
        }

        #endregion

        #region 07. Properties, Indexers

        public Rectangle Box => new Rectangle((int) position.X, (int) position.Y, background.Width, background.Height);

        #endregion

        #region 09. Override Methode

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color colour = Color.White;

            if (hover) colour = Color.Gray;

            spriteBatch.Draw(background, Box, colour);

            if (!string.IsNullOrEmpty(text))
            {
                float x = Box.X + Box.Width / 2 - font.MeasureString(text).X / 2;
                float y = Box.Y + Box.Height / 2 - font.MeasureString(text).Y / 2;

                spriteBatch.DrawString(font, text, new Vector2(x, y), textColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            hover = false;
            Rectangle MouseBox = new Rectangle(mouseCurrent.X, mouseCurrent.Y, 1, 1);

            if (MouseBox.Intersects(Box))
            {
                hover = true;

                if (mouseCurrent.LeftButton == ButtonState.Released && mousePrevious.LeftButton == ButtonState.Pressed) Click?.Invoke();
            }
        }

        #endregion

        #region 11. Methods

        public event EventHandler Click;

        #endregion
    }
}