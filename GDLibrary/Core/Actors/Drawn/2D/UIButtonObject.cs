using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws a texture and text to the screen to create a button with a user-defined text string and font. Used primarily by the menu manager
    /// </summary>
    public class UIButtonObject : UITextureObject
    {
        //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        private static float TEXT_LAYER_DEPTH_MULTIPLIER = 0.95f;

        #region Fields
        private string text;
        private SpriteFont spriteFont;
        private Color textColor;
        private Vector2 textOrigin, textOffset;

        #endregion Fields

        #region Properties

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = (value.Length >= 0) ? value : "Default";
                textOrigin = spriteFont.MeasureString(text) / 2.0f;
            }
        }

        public SpriteFont SpriteFont
        {
            get
            {
                return spriteFont;
            }
            set
            {
                spriteFont = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
            }
        }

        public Vector2 TextOffset
        {
            get
            {
                return textOffset;
            }
            set
            {
                textOffset = value;
            }
        }

        public Vector2 TextScale { get; set; }

        #endregion Properties

        #region Constructors & Core

        public UIButtonObject(string id, ActorType actorType, StatusType statusType,
        Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
        Texture2D texture, Rectangle sourceRectangle,
         string text, SpriteFont spriteFont, Vector2 textScale, Color textColor, Vector2 textOffset)
         : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects, texture, sourceRectangle)
        {
            //bug - fixed - dylan!
            SpriteFont = spriteFont;
            Text = text;
            TextColor = textColor;
            TextOffset = textOffset;
            this.TextScale = textScale;
            this.textOrigin = spriteFont.MeasureString(text) / 2;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw the texture
            base.Draw(gameTime, spriteBatch);

            //draw text
            spriteBatch.DrawString(spriteFont, text,
                Transform2D.Translation + textOffset,
                this.textColor,
                Transform2D.RotationInRadians,
                this.textOrigin, //giving the text its own origin?
                this.TextScale,
                SpriteEffects,
                LayerDepth * TEXT_LAYER_DEPTH_MULTIPLIER); //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        }

        public override bool Equals(object obj)
        {
            return obj is UIButtonObject @object &&
                   base.Equals(obj) &&
                   text == @object.text &&
                   EqualityComparer<SpriteFont>.Default.Equals(spriteFont, @object.spriteFont) &&
                   TextScale.Equals(@object.TextScale) &&
                   textColor.Equals(@object.textColor) &&
                   textOffset.Equals(@object.textOffset);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(text);
            hash.Add(spriteFont);
            hash.Add(TextScale);
            hash.Add(textColor);
            hash.Add(textOffset);
            return hash.ToHashCode();
        }

        public new object Clone()
        {
            UIButtonObject uiButtonObject = new UIButtonObject("clone - " + ID, ActorType, StatusType,
                Transform2D.Clone() as Transform2D, Color, LayerDepth, SpriteEffects,
                Texture, //shallow - reference
                SourceRectangle,
                text,
                spriteFont,   //shallow - reference
                this.TextScale,
                textColor, textOffset);
            uiButtonObject.ControllerList.AddRange(GetControllerListClone());
            return uiButtonObject;
        }

        #endregion Constructors & Core
    }
}