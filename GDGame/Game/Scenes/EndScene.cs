using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scenes
{
    public class EndScene : Scene
    {
        #region Constructors

        public EndScene(Main main, SceneType sceneType, bool unloadsContent) : base(main, sceneType, unloadsContent)
        {
            backgroundColor = Color.Black;
        }

        #endregion

        #region Initialization

        public override void Initialize()
        {
            InitializeCamera();
            uiSceneManager.InitUi();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Main.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Main.CameraManager.Add(camera3D);
        }

        #endregion

        #region Override Methode


        protected override void Terminate()
        {
        }

        protected override void UpdateScene()
        {
        }

        #endregion
    }
}