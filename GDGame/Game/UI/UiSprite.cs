using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.UI
{
    public class UiSprite : UiElement
    {
        #region 05. Private variables

        private Color color;
        private float layerDepth;
        private Vector2 origin;
        private Rectangle rectangle;
        private float rotation;
        private Rectangle? sourceRectangle;
        private SpriteEffects spriteEffects;
        private Texture2D texture2D;

        #endregion

        #region 06. Constructors

        public UiSprite(StatusType statusType, Texture2D texture2D, Rectangle rectangle, Rectangle? sourceRectangle,
            Color color, float rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth) : base(
            statusType)
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

        public UiSprite(StatusType statusType, Texture2D texture2D, Rectangle rectangle, Color color,
            bool originAtCenter = true) : base(statusType)
        {
            this.texture2D = texture2D;
            this.rectangle = rectangle;
            this.color = color;
            if (originAtCenter) origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
        }

        #endregion

        #region 09. Override Methode

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, rectangle, sourceRectangle, color, rotation, origin, spriteEffects, layerDepth);
        }

        public override void Update(GameTime gameTime)
        {
        }

        #endregion

        #region 11. Methods

        public void SetRotation(float rotation)
        {
            this.rotation = rotation;
        }

        #endregion
    }
}