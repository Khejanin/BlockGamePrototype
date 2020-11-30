using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Managers;
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
        protected UiSceneManager uiSceneManager;
        private bool unloadsContent;

        #endregion

        #region 06. Constructors

        protected Scene(Main game, SceneType sceneType, bool unloadsContent = false)
        {
            uiSceneManager = new UiSceneManager(this);
            Game = game;
            SceneType = sceneType;
            this.unloadsContent = unloadsContent;
        }

        #endregion

        #region 07. Properties, Indexers

        public Main Game { get; }

        public string SceneName { get; set; }
        public SceneType SceneType { get; set; }

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
            Game.UiManager.UIObjectList.Clear();
            PreTerminate();
        }

        public void Update(GameTime gameTime)
        {
            PreUpdate();

            if (!terminateOnNextTick)
            {
                UpdateScene(gameTime);
                uiSceneManager.Update(gameTime);
            }
            else
            {
                terminateOnNextTick = false;
            }
        }

        protected abstract void UpdateScene(GameTime gameTime);

        #endregion
    }

    public enum SceneType
    {
        Game,
        Menu,
        End,
        Options,
        Info
    }
}