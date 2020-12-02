using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Managers;
using Microsoft.Xna.Framework;

namespace GDGame.Scenes
{
    public abstract class Scene
    {
        #region Delegates

        public delegate void OnUnloaded();

        #endregion

        #region Private variables

        private OnUnloaded onUnloadedCallback;
        private bool terminateOnNextTick;
        protected UiSceneManager uiSceneManager;
        private bool unloadsContent;

        #endregion

        #region Constructors

        protected Scene(Main main, SceneType sceneType, bool unloadsContent = false)
        {
            uiSceneManager = new UiSceneManager(this);
            this.unloadsContent = unloadsContent;
            Main = main;
            SceneType = sceneType;
        }

        #endregion

        #region Properties, Indexers

        public Main Main { get; }

        public string SceneName { get; set; }
        public SceneType SceneType { get; }

        #endregion

        #region Initialization

        public virtual void Initialize()
        {
            EventManager.FireEvent(new SceneEventInfo {sceneActionType = SceneActionType.OnSceneLoaded, LevelName = SceneName});
        }

        #endregion

        #region Methods

        protected virtual void PreTerminate()
        {
        }

        private void PreUpdate()
        {
            if (terminateOnNextTick)
            {
                Terminate();
                Main.UiManager.UIObjectList.Clear();
                onUnloadedCallback.Invoke();
            }
        }

        protected abstract void Terminate();

        public void UnloadScene(OnUnloaded cb)
        {
            onUnloadedCallback = cb;
            if (unloadsContent)
                Main.Content.Unload();
            terminateOnNextTick = true;
            PreTerminate();
        }

        public void Update(GameTime gameTime)
        {
            PreUpdate();

            if (!terminateOnNextTick)
            {
                UpdateScene();
                uiSceneManager.Update(gameTime);
            }
            else
            {
                terminateOnNextTick = false;
            }
        }

        protected abstract void UpdateScene();

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