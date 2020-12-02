using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;
using GDGame.Game.Tiles;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
using GDGame.Game.Actors.Tiles;
using GDGame.Game.UI;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace GDGame.Scenes
{
    public class MainScene : Scene
    {
        private Dictionary<string, Model> models;
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, DrawnActor3D> drawnActors;
        private MouseManager mouseManager;

        private string levelName;
        private bool optionsToggle;

        private ModelObject coffee;
        private ModelObject player;

        public static Transform3D playerTransform3D;


        ////FOR SKYBOX____ TEMP
        private PrimitiveObject archetypalTexturedQuad, primitiveObject;
        private TestEffectParameters testEffectParameters;

        public MainScene(Main game, string levelName) : base(game)
        {
            mouseManager = new MouseManager(game, false);
            this.levelName = @"Game\LevelFiles\" + levelName;
        }

        public override void Initialize()
        {
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();

            SetTargetToCamera();
            InitEvents();
            base.Initialize();
        }

        private void SetTargetToCamera()
        {
            RotationAroundActor cam = (RotationAroundActor) CameraManager.ActiveCamera.ControllerList[0];
            cam.Target = ObjectManager.player;
        }

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        private void InitLoadContent()
        {
            LoadTextures();
            LoadSounds();
            LoadModels();
        }


        private void InitDrawnContent() //formerly InitPrimitives
        {
            //adds origin helper etc
            InitHelpers();

            //models

            //grids
            InitGrid();

            InitUi();

            //Skybox
            InitArchetypalQuad();

            InitCoffee();
            
            Effect coffeePostProcess = Content.Load<Effect>("Assets/Effects/Coffee");
            Effect dissolvePP = Content.Load<Effect>("Assets/Effects/Normal");
                        
            Texture2D dis = Content.Load<Texture2D>("Assets/Textures/uvalex");
            Texture2D flow = Content.Load<Texture2D>("Assets/Textures/flowmap2");
            
            //Texture2D normal = Content.Load<Texture2D>("Assets/Textures/norma1k");
            
            ObjectManager.coffeePostProcess = coffeePostProcess;
            ObjectManager.testTexture = textures["uvtest"];
            ObjectManager.displacement = dis;
            ObjectManager.flowMap = flow;

            ObjectManager.normalPP = dissolvePP;
            //ObjectManager.playerNormal = normal;
            ObjectManager.playerTexture = textures["Player"];

            ObjectManager.screenSpace = Game.ScreenCentre*2;
            
            ObjectManager.coffee = coffee;

            RenderTarget2D renderTarget2D = renderTarget2D = new RenderTarget2D(GraphicsDevice, 
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            ObjectManager.withCoffee = renderTarget2D;
            
            renderTarget2D = new RenderTarget2D(GraphicsDevice, 
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            ObjectManager.withoutCoffee = renderTarget2D;
            
            ObjectManager.spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private void InitCoffee()
        {
            CoffeeEffectParameters coffeeEffect = new CoffeeEffectParameters(null,null,null,Color.Brown);
            Transform3D transform3D = new Transform3D(Vector3.Down * 3, -Vector3.Forward, Vector3.Up);
            transform3D.Scale = Vector3.One /5;
            coffee = new ModelObject("coffee - plane",ActorType.Primitive,StatusType.Update,transform3D,coffeeEffect,models["Plane"]);
            coffee.ControllerList.Add(new MoveController("coffee move controller", ControllerType.Pan, Vector3.Up / 5.0f));
            
            ObjectManager.Add(coffee);
        }


        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 20), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera,
                KeyboardManager, 20, 20));

            transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward,Vector3.Up);
            Camera3D camera3D2 = new Camera3D("cam",ActorType.Camera3D,StatusType.Update, transform3D,ProjectionParameters.StandardDeepFourThree);
            camera3D2.ControllerList.Add(new FirstPersonController("fp",ControllerType.FirstPerson,KeyboardManager,MouseManager,0.1f,0.1f,0.01f));
            
            CameraManager.Add(camera3D);
            CameraManager.Add(camera3D2);
            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors));
            grid.GenerateGrid(levelName);
        }

        private void InitHelpers()
        {
            //step 1 - vertices
            VertexPositionColor[] vertices = VertexFactory.GetVerticesPositionColorOriginHelper(
                out var primitiveType, out int primitiveCount);

            //step 2 - make vertex data that provides Draw()
            IVertexData vertexData = new VertexData<VertexPositionColor>(vertices,
                primitiveType, primitiveCount);

            //step 3 - make the primitive object
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 10),
                Vector3.Zero, new Vector3(1, 1, 1),
                Vector3.UnitZ, Vector3.UnitY);

            BasicEffectParameters effectParameters = new BasicEffectParameters(UnlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject actor = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            ObjectManager.Add(actor);
        }


        private void InitUi()
        {
            float screenHeight = GraphicsDevice.Viewport.Height;
            float screenWidth = GraphicsDevice.Viewport.Width;

            float halfWidth = screenWidth / 2f;

            int heightFromBottom = 25;
            Point location = new Point((int) halfWidth, (int) (screenHeight - heightFromBottom));
            Point size = new Point((int) screenWidth, heightFromBottom * 2);
            Rectangle pos = new Rectangle(location, size);
            UiSprite uiSprite = new UiSprite(StatusType.Drawn, textures["WhiteSquare"], pos, Color.White);
            UiManager.AddUiElement("WhiteBarBottom", uiSprite);

            heightFromBottom = 50;
            location = new Point((int) halfWidth, (int) (screenHeight - heightFromBottom));
            size = new Point(120, heightFromBottom * 2);
            pos = new Rectangle(location, size);
            uiSprite = new UiSprite(StatusType.Drawn, textures["WhiteSquare"], pos, Color.White);
            UiManager.AddUiElement("WhiteBarBottomMiddle", uiSprite);

            int border = 10;
            location = new Point((int) (screenWidth - 50) - border, border + 50);
            size = new Point(100);
            pos = new Rectangle(location, size);

            uiSprite = new UiSprite(StatusType.Drawn, textures["Circle"], pos, Color.White);
            UiManager.AddUiElement("Circle", uiSprite);

            uiSprite = new UiSprite(StatusType.Drawn, textures["Compass"], pos, Color.White);
            UiManager.AddUiElement("Compass", uiSprite);

            heightFromBottom = 75;
            string text = "Moves";
            Vector2 position = new Vector2(halfWidth, screenHeight - heightFromBottom);
            UiText uiText = new UiText(StatusType.Drawn, text, Game.Fonts["UI"], position, Color.Black);
            UiManager.AddUiElement("Moves", uiText);

            heightFromBottom = 25;
            text = "Current Level";
            position = new Vector2(x: halfWidth / 2f, screenHeight - heightFromBottom);
            uiText = new UiText(StatusType.Drawn, text, Game.Fonts["UI"], position, Color.Black);
            UiManager.AddUiElement("Current Level", uiText);

            text = "Time : 00:00:00";
            position = new Vector2(x: 3 * halfWidth / 2f, screenHeight - heightFromBottom);
            uiText = new UiText(StatusType.Drawn, text, Game.Fonts["UI"], position, Color.Black);
            UiManager.AddUiElement("Time", uiText);

            text = "0";
            position = new Vector2(halfWidth, screenHeight - heightFromBottom);
            uiText = new UiText(StatusType.Drawn, text, Game.Fonts["UI"], position, Color.Black);
            UiManager.AddUiElement("MovesNumeric", uiText);

            text = "Hold Space To Attach";
            uiText = new UiText(StatusType.Off, text, Game.Fonts["UI"], Vector2.Zero, Color.White, false);
            UiManager.AddUiElement("ToolTip", uiText);

            float screenHeightFull = GraphicsDevice.Viewport.Height - 768;
            float screenWidthFull = GraphicsDevice.Viewport.Width - 1024;

            position = new Vector2(screenWidthFull, screenHeightFull);
            UiQuickOptions uiOptionsOverlay =
                new UiQuickOptions(StatusType.Off, position, " ", textures["options"], Game.Fonts["UI"]);
            UiManager.AddUiElement("OptionsOverlay", uiOptionsOverlay);

            //UiButton uiOptionsLogo = new UiButton(StatusType.Off, new Vector2(screenWidthFull, screenHeightFull), " ", textures["Logo"], Game.Fonts["UI"]);
            //UiManager.AddUiElement("OptionsLogo", uiOptionsLogo);

            UiButton uiOptionsButtonResume = new UiButton(StatusType.Update,
                new Vector2(screenWidthFull, screenHeightFull), "Resume", textures["optionsButton"], Game.Fonts["UI"]);
            UiManager.AddUiElement("OptionsButtonResume", uiOptionsButtonResume);
            uiOptionsButtonResume.Click += OptionsMenu;
        }

        //in game options menu trigger
        private void OptionsMenu()
        {
            if (optionsToggle)
            {
                optionsToggle = false;
                mouseManager.MouseVisible = false;
            }
            else
            {
                optionsToggle = true;
                mouseManager.MouseVisible = true;
            }

            UiManager.Options(optionsToggle);
        }

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
                VertexColorEnabled = true, TextureEnabled = true
            };

            Transform3D transform3D =
                new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            BasicEffectParameters effectParameters = new BasicEffectParameters(unlitTexturedEffect, textures["Wall"], /*bug*/ Color.White, 1);

            IVertexData vertexData =
                new VertexData<VertexPositionColorTexture>(vertices, PrimitiveType.TriangleStrip, 2);

            archetypalTexturedQuad = new PrimitiveObject("original texture quad", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, vertexData);
        }

        #endregion


        #region LoadContent

        private void LoadSounds()
        {
            //step 1 - load songs
            SoundEffect track01 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack02");
            SoundEffect track02 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack03");
            SoundEffect track03 = Content.Load<SoundEffect>("Assets/GameTracks/gameTrack04");
            SoundEffect track04 = Content.Load<SoundEffect>("Assets/Sound/Knock03");
            SoundEffect track05 = Content.Load<SoundEffect>("Assets/Sound/Click01");

            //Step 2- Make into sounds
            SoundManager.Add(new Sounds(track01, "gameTrack01", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track02, "gameTrack02", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track03, "gameTrack03", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track04, "playerMove", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track05, "playerAttach", ActorType.SoundEffect, StatusType.Update));


            SoundManager.NextSong();
        }

        private void LoadTextures()
        {
            Texture2D cubeTexture = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube");
            Texture2D test = Content.Load<Texture2D>("Assets/Textures/lampcolor");
            Texture2D basicBgFloor = Content.Load<Texture2D>("Assets/Textures/Block/BlockTextureBlue");
            Texture2D whiteSquareTexture = Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare");
            Texture2D compassTexture = Content.Load<Texture2D>("Assets/Textures/Base/BasicCompass");
            Texture2D circle = Content.Load<Texture2D>("Assets/Textures/circle");
            Texture2D logo = Content.Load<Texture2D>("Assets/Textures/Menu/logo");
            Texture2D logoMirror = Content.Load<Texture2D>("Assets/Textures/Menu/logo_mirror");
            Texture2D options = Content.Load<Texture2D>("Assets/Textures/Menu/menubaseres");
            Texture2D optionsButton = Content.Load<Texture2D>("Assets/Textures/Menu/button");
            Texture2D uvTest = Content.Load<Texture2D>("Assets/Textures/uvalex");

            Texture2D wall = Content.Load<Texture2D>("Assets/Textures/Block/block_green");
            Texture2D floor = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");

            Texture2D panel1 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall1");
            Texture2D panel2 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall2");
            Texture2D panel3 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall3");
            Texture2D panel4 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall4");
            Texture2D floor1 = Content.Load<Texture2D>("Assets/Textures/Skybox/tiles");

            textures = new Dictionary<string, Texture2D>
            {
                {"Player", test},
                {"Attachable", cubeTexture},
                {"Finish", cubeTexture},
                {"Box", basicBgFloor},
                {"WhiteSquare", whiteSquareTexture},
                {"Compass", compassTexture},
                {"Wall1", wall},
                {"Wall", floor},
                {"Floor", floor},
                {"Circle", circle},
                {"Logo", logo},
                {"LogoMirror", logoMirror},
                {"kWall1", panel1},
                {"kWall2", panel2},
                {"kWall3", panel3},
                {"kWall4", panel4},
                {"floor2", floor1},
                {"options", options},
                {"optionsButton", optionsButton},
                {"uvtest",uvTest}
            };
        }

        private void LoadModels()
        {
            Model redCube = Content.Load<Model>("Assets/Models/RedCube");
            Model blueCube = Content.Load<Model>("Assets/Models/blueCube");
            Model boxModel = Content.Load<Model>("Assets/Models/box2");
            Model enemyModel = Content.Load<Model>("Assets/Models/Enemy");
            Model planeModel = Content.Load<Model>("Assets/Models/plane");
            Model test = Content.Load<Model>("Assets/Models/LanternFBXNOTEXTURE");

            models = new Dictionary<string, Model>
            {
                {"Attachable", redCube}, {"Player", test}, {"Box", boxModel}, {"Enemy", enemyModel} , {"Plane",planeModel}
            };
        }

        private void InitEvents()
        {
            EventManager.RegisterListener<GameStateMessageEventInfo>(OnGameStateMessageReceived);
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
        }

        private void HandleDataManagerEvent(DataManagerEvent obj)
        {
            if (UiManager["MovesNumeric"] is UiText uiText)
                uiText.Text = Game.LevelDataManager.CurrentMovesCount.ToString();
        }

        private void OnGameStateMessageReceived(GameStateMessageEventInfo eventInfo)
        {
            switch (eventInfo.gameState)
            {
                case GameState.Won:
                    Game.SceneManager.NextScene();
                    break;
                case GameState.Lost:
                    //You know how it is on this bitch of an earth
                    break;
            }
        }

        #endregion

        #region Override Methodes

        protected override void UpdateScene(GameTime gameTime)
        {
            float angle = MathHelperFunctions.GetAngle(Vector3.Forward, CameraManager.ActiveCamera.Transform3D.Look);
            UiSprite uiSprite = UiManager["Compass"] as UiSprite;
            uiSprite?.SetRotation(angle);

            List<DrawnActor3D> players = ObjectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (players.Count > 0)
            {
                PlayerTile playerTile = players[0] as PlayerTile;
                UiManager["ToolTip"].StatusType =
                    playerTile?.AttachCandidates.Count > 0 ? StatusType.Drawn : StatusType.Off;
            }

            if (KeyboardManager.IsFirstKeyPress(Keys.C))
            {
                CameraManager.CycleActiveCamera();
                // this.cameraManager.ActiveCameraIndex++;
            }
            
            //Cycle Through Audio
            if (KeyboardManager.IsFirstKeyPress(Keys.M))
                SoundManager.NextSong();

            //Stop Music
            if (KeyboardManager.IsKeyDown(Keys.N))
                SoundManager.StopSong();

            //Volume Changes
            if (KeyboardManager.IsFirstKeyPress(Keys.L))
                SoundManager.volumeUp();
            else if (KeyboardManager.IsFirstKeyPress(Keys.K))
                SoundManager.volumeDown();

            //Pause/resume music
            if (KeyboardManager.IsFirstKeyPress(Keys.P))
                SoundManager.changeMusicState();

            //options menu
            if (KeyboardManager.IsFirstKeyPress(Keys.O))
                OptionsMenu();
        }

        protected override void DrawScene(GameTime gameTime)
        {
        }

        protected override void PreTerminate()
        {
            base.PreTerminate();
            ObjectManager.Enabled = false;
        }

        protected override void Terminate()
        {
            //We will do this with a bitmask in Scene base class later
            UiManager.Clear();

            ObjectManager.RemoveAll(actor3D => actor3D != null);
            SoundManager.RemoveIf(s => s != null);

            ObjectManager.Enabled = true;
            ObjectManager.Enabled = true;
        }

        #endregion
    }
}