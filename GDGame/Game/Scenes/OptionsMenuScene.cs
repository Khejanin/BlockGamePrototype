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
using System;
using System.Collections.Generic;
using GDLibrary.Managers;
using GDGame.Scenes;

namespace GDGame.Scenes
{
    class OptionsMenuScene : Scene
    {
        private Dictionary<string, Texture2D> textures;
        UiButton backUiButton, quitUiButton;
        public OptionsMenuScene(Main game, bool unloadsContent = false) : base(game, unloadsContent)
        {
            MouseManager mouseManager = new MouseManager(game, true);
            backgroundColor = Color.LightCyan;
        }

        public override void Initialize()
        {
            InitializeLoadContent();
            InitializeCamera();
            //InitializeText();
            InitialiseButtons();
        }

        private void InitialiseButtons()
        {
            backUiButton = new UiButton(StatusType.Drawn, new Vector2(Game.ScreenCentre.X - 93, Game.ScreenCentre.Y - 40), "Resume", textures["bStart"], Game.Fonts["UI"]);
            UiManager.AddUiElement("BackButton", backUiButton);

            backUiButton.Click += Click_BackBtn;

            quitUiButton = new UiButton(StatusType.Drawn, new Vector2(Game.ScreenCentre.X - 93, Game.ScreenCentre.Y + 80), "Quit", textures["bStart"], Game.Fonts["UI"]);
            UiManager.AddUiElement("QuitBotton", quitUiButton);

            quitUiButton.Click += Click_QuitBtn;
        }

        private void Click_QuitBtn()
        {
            Game.Exit();
        }

        private void Click_BackBtn()
        {
            Game.SceneManager.MenuSwitchScene();
        }

        private void InitializeLoadContent()
        {
            LoadTextures();
            LoadSounds();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update,
                new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up), Game.GlobalProjectionParameters);
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
            backUiButton.Update(gameTime);
            quitUiButton.Update(gameTime);

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
            CameraManager.Clear();
        }


        #region Load Content
        private void LoadSounds()
        {
            SoundEffect track01 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack02");
            SoundManager.Add(new Sounds(track01, "gameTrack01", ActorType.MusicTrack, StatusType.Update));
            //SoundManager.NextSong();
        }

        private void LoadTextures()
        {
            Texture2D cursor = Content.Load<Texture2D>("Assets/Textures/Block/block_red");
            Texture2D buttonStart = Content.Load<Texture2D>("Assets/Textures/Menu/button");
            Texture2D buttonBasic = Content.Load<Texture2D>("Assets/Textures/Block/block_yellow");
            Texture2D background = Content.Load<Texture2D>("Assets/Textures/Block/block_green");
            Texture2D panel = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");

            textures = new Dictionary<string, Texture2D>
            {
                {"bStart", buttonStart},
                {"bBasic", buttonBasic},
                {"bg", background},
                {"Panel", panel},
                {"Cursor", cursor }
            };
        }

        #endregion
    }
}
