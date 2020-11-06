using GDGame.Game.Managers;
using GDGame.Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GDLibrary;
using GDLibrary.Managers;
using GDLibrary.Actors;
using GDLibrary.Parameters;

namespace GDGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; }

        public BasicEffect ModelEffect { get; private set; }

        public BasicEffect UnlitTexturedEffect { get; private set; }

        public BasicEffect UnlitWireframeEffect { get; private set; }

        public CameraManager<Camera3D> CameraManager { get; private set; }

        public ObjectManager ObjectManager { get; private set; }

        public KeyboardManager KeyboardManager { get; private set; }

        public MouseManager MouseManager { get; private set; }

        public SpriteFont DebugFont { get; set; }

        public BasicEffect WireframeModelEffect { get; private set; }

        public RasterizerState WireframeRasterizerState { get; private set; }

        public Vector2 ScreenCentre { get; private set; } = Vector2.Zero;

        public SoundManager SoundManager { get; private set; }
        
        public SceneManager SceneManager { get; private set; }
        public UiManager UiManager { get; private set; }

        public ProjectionParameters GlobalProjectionParameters => ProjectionParameters.StandardDeepSixteenTen;

        private float worldScale = 3000;
        private PrimitiveObject primitiveObject = null;
        private MainScene mainScene;


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

            

            CreateScenes();
            InitManagers();
            InitFonts();
            InitEffect();

            InitGraphics(1024, 768);

            base.Initialize();
            
            SceneManager.Initialize();
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
            mainScene = new MainScene(this);
        }

        private void InitManagers()
        {
            //Camera
            CameraManager = new CameraManager<Camera3D>(this);
            Components.Add(CameraManager);
            
            //Scene
            SceneManager = new SceneManager(this,mainScene);
            Components.Add(SceneManager);

            //Keyboard
            KeyboardManager = new KeyboardManager(this);
            Components.Add(KeyboardManager);

            //Mouse
            MouseManager = new MouseManager(this, false);
            Components.Add(MouseManager);

            //Sound
            SoundManager = new SoundManager(this);
            Components.Add(SoundManager);
            
            //Object
            ObjectManager = new ObjectManager(this, 6, 10, CameraManager);
            Components.Add(ObjectManager);
            
            UiManager = new UiManager(this);
            Components.Add(UiManager);

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
            ScreenCentre = new Vector2(width / 2, height / 2);

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion
    }
}