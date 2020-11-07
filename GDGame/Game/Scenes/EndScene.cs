using GDGame.Game.UI;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.Scenes
{
    public class EndScene : Scene
    {
        private Text2D menuText;

        public EndScene(Main game,bool unloadsContent = false) : base(game,unloadsContent)
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
            Text2D winText = new Text2D(StatusType.Drawn, "You won!!! Press ESC to close the Game!", Game.Fonts["UI"],
                Game.ScreenCentre, Color.Wheat);
            UiManager.AddUiElement("MenuText", winText);
        }

        protected override void UpdateScene(GameTime gameTime)
        {
        }

        protected override void DrawScene(GameTime gameTime)
        {
        }


        public override void Terminate()
        {
            UiManager.Clear();
        }
    }
}