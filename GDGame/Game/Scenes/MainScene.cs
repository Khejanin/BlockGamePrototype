using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
using GDGame.Game.Parameters.Effect;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

namespace GDGame.Scenes
{
    public class MainScene : Scene
    {
        #region Private variables

        private readonly string levelName;

        ////FOR SKYBOX____ TEMP
        private OurPrimitiveObject archetypalTexturedQuad, primitiveObject;

        private Curve3DController curve3DController;

        private Dictionary<string, OurDrawnActor3D> drawnActors;
        private Vector3 levelBounds;
        private Transform3D light;

        private OurDrawnActor3D player;
        private Tile test;
        private Transform3DCurve transform3DCurve;
        private float currentMovementCoolDown;

        #endregion

        #region Constructors

        public MainScene(Main main, string levelName, SceneType sceneType = SceneType.Game, bool unloadsContent = false)
            : base(main, sceneType, unloadsContent)
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

            Transform3D transform3D =
                new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            BasicEffectParameters effectParameters =
                new BasicEffectParameters(unlitTexturedEffect, Main.Textures["kWall1"], Color.White, 1);
            IVertexData vertexData =
                new VertexData<VertexPositionColorTexture>(vertices, PrimitiveType.TriangleStrip, 2);
            archetypalTexturedQuad = new OurPrimitiveObject("original texture quad", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, vertexData);
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
        
        /*
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
        }*/

