using Microsoft.Xna.Framework;

namespace GDGame.Game.Scenes
{
    public class SceneManager : DrawableGameComponent
    {
        private Scene current;

        //The Game needs a SceneManager, and the SceneManager needs a Scene.
        public SceneManager(Microsoft.Xna.Framework.Game game,Scene s) : base(game)
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            current.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            current.Draw(gameTime);
        }
    }
}