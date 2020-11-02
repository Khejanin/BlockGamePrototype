using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BlockGame.Scenes
{
    public abstract class Scene
    {
        protected Main game;

        public Scene(Main game)
        {
            this.game = game;
        }
        
        protected GraphicsDevice GraphicsDevice => game.GraphicsDevice;
        protected GraphicsDeviceManager _graphics => game.Graphics;
        protected ContentManager Content => game.Content;
        protected ObjectManager ObjectManager => game.ObjectManager;
        protected KeyboardManager KeyboardManager => game.KeyboardManager;
        protected CameraManager CameraManager => game.CameraManager;
        protected MouseManager MouseManager => game.MouseManager;
        public abstract void Initialize();

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}