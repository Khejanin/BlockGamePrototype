using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws a texture and text to the screen to create a button with a user-defined text string and font. Used primarily by the menu manager
    /// </summary>
    public class UIButtonObject : UITextureObject
    {
        //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        protected static float TEXT_LAYER_DEPTH_MULTIPLIER = 0.95f;

        #region Fields

        private string text;
        private SpriteFont spriteFont;
        private Color textColor;
        protected Vector2 textOrigin;
        private Vector2 textOffset;

        #endregion Fields

        #region Properties

        public string Text
        {
            get { return text; }
            set
            {
                text = (value.Length >= 0) ? value : "Default";
                textOrigin = spriteFont.MeasureString(text) / 2.0f;
            }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Vector2 TextOffset
        {
            get { return textOffset; }
            set { textOffset = value; }
        }

        #endregion Properties

        #region Constructors & Core

        public UIButtonObject(string id, ActorType actorType, StatusType statusType, Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
            Texture2D texture, Rectangle sourceRectangle, string text, SpriteFont spriteFont, Color textColor, Vector2 textOffset) : base(id, actorType, statusType, transform2D,
            color, layerDepth, spriteEffects, texture, sourceRectangle)
        {
            SpriteFont = spriteFont;
            Text = text;
            TextColor = textColor;
            TextOffset = textOffset;
        }

        //to do...Draw, Equals, GetHashCode, Clone

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw the texture
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(spriteFont, text,
                Transform2D.Translation + textOffset,
                Color,
                Transform2D.RotationInRadians,
                Transform2D.Origin, //giving the text its own origin?
                Transform2D.Scale,
                SpriteEffects,
                LayerDepth * TEXT_LAYER_DEPTH_MULTIPLIER); //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        }

        #endregion Constructors & Core
    }
}