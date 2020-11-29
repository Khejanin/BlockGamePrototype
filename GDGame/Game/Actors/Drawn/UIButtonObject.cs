using System;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors.Drawn
{
    public class UiButtonObject : UIButtonObject, ICloneable
    {
        #region 06. Constructors

        public UiButtonObject(string id, ActorType actorType, StatusType statusType, Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
            Texture2D texture, Rectangle sourceRectangle, string text, SpriteFont spriteFont, Color textColor, Vector2 textOffset) : base(id, actorType,
            statusType, transform2D, color, layerDepth, spriteEffects, texture, sourceRectangle, text, spriteFont, textColor, textOffset)
        {
        }

        #endregion

        #region 09. Override Methode

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Transform2D.Translation, SourceRectangle, Color, Transform2D.RotationInRadians, Transform2D.Origin, Transform2D.Scale, SpriteEffects,
                LayerDepth);

            spriteBatch.DrawString(SpriteFont, Text, Transform2D.Translation, Color, Transform2D.RotationInRadians, textOrigin, Transform2D.Scale, SpriteEffects,
                LayerDepth * TEXT_LAYER_DEPTH_MULTIPLIER);
        }

        #endregion

        #region 11. Methods

        public new object Clone()
        {
            return new UiButtonObject(ID, ActorType, StatusType, Transform2D.Clone() as Transform2D, Color, LayerDepth, SpriteEffects, Texture, SourceRectangle, Text, SpriteFont,
                TextColor, TextOffset);
        }

        #endregion
    }
}