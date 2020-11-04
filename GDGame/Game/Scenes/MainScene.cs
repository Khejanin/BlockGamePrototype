using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using GDGame.Game.Controllers;
using GDGame.Game.Controllers.CameraControllers;
using GDGame.Game.Factory;
using GDGame.Game.Scenes;
using GDGame.Game.UI;
using GDGame.Game.Utilities;
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

namespace GDGame.Scenes
{
    public class MainScene : Scene
    {
        private ModelObject archetypalBoxWireframe;
        private VertexPositionColorTexture[] vertices;
        private Texture2D grass;
        private PrimitiveObject archetypalTexturedQuad;

        private SoundEffect track01, track02, track03, track04, track05;

        private Texture2D backSky;
        private Texture2D leftSky;
        private Texture2D rightSky;
        private Texture2D frontSky;
        private Texture2D topSky;
        private Texture2D cubeTexture;
        private Texture2D createTexture;


        private SpriteFont uiFont;

        private Model playerModel;
        private Model attachableModel;
        private Model boxModel;

        public SpriteFont UiFont
        {
            get => uiFont;
            set => uiFont = value;
        }


        public MainScene(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            InitCameras3D();
            InitTextures();
            InitFonts();
            InitDrawnContent();
            InitSound();
        }

        private void InitFonts()
        {
            UiFont = Content.Load<SpriteFont>("Assets/Fonts/Arial");
        }

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        private void InitDrawnContent() //formerly InitPrimitives
        {
            //add archetypes that can be cloned
            InitPrimitiveArchetypes();

            //adds origin helper etc
            InitHelpers();

            //add skybox
            //InitSkybox();

            //add grass plane
            //InitGround();

            //models
            InitStaticModels();

            //grid
            InitGrid();

            InitHud();
        }

        private void InitTextures()
        {
            //step 1 - texture
            backSky = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            leftSky = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            rightSky = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            frontSky = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            topSky = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");
            grass = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");

            cubeTexture = Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube");
            createTexture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 20), -Vector3.Forward, Vector3.Up);
            Camera3D camera3D = new Camera3D("cam", ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepFourThree);
            camera3D.ControllerList.Add(new RotationAroundActor("main_cam", ControllerType.FlightCamera,
                KeyboardManager, 1));

            CameraManager.Add(camera3D);
            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            List<Model> models = new List<Model> {boxModel, attachableModel, playerModel};
            List<Texture2D> textures = new List<Texture2D> {createTexture, cubeTexture};
            Grid grid = new Grid(new TileFactory(KeyboardManager, ObjectManager, game.ModelEffect, UiFont, models,
                textures));
            grid.GenerateGrid(@"Game\LevelFiles\AttachTest.json");

