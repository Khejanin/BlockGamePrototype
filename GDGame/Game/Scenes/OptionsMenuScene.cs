using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scenes
{
    public class OptionsMenuScene : Scene
    {
        #region Constructors

        public OptionsMenuScene(Main main, SceneType sceneType, bool unloadsContent) : base(main, sceneType, unloadsContent)
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
            LoadSounds();
        }

        #endregion

        #region Override Methode

        protected override void Terminate()
        {
            Main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Menu_Camera");
        }


        protected override void UpdateScene()
        {
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.Space)) Main.SceneManager.NextScene();
        }

        #endregion

        #region Load Methods

        private void LoadSounds()
        {
            SoundEffect track01 = Main.Content.Load<SoundEffect>("Assets/GameTracks/GameTrack02");
            Main.SoundManager.AddMusic("gametrack01", track01);

            Main.SoundManager.StartMusicQueue();
        }

        private void LoadTextures()
        {
            Main.Textures.Load("Assets/Textures/Block/block_red", "Cursor");
            Main.Textures.Load("Assets/Textures/Menu/button", "bStart");
            Main.Textures.Load("Assets/Textures/Block/block_yellow", "bBasic");
            Main.Textures.Load("Assets/Textures/Block/block_green", "bg");
            Main.Textures.Load("Assets/Textures/Skybox/floor_neon", "Panel");
        }

        #endregion
    }
}