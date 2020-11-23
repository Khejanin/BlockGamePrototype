using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
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


        ////FOR SKYBOX____ TEMP
        private PrimitiveObject archetypalTexturedQuad, primitiveObject;
        private BasicTile test;
        private Transform3DCurve transform3DCurve;
        private Curve3DController curve3DController;

        public MainScene(Main game, string levelName) : base(game)
        {
            mouseManager = new MouseManager(game, false);
            this.levelName = @"Game\LevelFiles\" + levelName;
        }

        public override void Initialize()
        {
            InitTransform3DCurve();
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();

            InitEvents();
        }

        private void SetTargetToCamera()
        {
            List<DrawnActor3D> players = ObjectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (players.Count > 0)
            {
                if (CameraManager.ActiveCamera.ControllerList[0] is RotationAroundActor cam) cam.Target = players[0];

                players[0].StatusType = StatusType.Drawn | StatusType.Update;
            }
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
            InitStaticModels();
            TestingPlatform();

            //grids
            InitGrid();

            InitUi();

            //Skybox
            InitArchetypalQuad();
            InitSkybox();
        }


        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera,
                KeyboardManager, 35, 20));

            CameraManager.Add(camera3D);

            transform3D = new Transform3D(new Vector3(0, 0, 0), -Vector3.Forward, Vector3.Up);
            camera3D = new Camera3D("Curve Camera", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            curve3DController = new Curve3DController("CurveCameraFlight", ControllerType.Curve, transform3DCurve);
            camera3D.ControllerList.Add(curve3DController);

            CameraManager.Add(camera3D);
            CameraManager.ActiveCameraIndex = 1;
        }

        private void InitTransform3DCurve()
        {
            transform3DCurve = new Transform3DCurve(CurveLoopType.Oscillate);
            transform3DCurve.Add(new Vector3(0, 20, 2), -Vector3.Up, Vector3.Forward, 0);
            transform3DCurve.Add(new Vector3(0, 14, 2), -Vector3.Up, Vector3.Forward, 3000);
            transform3DCurve.Add(new Vector3(0, 10, 2), Vector3.Right, Vector3.Up, 5000);
            transform3DCurve.Add(new Vector3(10, 10, 2), Vector3.Right, Vector3.Up, 8000);
            transform3DCurve.Add(new Vector3(15, 10, 2), new Vector3(0, -0.5f, 0.5f), Vector3.Up, 11000);
            transform3DCurve.Add(new Vector3(18, 10, 10), new Vector3(0, -0.5f, 1), Vector3.Up, 14000);
            transform3DCurve.Add(new Vector3(18, 10, 23), new Vector3(0, -1, 1), Vector3.Up, 16000);
            transform3DCurve.Add(new Vector3(18, 10, 23), -Vector3.Right, Vector3.Up, 19000);
            transform3DCurve.Add(new Vector3(3, 5, 23), -Vector3.Up, -Vector3.Right, 23000);
            transform3DCurve.Add(new Vector3(3, 5, 23), -Vector3.Up, -Vector3.Right, 25000);
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors));
            grid.GenerateGrid(levelName);
        }

        private void InitStaticModels()
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            #region StaticTiles

            var effectParameters = new EffectParameters(ModelEffect, textures["Chocolate"], Color.White, 1);
            BasicTile chocoloateTile = new BasicTile("ChocolateTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Cube"]);
            chocoloateTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["WChocolate"], Color.White, 1);
            BasicTile whiteChocolateTile = new BasicTile("WhiteChocolateTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Cube"]);
            whiteChocolateTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["DChocolate"], Color.White, 1);
            BasicTile darkChocolateTile = new BasicTile("WhiteChocolateTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Cube"]);
            darkChocolateTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ButtonTile button = new ButtonTile("Button", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Button"]);
            button.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            SpikeTile spike = new SpikeTile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Pyramid"]);
            spike.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            StarPickupTile starPickup = new StarPickupTile("Star", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Mug"]);
            starPickup.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f,
                ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            GoalTile goal = new GoalTile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["SugarBox"]);
            goal.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject forkModelObject = new ModelObject("fork", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Fork"]);
            forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject plateModelObject = new ModelObject("plates", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["PlateStack"]);
            plateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject knifeModelObject = new ModelObject("knife", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Knife"]);
            knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject singlePlateModelObject = new ModelObject("singlePlate", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["SinglePlate"]);
            singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion

            #region MovableTiles

            effectParameters = new EffectParameters(ModelEffect, textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Cube"]);
            attachableTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            attachableTile.ControllerList.Add(new TileMovementComponent(300, new Curve1D(CurveLoopType.Cycle), true));

            effectParameters = new EffectParameters(ModelEffect, textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn,
                transform3D, effectParameters, models["Cube"]);
            playerTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            playerTile.ControllerList.Add(new PlayerController(KeyboardManager, GamePadManager));
            playerTile.ControllerList.Add(new SoundController(KeyboardManager, SoundManager, "playerMove",
                "playerAttach"));
            playerTile.ControllerList.Add(new RotationComponent());
            playerTile.ControllerList.Add(new TileMovementComponent(300, new Curve1D(CurveLoopType.Cycle), true));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            EnemyTile enemy = new EnemyTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Drop"]);
            enemy.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f, ColliderType.CheckOnly));
            enemy.ControllerList.Add(new TileMovementComponent(300, new Curve1D(CurveLoopType.Cycle), true));
            enemy.ControllerList.Add(new RotationComponent());

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            MovingPlatformTile platform = new MovingPlatformTile("MovingPlatform", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["SinglePlate"]);
            platform.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            platform.ControllerList.Add(new TileMovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            #endregion

            drawnActors = new Dictionary<string, DrawnActor3D>
            {
                {"StaticTile", chocoloateTile}, {"WhiteChocolateTile", whiteChocolateTile},
                {"DarkChocolateTile", darkChocolateTile}, {"AttachableBlock", attachableTile},
                {"PlayerBlock", playerTile},
                {"GoalTile", goal}, {"EnemyTile", enemy}, {"ButtonTile", button}, {"MovingPlatformTile", platform},
                {"SpikeTile", spike}, {"StarPickupTile", starPickup}
            };
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

            EffectParameters effectParameters = new EffectParameters(UnlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject actor = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            ObjectManager.Add(actor);
        }

        //TEMP
        private void TestingPlatform()
        {
            EffectParameters effectParameters = new EffectParameters(ModelEffect, textures["Box"], Color.White, 1);
            var transform3D = new Transform3D(new Vector3(5, 0, 0), Vector3.UnitZ, Vector3.UnitY);
            this.test = new BasicTile("StaticTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Knife"]);
            this.test.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            drawnActors.Add("StaticTile2", test);
            ObjectManager.Add(test);
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

            text = "5";
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

            EffectParameters effectParameters =
                new EffectParameters(unlitTexturedEffect, textures["Wall"], Color.White, 1);

            IVertexData vertexData =
                new VertexData<VertexPositionColorTexture>(vertices, PrimitiveType.TriangleStrip, 2);

            archetypalTexturedQuad = new PrimitiveObject("original texture quad", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, vertexData);
        }

        private void InitSkybox()
        {
            float worldScale = 100;

            //Floor
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Floor";
                primitiveObject.EffectParameters.Texture = textures["floor2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 90);
                primitiveObject.Transform3D.Translation = new Vector3(0, -worldScale / 2.0f, 0);
                ObjectManager.Add(primitiveObject);
            }

            //Back 
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "back";
                primitiveObject.EffectParameters.Texture = textures["kWall1"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
                ObjectManager.Add(primitiveObject);
            }

            //Front 
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "front";
                primitiveObject.EffectParameters.Texture = textures["kWall2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
                ObjectManager.Add(primitiveObject);
            }

            //RWall
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Right wall";
                primitiveObject.EffectParameters.Texture = textures["kWall3"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(270, 90, 270);
                primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
                ObjectManager.Add(primitiveObject);
            }

            //LWall
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Left wall";
                primitiveObject.EffectParameters.Texture = textures["kWall4"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(-270, -90, 270);
                primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
                ObjectManager.Add(primitiveObject);
            }
        }

        #endregion


        #region LoadContent

        private void LoadSounds()
        {
            SoundManager.StopSong();
            //step 1 - load songs
            SoundEffect track01 = Content.Load<SoundEffect>("Assets/GameTracks/testTrack01");
            SoundEffect track02 = Content.Load<SoundEffect>("Assets/GameTracks/testTrack02");
            SoundEffect track03 = Content.Load<SoundEffect>("Assets/GameTracks/testTrack03");
            SoundEffect track04 = Content.Load<SoundEffect>("Assets/Sound/Knock04");
            SoundEffect track05 = Content.Load<SoundEffect>("Assets/Sound/Click02");
            SoundEffect track06 = Content.Load<SoundEffect>("Assets/GameTracks/testTrack06");

            //Step 2- Make into sounds
            SoundManager.Add(new Sounds(track01, "gameTrack01", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track02, "gameTrack02", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track03, "gameTrack03", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track04, "playerMove", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track05, "playerAttach", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track06, "endTheme", ActorType.specialTrack, StatusType.Update));

            SoundManager.NextSong();
        }

        private void LoadTextures()
        {
            Texture2D cubeTexture = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube");
            Texture2D basicBgFloor = Content.Load<Texture2D>("Assets/Textures/Block/BlockTextureBlue");
            Texture2D whiteSquareTexture = Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare");
            Texture2D compassTexture = Content.Load<Texture2D>("Assets/Textures/Base/BasicCompass");
            Texture2D circle = Content.Load<Texture2D>("Assets/Textures/circle");
            Texture2D logo = Content.Load<Texture2D>("Assets/Textures/Menu/logo");
            Texture2D logoMirror = Content.Load<Texture2D>("Assets/Textures/Menu/logo_mirror");
            Texture2D options = Content.Load<Texture2D>("Assets/Textures/Menu/menubaseres");
            Texture2D optionsButton = Content.Load<Texture2D>("Assets/Textures/Menu/button");

            Texture2D wall = Content.Load<Texture2D>("Assets/Textures/Block/block_green");
            Texture2D floor = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");

            Texture2D panel1 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall1");
            Texture2D panel2 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall2");
            Texture2D panel3 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall3");
            Texture2D panel4 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall4");
            Texture2D floor1 = Content.Load<Texture2D>("Assets/Textures/Skybox/tiles");

            Texture2D choc1 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile");
            Texture2D choc2 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile-white");
            Texture2D choc3 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile-white");

            Texture2D ceramic = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/ceramicColoringBaked");
            Texture2D sugarWhite = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/sugar01");
            Texture2D sugarBrown = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/sugar02");

            textures = new Dictionary<string, Texture2D>
            {
                {"Player", cubeTexture}, {"Attachable", cubeTexture}, {"Finish", cubeTexture}, {"Box", basicBgFloor},
                {"WhiteSquare", whiteSquareTexture}, {"Compass", compassTexture}, {"Wall1", wall}, {"Wall", floor},
                {"Floor", floor}, {"Circle", circle}, {"Logo", logo}, {"LogoMirror", logoMirror}, {"kWall1", panel1},
                {"kWall2", panel2}, {"kWall3", panel3}, {"kWall4", panel4}, {"floor2", floor1}, {"options", options},
                {"optionsButton", optionsButton}, {"Chocolate", choc1}, {"WChocolate", choc2}, {"Ceramic", ceramic},
                {"SugarW", sugarWhite}, {"SugarB", sugarBrown}, {"DChocolate", choc3}
            };
        }

        private void LoadModels()
        {
            Model blueCube = Content.Load<Model>("Assets/Models/blueCube");
            Model boxModel = Content.Load<Model>("Assets/Models/box2");
            Model buttonModel = Content.Load<Model>("Assets/Models/Button");
            Model dropWithEyesModel = Content.Load<Model>("Assets/Models/DropWithEyes");
            Model enemyModel = Content.Load<Model>("Assets/Models/Enemy");
            Model forkModel = Content.Load<Model>("Assets/Models/Fork");
            Model knifeModel = Content.Load<Model>("Assets/Models/Knife");
            Model mugModel = Content.Load<Model>("Assets/Models/Mug");
            Model plateStackModel = Content.Load<Model>("Assets/Models/PlateStack");
            Model playerModel = Content.Load<Model>("Assets/Models/Player");
            Model pyramidModel = Content.Load<Model>("Assets/Models/Pyramid");
            Model redCube = Content.Load<Model>("Assets/Models/RedCube");
            Model singlePlateCube = Content.Load<Model>("Assets/Models/SinglePlate");
            Model sugarBoxCube = Content.Load<Model>("Assets/Models/SugarBox");


            models = new Dictionary<string, Model>
            {
                {"RedCube", redCube}, {"Player", blueCube}, {"Cube", boxModel}, {"Enemy", enemyModel},
                {"Button", buttonModel}, {"Drop", dropWithEyesModel}, {"Fork", forkModel}, {"Knife", knifeModel},
                {"Mug", mugModel}, {"PlateStack", plateStackModel}, {"PlayerModel", playerModel},
                {"Pyramid", pyramidModel}, {"SinglePlate", singlePlateCube}, {"SugarBox", sugarBoxCube}
            };
        }

        private void InitEvents()
        {
            EventManager.RegisterListener<GameStateMessageEventInfo>(OnGameStateMessageReceived);
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

            if (curve3DController != null && curve3DController.ElapsedTimeInMs > 25000)
            {
                transform3DCurve.Clear();
                curve3DController = null;
                EventManager.FireEvent(new CameraEvent());
                CameraManager.ActiveCameraIndex = 0;
                SetTargetToCamera();
            }


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

            //Test
            if (KeyboardManager.IsFirstKeyPress(Keys.G))
                this.test.Transform3D.TranslateBy(new Vector3(0, -1, 0));
            else if (KeyboardManager.IsFirstKeyPress(Keys.H))
                this.test.Transform3D.TranslateBy(new Vector3(0, 1, 0));
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
        }

        #endregion
    }
}