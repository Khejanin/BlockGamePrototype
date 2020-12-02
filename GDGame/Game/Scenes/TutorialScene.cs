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
        #region Constructors

        public TutorialScene(Main main, SceneType sceneType, bool unloadsContent) : base(main, sceneType, unloadsContent)
        {
        }

        #endregion

        #region Initialization

        public override void Initialize()
        {
            InitializeLoadContent();
            InitializeCamera();
            uiSceneManager.InitUi();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Main.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Main.CameraManager.Add(camera3D);
        }

        private void InitializeLoadContent()
        {
            LoadTextures();
        }

        #endregion

        #region Override Methode

        protected override void Terminate()
        {
            Main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Menu_Camera");
            Main.Textures.Dispose();
        }

        protected override void UpdateScene()
        {
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.Space)) Main.SceneManager.NextScene();
        }

        #endregion

        #region Load Methods

        private void LoadTextures()
        {
            Main.Textures.Load("Assets/Textures/Menu/tutorial", "Tutorial");
        }

        #endregion
    }
}