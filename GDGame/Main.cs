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
        public GraphicsDeviceManager Graphics { get; }

        public LevelDataManager LevelDataManager { get; set; }

        public BasicEffect ModelEffect { get; private set; }

        public BasicEffect UnlitTexturedEffect { get; private set; }

        public BasicEffect UnlitWireframeEffect { get; private set; }

        public CameraManager<Camera3D> CameraManager { get; private set; }

        public ObjectManager ObjectManager { get; private set; }

        public KeyboardManager KeyboardManager { get; private set; }

        public GamePadManager GamePadManager { get; private set; }

        public MouseManager MouseManager { get; private set; }

        public SpriteFont DebugFont { get; set; }

        public BasicEffect WireframeModelEffect { get; private set; }

        public RasterizerState WireframeRasterizerState { get; private set; }

        public Vector2 ScreenCentre { get; private set; } = Vector2.Zero;

        public SoundManager SoundManager { get; private set; }

        public SceneManager SceneManager { get; private set; }
        public UiManager UiManager { get; private set; }


        public Dictionary<string, SpriteFont> Fonts { get; private set; }


        public ProjectionParameters GlobalProjectionParameters => ProjectionParameters.StandardDeepSixteenTen;

        private float worldScale = 3000;
        private PrimitiveObject primitiveObject = null;
        public Effect testEffect;


        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region Initialization - Managers, Cameras, Effects, Textures, Audio

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "My Amazing Game";

            InitManagers();
            CreateScenes();
            LoadEffects();
            InitEffect();
            LoadFonts();

            InitGraphics(1024, 768);

            base.Initialize();
        }


        private void InitDebug()
        {
            /*Components.Add(new DebugDrawer(this, _spriteBatch, this.debugFont,
                this.cameraManager, this.objectManager));*/
        }

        private void InitFonts()
        {
            DebugFont = Content.Load<SpriteFont>("Assets/Fonts/debug");
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
            //thows back to menu eventually
            SceneManager.AddScene("End", new EndScene(this));

            //shouldnt be able to "next scene" to this
            SceneManager.AddScene("Options", new OptionsMenuScene(this));
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
            SoundManager = new SoundManager();
            //Components.Add(SoundManager);

            //Object
            ObjectManager = new ObjectManager(this, StatusType.Update | StatusType.Drawn, 6, 10, CameraManager);
            Components.Add(ObjectManager);

            UiManager = new UiManager(this);
            Components.Add(UiManager);

            LevelDataManager = new LevelDataManager();
        }

        private void LoadEffects()
        {
            testEffect = Content.Load<Effect>("Assets/Effects/test");
        }

        private void InitEffect()
        {
            //to do...
            UnlitTexturedEffect = new BasicEffect(Graphics.GraphicsDevice);
            UnlitTexturedEffect.VertexColorEnabled = true; //otherwise we wont see RGB
            UnlitTexturedEffect.TextureEnabled = true;

            //wireframe primitives with no lighting and no texture
            UnlitWireframeEffect = new BasicEffect(Graphics.GraphicsDevice);
            UnlitWireframeEffect.VertexColorEnabled = true;

            //model effect
            //add a ModelObject
            ModelEffect = new BasicEffect(Graphics.GraphicsDevice);
            ModelEffect.TextureEnabled = true;
            //this.modelEffect.LightingEnabled = true;
            //this.modelEffect.EnableDefaultLighting();

            WireframeModelEffect = new BasicEffect(Graphics.GraphicsDevice);
            WireframeModelEffect.TextureEnabled = false;
            WireframeModelEffect.VertexColorEnabled = true;

            WireframeRasterizerState = new RasterizerState();
            WireframeRasterizerState.FillMode = FillMode.WireFrame;
        }

        private void LoadFonts()
        {
            SpriteFont uiFont = Content.Load<SpriteFont>("Assets/Fonts/Arial");

            Fonts = new Dictionary<string, SpriteFont> {{"UI", uiFont}};
        }

        #endregion

        #region Load and Unload Content

        protected override void LoadContent()
        {
            InitDebug();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

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
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            Graphics.GraphicsDevice.SamplerStates[0] = samplerState;
        }


        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion
    }

    public class LevelStats
    {
        public int time;
        public int moveCount;

        public LevelStats()
        {
            time = Int32.MaxValue;
            moveCount = Int32.MaxValue;
        }
    }
}