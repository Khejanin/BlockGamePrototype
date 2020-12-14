using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Controllers;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
using GDGame.Game.Actors;
using GDGame.Game.Parameters.Effect;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Parameters;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    /// <summary>
    ///     Class that sets everything up for the game to start. It loads the level and uses the loaded content to create the
    ///     objects in the game.
    /// </summary>
    public class GameManager
    {
        #region Private variables

        private Coffee coffee;

        private Camera3D curveCamera;

        /// <summary>
        ///     Dictionary with all actors that we use in the game. These are not in the ObjectManager and are only slaves that we
        ///     should Clone.
        /// </summary>
        private Dictionary<string, OurDrawnActor3D> drawnActors;

        private LevelData levelData;
        private LevelDataManager levelDataManager;
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
            InitCoffee();
            levelDataManager = new LevelDataManager();
        }

        /// <summary>
        ///     Creates the Cinematic intro camera.
        /// </summary>
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

        private void InitCoffee()
        {
            Color coffeeColor = new Color(111 / 255.0f, 78 / 255.0f, 55 / 255.0f, 0.95f);
            CoffeeEffectParameters coffeeEffect = new CoffeeEffectParameters(main.Effects["Coffee"],
                main.Textures["CoffeeUV"], main.Textures["CoffeeFlow"], coffeeColor);
            Transform3D transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            coffee = new Coffee("Coffee", ActorType.Primitive,
                StatusType.Update | StatusType.Drawn, transform3D, coffeeEffect,
                main.Models["CoffeePlane"], levelData.coffeeInfo,
                main.ObjectManager.ActorList.Find(actor3D => actor3D.ActorType == ActorType.Player) as PlayerTile);
            //Most of these constructor arguments are not used, need to refactor the entire structure.
            coffee.ControllerList.Add(new CoffeeMovementComponent("cmc", ControllerType.Movement,
                ActivationType.Activated, 0, Smoother.SmoothingMethod.Smooth, coffee));
            coffee.ControllerList.Add(new ColliderComponent("cc Coffee", ControllerType.Collider,
                (skin0, skin1) =>
                {
                    if (skin1.Owner.ExternalData is Tile tile)
                    {
                        if (tile.TileType == ETileType.Player)
                            EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Lost});

                        if (tile.SpawnPos.Y < ((Coffee) skin0.Owner.ExternalData).Transform3D.Translation.Y)
                        {
                            EventManager.FireEvent(new RemoveActorEvent {body = tile.Body});
                            EventManager.FireEvent(new TileEventInfo {Id = tile.ID, Type = TileEventType.Consumed});
                        }
                        else
                        {
                            tile.Body.SetInactive();
                            EventManager.FireEvent(new TileEventInfo
                                {Id = tile.ID, Type = TileEventType.Reset, IsEasy = main.IsEasy});
                        }

                        return true;
                    }

                    return false;
                }));

            main.ObjectManager.Add(coffee);
            UiTimeController controller =
                main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText")?.ControllerList[0] as
                    UiTimeController;
            controller?.SetCoffee(coffee);
        }

        /// <summary>
        ///     If we need to initialize listening to any events we do it here.
        /// </summary>
        private void InitGameEvents()
        {
        }


        /// <summary>
        ///     Loads the JSON Level File. Clones the archetypes to create the level, establishes all links between objects etc.
        /// </summary>
        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(main.ObjectManager, drawnActors, main.Textures, main.IsEasy));
            levelData = grid.GenerateGrid(levelName);

            curveCamera.ControllerList.Add(new Curve3DController("CCFC", ControllerType.Curve,
                levelData.startCameraCurve));
        }

        /// <summary>
        ///     Sets and puts level Decor at it's place.
        /// </summary>
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

        /// <summary>
        ///     Our beautiful background is created by this method
        /// </summary>
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

        /// <summary>
        ///     Our Slave objects that we will later clone are all made here. Like "templates" of objects. (Like Prefabs in Unity)
        /// </summary>
        private void InitStaticModels()
        {
            /*
             * Some initialization
             */

            Color coffeeColor = new Color(111 / 255.0f, 78 / 255.0f, 55 / 255.0f, 0.95f);

            CoffeeEffectParameters coffeeEffect = new CoffeeEffectParameters(main.Effects["Coffee"],
                main.Textures["CoffeeUV"], main.Textures["CoffeeFlow"], coffeeColor);
            Transform3D transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            NormalEffectParameters normalEffectParameters = new NormalEffectParameters(main.Effects["Normal"],
                main.Textures["Chocolate"], main.Textures["big-normalmap"],
                main.Textures["big-displacement"], Color.White, 1, light);

            /*
             * Here we make the static Tiles.
             */

            #region StaticTiles

            //Create the Basic Tile

            Tile chocolateTile = new Tile("ChocolateTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, normalEffectParameters, main.Models["Cube"],
                true, ETileType.Static);

            //Create the Plate Stacks
            BasicEffectParameters effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["Ceramic"], Color.White, 1);
            Tile plateStackTile = new Tile("plateStackTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, main.Models["PlateStack"],
                true, ETileType.Static);

            //Create the Fork Model
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Finish"], Color.White, 1);
            OurModelObject forkModelObject =
                new OurModelObject("fork", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, main.Models["Fork"]);
            //forkModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            //Create the Knife Model
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Knife"], Color.White, 1);
            OurModelObject knifeModelObject =
                new OurModelObject("knife", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D,
                    effectParameters, main.Models["Knife"]);
            //knifeModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            //Create the Single Plate Model
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Finish"], Color.White, 1);
            OurModelObject singlePlateModelObject = new OurModelObject("singlePlate", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["SinglePlate"]);
            //singlePlateModelObject.ControllerList.Add(new RandomRotatorController("rotator", ControllerType.Curve));

            #endregion StaticTiles

            /*
             * Here we create the Tiles that interact with you on collision.
             */

            #region InteractableTiles

            //Create Button Tile
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Button"], Color.White, 1);
            ActivatableTile activatable = new ActivatableTile("Button", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Button"], false, ETileType.Button);
            activatable.ControllerList.Add(new ColliderComponent("ButtonCC", ControllerType.Collider,
                OnActivatableCollisionEnter));

            //Create the Puddle (We call them spikes because they kill the player on collision)
            coffeeEffect = (CoffeeEffectParameters) coffeeEffect.Clone();
            coffeeEffect.UvTilesTexture = main.Textures["DropUV"];
            coffeeEffect.CoffeeColor = new Color(Color.Green, 255);
            Tile spike = new Tile("Spike", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                coffeeEffect, main.Models["Puddle"], false, ETileType.Spike);
            spike.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnHostileCollision));

            //Create the Mug Pickups
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Mug"], Color.White, 1);
            Tile pickup = new Tile("Mug", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, main.Models["Mug"], false, ETileType.Star);
            pickup.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));
            pickup.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnCollectibleCollision));

            //Create the Goal Tile
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["sugarbox"], Color.White, 1);
            Tile goal = new Tile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, main.Models["SugarBox"], false, ETileType.Win);
            goal.ControllerList.Add(new ColliderComponent("CCG", ControllerType.Collider, OnGoalCollided));

            //Create the Checkpoint Tile
            effectParameters =
                new BasicEffectParameters(main.ModelEffect, main.Textures["WhiteSquare"], Color.White, 1);
            Tile checkpoint = new Tile("Checkpoint", ActorType.Primitive, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, main.Models["Smarties"], false, ETileType.Checkpoint);
            checkpoint.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnCheckPointCollision));

            #endregion

            /*
             * Here we create the Tiles that can Move
             */

            #region MovableTiles

            //Create the Attachable Tiles
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["SugarB"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Cube"], ETileType.Attachable);
            attachableTile.ControllerList.Add(new TileMovementComponent("AttachableTileMC", ControllerType.Movement,
                300));
            attachableTile.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));

            //Create the Player Tile
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["SugarW"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player", ActorType.Player, StatusType.Drawn, transform3D,
                effectParameters, main.Models["Cube"], ETileType.Player);
            playerTile.ControllerList.Add(new PlayerController("PlayerPC", ControllerType.Player, main.KeyboardManager,
                main.CameraManager));
            TileMovementComponent tileMovementComponent = new TileMovementComponent("PTMC", ControllerType.Movement,
                300);
            playerTile.ControllerList.Add(tileMovementComponent);
            playerTile.ControllerList.Add(new PlayerMovementComponent("PlayerMC", ControllerType.Movement));
            playerTile.ControllerList.Add(new PlayerDeathComponent("PDC", ControllerType.Event));

            //Create the Enemy Tiles
            coffeeColor = new Color(coffeeColor, 255);
            coffeeEffect = new CoffeeEffectParameters(main.Effects["Coffee"], main.Textures["DropUV"],
                main.Textures["CoffeeFlow"], coffeeColor);
            PathMoveTile enemy = new PathMoveTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update,
                transform3D, coffeeEffect, main.Models["Drop"], false, ETileType.Enemy);
            enemy.ControllerList.Add(new EnemyMovementComponent("emc", ControllerType.Movement, ActivationType.AlwaysOn,
                0.5f, Smoother.SmoothingMethod.Smooth));
            enemy.ControllerList.Add(new ColliderComponent("CC", ControllerType.Collider, OnHostileCollision));

            //Create the Moving Platform Tiles
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Biscuit"], Color.White, 1);
            MovingPlatformTile movingPlatform = new MovingPlatformTile("MovingPlatform", ActorType.Platform,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Biscuit"], true, ETileType.MovingPlatform);
            movingPlatform.ControllerList.Add(new PathMovementComponent("platformpmc", ControllerType.Movement,
                ActivationType.Activated, 0.5f, Smoother.SmoothingMethod.Decelerate));

            //Create the Doors Tiles
            effectParameters = new BasicEffectParameters(main.ModelEffect, main.Textures["Biscuit"], Color.White, 1);
            PathMoveTile doorTile = new PathMoveTile("Door Tile", ActorType.Platform,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters,
                main.Models["Cube"], true, ETileType.Door);
            doorTile.ControllerList.Add(new DoorMovementComponent("doorPMC", ControllerType.Movement,
                ActivationType.Activated, 0.5f, Smoother.SmoothingMethod.Accelerate));

            #endregion MovableTiles

            //Now we add them all to our dictionary to later clone.
            drawnActors = new Dictionary<string, OurDrawnActor3D>
            {
                {"StaticTile", chocolateTile},
                {"PlateStackTile", plateStackTile},
                {"AttachableBlock", attachableTile},
                {"PlayerBlock", playerTile},
                {"GoalTile", goal},
                {"EnemyTile", enemy},
                {"ButtonTile", activatable},
                {"MovingPlatformTile", movingPlatform},
                {"DoorTile", doorTile},
                {"SpikeTile", spike},
                {"StarPickupTile", pickup},
                {"CheckpointTile", checkpoint},
                {"Knife", knifeModelObject},
                {"Fork", forkModelObject},
                {"SinglePlate", singlePlateModelObject},
                {"Coffee", coffee}
            };
        }

        #endregion

        #region Public Method

        /// <summary>
        ///     This just removes the cinematic camera after it's done playing.
        /// </summary>
        public void RemoveCamera()
        {
            main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == curveCamera.ID);
        }

        public void UnRegisterGame()
        {
        }


        public void Update()
        {
            //Update Lights to look in view direction
            if (light != null) light.Look = main.CameraManager.ActiveCamera.Transform3D.Look;

            //Remove Cinematic camera if appropriate
            if (curveCamera.ControllerList.Count > 0)
                if (curveCamera.ControllerList[0] is Curve3DController sceneCameraController &&
                    sceneCameraController.ElapsedTimeInMs > levelData.cameraMaxTime)
                    if (main.CameraManager.RemoveFirstIf(camera3D => camera3D.ID == curveCamera.ID))
                        main.CameraManager.ActiveCameraIndex = 0;
        }

        #endregion

        #region Events

        private bool OnActivatableCollisionEnter(CollisionSkin skin0, CollisionSkin skin1)
        {
            Debug.WriteLine("Collision Enter!");
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Player:
                    case ETileType.Attachable:
                        EventManager.FireEvent(new ActivatorEventInfo
                            {type = ActivatorEventType.Activate, id = ((Tile) skin0.Owner.ExternalData).activatorId});
                        break;
                }

            return true;
        }

        private bool OnCheckPointCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Player:
                        EventManager.FireEvent(new RemoveActorEvent {body = (skin0.Owner.ExternalData as Tile)?.Body});
                        EventManager.FireEvent(new PlayerEventInfo
                            {type = PlayerEventType.SetCheckpoint, position = skin0.Owner.Position});
                        break;
                }

            return true;
        }

        private bool OnCollectibleCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Player:
                        Tile collectible = skin0.Owner.ExternalData as Tile;
                        Actor2D mug = main.UiManager.UIObjectList.Find(actor2D =>
                            collectible != null && actor2D.ID.Equals(collectible.ID));
                        if (mug != null) mug.StatusType = StatusType.Drawn;


                        //Trigger Collectible animation
                        if (collectible != null)
                        {
                            EventManager.FireEvent(new RemoveActorEvent {body = collectible.Body});
                            collectible.MoveTo(new AnimationEventData
                            {
                                isRelative = false, destination = collectible.Transform3D.Translation + Vector3.Up,
                                maxTime = 500,
                                smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PlayOnce
                            });

                            collectible.ScaleTo(new AnimationEventData
                            {
                                isRelative = false, destination = Vector3.One * 1.5f,
                                maxTime = 1000,
                                smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PingPongOnce,
                                callback = () => EventManager.FireEvent(new RemoveActorEvent {actor3D = collectible})
                            });

                            collectible.RotateTo(new AnimationEventData
                            {
                                isRelative = true, destination = Vector3.Up * 720,
                                maxTime = 1000,
                                smoothing = Smoother.SmoothingMethod.Smooth
                            });

                            EventManager.FireEvent(new SoundEventInfo
                            {
                                soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.CollectibleCollected,
                                listenerTransform = collectible.Transform3D
                            });
                        }

                        break;
                }

            return true;
        }

        private bool OnGoalCollided(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.Owner.ExternalData is Tile collide)
                switch (collide.TileType)
                {
                    case ETileType.Player:
                        EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Won});
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
                        EventManager.FireEvent(new TileEventInfo
                            {Type = TileEventType.Reset, IsEasy = main.IsEasy, Id = collide.ID});
                        break;
                    case ETileType.Player:
                        if (((PlayerTile) collide).IsAlive)
                            EventManager.FireEvent(new TileEventInfo
                                {Type = TileEventType.PlayerKill, IsEasy = main.IsEasy, Id = collide.ID});
                        break;
                }

            return true;
        }

        #endregion

        /*
         * Here we define all the callbacks that our objects with colliders will use.
         * We didn't want to make custom classes so we just pass the callback in the constructor of the component.
         */
    }
}