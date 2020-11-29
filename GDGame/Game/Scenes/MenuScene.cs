using GDGame.Actors;
using GDGame.Actors.Drawn;
using GDGame.Game.UI;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GDLibrary.Managers;
using System;

namespace GDGame.Scenes
{
    public class MenuScene : Scene
    {
        private Dictionary<string, Texture2D> textures;
        UiButton playUiButton, optionsUiButton, quitUiButton;
        MouseManager mouseManager;

        public MenuScene(Main game, bool unloadsContent = false) : base(game, unloadsContent)
        {
            mouseManager = game.MouseManager;
            //backgroundColor = Color.LightCyan;
            

        }

        public override void Initialize()
        {
            InitializeLoadContent();
            InitializeCamera();
            InitialiseBackground();
            InitializeText();
            InitialiseButtons();
            
        }

        private void InitialiseBackground()
        {
            UiSprite bg = new UiSprite(StatusType.Drawn, textures["bg"], new Rectangle(0, 0, (int)(Game.ScreenCentre.X * 2), (int)(Game.ScreenCentre.Y * 2)), Color.White, false);
            UiManager.AddUiElement("bg",bg);
        }

        private void InitialiseButtons()
        {
            mouseManager.MouseVisible = true;
            playUiButton = new UiButton(StatusType.Drawn | StatusType.Update,
                new Vector2(Game.ScreenCentre.X+100, Game.ScreenCentre.Y-160), "Play", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("PlayButton", playUiButton);
            playUiButton.Click += Click_PlayBtn;

            optionsUiButton = new UiButton(StatusType.Drawn | StatusType.Update,
                new Vector2(Game.ScreenCentre.X +100, Game.ScreenCentre.Y + 0), "Options", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("OptionsButton", optionsUiButton);
            optionsUiButton.Click += Click_OptionsBtn;

            quitUiButton = new UiButton(StatusType.Drawn | StatusType.Update,
                new Vector2(Game.ScreenCentre.X +100, Game.ScreenCentre.Y + 160), "Quit", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("QuitButton", quitUiButton);
            quitUiButton.Click += Click_QuitBtn;
        }
        #region 05. Private variables

        private UiButton playUiButton, optionsUiButton, quitUiButton;

        #endregion

        #region 06. Constructors

        public MenuScene(Main game, bool unloadsContent = false) : base(game, unloadsContent)
        {
            backgroundColor = Color.LightCyan;
        }

        #endregion

        #region 08. Initialization

        private void InitialiseButtons()
        {
            string text = "Play";
            if (((UiButtonObject) Game.UiArchetypes["button"]).Clone() is UiButtonObject button)
            {
                button.Text = text;
                button.Transform2D.Translation = Game.ScreenCentre - Vector2.UnitY * 120;
                Game.UiManager.Add(button);
            }

            text = "Options";
            button = ((UiButtonObject) Game.UiArchetypes["button"]).Clone() as UiButtonObject;
            if (button != null)
            {
                button.Text = text;
                button.Transform2D.Translation = Game.ScreenCentre;
                Game.UiManager.Add(button);
            }

            text = "Quit";
            button = ((UiButtonObject) Game.UiArchetypes["button"]).Clone() as UiButtonObject;
            if (button != null)
            {
                button.Text = text;
                button.Transform2D.Translation = Game.ScreenCentre + Vector2.UnitY * 120;
                Game.UiManager.Add(button);
            }
        }

        public override void Initialize()
        {
            InitializeLoadContent();
            InitializeCamera();
            InitialiseButtons();
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
            UiText titleUiText = new UiText(StatusType.Drawn, "B_Logic", Game.Fonts["UI"],
                new Vector2(Game.ScreenCentre.X -275, Game.ScreenCentre.Y - 25), Color.SaddleBrown);
            UiManager.AddUiElement("TitleText", titleUiText);
            UiText subTitleUiText = new UiText(StatusType.Drawn, "Caffeine Edition!", Game.Fonts["UI"],
                new Vector2(Game.ScreenCentre.X - 275, Game.ScreenCentre.Y + 50), Color.SaddleBrown);
            UiManager.AddUiElement("SubTitleText", subTitleUiText);

            Game.UiManager.Dispose();
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

        #region 11. Methods

        private void Click_OptionsBtn()
        {
            Game.SceneManager.OptionsSwitchScene();
        }

        private void Click_PlayBtn()
        {
            Texture2D cursor = Content.Load<Texture2D>("Assets/Textures/Block/block_red");
            Texture2D buttonStart = Content.Load<Texture2D>("Assets/Textures/Menu/button");
            Texture2D buttonBasic = Content.Load<Texture2D>("Assets/Textures/Block/block_yellow");
            Texture2D background = Content.Load<Texture2D>("Assets/Textures/Menu/menubaseres");
            Texture2D panel = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");
            Game.SceneManager.NextScene();
        }

        private void Click_QuitBtn()
        {
            Game.Exit();
        }

        #endregion
    }
}