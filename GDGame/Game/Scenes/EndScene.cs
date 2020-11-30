using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scenes
{
    public class EndScene : Scene
    {
        #region 06. Constructors

        public EndScene(Main game, bool unloadsContent = false) : base(game, SceneType.End, unloadsContent)
        {
            backgroundColor = Color.Black;
        }

        #endregion

        #region 08. Initialization

        public override void Initialize()
        {
            InitializeCamera();
            uiSceneManager.InitUi();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Game.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Game.CameraManager.Add(camera3D);
        }

        #endregion

        #region 09. Override Methode

        protected override void DrawScene(GameTime gameTime)
        {
        }


        protected override void Terminate()
        {
        }

        protected override void UpdateScene(GameTime gameTime)
        {
        }

        #endregion
    }
}