            List<DrawnActor3D> players = ObjectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (players.Count > 0)
            {
                RotationAroundActor rotationAroundActor =
                    (RotationAroundActor) CameraManager.ActiveCamera.ControllerList[0];
                rotationAroundActor.Target = players[0];
            }
        }

        private void InitStaticModels()
        {
            //transform
            Transform3D transform3D =
                new Transform3D(Vector3.Up, Vector3.Zero, Vector3.One, -Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(game.ModelEffect,
                Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"), Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/box2");

            EffectParameters wireframeEffectParameters =
                new EffectParameters(game.ModelEffect, null, Color.White, 1);

            archetypalBoxWireframe = new ModelObject("original wireframe box mesh",
                ActorType.Helper, StatusType.Update | StatusType.Drawn, transform3D, wireframeEffectParameters, model,
                game.WireframeRasterizerState);

            ObjectManager.Add(archetypalBoxWireframe);

            attachableModel = Content.Load<Model>("Assets/Models/BlueCube");
            playerModel = Content.Load<Model>("Assets/Models/RedCube");
            boxModel = Content.Load<Model>("Assets/Models/box2");
        }

        private void InitVertices()
        {
            vertices
                = new VertexPositionColorTexture[4];

            float halfLength = 0.5f;
            //TL
            vertices[0] = new VertexPositionColorTexture(
                new Vector3(-halfLength, halfLength, 0),
                new Color(255, 255, 255, 255), new Vector2(0, 0));

            //BL
            vertices[1] = new VertexPositionColorTexture(
                new Vector3(-halfLength, -halfLength, 0),
                Color.White, new Vector2(0, 1));

            //TR
            vertices[2] = new VertexPositionColorTexture(
                new Vector3(halfLength, halfLength, 0),
                Color.White, new Vector2(1, 0));

            //BR
            vertices[3] = new VertexPositionColorTexture(
                new Vector3(halfLength, -halfLength, 0),
                Color.White, new Vector2(1, 1));
        }

        private void InitPrimitiveArchetypes() //formerly InitTexturedQuad
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(game.UnlitTexturedEffect,
                grass, /*bug*/ Color.White, 1);

            IVertexData vertexData = new VertexData<VertexPositionColorTexture>(
                vertices, PrimitiveType.TriangleStrip, 2);

            archetypalTexturedQuad = new PrimitiveObject("original texture quad",
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform3D, effectParameters, vertexData);
        }

        //VertexPositionColorTexture - 4 bytes x 3 (x,y,z) + 4 bytes x 3 (r,g,b) + 4bytes x 2 = 26 bytes
        //VertexPositionColor -  4 bytes x 3 (x,y,z) + 4 bytes x 3 (r,g,b) = 24 bytes
        private void InitHelpers()
        {
            //to do...add wireframe origin
            PrimitiveType primitiveType;
            int primitiveCount;

            //step 1 - vertices
            VertexPositionColor[] vertices = VertexFactory.GetVerticesPositionColorOriginHelper(
                out primitiveType, out primitiveCount);

            //step 2 - make vertex data that provides Draw()
            IVertexData vertexData = new VertexData<VertexPositionColor>(vertices,
                primitiveType, primitiveCount);

            //step 3 - make the primitive object
            Transform3D transform3D = new Transform3D(new Vector3(0, 20, 0),
                Vector3.Zero, new Vector3(10, 10, 10),
                Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(game.UnlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject primitiveObject = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            ObjectManager.Add(primitiveObject);
        }

        private void InitSound()
        {
            //step 1 - load songs
            track01 = Content.Load<SoundEffect>("Assets/Sound/GameTrack01");
            track02 = Content.Load<SoundEffect>("Assets/Sound/Ambiance02");
            track03 = Content.Load<SoundEffect>("Assets/Sound/Knock03");
            track04 = Content.Load<SoundEffect>("Assets/Sound/Chains01");
            track05 = Content.Load<SoundEffect>("Assets/Sound/Click01");

            //Step 2- Make into sounds
            SoundManager.Add(new Sounds(track01, "main", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track02, "ambiance", ActorType.MusicTrack, StatusType.Update));
            SoundManager.Add(new Sounds(track03, "playerMove", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track04, "chainRattle", ActorType.SoundEffect, StatusType.Update));
            SoundManager.Add(new Sounds(track05, "playerAttach", ActorType.SoundEffect, StatusType.Update));

            SoundManager.playSoundEffect("main");
        }

        #endregion

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

            //use g and space
            //RaycastTests();

            //Cycle Through Audio
            if (KeyboardManager.IsFirstKeyPress(Keys.M))
            {
                SoundManager.nextSong();
            }
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void Terminate()
        {
        }

        private void RaycastTests()
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.G))
            {
                ModelObject o = (ModelObject) archetypalBoxWireframe.Clone();
                o.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1));
                o.Transform3D = new Transform3D(Vector3.Up * 5, -Vector3.Forward, Vector3.Up);
                ObjectManager.Add(o);

                o = (ModelObject) o.Clone();
                o.Transform3D.Translation = new Vector3(5, 5, 0);
                ObjectManager.Add(o);
            }

            if (KeyboardManager.IsFirstKeyPress(Keys.Space))
            {
                List<Raycaster.HitResult> hit = Raycaster.RaycastAll(new Vector3(0, 5, -5), new Vector3(0, 0, 1),
                    ObjectManager.FindAll(a => a != null));

                Debug.WriteLine("NEW HIT : MULTI");

                Debug.WriteLine("List size : " + hit.Count);

                foreach (Raycaster.HitResult result in hit)
                {
                    Debug.WriteLine("DISTANCE : " + result.distance + " ,ACTOR:" + result.actor);
                }

                hit.Sort((result, hitResult) => (int) (result.distance - hitResult.distance));

                hit = Raycaster.RaycastAll(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    ObjectManager.FindAll(a => a != null));

                Debug.WriteLine("NEW HIT : MULTI");

                Debug.WriteLine("List size : " + hit.Count);

                hit.Sort((result, hitResult) => (int) (result.distance - hitResult.distance));

                foreach (Raycaster.HitResult result in hit)
                {
                    Debug.WriteLine("DISTANCE : " + result.distance + " ,ACTOR:" + result.actor);
                }

                Debug.WriteLine("NEW HIT : SINGLE");

                Raycaster.HitResult hitSingle = Raycaster.Raycast(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    ObjectManager.FindAll(a => a != null));

                Debug.WriteLine("DISTANCE : " + hitSingle.distance + " ,ACTOR:" + hitSingle.actor);
            }
        }

        private void InitHud()
        {
            Hud hud = new Hud(game, Content.Load<Texture2D>("Assets/Textures/Base/WhiteSquare"),
                UiFont, new SpriteBatch(GraphicsDevice),
                Content.Load<Texture2D>("Assets/Textures/Base/BasicCompass"),
                CameraManager.ActiveCamera);
            game.Components.Add(hud);
        }
    }
}