using System;
using System.Collections.Generic;
using GDGame.EventSystem;
using GDGame.Managers;
using GDGame.Scenes;
using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Fields

        public GraphicsDeviceManager Graphics { get; }
        public LevelDataManager LevelDataManager { get; private set; }
        public BasicEffect ModelEffect { get; private set; }
        public BasicEffect UnlitTexturedEffect { get; private set; }
        public BasicEffect UnlitWireframeEffect { get; private set; }
        public CameraManager<Camera3D> CameraManager { get; private set; }
        public ObjectManager ObjectManager { get; private set; }
        public KeyboardManager KeyboardManager { get; private set; }
        public GamePadManager GamePadManager { get; private set; }
        public MouseManager MouseManager { get; private set; }
        public RasterizerState WireframeRasterizerState { get; private set; }
        public Vector2 ScreenCentre { get; private set; } = Vector2.Zero;
        public SoundManager SoundManager { get; private set; }
        public SceneManager SceneManager { get; private set; }
        public UiManager UiManager { get; private set; }
        public Dictionary<string, SpriteFont> Fonts { get; private set; }
        public ProjectionParameters GlobalProjectionParameters => ProjectionParameters.StandardDeepSixteenTen;

        #endregion

        #region Initialization

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "My Amazing Game";

            InitManagers();
            CreateScenes();
            InitEffect();
            LoadFonts();

            InitGraphics(1024, 768);
            base.Initialize();
        }

        private void InitManagers()
        {
            //Events
            Components.Add(new EventManager(this));

            Components.Add(new EventDispatcher(this));

            //Camera
            CameraManager = new CameraManager<Camera3D>(this);
            Components.Add(CameraManager);

            //Scene
            SceneManager = new SceneManager(this);
            Components.Add(SceneManager);

            //Keyboard
            KeyboardManager = new KeyboardManager(this);
            Components.Add(KeyboardManager);

            //Gamepad
            GamePadManager = new GamePadManager(this, 1);
            Components.Add(GamePadManager);

            //Mouse
            MouseManager = new MouseManager(this, false);
            Components.Add(MouseManager);

            //Sound
            SoundManager = new SoundManager(this);
            Components.Add(SoundManager);

            //Object
            ObjectManager = new ObjectManager(this, StatusType.Update | StatusType.Drawn, 6, 10, CameraManager);
            Components.Add(ObjectManager);

            UiManager = new UiManager(this);
            Components.Add(UiManager);

            LevelDataManager = new LevelDataManager();
        }

        private void CreateScenes()
        {
            SceneManager.AddScene("Menu", new MenuScene(this));
            //SceneManager.AddScene("Test", new MainScene(this, "test_Enemy_path.json"));
            SceneManager.AddScene("Level 7", new MainScene(this, "Big_Level.json"));

            /*
            SceneManager.AddScene("Tutorial", new TutorialScene(this));
            SceneManager.AddScene("Level1", new MainScene(this, "Paul_Level_1.json"));
            SceneManager.AddScene("Level2", new MainScene(this, "Paul_Level_2.json"));
            SceneManager.AddScene("Level3", new MainScene(this, "Paul_Level_3.json"));
            SceneManager.AddScene("Level4", new MainScene(this, "Paul_Level_4.json"));
            SceneManager.AddScene("Level5", new MainScene(this, "Paul_Level_5.json"));
            SceneManager.AddScene("Level6", new MainScene(this, "Paul_Level_6.json"));
            */
            //throws back to menu eventually
            SceneManager.AddScene("End", new EndScene(this));

            //shouldn't be able to "next scene" to this
            SceneManager.AddScene("Options", new OptionsMenuScene(this));
        }

        private void InitEffect()
        {
            //to do...
            UnlitTexturedEffect = new BasicEffect(Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true, TextureEnabled = true
            };
            //otherwise we wont see RGB

            //wireframe primitives with no lighting and no texture
            UnlitWireframeEffect = new BasicEffect(Graphics.GraphicsDevice) {VertexColorEnabled = true};

            //model effect
            //add a ModelObject
            ModelEffect = new BasicEffect(Graphics.GraphicsDevice) {TextureEnabled = true};
            //this.modelEffect.LightingEnabled = true;
            //this.modelEffect.EnableDefaultLighting();

            WireframeRasterizerState = new RasterizerState {FillMode = FillMode.WireFrame};
        }

        private void LoadFonts()
        {
            SpriteFont uiFont = Content.Load<SpriteFont>("Assets/Fonts/Arial");

            Fonts = new Dictionary<string, SpriteFont> {{"UI", uiFont}};
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            Graphics.ApplyChanges();

            //set screen centre based on resolution
            ScreenCentre = new Vector2(width / 2f, height / 2f);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState {CullMode = CullMode.None};
            Graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp
            };
            Graphics.GraphicsDevice.SamplerStates[0] = samplerState;
        }

        #endregion

        #region Override Methodes

        protected override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        #endregion
    }
}