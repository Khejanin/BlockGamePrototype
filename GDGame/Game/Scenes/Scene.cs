using GDGame.Enums;
using GDGame.EventSystem;
using Microsoft.Xna.Framework;

namespace GDGame.Scenes
{
    public abstract class Scene
    {
        #region 01. Delegates

        public delegate void OnUnloaded();

        #endregion

        #region 05. Private variables

        protected Color backgroundColor = Color.CornflowerBlue;
        private OnUnloaded onUnloadedCallback;
        private bool terminateOnNextTick;
        private bool unloadsContent;

        #endregion

        #region 06. Constructors

        protected Scene(Main game, bool unloadsContent = false)
        {
            Game = game;
            this.unloadsContent = unloadsContent;
        }

        #endregion

        #region 07. Properties, Indexers

        protected Main Game { get; }

        public string SceneName { get; set; }

        #endregion

        #region 08. Initialization

        public virtual void Initialize()
        {
            EventManager.FireEvent(new SceneEventInfo {sceneActionType = SceneActionType.OnSceneLoaded, LevelName = SceneName});
        }

        #endregion

        #region 11. Methods

        public void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(backgroundColor);
            DrawScene(gameTime);
        }

        protected abstract void DrawScene(GameTime gameTime);

        protected virtual void PreTerminate()
        {
        }

        private void PreUpdate()
        {
            if (terminateOnNextTick)
            {
                Terminate();

                onUnloadedCallback.Invoke();
            }
        }

        protected abstract void Terminate();

        public void UnloadScene(OnUnloaded cb)
        {
            onUnloadedCallback = cb;
            if (unloadsContent)
                Game.Content.Unload();
            terminateOnNextTick = true;
            PreTerminate();
        }

        public void Update(GameTime gameTime)
        {
            PreUpdate();

            if (!terminateOnNextTick)
                UpdateScene(gameTime);
            else
                terminateOnNextTick = false;
        }

        protected abstract void UpdateScene(GameTime gameTime);

        #endregion
    }
}