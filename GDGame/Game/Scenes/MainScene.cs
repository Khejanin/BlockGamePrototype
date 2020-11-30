using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
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

namespace GDGame.Scenes
{
    public class MainScene : Scene
    {
        #region 05. Private variables

        private readonly string levelName;
        private readonly MouseManager mouseManager;

        ////FOR SKYBOX____ TEMP
        private PrimitiveObject archetypalTexturedQuad, primitiveObject;

        private Curve3DController curve3DController;

        private Dictionary<string, DrawnActor3D> drawnActors;
        private Vector3 levelBounds;

        private bool optionsToggle;
        private DrawnActor3D player;
        private BasicTile test;
        private Transform3DCurve transform3DCurve;

        #endregion

        #region Constructors

        public MainScene(Main main, string levelName, SceneType sceneType = SceneType.Game,  bool unloadsContent = false) : base(main, sceneType, unloadsContent)
        {
            this.levelName = @"Game\LevelFiles\" + levelName;
        }

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

            BasicEffect unlitTexturedEffect = new BasicEffect(Main.Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            EffectParameters effectParameters = new EffectParameters(unlitTexturedEffect, Main.Textures["kWall1"], Color.White, 1);
            IVertexData vertexData =
                new VertexData<VertexPositionColorTexture>(vertices, PrimitiveType.TriangleStrip, 2);
            archetypalTexturedQuad = new PrimitiveObject("original texture quad", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                vertexData);
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0), -Vector3.Forward, Vector3.Up);
            Camera3D mainCamera = new Camera3D("MainCamera", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree,
                new Viewport(0, 0, 1024, 768));
            mainCamera.ControllerList.Add(new RotationAroundActor("RAAC", ControllerType.FlightCamera,
                Main.KeyboardManager, 35, 20));
            Main.CameraManager.Add(mainCamera);

            if (mainCamera.Clone() is Camera3D camera3D)
            {
                camera3D.ID = "FlightCamera";
                camera3D.ControllerList.Clear();
                camera3D.ControllerList.Add(new FlightController("FPC", ControllerType.FlightCamera,
                    Main.KeyboardManager, Main.MouseManager, 0.01f, 0.01f, 0.01f));
                Main.CameraManager.Add(camera3D);
            }

            camera3D = mainCamera.Clone() as Camera3D;
            if (camera3D != null)
            {
                camera3D.ID = "CurveCamera";
                camera3D.ControllerList.Clear();
                curve3DController = new Curve3DController("CCFC", ControllerType.Curve, transform3DCurve);
                camera3D.ControllerList.Add(curve3DController);
                Main.CameraManager.Add(camera3D);
            }

