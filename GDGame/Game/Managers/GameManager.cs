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
using GDLibrary.Parameters;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    public class GameManager
    {
        #region Private variables

        private Camera3D curveCamera;
        private Dictionary<string, OurDrawnActor3D> drawnActors;
        private LevelData levelData;
        private string levelName;
        private Transform3D light;

        private Main main;

        #endregion

        #region Constructors

        public GameManager(Main main, string levelName = "Big_Level.json")
        {
            this.main = main;
            this.levelName = @"Game\LevelFiles\" + levelName;
        }

        #endregion

        #region Initialization

        public void Init()
        {
            InitCamera();
            InitGameEvents();
            InitLight();
            InitStaticModels();
            InitLevelDecor();
            InitGrid();
            InitSkyBox();
        }

        private void InitCamera()
        {
            Camera3D camera3D = main.CameraManager.ActiveCamera.Clone() as Camera3D;
            if (camera3D != null)
            {
                camera3D.ID = "CurveCamera";
                camera3D.ControllerList.Clear();
                main.CameraManager.Add(camera3D);
            }

            main.CameraManager.ActiveCameraIndex = 1;
            curveCamera = camera3D;
        }

        private void InitGameEvents()
        {
            EventManager.RegisterListener<PlayerEventInfo>(OnPlayerEventMessageReceived);
        }


        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(main.ObjectManager, drawnActors, main.Textures, main.IsEasy));
            levelData = grid.GenerateGrid(levelName);

            curveCamera.ControllerList.Add(new Curve3DController("CCFC", ControllerType.Curve,
                levelData.startCameraCurve));
        }

        private void InitLevelDecor()
        {
            float size = 1.5f;
            Vector3 scale = new Vector3(size, size, size);

            BasicEffectParameters effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["Wood"], Color.White, 1);
            Transform3D transform3D = new Transform3D(new Vector3(10, -15, 15), Vector3.UnitZ, Vector3.UnitY);
            OurModelObject table = new OurModelObject("Table", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Table"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(table);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Ceramic"], Color.White, 1);
            OurModelObject cups = new OurModelObject("Cups", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Cups"])
            {
                Transform3D = {Scale = scale}
            };
            drawnActors.Add("Cups", cups);
            main.ObjectManager.Add(cups);

            effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["WhiteChocolate"], Color.White, 1);
            OurModelObject choco = new OurModelObject("Chocolate", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Chocolate"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(choco);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["blackTile"], Color.White, 1);
            OurModelObject cat = new OurModelObject("Cat", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Cat"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(cat);

            OurModelObject coffeePot = new OurModelObject("CPot", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Pot"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(coffeePot);

            effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["coffeeSpill"], Color.White, 1);
            OurModelObject coffeeSpill = new OurModelObject("potSpill", ActorType.Primitive, StatusType.Drawn,
                transform3D, effectParameters, main.Models["Spill"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(coffeeSpill);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Checkers"], Color.White, 1);
            OurModelObject catBed = new OurModelObject("Catbed", ActorType.Primitive, StatusType.Drawn, transform3D,
                effectParameters, main.Models["CatBed"])
            {
                Transform3D = {Scale = scale}
            };
            main.ObjectManager.Add(catBed);
        }

        private void InitLight()
        {
            light = new Transform3D(new Vector3(-0.2f, 1, 0.4f), -Vector3.Forward, Vector3.Up);
        }

        private void InitSkyBox()
        {
            float worldScale = 100;


            //Floor
            if (main.ArchetypalTexturedQuad.Clone() is OurPrimitiveObject primitiveObject)
            {
                primitiveObject.ID = "Floor";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = main.Textures["floor2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 90);
                primitiveObject.Transform3D.Translation = new Vector3(0, -worldScale / 2.0f, 0);
                main.ObjectManager.Add(primitiveObject);
            }

            //Back
            primitiveObject = main.ArchetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "back";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = main.Textures["kWall1"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
                main.ObjectManager.Add(primitiveObject);
            }

            //Front
            primitiveObject = main.ArchetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "front";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = main.Textures["kWall2"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
                main.ObjectManager.Add(primitiveObject);
            }

            //RWall
            primitiveObject = main.ArchetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Right wall";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = main.Textures["kWall3"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(-90, 90, -90);
                primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
                main.ObjectManager.Add(primitiveObject);
            }

            //LWall
            primitiveObject = main.ArchetypalTexturedQuad.Clone() as OurPrimitiveObject;
            if (primitiveObject != null)
            {
                primitiveObject.ID = "Left wall";
                ((BasicEffectParameters) primitiveObject.EffectParameters).Texture = main.Textures["kWall4"];
                primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
                primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, -90);
                primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
                main.ObjectManager.Add(primitiveObject);
            }
        }

        private void InitStaticModels()
        {
            #region StaticTiles

            Color coffeeColor = new Color(111 / 255.0f, 78 / 255.0f, 55 / 255.0f, 0.95f);

            CoffeeEffectParameters coffeeEffect = new CoffeeEffectParameters(main.Effects["Coffee"],
                main.Textures["CoffeeUV"], main.Textures["CoffeeFlow"], coffeeColor);
            Transform3D transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            OurModelObject coffee = new OurModelObject("coffee - plane", ActorType.Primitive,
                StatusType.Update | StatusType.Drawn, transform3D, coffeeEffect,
                main.Models["CoffeePlane"]);
            main.ObjectManager.Add(coffee);

            NormalEffectParameters normalEffectParameters = new NormalEffectParameters(main.Effects["Normal"],
                main.Textures["Chocolate"], main.Textures["big-normalmap"],
                main.Textures["big-displacement"], Color.White, 1, light);
            Tile chocolateTile = new Tile("ChocolateTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, normalEffectParameters, main.Models["Cube"],
                true, ETileType.Static);

            BasicEffectParameters effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["Ceramic"], Color.White, 1);
            Tile plateStackTile = new Tile("plateStackTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, main.Models["PlateStack"],
                true, ETileType.Static);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Button"], Color.White, 1);
            ActivatableTile activatable = new ActivatableTile("Button", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Button"], false, ETileType.Button);

            coffeeEffect = (CoffeeEffectParameters) coffeeEffect.Clone();
            coffeeEffect.UvTilesTexture = main.Textures["DropUV"];
            coffeeEffect.CoffeeColor = new Color(coffeeEffect.CoffeeColor * 0.8f, 255);
            Tile spike = new Tile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                coffeeEffect, main.Models["Puddle"], false, ETileType.Spike);
            spike.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnHostileCollision));

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Mug"], Color.White, 1);
            Tile starPickup = new Tile("Star", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, main.Models["Mug"], false, ETileType.Star);
            starPickup.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));
            starPickup.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnCollectibleCollision));

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["sugarbox"], Color.White, 1);
            Tile goal = new Tile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, main.Models["SugarBox"], false, ETileType.Win);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Knife"], Color.White, 1);
            Tile checkpoint = new Tile("Checkpoint", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, main.Models["Knife"],
                false, ETileType.Checkpoint);

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Finish"], Color.White, 1);
            OurModelObject forkModelObject =
                new OurModelObject("fork", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, main.Models["Fork"]);
            //forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));


            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Knife"], Color.White, 1);
            OurModelObject knifeModelObject =
                new OurModelObject("knife", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, main.Models["Knife"]);
            //knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Finish"], Color.White, 1);
            OurModelObject singlePlateModelObject = new OurModelObject("singlePlate", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["SinglePlate"]);
            //singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion StaticTiles

            #region MovableTiles

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Cube"], ETileType.Attachable);
            attachableTile.ControllerList.Add(new TileMovementComponent("AttachableTileMC", ControllerType.Movement,
                300, new Curve1D(CurveLoopType.Cycle)));
            attachableTile.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Cube"], ETileType.Player);
            playerTile.ControllerList.Add(new PlayerController("PlayerPC", ControllerType.Player, main.KeyboardManager,
                main.CameraManager));
            TileMovementComponent tileMovementComponent = new TileMovementComponent("PTMC", ControllerType.Movement,
                300, new Curve1D(CurveLoopType.Cycle));
            playerTile.ControllerList.Add(tileMovementComponent);
            playerTile.ControllerList.Add(new PlayerMovementComponent("PlayerMC", ControllerType.Movement));
            playerTile.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));

            coffeeColor = new Color(coffeeColor, 255);
            coffeeEffect = new CoffeeEffectParameters(main.Effects["Coffee"], main.Textures["DropUV"],
                main.Textures["CoffeeFlow"], coffeeColor);
            PathMoveTile enemy = new PathMoveTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update,
                transform3D, coffeeEffect, main.Models["Drop"], false, ETileType.Enemy);
            enemy.ControllerList.Add(new EnemyMovementComponent("emc", ControllerType.Movement, ActivationType.AlwaysOn,
                0.5f, Smoother.SmoothingMethod.Smooth));
            enemy.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnHostileCollision));

            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Finish"], Color.White, 1);
            MovingPlatformTile platform = new MovingPlatformTile("MovingPlatform", ActorType.Platform,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["SinglePlate"], true, ETileType.MovingPlatform); //-1 = X, 1 = Y, 0 = Z
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

        #endregion

        #region Public Method

        public void RemoveCamera()
        {
            main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == curveCamera.ID);
        }

        public void UnRegisterGame()
        {
            EventManager.UnregisterListener<PlayerEventInfo>(OnPlayerEventMessageReceived);
        }

        public void Update()
        {
            if (light != null) light.Look = main.CameraManager.ActiveCamera.Transform3D.Look;

            if (curveCamera.ControllerList.Count > 0)
                if (curveCamera.ControllerList[0] is Curve3DController sceneCameraController &&
                    sceneCameraController.ElapsedTimeInMs > levelData.cameraMaxTime)
                    if (main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == curveCamera.ID))
                        main.CameraManager.ActiveCameraIndex = 0;
        }

        #endregion

        #region Events

        private bool OnCollectibleCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Player:
                        EventManager.FireEvent(new PlayerEventInfo
                            {type = PlayerEventType.PickupMug, tile = skin0.Owner.ExternalData as Tile});
                        break;
                }

            return true;
        }

        private bool OnHostileCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Attachable:
                        EventManager.FireEvent(new TileEventInfo {Type = TileEventType.AttachableKill});
                        break;
                    case ETileType.Player:
                        if (((PlayerTile) collide).IsAlive)
                            EventManager.FireEvent(new TileEventInfo {Type = TileEventType.PlayerKill, IsEasy = main.IsEasy});
                        break;
                }

            return true;
        }

        private void OnPlayerEventMessageReceived(PlayerEventInfo obj)
        {
            if (obj.type == PlayerEventType.PickupMug)
            {
                Actor2D mug = main.UiManager.UIObjectList.Find(actor2D => actor2D.ID.Equals(obj.tile.ID));
                if (mug != null) mug.StatusType = StatusType.Drawn;
            }
        }

        #endregion
    }
}