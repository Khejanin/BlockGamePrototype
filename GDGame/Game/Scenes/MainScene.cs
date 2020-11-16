using System;
using System.Collections.Generic;
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
using GDLibrary.Enums;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
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

        private string levelname;


        ////FOR SKYBOX____ TEMP
        private PrimitiveObject archetypalTexturedQuad, primitiveObject;

        public MainScene(Main game,string levelname = "Paul_Level_6.json") : base(game)
        {
            this.levelname = @"Game\LevelFiles\" + levelname;
        }

        public override void Initialize()
        {
            InitCameras3D();
            InitLoadContent();
            InitDrawnContent();

            SetTargetToCamera();
            InitEvents();
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

            //grids
            InitGrid();

            InitUi();

            //Skybox
            InitArchetypalQuad();
            InitSkybox();
            
        }


        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 20), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera,
                KeyboardManager, 20, 20));

            CameraManager.Add(camera3D);
            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors));
            grid.GenerateGrid(levelname);
        }

        private void InitStaticModels()
        {
            EffectParameters effectParameters = new EffectParameters(ModelEffect, textures["Box"], Color.White, 1);
            var transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            BasicTile staticTile = new BasicTile("StaticTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Box"]);
            staticTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Attachable"], Color.White, 1);
            MovableTile movableTile = new MovableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["Attachable"]);
            movableTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            movableTile.ControllerList.Add(new MovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            effectParameters = new EffectParameters(ModelEffect, textures["Player"], Color.White, 1);
            PlayerTile playerTile = new PlayerTile("Player1", ActorType.Player, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["Player"]);
            playerTile.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            playerTile.ControllerList.Add(new PlayerController(KeyboardManager,GamePadManager));
            playerTile.ControllerList.Add(new SoundController(KeyboardManager, SoundManager, "playerMove", "playerAttach"));
            playerTile.ControllerList.Add(new RotationComponent());
            playerTile.ControllerList.Add(new MovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            effectParameters = new EffectParameters(ModelEffect, textures["Finish"], Color.White, 1);
            GoalTile goal = new GoalTile("Goal", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Box"]);
            goal.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f,ColliderType.CheckOnly));

            EnemyTile enemy = new EnemyTile("Enemy", ActorType.NonPlayer, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Box"]);
            enemy.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));
            enemy.ControllerList.Add(new MovementComponent(300, new Curve1D(CurveLoopType.Cycle)));

            ButtonTile button = new ButtonTile("Button", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, models["Box"]);
            button.ControllerList.Add(new CustomBoxColliderController(ColliderShape.Cube, 1f));

            drawnActors = new Dictionary<string, DrawnActor3D>
            {
                {"StaticTile", staticTile}, {"AttachableBlock", movableTile}, {"PlayerBlock", playerTile},
                {"GoalTile", goal}, {"EnemyTile", enemy}, {"ButtonTile", button}
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
            
            uiSprite = new UiSprite(StatusType.Drawn, textures["Circle"], pos, Color.White);
            UiManager.AddUiElement("Circle", uiSprite);
            
            uiSprite = new UiSprite(StatusType.Drawn,textures["Compass"],pos,Color.White);
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
        }

        private void InitArchetypalQuad()
        {
            //SKYBOX
            float halfLength = 0.5f;
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

            vertices[0] = new VertexPositionColorTexture(new Vector3(-halfLength, halfLength, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(-halfLength, -halfLength, 0), Color.White, new Vector2(0, 1));
            vertices[2] = new VertexPositionColorTexture(new Vector3(halfLength, halfLength, 0), Color.White, new Vector2(1, 0));
            vertices[3] = new VertexPositionColorTexture(new Vector3(halfLength, -halfLength, 0), Color.White, new Vector2(1, 1));


            BasicEffect unlitTexturedEffect = new BasicEffect(Graphics.GraphicsDevice);
            unlitTexturedEffect.VertexColorEnabled = true; 
            unlitTexturedEffect.TextureEnabled = true;

            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(unlitTexturedEffect, textures["Wall"], /*bug*/ Color.White, 1);

            IVertexData vertexData = new VertexData<VertexPositionColorTexture>(vertices, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 2);

            this.archetypalTexturedQuad = new PrimitiveObject("original texture quad", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, vertexData);
        }

        private void InitSkybox()
        {
            float worldScale = 35;

            //Floor
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "Floor";
            primitiveObject.EffectParameters.Texture = textures["floor2"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 90);
            primitiveObject.Transform3D.Translation = new Vector3(0, -worldScale / 2.0f, 0);
            ObjectManager.Add(primitiveObject);

            //Back 
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "back";
            primitiveObject.EffectParameters.Texture = textures["kWall1"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            ObjectManager.Add(primitiveObject);

            //Front 
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "front";
            primitiveObject.EffectParameters.Texture = textures["kWall2"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            ObjectManager.Add(primitiveObject);

            //RWall
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "Right wall";
            primitiveObject.EffectParameters.Texture = textures["kWall3"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(270, 90, 270);
            primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            ObjectManager.Add(primitiveObject);

            //LWall
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "Left wall";
            primitiveObject.EffectParameters.Texture = textures["kWall4"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(-270, -90, 270);
            primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            ObjectManager.Add(primitiveObject);
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
            Texture2D basicFloor = Content.Load<Texture2D>("Assets/Textures/Block/BasicFloor");
            Texture2D basicYGFloor = Content.Load<Texture2D>("Assets/Textures/Block/floor");
            Texture2D basicBGFloor = Content.Load<Texture2D>("Assets/Textures/Block/BlockTextureBlue");
            Texture2D playerTexture = Content.Load<Texture2D>("Assets/Textures/Block/block_purple");
            Texture2D attachableTexture = Content.Load<Texture2D>("Assets/Textures/Block/block_green");
            Texture2D finishTexture = Content.Load<Texture2D>("Assets/Textures/Block/block_lime");
            Texture2D whiteSquareTexture = Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare");
            Texture2D compassTexture = Content.Load<Texture2D>("Assets/Textures/Base/BasicCompass");
            Texture2D cubeFloor = Content.Load<Texture2D>("Assets/Textures/Block/block_yellow");
            Texture2D circle = Content.Load<Texture2D>("Assets/Textures/circle");
            Texture2D logo = Content.Load<Texture2D>("Assets/Textures/Menu/logo");
            Texture2D logoMirror = Content.Load<Texture2D>("Assets/Textures/Menu/logo_mirror");

            Texture2D wall = Content.Load<Texture2D>("Assets/Textures/Block/block_green");
            Texture2D floor = Content.Load<Texture2D>("Assets/Textures/Skybox/floor_neon");

            Texture2D panel1 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall1");
            Texture2D panel2 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall2");
            Texture2D panel3 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall3");
            Texture2D panel4 = Content.Load<Texture2D>("Assets/Textures/Skybox/kWall4");
            Texture2D floor1 = Content.Load<Texture2D>("Assets/Textures/Skybox/tiles");

            textures = new Dictionary<string, Texture2D>
            {
                {"Player", cubeTexture}, 
                {"Attachable", cubeTexture}, 
                {"Finish", cubeTexture}, 
                {"Box", basicBGFloor}, 
                {"WhiteSquare", whiteSquareTexture},
                {"Compass", compassTexture},
                {"Wall1", wall },
                {"Wall", floor },
                {"Floor", floor },
                {"Circle",circle},
                {"Logo", logo },
                {"LogoMirror", logoMirror },
                {"kWall1", panel1 },
                {"kWall2", panel2 },
                {"kWall3", panel3 },
                {"kWall4", panel4 },
                {"floor2", floor1 }
            };
        }

        private void LoadModels()
        {
            Model attachableModel = Content.Load<Model>("Assets/Models/AttachableCube");
            Model redCube = Content.Load<Model>("Assets/Models/RedCube");
            Model blueCube = Content.Load<Model>("Assets/Models/blueCube");
            Model playerModel = Content.Load<Model>("Assets/Models/Player");
            Model boxModel = Content.Load<Model>("Assets/Models/box2");
            Model enemyModel = Content.Load<Model>("Assets/Models/Enemy");

            models = new Dictionary<string, Model>
            {
                {"Attachable", redCube}, {"Player", blueCube}, {"Box", boxModel}, {"Enemy", enemyModel}
            };
        }

        private void InitEvents()
        {
            EventSystem.EventSystem.RegisterListener<GameStateMessageEventInfo>(OnGameStateMessageReceived);
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
            float angle = GetAngle(Vector3.Forward, CameraManager.ActiveCamera.Transform3D.Look);
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
            if(KeyboardManager.IsFirstKeyPress(Keys.P))
                SoundManager.changeMusicState();
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