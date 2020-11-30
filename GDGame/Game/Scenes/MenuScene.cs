using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scenes
{
    public class MenuScene : Scene
    {
        #region 06. Constructors

        public MenuScene(Main game, bool unloadsContent = false) : base(game, SceneType.Menu, unloadsContent)
        {
            backgroundColor = Color.LightCyan;
        }

        #endregion

        #region 08. Initialization

        public override void Initialize()
        {
            InitializeLoadContent();
            InitializeCamera();
            uiSceneManager.InitUi();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Game.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Game.CameraManager.Add(camera3D);
        }

        private void InitializeLoadContent()
        {
            LoadTextures();
            LoadSounds();
        }

        #endregion

        #region 09. Override Methode

        protected override void DrawScene(GameTime gameTime)
        {
        }


        protected override void Terminate()
        {
            Game.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Menu_Camera");
        }

        protected override void UpdateScene(GameTime gameTime)
        {
            if (Game.KeyboardManager.IsFirstKeyPress(Keys.Space)) Game.SceneManager.NextScene();
        }

        #endregion

        #region 10. Load Methods

        private void LoadSounds()
        {
            SoundEffect mainTheme = Game.Content.Load<SoundEffect>("Assets/GameTracks/testTrack04");
            Game.SoundManager.Add(new Sounds(mainTheme, "mainTheme", ActorType.SpecialTrack, StatusType.Update));
            Game.SoundManager.PlaySoundEffect("mainTheme");
        }

        private void LoadTextures()
        {
            Game.Textures.Load("Assets/Textures/Block/block_red", "Cursor");
            Game.Textures.Load("Assets/Textures/Menu/button", "bStart");
            Game.Textures.Load("Assets/Textures/Block/block_yellow", "bBasic");
            Game.Textures.Load("Assets/Textures/Block/block_green", "bg");
            Game.Textures.Load("Assets/Textures/Skybox/floor_neon", "Panel");
        }

        #endregion
    }
}