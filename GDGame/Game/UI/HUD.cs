using System;
using GDLibrary.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GDGame.Game.UI
{
    public class Hud : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture2D;
        private Texture2D compass;
        private Camera3D camera3D;
        private SpriteFont spriteFont;

        public Hud(Microsoft.Xna.Framework.Game game, Texture2D texture2D, SpriteFont spriteFont,
            SpriteBatch spriteBatch, Texture2D compass, Camera3D camera3D) : base(game)
        {
            this.texture2D = texture2D;
            this.spriteFont = spriteFont;
            this.spriteBatch = spriteBatch;
            this.compass = compass;
            this.camera3D = camera3D;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            float halfWidth = GraphicsDevice.Viewport.Width / 2f;
            float screenHeight = GraphicsDevice.Viewport.Height;
            float screenWidth = GraphicsDevice.Viewport.Width;


            int height = 50;
            Point location = new Point(0, GameConstants.screenHeight - height);
            Point size = new Point(GameConstants.screenWidth, height);

            DrawTexture(texture2D, location, size);

            height = 90;
            int width = 120;
            int screenWidthCostume = (int) ((screenWidth - width) / 2f);
            location = new Point(screenWidthCostume, GameConstants.screenHeight - height);
            size = new Point(width, height);

            DrawTexture(texture2D, location, size);

            int border = 10;
            Vector2 origin = new Vector2(compass.Width / 2f, compass.Height / 2f);
            float angle = GetAngle(Vector3.Forward, camera3D.Transform3D.Look);
            Rectangle rectangle = new Rectangle(new Point((int) (screenWidth - 60) + border, border + 50),
                new Point(100, 100));
            spriteBatch.Draw(compass, rectangle, null, Color.White, angle, origin, SpriteEffects.None, 0);


            float heightFromBottom = 45;

            string text = "moves";
            Vector2 position = new Vector2(halfWidth - (text.Length - 1) * 12, screenHeight - heightFromBottom * 2);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "5";
            position = new Vector2(halfWidth - text.Length * 12, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "Current Level";
            position = new Vector2(x: halfWidth / 4f, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            text = "Time : 00:00:00";
            position = new Vector2(x: halfWidth + halfWidth / 4f, screenHeight - heightFromBottom);
            spriteBatch.DrawString(spriteFont, text, position, Color.Black);

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }

        private float GetAngle(Vector3 u, Vector3 v)
        {
            Vector3 noY = v * (Vector3.Forward + Vector3.Right);
            noY.Normalize();
            
            Vector3 cross = Vector3.Cross(u, noY);
            double dot = Vector3.Dot(u, noY);
            
            double angle = Math.Atan2(cross.Length(), dot);

            double test = Vector3.Dot(Vector3.Up, cross);
            if (test < 0.0f) angle = -angle;
            return (float) -(angle + Math.PI);
        }

        private void DrawTexture(Texture2D texture2D, Point location, Point size)
        {
            Rectangle rectangle = new Rectangle(location, size);
            spriteBatch.Draw(texture2D, rectangle, Color.White);
        }
    }
}