        private void InitDrawnContent() //formerly InitPrimitives
        {
            //models
            InitLight();
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

            BasicEffectParameters effectParameters =
                new BasicEffectParameters(Main.ModelEffect, Main.Textures["Wood"], Color.White, 1);
            Transform3D transform3D = new Transform3D(new Vector3(10, -15, 15), Vector3.UnitZ, Vector3.UnitY);

            Tile table = new Tile("Table", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Table"], true,ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            table.AddPrimitive(new Box(table.Transform3D.Translation, Matrix.Identity, table.Transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            table.Enable(true, 1);
            Main.ObjectManager.Add(table);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Ceramic"], Color.White, 1);
            Tile cups = new Tile("Cups", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Cups"], false,ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            cups.AddPrimitive(new Box(cups.Transform3D.Translation, Matrix.Identity, cups.Transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            cups.Enable(true, 1);
            drawnActors.Add("Cups", cups);
            Main.ObjectManager.Add(cups);

            effectParameters =
                new BasicEffectParameters(Main.ModelEffect, Main.Textures["WhiteChocolate"], Color.White, 1);
            Tile choco = new Tile("Chocolate", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["Chocolate"], true,ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            choco.AddPrimitive(new Box(choco.Transform3D.Translation, Matrix.Identity, choco.Transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            choco.Enable(true, 1);
            Main.ObjectManager.Add(choco);

            //effectParameters = new BasicEffectParameters(unlitTexturedEffect, textures["Wall"], /*bug*/ Color.White, 1);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["blackTile"], Color.White, 1);
            Tile cat = new Tile("Cat", ActorType.Primitive, StatusType.Drawn, transform3D, effectParameters,
                Main.Models["Cat"], true,ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            cat.AddPrimitive(new Box(cat.Transform3D.Translation, Matrix.Identity, cat.Transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            cat.Enable(true, 1);
            Main.ObjectManager.Add(cat);

            //Tile coffeePot = new Tile("CoffeePot", ActorType.Primitive,
            //    StatusType.Drawn, transform3D, effectParameters, Main.Models["Pot"], true, ETileType.Static)
            //{
            //    Transform3D = { Scale = scale }
            //};
            //coffeePot.AddPrimitive(new Box(coffeePot.Transform3D.Translation, Matrix.Identity, coffeePot.Transform3D.Scale),
            //    new MaterialProperties(0.2f, 0.8f, 0.7f));
            //coffeePot.Enable(true, 1);
            //Main.ObjectManager.Add(coffeePot);

            //effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["coffeeSpill"], Color.White, 1);
            //Tile coffeeSpill = new Tile("coffeeSpill", ActorType.Primitive,
            //    StatusType.Drawn, transform3D, effectParameters, Main.Models["Spill"], true, ETileType.Static)
            //{
            //    Transform3D = { Scale = scale }
            //};
            //coffeeSpill.AddPrimitive(new Box(coffeeSpill.Transform3D.Translation, Matrix.Identity, coffeeSpill.Transform3D.Scale),
            //    new MaterialProperties(0.2f, 0.8f, 0.7f));
            //coffeeSpill.Enable(true, 1);
            //Main.ObjectManager.Add(coffeeSpill);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Checkers"], Color.White, 1);
            Tile catBed = new Tile("Catbed", ActorType.Primitive,
                StatusType.Drawn, transform3D, effectParameters, Main.Models["CatBed"], true,ETileType.Static)
            {
                Transform3D = {Scale = scale}
            };
            catBed.AddPrimitive(new Box(catBed.Transform3D.Translation, Matrix.Identity, catBed.Transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            catBed.Enable(true, 1);
            Main.ObjectManager.Add(catBed);
        }

        private void InitLight()
        {
            light = new Transform3D(new Vector3(-0.2f, 1, 0.4f), -Vector3.Forward, Vector3.Up);
        }

        private void InitLoadContent()
        {
            LoadTextures();
            LoadSounds();
            LoadModels();
            LoadEffects();
        }

        private void InitSkyBox()
        {
            float worldScale = 100;

            //Floor
            primitiveObject = archetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Floor";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = Main.Textures["floor2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 90);
                primitiveObject.Transform3D.Translation = new Vector3(0, -worldScale / 2.0f, 0);
                Main.ObjectManager.Add(primitiveObject);
            }

            //Back
            primitiveObject = archetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "back";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = Main.Textures["kWall1"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
                Main.ObjectManager.Add(primitiveObject);
            }

            //Front
            primitiveObject = archetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "front";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = Main.Textures["kWall2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
                Main.ObjectManager.Add(primitiveObject);
            }

            //RWall
            primitiveObject = archetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Right wall";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = Main.Textures["kWall3"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(-90, 90, -90);
                primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
                Main.ObjectManager.Add(primitiveObject);
            }

            //LWall
            primitiveObject = archetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Left wall";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = Main.Textures["kWall4"];
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

            BasicEffectParameters effectParameters =
                new BasicEffectParameters(Main.ModelEffect, Main.Textures["Chocolate"], Color.White, 1);
            NormalEffectParameters normalEffectParameters = new NormalEffectParameters(Main.Effects["Normal"],
                Main.Textures["Chocolate"], Main.Textures["big-normalmap"],
                Main.Textures["big-displacement"], Color.White, 1, light);
            Tile chocolateTile = new Tile("ChocolateTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, normalEffectParameters, Main.Models["Cube"],
                true, ETileType.Static);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Ceramic"], Color.White, 1);
            Tile plateStackTile = new Tile("plateStackTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, Main.Models["PlateStack"],
                true, ETileType.Static);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            ActivatableTile activatable = new ActivatableTile("Button", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["Button"], false, ETileType.Button);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            Tile spike = new Tile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, Main.Models["Pyramid"], false,ETileType.Spike);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            Tile starPickup = new Tile("Star", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, Main.Models["Mug"], false,ETileType.Star);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["sugarbox"], Color.White, 1);
            Tile goal = new Tile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, Main.Models["SugarBox"], false,ETileType.Win);

            Tile checkpoint = new Tile("Checkpoint", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, Main.Models["Knife"],
                false, ETileType.Checkpoint);

            Color coffeeColor = new Color(111 / 255.0f, 78 / 255.0f, 55 / 255.0f, 0.95f);
            
            CoffeeEffectParameters coffeeEffect = new CoffeeEffectParameters(Main.Effects["Coffee"],
                Main.Textures["CoffeeUV"], Main.Textures["CoffeeFlow"],coffeeColor);
            transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            OurModelObject coffee = new OurModelObject("coffee - plane", ActorType.Primitive,
                StatusType.Update | StatusType.Drawn,
                transform3D, coffeeEffect, Main.Models["CoffeePlane"]);
            Main.ObjectManager.Add(coffee);

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            OurModelObject forkModelObject =
                new OurModelObject("fork", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, Main.Models["Fork"]);
            //forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            OurModelObject plateModelObject = new OurModelObject("plates", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["PlateStack"]);
            //plateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            OurModelObject knifeModelObject =
                new OurModelObject("knife", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, Main.Models["Knife"]);
            //knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            OurModelObject singlePlateModelObject = new OurModelObject("singlePlate", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["SinglePlate"]);
            //singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion StaticTiles

            #region MovableTiles

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["Cube"], ETileType.Attachable);
            attachableTile.ControllerList.Add(new TileMovementComponent("AttachableTileMC", ControllerType.Movement,
                300, new Curve1D(CurveLoopType.Cycle)));

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn, transform3D,
                effectParameters, Main.Models["Cube"], ETileType.PlayerStart);
            playerTile.ControllerList.Add(new PlayerController("PlayerPC", ControllerType.Player, Main.KeyboardManager,
                Main.CameraManager));
            playerTile.ControllerList.Add(new SoundController("PlayerSC", ControllerType.Sound, Main.KeyboardManager,
                Main.SoundManager, "playerMove", "playerAttach"));
            TileMovementComponent tileMovementComponent = new TileMovementComponent("PTMC", ControllerType.Movement,
                300, new Curve1D(CurveLoopType.Cycle));
            playerTile.ControllerList.Add(tileMovementComponent);
            playerTile.ControllerList.Add(new PlayerMovementComponent("PlayerMC", ControllerType.Movement));
            
            coffeeColor = new Color(coffeeColor,255);
            coffeeEffect = new CoffeeEffectParameters(Main.Effects["Coffee"], Main.Textures["DropUV"],Main.Textures["CoffeeFlow"],coffeeColor);
            EnemyTile enemy = new EnemyTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update,
                transform3D, coffeeEffect, Main.Models["Drop"],
                false,ETileType.Enemy);
            enemy.ControllerList.Add(new EnemyMovementComponent("emc",ControllerType.Movement,ActivationType.AlwaysOn,0.5f,Smoother.SmoothingMethod.Smooth));

            effectParameters = new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            MovingPlatformTile platform = new MovingPlatformTile("MovingPlatform", ActorType.Platform,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                Main.Models["SinglePlate"],true, ETileType.MovingPlatform); //-1 = X, 1 = Y, 0 = Z
            platform.ControllerList.Add(new PathMovementComponent("platformpmc", ControllerType.Movement,
                ActivationType.Activated, 0.5f, Smoother.SmoothingMethod.Decelerate));

            #endregion MovableTiles

            drawnActors = new Dictionary<string, OurDrawnActor3D>
            {
                {"StaticTile", chocolateTile},
                {"PlateStackTile", plateStackTile},
                {"AttachableBlock", attachableTile},
                {"PlayerBlock", playerTile},
                {"GoalTile", goal},
                {"EnemyTile", enemy},
                {"ButtonTile", activatable},
                {"MovingPlatformTile", platform},
                {"SpikeTile", spike},
                {"StarPickupTile", starPickup},
                {"CheckpointTile", checkpoint},
                {"Knife", knifeModelObject},
                {"Fork", forkModelObject},
                {"SinglePlate", singlePlateModelObject}
            };
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

        protected override void UpdateScene(GameTime gameTime)
        {
            light.Look = Main.CameraManager.ActiveCamera.Transform3D.Look;

            if (currentMovementCoolDown <= 0)
            {
                currentMovementCoolDown = Constants.GameConstants.MOVEMENT_COOLDOWN;
                EventManager.FireEvent(new MovingTilesEventInfo());
            }

            currentMovementCoolDown -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (curve3DController != null && curve3DController.ElapsedTimeInMs > 25000)
            {
                Main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == "Curve Camera");
                Main.CameraManager.ActiveCameraIndex = 0;
                transform3DCurve.Clear();
                curve3DController = null;
            }

            if (player == null)
            {
                OurDrawnActor3D drawnActor3D =
                    Main.ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player");
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
                EventManager.FireEvent(new SoundEventInfo {soundEventType = SoundEventType.PlayNextMusic});
            //Stop Music
            if (Main.KeyboardManager.IsKeyDown(Keys.N))
                EventManager.FireEvent(new SoundEventInfo {soundEventType = SoundEventType.PauseMusic});
            //Volume Changes
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.L))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.IncreaseVolume, soundVolumeType = SoundVolumeType.Master});
            else if (Main.KeyboardManager.IsFirstKeyPress(Keys.K))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.DecreaseVolume, soundVolumeType = SoundVolumeType.Master});
            //Pause/resume music
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.P))
                EventManager.FireEvent(new SoundEventInfo
                    {soundEventType = SoundEventType.ToggleMusicPlayback, soundVolumeType = SoundVolumeType.Master});

            //Test
            if (Main.KeyboardManager.IsFirstKeyPress(Keys.G))
                test.Transform3D.TranslateBy(new Vector3(0, -1, 0));
            else if (Main.KeyboardManager.IsFirstKeyPress(Keys.H))
                test.Transform3D.TranslateBy(new Vector3(0, 1, 0));
        }

        #endregion

        #region Load Methods

        private void LoadEffects()
        {
            Main.Effects.Load("Assets/Effects/Normal");
            Main.Effects.Load("Assets/Effects/Coffee");
        }

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
            Main.Models.Load("Assets/Models/Decor/table01", "Table");
            Main.Models.Load("Assets/Models/Decor/cups01", "Cups");
            Main.Models.Load("Assets/Models/Decor/choco01", "Chocolate");
            Main.Models.Load("Assets/Models/Decor/cat01", "Cat");
            Main.Models.Load("Assets/Models/Decor/bed01", "CatBed");
            Main.Models.Load("Assets/Models/plane", "CoffeePlane");
            //Main.Models.Load("Assets/Models/coffeePot02", "Pot");
            //Main.Models.Load("Assets/Models/coffee spill", "Spill");
        }

        private void LoadSounds()
        {
            SoundEffect track01 = Main.Content.Load<SoundEffect>("Assets/GameTracks/track01");
            SoundEffect track02 = Main.Content.Load<SoundEffect>("Assets/GameTracks/track02");
            SoundEffect track03 = Main.Content.Load<SoundEffect>("Assets/GameTracks/track03");
            SoundEffect track04 = Main.Content.Load<SoundEffect>("Assets/Sound/Knock04");
            SoundEffect track05 = Main.Content.Load<SoundEffect>("Assets/Sound/Click02");
            SoundEffect track06 = Main.Content.Load<SoundEffect>("Assets/GameTracks/track04");
            SoundEffect track07 = Main.Content.Load<SoundEffect>("Assets/GameTracks/track05");

            Main.SoundManager.AddMusic("endTheme", track01);
            Main.SoundManager.AddMusic("gameTrack01", track02);
            Main.SoundManager.AddMusic("gameTrack02", track03);
            Main.SoundManager.AddMusic("gameTrack03", track06);
            Main.SoundManager.AddMusic("titleTheme", track07);
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

            //Skybox
            Main.Textures.Load("Assets/Textures/Skybox/kWall1");
            Main.Textures.Load("Assets/Textures/Skybox/kWall2");
            Main.Textures.Load("Assets/Textures/Skybox/kWall3");
            Main.Textures.Load("Assets/Textures/Skybox/kWall4");
            Main.Textures.Load("Assets/Textures/Skybox/tiles", "floor2");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/sugarbox");

            //Normals
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap-choco", "big-normalmap_choco");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap-b_logic", "big-normalmap_b_logic");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap8x");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/DisplacementMap", "big-displacement");

            //Chocolate
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco", "Chocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco_choco", "Chocolate_choco");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco_b_logic", "Chocolate_b_logic");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-4x", "Chocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-8x", "Chocolate8x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white", "WhiteChocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white_choco", "WhiteChocolate_choco");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white_b_logic", "WhiteChocolate_b_logic");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white4x", "WhiteChocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white8x", "WhiteChocolate8x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark", "DarkChocolate");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark_choco", "DarkChocolate_choco");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark_b_logic", "DarkChocolate_b_logic");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark4x", "DarkChocolate4x");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark8x", "DarkChocolate8x");

            //Coffee
            Main.Textures.Load("Assets/Textures/uvalex", "CoffeeUV");
            Main.Textures.Load("Assets/Textures/uvCoffeeDrop", "DropUV");
            Main.Textures.Load("Assets/Textures/flowmap2", "CoffeeFlow");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/ceramicColoring", "Ceramic");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/sugar01", "SugarW");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/sugar02", "SugarB");

            Main.Textures.Load("Assets/Textures/Props/GameTextures/wood", "Wood");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/blackTile");
            Main.Textures.Load("Assets/Textures/Props/GameTextures/checkers", "Checkers");
            //Main.Textures.Load("Assets/Textures/Props/GameTextures/coffeeStrip", "coffeeSpill");
        }

        #endregion

        #region Methods

        //TEMP
        private void TestingPlatform()
        {
            BasicEffectParameters effectParameters =
                new BasicEffectParameters(Main.ModelEffect, Main.Textures["Finish"], Color.White, 1);
            Transform3D transform3D = new Transform3D(new Vector3(5, 0, 0), Vector3.UnitZ, Vector3.UnitY);
            test = new Tile("StaticTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, Main.Models["Knife"], true,ETileType.Static);
            drawnActors.Add("StaticTile2", test);

            test.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale),
                MaterialProperties.Unset);
            test.Enable(true, 1);
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