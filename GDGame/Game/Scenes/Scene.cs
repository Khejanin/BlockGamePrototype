using GDGame.Game.Managers;
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
        private Main game;
        private bool unloadsContent;

        protected Color BackgroundColor = Color.CornflowerBlue;

        protected Scene(Main game, bool unloadsContent = true)
        {
            this.game = game;
            this.unloadsContent = unloadsContent;
        }
        
        #region Parameters
        protected GraphicsDevice GraphicsDevice => game.GraphicsDevice;
        protected GraphicsDeviceManager Graphics => game.Graphics;
        protected ContentManager Content => game.Content;
        protected ObjectManager ObjectManager => game.ObjectManager;
        protected KeyboardManager KeyboardManager => game.KeyboardManager;
        protected CameraManager<Camera3D> CameraManager => game.CameraManager;
        protected MouseManager MouseManager => game.MouseManager;
        protected SoundManager SoundManager => game.SoundManager;
        protected BasicEffect ModelEffect => game.ModelEffect;
        protected BasicEffect UnlitWireframeEffect => game.UnlitWireframeEffect;
        protected BasicEffect UnlitTexturedEffect => game.UnlitTexturedEffect;
        protected RasterizerState WireframeRasterizerState => game.WireframeRasterizerState;
        protected GameComponentCollection Components => game.Components;

        protected Main Game => game;
        
        #endregion
        
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        
        public virtual void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);
        }
        
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