            Main.CameraManager.ActiveCameraIndex = 2;
        }

        private void InitCoffee()
        {
            EffectParameters staticColorEffect = new EffectParameters(Main.ModelEffect, Main.Textures["Chocolate"], Color.White, .8f);
            Transform3D transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            ModelObject coffee = new ModelObject("coffee - plane", ActorType.Primitive, StatusType.Update | StatusType.Drawn, transform3D, staticColorEffect,
                Main.Models["CoffeePlane"]);
            Main.ObjectManager.Add(coffee);
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

                    Main.ObjectManager.Add(decoActor);
                }
            }
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

            uiSceneManager.InitUi();

            //Skybox
            InitArchetypalQuad();
            InitSkyBox();

            //InitDecoration(1010);
        }

        private void InitEvents()
        {
            EventManager.RegisterListener<GameStateMessageEventInfo>(OnGameStateMessageReceived);
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(Main.ObjectManager, drawnActors, Main.Textures));
            levelBounds = grid.GetGridBounds();
            grid.GenerateGrid(levelName);
        }

        private void InitHelpers()
        {
            //step 1 - vertices
            VertexPositionColor[] vertices = VertexFactory.GetVerticesPositionColorOriginHelper(
                out PrimitiveType primitiveType, out int primitiveCount);

            //step 2 - make vertex data that provides Draw()
            IVertexData vertexData = new VertexData<VertexPositionColor>(vertices,
                primitiveType, primitiveCount);

            //step 3 - make the primitive object
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 10),
                Vector3.Zero, new Vector3(1, 1, 1),
                Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(Main.UnlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject actor = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            Main.ObjectManager.Add(actor);
        }

        public override void Initialize()
        {
            InitTransform3DCurve();
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();
            InitEvents();
            base.Initialize();
        }

        private void InitLevelDecor()
        {
            float size = 1.5f;
            Vector3 scale = new Vector3(size, size, size);

            EffectParameters effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Wood"], Color.White, 1);
            Transform3D transform3D = new Transform3D(new Vector3(10, -15, 15), Vector3.UnitZ, Vector3.UnitY);

            BasicTile table = new BasicTile("Table", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Table"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            Main.ObjectManager.Add(table);

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Ceramic"], Color.White, 1);
            BasicTile cups = new BasicTile("Cups", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Cups"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            drawnActors.Add("Cups", cups);
            Main.ObjectManager.Add(cups);

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["WhiteChocolate"], Color.White, 1);
            BasicTile choco = new BasicTile("Chocolate", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Chocolate"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            Main.ObjectManager.Add(choco);

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Checkers"], Color.White, 1);
            BasicTile cat = new BasicTile("Cat", ActorType.Primitive, StatusType.Drawn, transform3D, effectParameters,
                Main.Models["Cat"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            Main.ObjectManager.Add(cat);

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["blackTile"], Color.White, 1);
            BasicTile catBed = new BasicTile("Catbed", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["CatBed"], ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            Main.ObjectManager.Add(catBed);
        }

        private void InitLoadContent()
        {
            LoadTextures();
            LoadSounds();
            LoadModels();
        }

        private void InitSkyBox()
        {
            float worldScale = 100;

            //Floor
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Floor";
                primitiveObject.EffectParameters.Texture = Main.Textures["floor2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 90);
                primitiveObject.Transform3D.Translation = new Vector3(0, -worldScale / 2.0f, 0);
                Main.ObjectManager.Add(primitiveObject);
            }

            //Back
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "back";
                primitiveObject.EffectParameters.Texture = Main.Textures["kWall1"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
                Main.ObjectManager.Add(primitiveObject);
            }

            //Front
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "front";
                primitiveObject.EffectParameters.Texture = Main.Textures["kWall2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
                Main.ObjectManager.Add(primitiveObject);
            }

            //RWall
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Right wall";
                primitiveObject.EffectParameters.Texture = Main.Textures["kWall3"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(-90, 90, -90);
                primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
                Main.ObjectManager.Add(primitiveObject);
            }

            //LWall
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Left wall";
                primitiveObject.EffectParameters.Texture = Main.Textures["kWall4"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, -90);
                primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
                Main.ObjectManager.Add(primitiveObject);
            }
        }

        private void InitStaticModels()
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            #region StaticTiles

            EffectParameters effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Chocolate"], Color.White, 1);
            BasicTile chocolateTile = new BasicTile("ChocolateTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Cube"],
                ETileType.Static);
            chocolateTile.ControllerList.Add(new CustomBoxColliderController("BasicTileBCC", ControllerType.Collider, ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Ceramic"], Color.White, 1);
            BasicTile plateStackBasicTile = new BasicTile("plateStackTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["PlateStack"], ETileType.Static);
            plateStackBasicTile.ControllerList.Add(new CustomBoxColliderController("PlateStackBCC", ControllerType.Collider, ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ButtonTile button = new ButtonTile("Button", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Button"],
                ETileType.Button);
            button.ControllerList.Add(new CustomBoxColliderController("ButtonBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            SpikeTile spike = new SpikeTile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Pyramid"],
                ETileType.Spike);
            spike.ControllerList.Add(new CustomBoxColliderController("SpikeBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            PickupTile starPickup = new PickupTile("Star", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Mug"],
                ETileType.Star);
            starPickup.ControllerList.Add(new CustomBoxColliderController("StarPickUpBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            GoalTile goal = new GoalTile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["SugarBox"], ETileType.Win);
            goal.ControllerList.Add(new CustomBoxColliderController("GoalBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            CheckpointTile checkpoint = new CheckpointTile("Checkpoint", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["Knife"], ETileType.Checkpoint);
            checkpoint.ControllerList.Add(new CustomBoxColliderController("CheckpointBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ModelObject forkModelObject = new ModelObject("fork", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Fork"]);
            //forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ModelObject plateModelObject = new ModelObject("plates", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["PlateStack"]);
            //plateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ModelObject knifeModelObject = new ModelObject("knife", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Knife"]);
            //knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ModelObject singlePlateModelObject = new ModelObject("singlePlate", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["SinglePlate"]);
            //singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion StaticTiles

            #region MovableTiles

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["Cube"], ETileType.Attachable);
            attachableTile.ControllerList.Add(new CustomBoxColliderController("AttachableTileBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            attachableTile.ControllerList.Add(new TileMovementComponent("AttachableTileMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn, transform3D, effectParameters, Main.Models["Cube"], ETileType.PlayerStart);
            playerTile.ControllerList.Add(new CustomBoxColliderController("PlayerBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            playerTile.ControllerList.Add(new PlayerController("PlayerPC", ControllerType.Player, Main.KeyboardManager, Main.CameraManager));
            playerTile.ControllerList.Add(new SoundController("PlayerSC", ControllerType.Sound, Main.KeyboardManager, Main.SoundManager, "playerMove", "playerAttach"));
            playerTile.ControllerList.Add(new TileMovementComponent("PlayerMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));
            playerTile.ControllerList.Add(new RotationComponent("PlayerRC", ControllerType.Rotation));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            EnemyTile enemy = new EnemyTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Drop"],
                ETileType.Enemy);
            enemy.ControllerList.Add(new CustomBoxColliderController("EnemyBCC", ControllerType.Collider, ColliderShape.Cube, 1f, ColliderType.CheckOnly));
            enemy.ControllerList.Add(new TileMovementComponent("EnemyMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle), true));
            enemy.ControllerList.Add(new RotationComponent("EnemyRC", ControllerType.Rotation));

            effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            MovingPlatformTile platform = new MovingPlatformTile("MovingPlatform", ActorType.Platform, StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["SinglePlate"], ETileType.MovingPlatform, 3, -1); //-1 = X, 1 = Y, 0 = Z
            platform.ControllerList.Add(new CustomBoxColliderController("PlatformBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            platform.ControllerList.Add(new TileMovementComponent("PlatformMC", ControllerType.Movement, 300, new Curve1D(CurveLoopType.Cycle)));

            #endregion MovableTiles

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

        #endregion

        #region Override Methode


        protected override void PreTerminate()
        {
            base.PreTerminate();
            Main.ObjectManager.Enabled = false;
        }

        protected override void Terminate()
        {
            //We will do this with a bitmask in Scene base class later

            Main.ObjectManager.RemoveAll(actor3D => actor3D != null);

            Main.ObjectManager.Enabled = true;

            Main.Models.Dispose();
            Main.Textures.Dispose();
        }

        protected override void UpdateScene()
        {
            if (curve3DController != null && curve3DController.ElapsedTimeInMs > 25000)
            {
                Main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Curve Camera");
                Main.CameraManager.ActiveCameraIndex = 0;
                transform3DCurve.Clear();
                curve3DController = null;
            }

            if (player == null)
            {
                DrawnActor3D drawnActor3D = Main.ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player");
                if (Main.CameraManager.ActiveCamera.ControllerList[0] is RotationAroundActor cam &&
                    drawnActor3D != null)
                {
                    cam.Target = drawnActor3D;
                    player = drawnActor3D;
                    drawnActor3D.StatusType = StatusType.Drawn | StatusType.Update;
                }
            }

            if (player != null)
                player.StatusType = Main.CameraManager.ActiveCameraIndex switch
                {
                    0 => StatusType.Drawn | StatusType.Update,
                    1 => StatusType.Drawn,
                    2 => StatusType.Drawn,
                    _ => player.StatusType
                };

            if (Main.KeyboardManager.IsFirstKeyPress(Keys.C)) Main.CameraManager.CycleActiveCamera();

            //Cycle Through Audio
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.M))
                EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.PlayNextMusic });
            //Stop Music
            if (Main.KeyboardManager.IsKeyDown(Keys.N))
                EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.PauseMusic });
            //Volume Changes
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.L))
                EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.IncreaseVolume, soundVolumeType = SoundVolumeType.Master });
            else if (Main.KeyboardManager.IsFirstKeyPress(Keys.K))
                EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.DecreaseVolume, soundVolumeType = SoundVolumeType.Master });
            //Pause/resume music
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.P))
                EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.ToggleMusicPlayback, soundVolumeType = SoundVolumeType.Master });

            //Test
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.G))
                test.Transform3D.TranslateBy(new Vector3(0, -1, 0));
            else if (Main.KeyboardManager.IsFirstKeyPress(Keys.H))
                test.Transform3D.TranslateBy(new Vector3(0, 1, 0));
        }

        #endregion

        #region Load Methods

        private void LoadModels()
        {
            Main.Models.Load("Assets/Models/box2", "Cube");
            Main.Models.Load("Assets/Models/Button");
            Main.Models.Load("Assets/Models/DropWithEyes", "Drop");
            Main.Models.Load("Assets/Models/Fork");
            Main.Models.Load("Assets/Models/Knife");
            Main.Models.Load("Assets/Models/Mug");
            Main.Models.Load("Assets/Models/PlateStack");
            Main.Models.Load("Assets/Models/Player");
            Main.Models.Load("Assets/Models/Pyramid");
            Main.Models.Load("Assets/Models/SinglePlate");
            Main.Models.Load("Assets/Models/SugarBox");
            Main.Models.Load("Assets/Models/Decor/table03", "Table");
            Main.Models.Load("Assets/Models/Decor/cups03", "Cups");
            Main.Models.Load("Assets/Models/Decor/choco03", "Chocolate");
            Main.Models.Load("Assets/Models/Decor/cat03", "Cat");
            Main.Models.Load("Assets/Models/Decor/catbed03", "CatBed");
            Main.Models.Load("Assets/Models/plane", "CoffeePlane");
        }

        private void LoadSounds()
        {
            SoundEffect track01 = Main.Content.Load<SoundEffect>("Assets/GameTracks/testTrack01");
            SoundEffect track02 = Main.Content.Load<SoundEffect>("Assets/GameTracks/testTrack02");
            SoundEffect track03 = Main.Content.Load<SoundEffect>("Assets/GameTracks/testTrack03");
            SoundEffect track04 = Main.Content.Load<SoundEffect>("Assets/Sound/Knock04");
            SoundEffect track05 = Main.Content.Load<SoundEffect>("Assets/Sound/Click02");
            SoundEffect track06 = Main.Content.Load<SoundEffect>("Assets/GameTracks/testTrack06");
            
            Main.SoundManager.AddMusic("gameTrack01", track01);
            Main.SoundManager.AddMusic("gameTrack02", track02);
            Main.SoundManager.AddMusic("gameTrack03", track03);
            Main.SoundManager.AddMusic("endTheme", track06);
            Main.SoundManager.AddSoundEffect(SfxType.PlayerMove, track04);
            Main.SoundManager.AddSoundEffect(SfxType.PlayerAttach, track05);

            Main.SoundManager.StartMusicQueue();
        }

        private void LoadTextures()
        {
            Main.Textures.Load("Assets/Textures/Props/GameTextures/TextureCube", "Finish");

            Main.Textures.Load("Assets/Textures/Base/WhiteSquare");

            Main.Textures.Load("Assets/Textures/Menu/menubaseres", "options");
            Main.Textures.Load("Assets/Textures/Menu/button", "optionsButton");

            Main.Textures.Load("Assets/Textures/Skybox/kWall1");
            Main.Textures.Load("Assets/Textures/Skybox/kWall2");
            Main.Textures.Load("Assets/Textures/Skybox/kWall3");
            Main.Textures.Load("Assets/Textures/Skybox/kWall4");
            Main.Textures.Load("Assets/Textures/Skybox/tiles", "floor2");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile", "Chocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile4x", "Chocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile8x", "Chocolate8x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile-white", "WhiteChocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile-white4x", "WhiteChocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/choco-tile-white8x", "WhiteChocolate8x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/dark_choco-tile", "DarkChocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/dark_choco-tile4x", "DarkChocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/dark_choco-tile8x", "DarkChocolate8x");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/ceramicColoring", "Ceramic");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/sugar01", "SugarW");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/sugar02", "SugarB");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/wood", "Wood");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/blackTile");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/checkers", "Checkers");
        }

        #endregion

        #region Methods

        //TEMP
        private void TestingPlatform()
        {
            EffectParameters effectParameters = new EffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            Transform3D transform3D = new Transform3D(new Vector3(5, 0, 0), Vector3.UnitZ, Vector3.UnitY);
            test = new BasicTile("StaticTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, Main.Models["Knife"], ETileType.Static);
            test.ControllerList.Add(new CustomBoxColliderController("testBCC", ControllerType.Collider, ColliderShape.Cube, 1f));
            drawnActors.Add("StaticTile2", test);
            Main.ObjectManager.Add(test);
        }

        #endregion

        #region Events

        private void OnGameStateMessageReceived(GameStateMessageEventInfo eventInfo)
        {
            switch (eventInfo.gameState)
            {
                case GameState.Won:
                    Main.SceneManager.NextScene();
                    break;

                case GameState.Lost:
                    //You know how it is on this bitch of an earth
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}