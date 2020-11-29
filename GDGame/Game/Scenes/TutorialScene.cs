using GDGame.Game.UI;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scenes
{
    public class TutorialScene : Scene
    {
        #region 06. Constructors

        public TutorialScene(Main game, bool unloadsContent = false) : base(game, unloadsContent)
        {
            backgroundColor = Color.Black;
        }

        #endregion

        #region 08. Initialization

        public override void Initialize()
        {
            InitializeCamera();
            InitializeText();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Game.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Game.CameraManager.Add(camera3D);
        }


        private void InitializeText()
        {
            Texture2D texture2D = Game.Content.Load<Texture2D>("Assets/Textures/Menu/tutorial");
            UiSprite tutorialText = new UiSprite(StatusType.Drawn, texture2D, new Rectangle(0, 0, (int) (Game.ScreenCentre.X * 2), (int) (Game.ScreenCentre.Y * 2)), Color.White,
                false);
            UiText menuUiText = new UiText(StatusType.Drawn, "Press SPACEBAR to continue!", Game.Fonts["Arial"],
                Game.ScreenCentre - new Vector2(0, Game.ScreenCentre.Y / 2), Color.Black);
            //Game.UiManager.AddUiElement("TutorialImage", tutorialText);
            // Game.UiManager.AddUiElement("TutorialText", menuUiText);
        }

        #endregion

        #region 09. Override Methode

        protected override void DrawScene(GameTime gameTime)
        {
        }


        protected override void Terminate()
        {
            Game.UiManager.Dispose();
            Game.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Menu_Camera");
        }

        protected override void UpdateScene(GameTime gameTime)
        {
            if (Game.KeyboardManager.IsFirstKeyPress(Keys.Space)) Game.SceneManager.NextScene();
        }

        #endregion
    }
}