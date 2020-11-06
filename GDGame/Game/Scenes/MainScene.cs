using System;
using System.Collections.Generic;
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
        private Dictionary<string, SpriteFont> fonts;
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
            LoadFonts();
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

            InitHud();
        }


        private void InitCameras3D()
        {
            
            
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 20), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera, KeyboardManager, 35, 20));

            CameraManager.Add(camera3D);
            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new TileFactory(ObjectManager, drawnActors));
            grid.GenerateGrid(@"Game\LevelFiles\AttachTest.json");
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
            staticTile.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1f));

            effectParameters = new EffectParameters(ModelEffect, textures["Cube"], Color.White, 1);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models["BlueCube"]);
            attachableTile.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1f));

            CubePlayer player = new CubePlayer("Player1", ActorType.Player, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models["RedCube"], fonts["UI"]);
            player.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1f));
            player.ControllerList.Add(new PlayerController(KeyboardManager));
            player.ControllerList.Add(new SoundController(KeyboardManager, SoundManager, "playerMove", "playerAttach"));


            ObjectManager.Add(archetypalBoxWireframe);
            drawnActors = new Dictionary<string, DrawnActor3D>
            {
                {"StaticTile", staticTile}, {"AttachableBlock", attachableTile}, {"PlayerBlock", player}
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


        private void InitHud()
        {
            Hud hud = new Hud(Game, textures["WhiteSquare"], fonts["UI"], new SpriteBatch(GraphicsDevice),
                textures["Compass"],
                CameraManager.ActiveCamera);
            Components.Add(hud);
        }

        #endregion


        #region LoadContent

        private void LoadFonts()
        {
            SpriteFont uiFont = Content.Load<SpriteFont>("Assets/Fonts/Arial");

            fonts = new Dictionary<string, SpriteFont> {{"UI", uiFont}};
        }

        private void LoadSounds()
        {
            //step 1 - load songs
            SoundEffect track01 = Content.Load<SoundEffect>("Assets/Sound/GameTrack01");
            SoundEffect track02 = Content.Load<SoundEffect>("Assets/Sound/Ambiance02");
            SoundEffect track03 = Content.Load<SoundEffect>("Assets/Sound/Knock03");
            SoundEffect track04 = Content.Load<SoundEffect>("Assets/Sound/Chains01");
            SoundEffect track05 = Content.Load<SoundEffect>("Assets/Sound/Click01");

            //Step 2- Make into sounds
            SoundManager.Add(new Sounds(track01, "gameTrack", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track02, "ambiance", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track03, "playerMove", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track04, "chainRattle", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track05, "playerAttach", ActorType.SoundEffect, StatusType.Update));

            //SoundManager.PlaySoundEffect("gameTrack");
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

            models = new Dictionary<string, Model>
            {
                {"BlueCube", attachableModel}, {"RedCube", playerModel}, {"Box", boxModel}
            };
        }

        #endregion

        #region Override Methodes

        public override void Update(GameTime gameTime)
        {
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
            //{
            //    SoundManager.nextSong();
            //}

            //if(KeyboardManager.IsFirstKeyPress(Keys.L)) { SoundManager.volumeUp(); }
            //else if(KeyboardManager.IsFirstKeyPress(Keys.K)) { SoundManager.volumeDown(); }
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void Terminate()
        {
        }

        #endregion
    }
}