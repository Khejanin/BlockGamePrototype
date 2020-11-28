using System;
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
        #region Variables

        private Dictionary<string, Model> models;
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, DrawnActor3D> drawnActors;
        private MouseManager mouseManager;
        private Vector3 levelBounds;
        private string levelName;

        private bool optionsToggle;

        ////FOR SKYBOX____ TEMP
        private PrimitiveObject archetypalTexturedQuad, primitiveObject;
        private BasicTile test;
        private Transform3DCurve transform3DCurve;
        private Curve3DController curve3DController;
        private DrawnActor3D player;

        #endregion

        #region Constructor - Update - Draw

        public MainScene(Main game, string levelName) : base(game)
        {
            mouseManager = new MouseManager(game, false, new PhysicsManager(Game, StatusType.Update));
            this.levelName = @"Game\LevelFiles\" + levelName;
        }

        protected override void UpdateScene(GameTime gameTime)
        {
            
            
            if (curve3DController != null && curve3DController.ElapsedTimeInMs > 25000)
            {
                CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Curve Camera");
                CameraManager.ActiveCameraIndex = 0;
                transform3DCurve.Clear();
                curve3DController = null;
            }

            if (player == null)
            {
                DrawnActor3D drawnActor3D = ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player");
                if (CameraManager.ActiveCamera.ControllerList[0] is RotationAroundActor cam && drawnActor3D != null)
                {
                    cam.Target = drawnActor3D;
                    player = drawnActor3D;
                    drawnActor3D.StatusType = StatusType.Drawn | StatusType.Update;
                }     
            }

            if (player != null)
            {
                player.StatusType = CameraManager.ActiveCameraIndex switch
                {
                    0 => StatusType.Drawn | StatusType.Update,
                    1 => StatusType.Drawn,
                    _ => player.StatusType
                };
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
                SoundManager.VolumeUp();
            else if (KeyboardManager.IsFirstKeyPress(Keys.K))
                SoundManager.VolumeDown();
            //Pause/resume music
            if (KeyboardManager.IsFirstKeyPress(Keys.P))
                SoundManager.ChangeMusicState();

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

        #endregion

        #region Initialization

        public override void Initialize()
        {
            InitTransform3DCurve();
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();
            InitEvents();
            base.Initialize();
        }

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

            //Level Decorators
            InitLevelDecor();

            //grids
            InitGrid();

            InitUi();

            //Skybox
            InitArchetypalQuad();
            InitSkybox();

            //InitDecoration(1010);
        }

        private void InitDecoration(int n)
        {
            float min = -10;
            float max = 10;

            Vector2 minBounds = new Vector2(min, max);
            Vector2 xMaxBounds = new Vector2(levelBounds.X, levelBounds.X + max);
            Vector2 zMaxBounds = new Vector2(levelBounds.Z, levelBounds.Z + max);
            Vector2 yBounds = new Vector2(0, 5);

            DrawnActor3D decoActor = null;
            for (int i = 0; i < n; i++)
            {
                int random = MathHelperFunctions.Rnd.Next(2);
                int x = 0;
                int y = MathHelperFunctions.Rnd.Next((int) yBounds.X, (int) yBounds.Y);
                int z = 0;
                switch (random)
                {
                    case 0:
                        x = MathHelperFunctions.Rnd.Next((int) minBounds.X, (int) minBounds.Y);
                        break;
                    case 1:
                        z = MathHelperFunctions.Rnd.Next((int) minBounds.X, (int) minBounds.Y);
                        break;
                    case 2:
                        x = MathHelperFunctions.Rnd.Next((int) xMaxBounds.X, (int) xMaxBounds.Y);
                        break;
                    case 3:
                        z = MathHelperFunctions.Rnd.Next((int) zMaxBounds.X, (int) zMaxBounds.Y);
                        break;
                }

                Vector3 pos = new Vector3(x, y, z);
                random = MathHelperFunctions.Rnd.Next(3);

                decoActor = random switch
                {
                    0 => drawnActors["Fork"].Clone() as DrawnActor3D,
                    1 => drawnActors["Knife"].Clone() as DrawnActor3D,
                    2 => drawnActors["SinglePlate"].Clone() as DrawnActor3D,
                    _ => decoActor
                };

                if (decoActor != null)
                {
                    decoActor.Transform3D.Translation = pos;

                    ObjectManager.Add(decoActor);
                }
            }
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0), -Vector3.Forward, Vector3.Up);
            Camera3D mainCamera = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D, ProjectionParameters.StandardDeepFourThree, new Viewport(0, 0, 1024, 768));
            mainCamera.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera, KeyboardManager, 35, 20));
            CameraManager.Add(mainCamera);

            if (mainCamera.Clone() is Camera3D camera3D)
            {
                camera3D.ID = "FlightCamera";
                camera3D.ControllerList.Clear();
                camera3D.ControllerList.Add(new FlightController("FPC", ControllerType.FlightCamera, KeyboardManager, MouseManager, 0.01f, 0.01f, 0.01f));
                CameraManager.Add(camera3D);
            }

            /*camera3D = mainCamera.Clone() as Camera3D;
            if (camera3D != null)
            {
                camera3D.ID = "Curve Camera";
                camera3D.ControllerList.Clear();
                curve3DController = new Curve3DController("CurveCameraFlight", ControllerType.Curve, transform3DCurve);
                camera3D.ControllerList.Add(curve3DController);
                CameraManager.Add(camera3D);
            }*/

            CameraManager.ActiveCameraIndex = 0;
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
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors, textures));
            levelBounds = grid.GetGridBounds();
            grid.GenerateGrid(levelName);
        }

        private void InitStaticModels()
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            #region StaticTiles

            var effectParameters = new EffectParameters(ModelEffect, textures["Chocolate"], Color.White, 1);
            BasicTile chocolateTile = new BasicTile("ChocolateTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Cube"], ETileType.Static);
            chocolateTile.ControllerList.Add(new CustomBoxColliderController("BasicTileBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Ceramic"], Color.White, 1);
            BasicTile plateStackBasicTile = new BasicTile("plateStackTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["PlateStack"],
                ETileType.Static);
            plateStackBasicTile.ControllerList.Add(new CustomBoxColliderController("PlateStackBCC",
                ControllerType.Collider, ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ButtonTile button = new ButtonTile("Button", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Button"], ETileType.Button);
            button.ControllerList.Add(new CustomBoxColliderController("ButtonBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            SpikeTile spike = new SpikeTile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Pyramid"], ETileType.Spike);
            spike.ControllerList.Add(new CustomBoxColliderController("SpikeBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            PickupTile starPickup = new PickupTile("Star", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Mug"], ETileType.Star);
            starPickup.ControllerList.Add(new CustomBoxColliderController("StarPickUpBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            GoalTile goal = new GoalTile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["SugarBox"], ETileType.Win);
            goal.ControllerList.Add(new CustomBoxColliderController("GoalBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            CheckpointTile checkpoint = new CheckpointTile("Checkpoint", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Knife"], ETileType.Checkpoint);
            checkpoint.ControllerList.Add(new CustomBoxColliderController("CheckpointBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject forkModelObject = new ModelObject("fork", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Fork"]);
            //forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject plateModelObject = new ModelObject("plates", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["PlateStack"]);
            //plateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            ModelObject knifeModelObject = new ModelObject("knife", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Knife"]);
            //knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);

            ModelObject singlePlateModelObject = new ModelObject("singlePlate", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["SinglePlate"]);
            //singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion

            #region MovableTiles

            effectParameters = new EffectParameters(ModelEffect, textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                models["Cube"], ETileType.Attachable);
            attachableTile.ControllerList.Add(new CustomBoxColliderController("AttachableTileBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            attachableTile.ControllerList.Add(new TileMovementComponent("AttachableTileMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));

            effectParameters = new EffectParameters(ModelEffect, textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn, transform3D, effectParameters, models["Cube"], ETileType.PlayerStart);
            playerTile.ControllerList.Add(new CustomBoxColliderController("PlayerBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            playerTile.ControllerList.Add(new PlayerController("PlayerPC", ControllerType.Player, KeyboardManager, CameraManager));
            playerTile.ControllerList.Add(new SoundController("PlayerSC", ControllerType.Sound, KeyboardManager, SoundManager, "playerMove", "playerAttach"));
            playerTile.ControllerList.Add(new TileMovementComponent("PlayerMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));
            playerTile.ControllerList.Add(new RotationComponent("PlayerRC", ControllerType.Rotation));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            EnemyTile enemy = new EnemyTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Drop"], ETileType.Enemy);
            enemy.ControllerList.Add(new CustomBoxColliderController("EnemyBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));
            enemy.ControllerList.Add(new TileMovementComponent("EnemyMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));
            enemy.ControllerList.Add(new RotationComponent("EnemyRC", ControllerType.Rotation));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            MovingPlatformTile platform = new MovingPlatformTile("MovingPlatform", ActorType.Platform, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                models["SinglePlate"], ETileType.MovingPlatform, new Vector3(3, 0, 0));
            platform.ControllerList.Add(new CustomBoxColliderController("PlatformBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            platform.ControllerList.Add(new TileMovementComponent("PlatformMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle)));

            #endregion

            drawnActors = new Dictionary<string, DrawnActor3D>
            {
                {"StaticTile", chocolateTile},
                {"PlateStackTile", plateStackBasicTile},
                {"AttachableBlock", attachableTile},
                {"PlayerBlock", playerTile},
                {"GoalTile", goal},
                {"EnemyTile", enemy},
                {"ButtonTile", button},
                {"MovingPlatformTile", platform},
                {"SpikeTile", spike},
                {"StarPickupTile", starPickup},
                {"CheckpointTile", checkpoint},
                {"Knife", knifeModelObject},
                {"Fork", forkModelObject},
                {"SinglePlate", singlePlateModelObject}
            };

            InitCoffee();
        }

        private void InitCoffee()
        {
            EffectParameters staticColorEffect = new EffectParameters(ModelEffect,
                textures["Chocolate"], /*new Color(111, 78, 55)*/Color.White, .8f);
            Transform3D transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            ModelObject coffee = new ModelObject("coffee - plane", ActorType.Primitive,
                StatusType.Update | StatusType.Drawn, transform3D, staticColorEffect, models["CoffeePlane"]);

            ObjectManager.Add(coffee);
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

        private void InitLevelDecor()
        {
            float size = 1.5f;
            Vector3 scale = new Vector3(size, size, size);

            EffectParameters effectParameters = new EffectParameters(ModelEffect, textures["Wood"], Color.White, 1);
            var transform3D = new Transform3D(new Vector3(10, -15, 15), Vector3.UnitZ, Vector3.UnitY);

            BasicTile table = new BasicTile("Table", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, models["Table"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            ObjectManager.Add(table);

            effectParameters = new EffectParameters(ModelEffect, textures["Ceramic"], Color.White, 1);
            BasicTile cups = new BasicTile("Cups", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, models["Cups"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            drawnActors.Add("Cups", cups);
            ObjectManager.Add(cups);

            effectParameters = new EffectParameters(ModelEffect, textures["WhiteChocolate"], Color.White, 1);
            BasicTile choco = new BasicTile("Chocolate", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, models["Chocolate"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            ObjectManager.Add(choco);

            effectParameters = new EffectParameters(ModelEffect, textures["Checkers"], Color.White, 1);
            BasicTile Cat = new BasicTile("Cat", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, models["Cat"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            ObjectManager.Add(Cat);

            effectParameters = new EffectParameters(ModelEffect, textures["blackTile"], Color.White, 1);
            BasicTile Catbed = new BasicTile("Catbed", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, models["CatBed"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            ObjectManager.Add(Catbed);
        }

        //TEMP
        private void TestingPlatform()
        {
            EffectParameters effectParameters = new EffectParameters(ModelEffect, textures["Box"], Color.White, 1);
            var transform3D = new Transform3D(new Vector3(5, 0, 0), Vector3.UnitZ, Vector3.UnitY);
            this.test = new BasicTile("StaticTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Knife"], ETileType.Static);
            this.test.ControllerList.Add(new CustomBoxColliderController("testBCC", ControllerType.Collider,
                ColliderShape.Cube, 1f));
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

            heightFromBottom = 75;
            string text = "Moves";
            Vector2 position = new Vector2(halfWidth, screenHeight - heightFromBottom);
            UiText uiText = new UiText(StatusType.Drawn, text, Game.Fonts["UI"], position, Color.Black);
            UiManager.AddUiElement("Moves", uiText);

            heightFromBottom = 25;

            text = SceneName;
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
            SoundManager.Add(new Sounds(track06, "endTheme", ActorType.SpecialTrack, StatusType.Update));

            SoundManager.NextSong();
        }

        private void LoadTextures()
        {
            Texture2D cubeTexture = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube");
            Texture2D basicBgFloor = Content.Load<Texture2D>("Assets/Textures/Block/BlockTextureBlue");
            Texture2D whiteSquareTexture = Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare");
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

            Texture2D choc = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile");
            Texture2D choc4X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile4x");
            Texture2D choc8X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile8x");
            Texture2D chocW = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile-white");
            Texture2D chocW4X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile-white4x");
            Texture2D chocW8X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/choco-tile-white8x");
            Texture2D chocB = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/dark_choco-tile");
            Texture2D chocB4X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/dark_choco-tile4x");
            Texture2D chocB8X = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/dark_choco-tile8x");

            Texture2D ceramic = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/ceramicColoring");

            Texture2D sugarWhite = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/sugar01");
            Texture2D sugarBrown = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/sugar02");

            Texture2D wood = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/wood");
            Texture2D blackTile = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/blackTile");
            Texture2D checkers = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/checkers");

            Texture2D newChoco1 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/big-choco");
            Texture2D newChoco2 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/big-choco-white");
            Texture2D newChoco3 = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/big-choco-dark");


            textures = new Dictionary<string, Texture2D>
            {
                {"Player", cubeTexture}, {"Attachable", cubeTexture}, {"Finish", cubeTexture}, {"Box", basicBgFloor},
                {"WhiteSquare", whiteSquareTexture},
                {"Wall1", wall}, {"Wall", floor}, {"Floor", floor},
                {"Circle", circle}, {"Logo", logo}, {"LogoMirror", logoMirror},
                {"kWall1", panel1}, {"kWall2", panel2}, {"kWall3", panel3}, {"kWall4", panel4}, {"floor2", floor1},
                {"options", options},
                {"optionsButton", optionsButton},
                {"Chocolate", choc}, {"Chocolate4x", choc4X}, {"Chocolate8x", choc8X},
                {"WhiteChocolate", chocW}, {"WhiteChocolate4x", chocW4X}, {"WhiteChocolate8x", chocW8X},
                {"DarkChocolate", chocB}, {"DarkChocolate4x", chocB4X}, {"DarkChocolate8x", chocB8X},
                {"SugarW", sugarWhite}, {"SugarB", sugarBrown},
                {"Ceramic", ceramic}, {"Wood", wood}, {"blackTile", blackTile}, {"Checkers", checkers},
                {"DChocolateLarge", newChoco3}, {"WChocolateLarge", newChoco2}, {"ChocolateLarge", newChoco1}
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
            Model table = Content.Load<Model>("Assets/Models/Decor/table03");
            Model cups = Content.Load<Model>("Assets/Models/Decor/cups03");
            Model chocolate = Content.Load<Model>("Assets/Models/Decor/choco03");
            Model cat = Content.Load<Model>("Assets/Models/Decor/cat03");
            Model catbed = Content.Load<Model>("Assets/Models/Decor/catbed03");
            Model coffeePlane = Content.Load<Model>("Assets/Models/plane");


            models = new Dictionary<string, Model>
            {
                {"RedCube", redCube}, {"Player", blueCube}, {"Cube", boxModel}, {"Enemy", enemyModel},
                {"Button", buttonModel}, {"Drop", dropWithEyesModel}, {"Fork", forkModel}, {"Knife", knifeModel},
                {"Mug", mugModel}, {"PlateStack", plateStackModel}, {"PlayerModel", playerModel},
                {"Pyramid", pyramidModel}, {"SinglePlate", singlePlateCube}, {"SugarBox", sugarBoxCube},
                {"Table", table}, {"Cups", cups}, {"Chocolate", chocolate}, {"Cat", cat}, {"CatBed", catbed},
                {"CoffeePlane", coffeePlane}
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Override Methodes

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