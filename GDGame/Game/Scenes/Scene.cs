using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Scenes
{
    public abstract class Scene
    {
        protected Main game;
        protected bool unloadsContent;

        public Scene(Main game, bool unloadsContent = true)
        {
            this.game = game;
            this.unloadsContent = unloadsContent;
        }
        
        #region Parameters
        protected GraphicsDevice GraphicsDevice => game.GraphicsDevice;
        protected GraphicsDeviceManager _graphics => game.Graphics;
        protected ContentManager Content => game.Content;
        protected ObjectManager ObjectManager => game.ObjectManager;
        protected KeyboardManager KeyboardManager => game.KeyboardManager;
        protected CameraManager<Camera3D> CameraManager => game.CameraManager;
        protected MouseManager MouseManager => game.MouseManager;
        protected SoundManager SoundManager => game.SoundManager;
        
        #endregion
        
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public abstract void Terminate();
        
        public virtual void UnloadScene()
        {
            if (unloadsContent) 
                Content.Unload();
            
            CameraManager.Clear();
            
            Terminate();
        }
    }
}