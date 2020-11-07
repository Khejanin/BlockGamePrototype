using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDGame.Game.Scenes
{
    public class SceneManager : DrawableGameComponent
    {
        private Scene CurrentScene
        {
            get { return SceneList[currentSceneIndex];}
        }
        
        //So you can access by key and then request Scene at index to be switched to
        public Dictionary<string, int> SceneIndexDictionary { get; private set; }
        
        private List<Scene> SceneList;
        private int currentSceneIndex;
        private int nextSceneIndex = -1;
        private bool currentlySwitching = false;

        //The Game needs a SceneManager, and the SceneManager needs a Scene.
        public SceneManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            SceneList = new List<Scene>();
            SceneIndexDictionary = new Dictionary<string, int>();
            currentSceneIndex = 0;
        }

        public void AddScene(string key, Scene s)
        {
            SceneIndexDictionary.Add(key,SceneList.Count);
            SceneList.Add(s);
        }

        public void Initialize()
        {
            SceneList[currentSceneIndex].Initialize();
        }

        public void NextScene()
        {
            SwitchScene(currentSceneIndex+1);
        }

        public void PreviousScene()
        {
            SwitchScene(currentSceneIndex-1);
        }

        //A loading screen would be nice but I don't have time to test and implement async operations, we just gotta live with the unresponsiveness for now.
        public void SwitchScene(int sceneIndex)
        {
            SceneList[currentSceneIndex].UnloadScene(OnCurrentSceneUnloaded);
            nextSceneIndex = sceneIndex;
            currentlySwitching = true;
        }

        public void OnCurrentSceneUnloaded()
        {
            currentSceneIndex = nextSceneIndex;
            nextSceneIndex = -1;
            CurrentScene.Initialize();
            currentlySwitching = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CurrentScene.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            CurrentScene.Draw(gameTime);
        }
    }
}