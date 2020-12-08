using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Constants;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame
{
    /// <summary>
    /// This is the class that instantiates the Managers, Archetypes and loads the "Scenes", scenes have been replaced by the GameManager.
    /// This doesn't load content, as the GameManager is supposed to set everything up.
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Private variables

        private float currentMovementCoolDown;
        private GameManager game;
        public PlayerTile player;

        private SpriteBatch spriteBatch;
        private bool isPlaying;

        #endregion

        #region Constructors

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsEasy = true;
        }

        #endregion

        //Declare all Managers and stuff that should be accessible from the GameManager
        #region Properties, Indexers

        public OurPrimitiveObject ArchetypalTexturedQuad { get; private set; }

        public CameraManager<Camera3D> CameraManager { get; private set; }
        public ContentDictionary<Effect> Effects { get; private set; }
        public ContentDictionary<SpriteFont> Fonts { get; private set; }
        private GraphicsDeviceManager Graphics { get; }
        public bool IsEasy { get; private set; }
        public KeyboardManager KeyboardManager { get; private set; }
        public LevelDataManager LevelDataManager { get; private set; }
        public OurMenuManager MenuManager { get; private set; }
        public BasicEffect ModelEffect { get; private set; }
        public ContentDictionary<Model> Models { get; private set; }
        public MouseManager MouseManager { get; private set; }
        public OurObjectManager ObjectManager { get; private set; }
        public OurPhysicsManager PhysicsManager { get; set; }
        private OurRenderManager RenderManager { get; set; }
        public Vector2 ScreenCentre { get; private set; } = Vector2.Zero;
        public SoundManager SoundManager { get; private set; }
        public ContentDictionary<Texture2D> Textures { get; private set; }
        public Dictionary<string, DrawnActor2D> UiArchetypes { get; private set; }
        public UIManager UiManager { get; private set; }
        public UiSceneManager uiSceneManager { get; private set; }


        #endregion

        #region Initialization

        private void InitArchetypalQuad()
        {
            //SKYBOX
            float halfLength = 0.5f;
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(new Vector3(-halfLength, halfLength, 0), Color.White,
                new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(-halfLength, -halfLength, 0), Color.White,
                new Vector2(0, 1));
            vertices[2] = new VertexPositionColorTexture(new Vector3(halfLength, halfLength, 0), Color.White,
                new Vector2(1, 0));
            vertices[3] = new VertexPositionColorTexture(new Vector3(halfLength, -halfLength, 0), Color.White,
                new Vector2(1, 1));

            BasicEffect unlitTexturedEffect = new BasicEffect(Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            Transform3D transform3D =
                new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            BasicEffectParameters effectParameters =
                new BasicEffectParameters(unlitTexturedEffect, Textures["kWall1"], Color.White, 1);
            IVertexData vertexData =
                new VertexData<VertexPositionColorTexture>(vertices, PrimitiveType.TriangleStrip, 2);
            ArchetypalTexturedQuad = new OurPrimitiveObject("original texture quad", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, vertexData);
        }

        //The Cameras never change, so we instantiate them here.
        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0), -Vector3.Forward, Vector3.Up);
            Camera3D mainCamera = new Camera3D("MainCamera", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen,
                new Viewport(0, 0, GameConstants.ScreenWidth, GameConstants.ScreenHeight));
            mainCamera.ControllerList.Add(new RotationAroundActor("RAAC", ControllerType.FlightCamera,
                KeyboardManager, 35, 20));
            CameraManager.Add(mainCamera);

            if (mainCamera.Clone() is Camera3D camera3D)
            {
                camera3D.ID = "FlightCamera";
                camera3D.ControllerList.Clear();
                camera3D.ControllerList.Add(new FlightController("FPC", ControllerType.FlightCamera,
                    KeyboardManager, MouseManager, 0.01f, 0.01f, 0.01f));
                //CameraManager.Add(camera3D);
            }

            CameraManager.ActiveCameraIndex = 0;
        }

        /// <summary>
        /// This is different to LoadManager's LoadEffect() as this one uses the existing BasicEffect provided by MonoGame
        /// </summary>
        private void InitEffect()
        {
            ModelEffect = new BasicEffect(Graphics.GraphicsDevice) {TextureEnabled = true};
        }

        private void InitEvents()
        {
            EventManager.RegisterListener<OptionsEventInfo>(HandleOptionsEvent);
            EventManager.RegisterListener<GameStateMessageEventInfo>(HandleGameStateEvent);
        }
        
        /// <summary>
        /// Instantiate the GameManager (NOTHING TO DO WITH XNA GAME CLASS)
        /// </summary>
        private void InitGame()
        {
            game = new GameManager(this);
            game.Init();
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
            InitGraphics(GameConstants.ScreenWidth, GameConstants.ScreenHeight);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            InitManagers();
            InitializeDictionaries();

            InitEvents();

            InitEffect();
            LoadContent();

            InitArchetypalQuad();
            InitUiArchetypes();

            InitUi();
            InitCameras3D();

            base.Initialize();
        }

        /// <summary>
        /// Initialize the Dictionaries that we will Load Content From
        /// </summary>
        private void InitializeDictionaries()
        {
            UiArchetypes = new Dictionary<string, DrawnActor2D>();
            Fonts = new ContentDictionary<SpriteFont>("fonts", Content);
            Textures = new ContentDictionary<Texture2D>("textures", Content);
            Models = new ContentDictionary<Model>("models", Content);
            Effects = new ContentDictionary<Effect>("effects", Content);
        }
        
        private void InitManagers()
        {
            //Events
            Components.Add(new EventManager(this));
            Components.Add(new EventDispatcher(this));

            //Physics
            PhysicsManager = new OurPhysicsManager(this, StatusType.Off);
            Components.Add(PhysicsManager);

            //Camera
            CameraManager = new CameraManager<Camera3D>(this, StatusType.Off);
            Components.Add(CameraManager);

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
            ObjectManager = new OurObjectManager(this, StatusType.Off, 10, 8);
            Components.Add(ObjectManager);

            //Render
            RenderManager = new OurRenderManager(this, StatusType.Drawn, ScreenLayoutType.Single, ObjectManager,
                CameraManager);
            Components.Add(RenderManager);

            //Animation
            Components.Add(new TransformAnimationManager(this, StatusType.Update));

            //Timing
            Components.Add(new TimeManager(this, StatusType.Update));

            //UI
            UiManager = new UIManager(this, StatusType.Off, spriteBatch, 10);
            Components.Add(UiManager);

            MenuManager = new OurMenuManager(this, StatusType.Drawn | StatusType.Update, spriteBatch, MouseManager,
                KeyboardManager);
            Components.Add(MenuManager);

            // OurPhysicsDebugDrawer physicsDebugDrawer = new OurPhysicsDebugDrawer(this,
            //     StatusType.Off, CameraManager, ObjectManager);
            // Components.Add(physicsDebugDrawer);

            //Raycast
            RaycastManager.Instance.ObjectManager = ObjectManager;

            //LevelData
            LevelDataManager = new LevelDataManager();
        }


        private void InitUi()
        {
            uiSceneManager = new UiSceneManager(this,StatusType.Update | StatusType.Drawn);
            uiSceneManager.InitUi();
            Components.Add(uiSceneManager);
        }

        private void InitUiArchetypes()
        {
            Texture2D texture = Textures["bStart"];
            Integer2 dimensions = new Integer2(texture.Width, texture.Height);
            Transform2D transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, Vector2.Zero, dimensions);
            UITextureObject uiTextureObject = new UITextureObject("texture", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0.6f,
                SpriteEffects.None, texture, new Rectangle(0, 0, texture.Width, texture.Height));

            string text = "";
            dimensions = new Integer2(Fonts["Arial"].MeasureString(text));
            transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, Vector2.Zero, dimensions);
            UITextObject uiTextObject = new UITextObject("text", ActorType.UIText, StatusType.Drawn | StatusType.Update,
                transform2D, Color.Black, 0.1f,
                SpriteEffects.None, text, Fonts["Arial"]);

            text = "";
            texture = Textures["bStart"];
            dimensions = new Integer2(texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            transform2D = new Transform2D(Vector2.Zero, 0, Vector2.One, origin, dimensions);
            UIButtonObject uiButtonObject = new UIButtonObject("button", ActorType.UIButtonObject,
                StatusType.Update | StatusType.Drawn, transform2D, Color.White, 0.5f,
                SpriteEffects.None, texture, new Rectangle(0, 0, texture.Width, texture.Height), text, Fonts["Arial"],
                Vector2.One, Color.White, Vector2.Zero);
            uiButtonObject.ControllerList.Add(new UiScaleLerpController("USC", ControllerType.Ui, MouseManager,
                new TrigonometricParameters(0.05f, 0.1f, 180)));

            UiArchetypes.Add("button", uiButtonObject);
            UiArchetypes.Add("texture", uiTextureObject);
            UiArchetypes.Add("text", uiTextObject);
        }

        #endregion

        #region Override Method

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aqua);
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            LoadManager loadManager = new LoadManager(this);
            loadManager.InitLoad();
        }
        
        protected override void Update(GameTime gameTime)
        {
            game?.Update();

            //The game has its own "global timer" that runs out periodically and is defined in the GameConstants.
            //Lots of temporal elements use this as a reference on how long a "turn" in the game is, its to give the player a sense of rhythm
            if (currentMovementCoolDown <= 0)
            {
                currentMovementCoolDown = GameConstants.MovementCooldown;
                EventManager.FireEvent(new MovingTilesEventInfo());
            }

            currentMovementCoolDown -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            //Fetch the Player and set the camera to target him is we don't have a reference to him.
            if (player == null)
            {
                OurDrawnActor3D drawnActor3D =
                    ObjectManager.OpaqueList.Find(actor3D => actor3D is Tile tile && tile.ActorType == ActorType.Player);
                if (CameraManager.ActiveCamera.ControllerList[0] is RotationAroundActor cam &&
                    drawnActor3D != null)
                {
                    cam.Target = drawnActor3D;
                    player = drawnActor3D as PlayerTile;
                    drawnActor3D.StatusType = StatusType.Drawn | StatusType.Update;
                }
            }

            //Don't update the Player if using wrong cam
            if (player != null)
                player.StatusType = CameraManager.ActiveCameraIndex switch
                {
                    0 => StatusType.Drawn | StatusType.Update,
                    1 => StatusType.Drawn,
                    2 => StatusType.Drawn,
                    _ => player.StatusType
                };

            if (KeyboardManager.IsFirstKeyPress(Keys.M))
                EventDispatcher.Publish(MenuManager.StatusType == StatusType.Off
                    ? new EventData(EventCategoryType.Menu, EventActionType.OnPause, null)
                    : new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
            
            if (KeyboardManager.IsFirstKeyPress(Keys.C)) CameraManager.CycleActiveCamera();

            //Cycle Through Audio
            if (KeyboardManager.IsFirstKeyPress(Keys.M))
                EventManager.FireEvent(new SoundEventInfo {soundEventType = SoundEventType.PlayNextMusic});
            //Stop Music
            if (KeyboardManager.IsKeyDown(Keys.N))
                EventManager.FireEvent(new SoundEventInfo {soundEventType = SoundEventType.PauseMusic});
            //Volume Changes
            if (KeyboardManager.IsFirstKeyPress(Keys.L))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.IncreaseVolume, soundVolumeType = SoundVolumeType.Master});
            else if (KeyboardManager.IsFirstKeyPress(Keys.K))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.DecreaseVolume, soundVolumeType = SoundVolumeType.Master});
            //Pause/resume music
            if (KeyboardManager.IsFirstKeyPress(Keys.P))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.ToggleMusicPlayback, soundVolumeType = SoundVolumeType.Master});

            if (KeyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        #endregion

        #region Private Method

        //Do everything to destroy current game instance -> load another on restart.
        private void DestroyGame()
        {
            if (game != null)
            {
                game?.UnRegisterGame();
                game?.RemoveCamera();
                ObjectManager.RemoveAll(actor3D => actor3D != null);
                Components.Remove(PhysicsManager);
                PhysicsManager = new OurPhysicsManager(this, StatusType.Update);
                Components.Add(PhysicsManager);
                player = null;
                CameraManager.ActiveCameraIndex = 0;
            }
        }


        private void ToggleOptions()
        {
            IsEasy = !IsEasy;
        }

        #endregion

        #region Events

        /// <summary>
        /// Handle whatever the new GameState is that was just published
        /// </summary>
        /// <param name="eventInfo">The event class holding necessary information for this event</param>
        private void HandleGameStateEvent(GameStateMessageEventInfo eventInfo)
        {
            switch (eventInfo.GameState)
            {
                case GameState.Start:
                    DestroyGame();
                    InitGame();
                    isPlaying = true;
                    break;
                case GameState.Resume:
                    if (!isPlaying)
                    {
                        DestroyGame();
                        InitGame();
                        isPlaying = true;
                    }
                    break;
                case GameState.Lost:
                    DestroyGame();
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
                    MenuManager.SetScene("LoseScreen");
                    isPlaying = false;
                    break;
                case GameState.Won:
                    isPlaying = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleOptionsEvent(OptionsEventInfo obj)
        {
            switch (obj.Type)
            {
                case OptionsType.Toggle:
                    ToggleOptions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}