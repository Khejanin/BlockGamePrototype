using Microsoft.Xna.Framework;

namespace GDGame.Game.Scenes
{
    public class SceneManager
    {
        private Scene current;

        //The Game needs a SceneManager, and the SceneManager needs a Scene.
        public SceneManager(Scene s)
        {
            current = s;
        }

        public void Initialize()
        {
            current.Initialize();
        }

        //A loading screen would be nice but I don't have time to test and implement async operations, we just gotta live with the unresponsiveness for now.
        public void SwitchScene(Scene newScene)
        {
            current.UnloadScene();

            current = newScene;
            newScene.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            current.Update(gameTime);
        }
        
        public void Draw(GameTime gameTime)
        {
            current.Draw(gameTime);
        }
        
    }
}