using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class UiSprite : UiElement
    {
        private Texture2D texture2D;
        private Rectangle rectangle;
        private Rectangle? sourceRectangle;
        private Color color;
        private float rotation;
        private Vector2 origin;
        private SpriteEffects spriteEffects;
        private float layerDepth;

        public UiSprite(StatusType statusType, Texture2D texture2D, Rectangle rectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth) : base(statusType)
        {
            this.texture2D = texture2D;
            this.rectangle = rectangle;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.spriteEffects = spriteEffects;
            this.layerDepth = layerDepth;
        }

        public UiSprite(StatusType statusType, Texture2D texture2D, Rectangle rectangle, Color color) : base(statusType)
        {
            this.texture2D = texture2D;
            this.rectangle = rectangle;
            this.color = color;
        }

        public void SetRotation(float rotation)
        {
            this.rotation = rotation;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, rectangle, sourceRectangle, color, rotation, origin, spriteEffects, layerDepth);
        }
    }
}