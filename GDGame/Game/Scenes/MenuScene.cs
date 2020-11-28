using GDGame.Actors;
using GDGame.Managers;
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
            //InitializeText();
            InitialiseBackground();
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
                new Vector2(Game.ScreenCentre.X+100, Game.ScreenCentre.Y-200), "Play", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("MenuButton", playUiButton);
            playUiButton.Click += Click_PlayBtn;

            optionsUiButton = new UiButton(StatusType.Drawn | StatusType.Update,
                new Vector2(Game.ScreenCentre.X +100, Game.ScreenCentre.Y + 80), "Options", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("OptionsButton", optionsUiButton);
            optionsUiButton.Click += Click_OptionsBtn;

            quitUiButton = new UiButton(StatusType.Drawn | StatusType.Update,
                new Vector2(Game.ScreenCentre.X +100, Game.ScreenCentre.Y + 200), "Quit", textures["bStart"],
                Game.Fonts["UI"]);
            UiManager.AddUiElement("QuitButton", quitUiButton);
            quitUiButton.Click += Click_QuitBtn;
        }

        private void Click_OptionsBtn()
        {
            Game.SceneManager.OptionsSwitchScene();
        }

        private void Click_QuitBtn()
        {
            Game.Exit();
        }

        private void Click_PlayBtn()
        {
            Game.SceneManager.NextScene();
        }

        private void InitializeLoadContent()
        {
            LoadTextures();
            LoadSounds();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Game.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            CameraManager.Add(camera3D);
        }

        private void InitializeText()
        {
            UiText menuUiText = new UiText(StatusType.Drawn, "Press SPACEBAR to start the Game!", Game.Fonts["UI"],
                Game.ScreenCentre, Color.Wheat);
            UiManager.AddUiElement("MenuText", menuUiText);
        }

        protected override void UpdateScene(GameTime gameTime)
        {
            playUiButton.Update(gameTime);
            quitUiButton.Update(gameTime);
            optionsUiButton.Update(gameTime);

            if (KeyboardManager.IsFirstKeyPress(Keys.Space))
            {
                Game.SceneManager.NextScene();
            }
        }

        protected override void DrawScene(GameTime gameTime)
        {
            
        }


        protected override void Terminate()
        {
            UiManager.Clear();
            CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Menu_Camera");
        }


        #region Load Content

        private void LoadSounds()
        {
            SoundEffect mainTheme = Content.Load<SoundEffect>("Assets/GameTracks/testTrack04");
            SoundManager.Add(new Sounds(mainTheme, "mainTheme", ActorType.SpecialTrack, StatusType.Update));
            SoundManager.PlaySoundEffect("mainTheme");
        }

        private void LoadTextures()
        {
            Texture2D cursor = Content.Load<Texture2D>("Assets/Textures/Block/block_red");
            Texture2D buttonStart = Content.Load<Texture2D>("Assets/Textures/Menu/button");
            Texture2D buttonBasic = Content.Load<Texture2D>("Assets/Textures/Block/block_yellow");
            Texture2D background = Content.Load<Texture2D>("Assets/Textures/Menu/menubaseres");
            Texture2D panel = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");

            textures = new Dictionary<string, Texture2D>
            {
                {"bStart", buttonStart},
                {"bBasic", buttonBasic},
                {"bg", background},
                {"Panel", panel},
                {"Cursor", cursor}
            };
        }

        #endregion
    }
}