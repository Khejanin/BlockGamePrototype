using System.Collections.Generic;
using GDGame.Controllers;
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
    public class Main : Game
    {
        #region Private variables

        private SpriteBatch spriteBatch;

        #endregion

        #region Constructors

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion

        #region Properties, Indexers

        public CameraManager<Camera3D> CameraManager { get; private set; }
        public ContentDictionary<SpriteFont> Fonts { get; private set; }
        public ProjectionParameters GlobalProjectionParameters => ProjectionParameters.StandardDeepSixteenTen;
        public GraphicsDeviceManager Graphics { get; }
        public KeyboardManager KeyboardManager { get; private set; }
        public LevelDataManager LevelDataManager { get; private set; }

        public MyMenuManager MenuManager { get; set; }

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

        public Dictionary<string, DrawnActor2D> UiArchetypes { get; set; }
        public OurUiManager UiManager { get; private set; }
        public BasicEffect UnlitWireframeEffect { get; private set; }

        #endregion

        #region Initialization

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

            InitGraphics(1024, 768);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            InitManagers();
            InitializeDictionaries();

            CreateScenes();
            InitEffect();
            LoadFonts();
            LoadBasicTextures();

            InitUiArchetypes();


            base.Initialize();
        }

        private void InitializeDictionaries()
        {
            UiArchetypes = new Dictionary<string, DrawnActor2D>();

            Fonts = new ContentDictionary<SpriteFont>("fonts", Content);
            Textures = new ContentDictionary<Texture2D>("textures", Content);
            Models = new ContentDictionary<Model>("models", Content);
        }

        private void InitManagers()
        {
            //Events
            Components.Add(new EventManager(this));
            Components.Add(new EventDispatcher(this));

            //Physics
            PhysicsManager = new PhysicsManager(this, StatusType.Off);
            Components.Add(PhysicsManager);

            //Camera
            CameraManager = new CameraManager<Camera3D>(this, StatusType.Off);
            Components.Add(CameraManager);

            //Scene
            SceneManager = new SceneManager(this, StatusType.Off);
            Components.Add(SceneManager);

            //Keyboard
            KeyboardManager = new KeyboardManager(this);
            Components.Add(KeyboardManager);

            //Mouse
            MouseManager = new MouseManager(this, true, PhysicsManager, ScreenCentre);
            Components.Add(MouseManager);

            //Sound
            SoundManager = new SoundManager();
            //Components.Add(SoundManager);

            //Object
            ObjectManager = new ObjectManager(this, StatusType.Off, 6, 10);
            Components.Add(ObjectManager);

            //Render
            RenderManager = new RenderManager(this, StatusType.Drawn, ScreenLayoutType.Single, ObjectManager, CameraManager);
            Components.Add(RenderManager);

            //UI
            UiManager = new OurUiManager(this, StatusType.Off, spriteBatch, 10);
            Components.Add(UiManager);

            MenuManager = new MyMenuManager(this, StatusType.Drawn | StatusType.Update, spriteBatch, MouseManager, KeyboardManager);
            Components.Add(MenuManager);

            //Raycast
            RaycastManager.Instance.ObjectManager = ObjectManager;

            //LevelData
            LevelDataManager = new LevelDataManager();
        }

        private void InitUiArchetypes()
        {
            Texture2D texture = Textures["bStart"];
            Integer2 dimensions = new Integer2(texture.Width, texture.Height);
            Transform2D transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, Vector2.Zero, dimensions);
            UITextureObject uiTextureObject = new UITextureObject("texture", ActorType.UITextureObject, StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0.6f,
                SpriteEffects.None, texture, new Rectangle(0, 0, texture.Width, texture.Height));

            string text = "";
            dimensions = new Integer2(Fonts["Arial"].MeasureString(text));
            transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, Vector2.Zero, dimensions);
            UITextObject uiTextObject = new UITextObject("text", ActorType.UIText, StatusType.Drawn | StatusType.Update, transform2D, Color.Black, 0.1f,
                SpriteEffects.None, text, Fonts["Arial"]);

            text = "";
            texture = Textures["bStart"];
            dimensions = new Integer2(texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, origin, dimensions);
            UIButtonObject uiButtonObject = new UIButtonObject("button", ActorType.UIButtonObject, StatusType.Update | StatusType.Drawn, transform2D, Color.White, 0.5f,
                SpriteEffects.None, texture, new Rectangle(0, 0, texture.Width, texture.Height), text, Fonts["Arial"], Vector2.One, Color.White, Vector2.Zero);
            uiButtonObject.ControllerList.Add(new UiScaleLerpController("USC", ControllerType.Ui, MouseManager, new TrigonometricParameters(0.05f, 0.1f, 180)));

            UiArchetypes.Add("button", uiButtonObject);
            UiArchetypes.Add("texture", uiTextureObject);
            UiArchetypes.Add("text", uiTextObject);
        }

        #endregion

        #region Override Methode

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aqua);
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        #endregion

        #region Load Methods

        private void LoadBasicTextures()
        {
            Textures.Load("Assets/Textures/Menu/button", "bStart");
        }

        private void LoadFonts()
        {
            Fonts.Load("Assets/Fonts/Arial");
        }

        #endregion

        #region Methods

        private void CreateScenes()
        {
            //SceneManager.AddScene("Test", new MainScene(this, "test_Enemy_path.json"));
            SceneManager.AddScene("Level 7", new MainScene(this, "Big_Level.json"));


            // SceneManager.AddScene("Tutorial", new TutorialScene(this));
            // SceneManager.AddScene("Level1", new MainScene(this, "Paul_Level_1.json"));
            // SceneManager.AddScene("Level2", new MainScene(this, "Paul_Level_2.json"));
            //SceneManager.AddScene("Level3", new MainScene(this, "Paul_Level_3.json"));
            //SceneManager.AddScene("Level4", new MainScene(this, "Paul_Level_4.json"));
            //SceneManager.AddScene("Level5", new MainScene(this, "Paul_Level_5.json"));
            //SceneManager.AddScene("Level6", new MainScene(this, "Paul_Level_6.json"));
        }

        #endregion
    }
}