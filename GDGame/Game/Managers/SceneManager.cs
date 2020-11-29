using System.Collections.Generic;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Scenes;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    public class SceneManager : DrawableGameComponent
    {
        #region 05. Private variables

        private int nextSceneIndex = -1;

        private List<Scene> sceneList;

        #endregion

        #region 06. Constructors

        //The Game needs a SceneManager, and the SceneManager needs a Scene.
        public SceneManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            sceneList = new List<Scene>();
            SceneIndexDictionary = new Dictionary<string, int>();
            CurrentSceneIndex = 0;
        }

        #endregion

        #region 07. Properties, Indexers

        private Scene CurrentScene => sceneList[CurrentSceneIndex];

        public int CurrentSceneIndex { get; private set; }

        //So you can access by key and then request Scene at index to be switched to
        private Dictionary<string, int> SceneIndexDictionary { get; }

        #endregion

        #region 08. Initialization

        public override void Initialize()
        {
            sceneList[CurrentSceneIndex].Initialize();
        }

        #endregion

        #region 09. Override Methode

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            CurrentScene.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CurrentScene.Update(gameTime);
        }

        #endregion

        #region 11. Methods

        public void AddScene(string key, Scene s)
        {
            SceneIndexDictionary.Add(key, sceneList.Count);
            sceneList.Add(s);
            s.SceneName = key;
        }

        //direct switch back to main menu - for going back from options and a quit game thing
        public void MenuSwitchScene()
        {
            sceneList[CurrentSceneIndex].UnloadScene(OnCurrentSceneUnloaded);
            nextSceneIndex = SceneIndexDictionary["Menu"];
        }

        public void NextScene()
        {
            SwitchScene(CurrentSceneIndex + 1);
        }

        //direct switch to options menu
        public void OptionsSwitchScene()
        {
            sceneList[CurrentSceneIndex].UnloadScene(OnCurrentSceneUnloaded);
            nextSceneIndex = SceneIndexDictionary["Options"];
        }

        public void PreviousScene()
        {
            SwitchScene(CurrentSceneIndex - 1);
        }

        //A loading screen would be nice but I don't have time to test and implement async operations, we just gotta live with the unresponsiveness for now.
        private void SwitchScene(int sceneIndex)
        {
            EventManager.FireEvent(new SceneEventInfo {sceneActionType = SceneActionType.OnSceneChange});
            sceneList[CurrentSceneIndex].UnloadScene(OnCurrentSceneUnloaded);
            nextSceneIndex = sceneIndex;
        }

        #endregion

        #region 12. Events

        public void OnCurrentSceneUnloaded()
        {
            CurrentSceneIndex = nextSceneIndex;
            CurrentScene.Initialize();
        }

        #endregion
    }
}