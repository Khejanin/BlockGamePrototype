using GDGame.Game.Managers;
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
        private bool terminateOnNextTick;
        private OnUnloaded onUnloadedCallback;

        protected Color backgroundColor = Color.CornflowerBlue;

        protected Scene(Main game, bool unloadsContent = false)
        {
            this.game = game;
            this.unloadsContent = unloadsContent;
        }

        public delegate void OnUnloaded();

        
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
        protected UiManager UiManager => game.UiManager;

        protected Main Game => game;
        
        #endregion
        
        public abstract void Initialize();

        private void PreUpdate()
        {
            if (terminateOnNextTick)
            {
                Terminate();
                
                onUnloadedCallback.Invoke();
            }
        }

        public void Update(GameTime gameTime)
        {
            PreUpdate();

            if (!terminateOnNextTick)
            {
                UpdateScene(gameTime);
            }
            else
            {
                terminateOnNextTick = false;
            }
        }

        protected abstract void UpdateScene(GameTime gameTime);
        
        public void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            DrawScene(gameTime);
        }
        
        protected abstract void DrawScene(GameTime gameTime);

        protected virtual void PreTerminate()
        {
        }

        protected abstract void Terminate();
        
        public virtual void UnloadScene(OnUnloaded cb)
        {
            onUnloadedCallback = cb;
            if (unloadsContent) 
                Content.Unload();
            
            CameraManager.Clear();

            terminateOnNextTick = true;
            PreTerminate();
        }
    }
}