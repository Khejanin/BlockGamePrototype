using GDGame.Game.UI;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.Scenes
{
    public class MenuScene : Scene
    {
        private Text2D menuText;

        public MenuScene(Main game,bool unloadsContent = false) : base(game,unloadsContent)
        {
            BackgroundColor = Color.Black;
        }

        public override void Initialize()
        {
            InitializeCamera();
            InitializeText();
        }


        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update,
                new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up), Game.GlobalProjectionParameters);
            CameraManager.Add(camera3D);
        }

        
        protected void InitializeText()
        {
            Text2D menuText = new Text2D(StatusType.Drawn, "Press SPACEBAR to start the Game!", Game.Fonts["UI"],
                Game.ScreenCentre, Color.Wheat);
            UiManager.AddUiElement("MenuText", menuText);
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.Space))
            {
                Game.SceneManager.NextScene();
            }
        }


        public override void Terminate()
        {
            UiManager.Clear();
        }
    }
}