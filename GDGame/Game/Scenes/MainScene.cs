using System;
using System.Collections.Generic;
using System.Security.Permissions;
using GDGame.Game.Actors.Audio;
using GDGame.Game.Controllers;
using GDGame.Game.Controllers.CameraControllers;
using GDGame.Game.Factory;
using GDGame.Game.Tiles;
using GDGame.Game.UI;
using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace GDGame.Game.Scenes
{
    public class MainScene : Scene
    {
        private ModelObject archetypalBoxWireframe;

        private Dictionary<string, Model> models;
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, DrawnActor3D> drawnActors;


        public MainScene(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();

            SetTargetToCamera();
        }

        private void SetTargetToCamera()
        {
            List<DrawnActor3D> players = ObjectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (players.Count > 0)
            {
                RotationAroundActor cam = (RotationAroundActor) CameraManager.ActiveCamera.ControllerList[0];
                cam.Target = players[0];
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

            //grid
            InitGrid();

            InitUi();
        }


        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 20), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera,
                KeyboardManager, 35, 20));

            CameraManager.Add(camera3D);
            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors));
            grid.GenerateGrid(@"Game\LevelFiles\PrototypeLevel.json");
        }

        private void InitStaticModels()
        {
            //transform
            Transform3D transform3D =
                new Transform3D(Vector3.Up, Vector3.Zero, Vector3.One, -Vector3.UnitZ, Vector3.UnitY);

            EffectParameters wireframeEffectParameters = new EffectParameters(ModelEffect, null, Color.White, 1);

            archetypalBoxWireframe = new ModelObject("original wireframe box mesh", ActorType.Helper,
                StatusType.Update | StatusType.Drawn, transform3D, wireframeEffectParameters, models["Box"]);

            EffectParameters effectParameters = new EffectParameters(ModelEffect, textures["Box"], Color.White, 1);
            transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            StaticTile staticTile = new StaticTile("StaticTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Box"]);
            staticTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Cube"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["BlueCube"]);
            attachableTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            attachableTile.ControllerList.Add(new MovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            CubePlayer player = new CubePlayer("Player1", ActorType.Player, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["RedCube"], Game.Fonts["UI"]);
            player.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            player.ControllerList.Add(new PlayerController(KeyboardManager));
            player.ControllerList.Add(new SoundController(KeyboardManager, SoundManager, "playerMove", "playerAttach"));
            player.ControllerList.Add(new RotationComponent());
            player.ControllerList.Add(new MovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            GoalTile goal = new GoalTile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Box"]);
            goal.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            ObjectManager.Add(archetypalBoxWireframe);
            drawnActors = new Dictionary<string, DrawnActor3D>
            {
                {"StaticTile", staticTile}, {"AttachableBlock", attachableTile}, {"PlayerBlock", player},
                {"GoalTile", goal}
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
            Transform3D transform3D = new Transform3D(new Vector3(0, 20, 0),
                Vector3.Zero, new Vector3(10, 10, 10),
                Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(UnlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject primitiveObject = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            ObjectManager.Add(primitiveObject);
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
            uiText = new UiText(StatusType.Off, text, Game.Fonts["UI"], Vector2.Zero, Color.Black, false);
            UiManager.AddUiElement("ToolTip", uiText);
        }

        private float GetAngle(Vector3 forward, Vector3 look)
        {
            Vector3 noY = look * (Vector3.Forward + Vector3.Right);
            noY.Normalize();

            Vector3 cross = Vector3.Cross(forward, noY);
            double dot = Vector3.Dot(forward, noY);

            double angle = Math.Atan2(cross.Length(), dot);

            double test = Vector3.Dot(Vector3.Up, cross);
            if (test < 0.0f) angle = -angle;
            return (float) -(angle + Math.PI);
        }

        #endregion


        #region LoadContent

        private void LoadSounds()
        {
            //step 1 - load songs
            SoundEffect track01 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack01");
            SoundEffect track02 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack02");
            SoundEffect track03 = Content.Load<SoundEffect>("Assets/GameTracks/GameTrack03");
            SoundEffect track04 = Content.Load<SoundEffect>("Assets/Sound/Knock03");
            SoundEffect track05 = Content.Load<SoundEffect>("Assets/Sound/Click01");

            //Step 2- Make into sounds
            SoundManager.Add(new Sounds(track01, "gameTrack01", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track02, "gameTrack02", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track03, "gameTrack03", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track04, "playerMove", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track05, "playerAttach", ActorType.SoundEffect, StatusType.Update));
        }

        private void LoadTextures()
        {
            Texture2D cubeTexture = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube");
            Texture2D createTexture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");
            Texture2D whiteSquareTexture = Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare");
            Texture2D compassTexture = Content.Load<Texture2D>("Assets/Textures/Base/BasicCompass");

            textures = new Dictionary<string, Texture2D>
            {
                {"Cube", cubeTexture}, {"Box", createTexture}, {"WhiteSquare", whiteSquareTexture},
                {"Compass", compassTexture}
            };
        }

        private void LoadModels()
        {
            Model attachableModel = Content.Load<Model>("Assets/Models/BlueCube");
            Model playerModel = Content.Load<Model>("Assets/Models/RedCube");
            Model boxModel = Content.Load<Model>("Assets/Models/box2");
            Model enemyModel = Content.Load<Model>("Assets/Models/Pyramid");

            models = new Dictionary<string, Model>
            {
                {"BlueCube", attachableModel}, {"RedCube", playerModel}, {"Box", boxModel}, {"Pyramid", enemyModel}
            };
        }

        #endregion

        #region Override Methodes

        public override void Update(GameTime gameTime)
        {
            float angle = GetAngle(Vector3.Forward, CameraManager.ActiveCamera.Transform3D.Look);
            UiSprite uiSprite = UiManager["Compass"] as UiSprite;
            uiSprite?.SetRotation(angle);

            List<DrawnActor3D> players = ObjectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (players.Count > 0)
            {
                CubePlayer player = players[0] as CubePlayer;
                UiManager["ToolTip"].StatusType =
                    player?.AttachCandidates.Count > 0 ? StatusType.Drawn : StatusType.Off;
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

            ////Cycle Through Audio
            //if (KeyboardManager.IsFirstKeyPress(Keys.M))
            //    SoundManager.NextSong();
            //if (KeyboardManager.IsKeyDown(Keys.Enter))
            //    SoundManager.StopSong();

            //if (KeyboardManager.IsKeyDown(Keys.L)) { SoundManager.volumeUp(); }
            //else if (KeyboardManager.IsKeyDown(Keys.K)) { SoundManager.volumeDown(); }
        }

        public override void Terminate()
        {
        }

        #endregion
    }
}