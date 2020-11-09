using System.Collections.Generic;
using GDGame.Scenes;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    public class SceneManager : DrawableGameComponent
    {
        private Scene CurrentScene => sceneList[currentSceneIndex];

        //So you can access by key and then request Scene at index to be switched to
        private Dictionary<string, int> SceneIndexDictionary { get; }

        private List<Scene> sceneList;
        private int currentSceneIndex;
        private int nextSceneIndex = -1;
        private bool currentlySwitching;

        //The Game needs a SceneManager, and the SceneManager needs a Scene.
        public SceneManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            sceneList = new List<Scene>();
            SceneIndexDictionary = new Dictionary<string, int>();
            currentSceneIndex = 0;
        }

        public void AddScene(string key, Scene s)
        {
            SceneIndexDictionary.Add(key,sceneList.Count);
            sceneList.Add(s);
        }

        public override void Initialize()
        {
            sceneList[currentSceneIndex].Initialize();
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
        private void SwitchScene(int sceneIndex)
        {
            sceneList[currentSceneIndex].UnloadScene(OnCurrentSceneUnloaded);
            nextSceneIndex = sceneIndex;
            currentlySwitching = false;
        }

        public void OnCurrentSceneUnloaded()
        {
            currentlySwitching = true;
            currentSceneIndex = nextSceneIndex;
            CurrentScene.Initialize();
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