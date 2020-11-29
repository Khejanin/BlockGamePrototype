using System.Collections.Generic;
using GDGame.Actors.Drawn;
using GDGame.EventSystem;
using GDGame.Managers;
using GDGame.Scenes;
using GDLibrary.Actors;
using GDLibrary.Containers;
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
        #region 06. Constructors

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion

        #region 07. Properties, Indexers

        public CameraManager<Camera3D> CameraManager { get; private set; }
        public ContentDictionary<SpriteFont> Fonts { get; private set; }
        public ProjectionParameters GlobalProjectionParameters => ProjectionParameters.StandardDeepSixteenTen;
        public GraphicsDeviceManager Graphics { get; }
        public KeyboardManager KeyboardManager { get; private set; }
        public LevelDataManager LevelDataManager { get; private set; }
        public BasicEffect ModelEffect { get; private set; }
        public ContentDictionary<Model> Models { get; private set; }
        public MouseManager MouseManager { get; private set; }
        public ObjectManager ObjectManager { get; private set; }
        private PhysicsManager PhysicsManager { get; set; }
        private RenderManager RenderManager { get; set; }
        public SceneManager SceneManager { get; private set; }
        public Vector2 ScreenCentre { get; private set; } = Vector2.Zero;
        public SoundManager SoundManager { get; private set; }
        public ContentDictionary<Texture2D> Textures { get; private set; }
        public UIManager UiManager { get; private set; }
        public BasicEffect UnlitWireframeEffect { get; private set; }

        #endregion

        #region 08. Initialization

        private void InitEffect()
        {
            //wireframe primitives with no lighting and no texture
            UnlitWireframeEffect = new BasicEffect(Graphics.GraphicsDevice) {VertexColorEnabled = true};

            //model effect
            ModelEffect = new BasicEffect(Graphics.GraphicsDevice) {TextureEnabled = true};
            //this.modelEffect.LightingEnabled = true;
            //this.modelEffect.EnableDefaultLighting();
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

        protected override void Initialize()
        {
            Window.Title = "B_Logic";

            InitManagers();
            InitializeDictionaries();

            CreateScenes();
            InitEffect();
            LoadFonts();
            LoadBasicTextures();

            InitUiArchetypes();

            InitGraphics(1024, 768);
            base.Initialize();
        }

        private void InitializeDictionaries()
        {
            UiArchetypes = new Dictionary<string, DrawnActor2D>();

            Fonts = new ContentDictionary<SpriteFont>("fonts", Content);
            Textures = new ContentDictionary<Texture2D>("textures", Content);
            Models = new ContentDictionary<Model>("models", Content);
        }

        public Dictionary<string, DrawnActor2D> UiArchetypes { get; set; }

        private void InitManagers()
        {
            //Events
            Components.Add(new EventManager(this));
            Components.Add(new EventDispatcher(this));

            //Physics
            PhysicsManager = new PhysicsManager(this, StatusType.Off);
            Components.Add(PhysicsManager);

            //Camera
            CameraManager = new CameraManager<Camera3D>(this, StatusType.Update);
            Components.Add(CameraManager);

            //Scene
            SceneManager = new SceneManager(this);
            Components.Add(SceneManager);

            //Keyboard
            KeyboardManager = new KeyboardManager(this);
            Components.Add(KeyboardManager);

            //Mouse
            MouseManager = new MouseManager(this, true, PhysicsManager);
            Components.Add(MouseManager);

            //Sound
            SoundManager = new SoundManager();
            //Components.Add(SoundManager);

            //Object
            ObjectManager = new ObjectManager(this, StatusType.Update, 6, 10);
            Components.Add(ObjectManager);

            //Render
            RenderManager = new RenderManager(this, StatusType.Drawn, ScreenLayoutType.Single, ObjectManager, CameraManager);
            Components.Add(RenderManager);

            //UI
            UiManager = new UIManager(this, StatusType.Update | StatusType.Drawn, new SpriteBatch(GraphicsDevice), 10);
            Components.Add(UiManager);

            //Raycast
            RaycastManager.Instance.ObjectManager = ObjectManager;

            //LevelData
            LevelDataManager = new LevelDataManager();
        }

        #endregion

        #region 09. Override Methode

        protected override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        #endregion

        #region 10. Load Methods

        private void LoadFonts()
        {
            Fonts.Load("Assets/Fonts/Arial");
        }

        #endregion

        #region 11. Methods

        private void CreateScenes()
        {
            // SceneManager.AddScene("Menu", new Scenes.MenuScene(this));
            //SceneManager.AddScene("Test", new MainScene(this, "test_Enemy_path.json"));
            SceneManager.AddScene("Level 7", new MainScene(this, "Big_Level.json"));


            //SceneManager.AddScene("Tutorial", new TutorialScene(this));
            // SceneManager.AddScene("Level1", new MainScene(this, "Paul_Level_1.json"));
            // SceneManager.AddScene("Level2", new MainScene(this, "Paul_Level_2.json"));
            //SceneManager.AddScene("Level3", new MainScene(this, "Paul_Level_3.json"));
            //SceneManager.AddScene("Level4", new MainScene(this, "Paul_Level_4.json"));
            //SceneManager.AddScene("Level5", new MainScene(this, "Paul_Level_5.json"));
            //SceneManager.AddScene("Level6", new MainScene(this, "Paul_Level_6.json"));
            //throws back to menu eventually
            // SceneManager.AddScene("End", new EndScene(this));

            //shouldn't be able to "next scene" to this
            // SceneManager.AddScene("Options", new OptionsMenuScene(this));
        }

        #endregion


        private void LoadBasicTextures()
        {
            Textures.Load("Assets/Textures/Menu/button", "bStart");
        }

        private void InitUiArchetypes()
        {
            string text = "Hello";
            Texture2D texture = Textures["bStart"];
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Integer2 dimensions = new Integer2(texture.Width, texture.Height);
            Transform2D transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, origin, dimensions);
            UiButtonObject button = new UiButtonObject("backButton", ActorType.UIButtonObject, StatusType.Update | StatusType.Drawn, transform2D, Color.White, 0.5f,
                SpriteEffects.None, texture, new Rectangle(0, 0, texture.Width, texture.Height), text, Fonts["Arial"], Color.Black, Vector2.Zero);

            UiArchetypes.Add("button", button);
        }
    }
}