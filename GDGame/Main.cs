using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Game.Controllers;
using GDGame.Game.Controllers.CameraControllers;
using GDGame.Game.Factory;
using GDGame.Game.Utilities;
using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Debug;
using GDLibrary.Enums;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BasicEffect unlitTexturedEffect, unlitWireframeEffect;
        private CameraManager<Camera3D> cameraManager;
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;

        private VertexPositionColorTexture[] vertices;
        private Texture2D backSky, leftSky, rightSky, frontSky, topSky, grass;
        private PrimitiveObject archetypalTexturedQuad;
        private float worldScale = 3000;
        private PrimitiveObject primitiveObject;
        private BasicEffect modelEffect;
        private SpriteFont debugFont;

        private ModelObject archetypalBoxWireframe;
        private BasicEffect wireframeModelEffect;
        private RasterizerState wireframeRasterizerState;

        #endregion Fields

        #region Constructors

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion Constructors

        #region Initialization - Managers, Cameras, Effects, Textures

        protected override void Initialize()
        {
            //set game title
            Window.Title = "My Amazing Game";

            //note that we moved this from LoadContent to allow InitDebug to be called in Initialize
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //managers
            InitManagers();

            //cameras
            InitCameras3D();

            //resources and effects
            InitVertices();
            InitTextures();
            InitFonts();
            InitEffect();

            //drawn content
            InitDrawnContent();

            //graphic settings
            InitGraphics(1024, 768);

            //debug info
            InitDebug();

            base.Initialize();
        }

        private void InitManagers()
        {
            //camera
            cameraManager = new CameraManager<Camera3D>(this);
            Components.Add(cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(keyboardManager);

            //mouse
            mouseManager = new MouseManager(this, false);
            Components.Add(mouseManager);

            //object
            objectManager = new ObjectManager(this, 6, 10, cameraManager) {DrawOrder = 1};
            //set the object manager to be drawn BEFORE the debug drawer to the screen
            Components.Add(objectManager);
        }

        private void InitDebug()
        {
            //create the debug drawer to draw debug info
            DebugDrawer debugDrawer = new DebugDrawer(this, spriteBatch, debugFont,
                cameraManager, objectManager) {DrawOrder = 2};

            //set the debug drawer to be drawn AFTER the object manager to the screen

            //add the debug drawer to the component list so that it will have its Update/Draw methods called each cycle.
            Components.Add(debugDrawer);
        }

        private void InitFonts()
        {
            debugFont = Content.Load<SpriteFont>("Assets/Fonts/debug");
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;

            #region Camera - First Person

            transform3D = new Transform3D(new Vector3(10, 10, 20),
                new Vector3(0, 0, -1), Vector3.UnitY);

            camera3D = new Camera3D("1st person",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);

            //attach a controller
            camera3D.ControllerList.Add(new RotationAroundActor("ROTATION_AROUND_ACTOR", ControllerType.FirstPerson,
                keyboardManager, GameConstants.rotateSpeed));
            cameraManager.Add(camera3D);

            #endregion Camera - First Person

            cameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitEffect()
        {
            //to do...
            unlitTexturedEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                VertexColorEnabled = true, TextureEnabled = true
            };
            //otherwise we wont see RGB

            //wireframe primitives with no lighting and no texture
            unlitWireframeEffect = new BasicEffect(graphics.GraphicsDevice) {VertexColorEnabled = true};

            //model effect
            //add a ModelObject
            modelEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true, LightingEnabled = true, PreferPerPixelLighting = true
            };
            //   this.modelEffect.SpecularPower = 512;
            //  this.modelEffect.SpecularColor = Color.Red.ToVector3();
            modelEffect.EnableDefaultLighting();
        }

        private void InitTextures()
        {
            //step 1 - texture
            backSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            leftSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            rightSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            frontSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            topSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");
            grass
                = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
        }

        #endregion Initialization - Managers, Cameras, Effects, Textures

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        private void InitDrawnContent() //formerly InitPrimitives
        {
            //add archetypes that can be cloned
            InitPrimitiveArchetypes();

            //adds origin helper etc
            InitHelpers();

            //add skybox
            InitSkybox();

            //add grass plane
            InitGround();

            //models
            InitStaticModels();

            InitGrid();
        }

        private void InitStaticModels()
        {
            //transform
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0),
                new Vector3(0, 0, 0), //rotation
                new Vector3(1, 1, 1), //scale
                -Vector3.UnitZ, //look
                Vector3.UnitY); //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/box2");

            //model object
            ModelObject archetypalBoxObject = new ModelObject("car", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, model);
            objectManager.Add(archetypalBoxObject);


            effectParameters = new EffectParameters(modelEffect,
                Content.Load<Texture2D>("Assets/Textures/Props/GameTextures/TextureCube"), Color.White, 1);
            model = Content.Load<Model>("Assets/Models/RedCube");
            archetypalBoxObject = new ModelObject("player", ActorType.Player, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, model);

            objectManager.Add(archetypalBoxObject);
            
            model = Content.Load<Model>("Assets/Models/BlueCube");
            archetypalBoxObject = new ModelObject("attachableblock1", ActorType.Pickups, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, model);

            objectManager.Add(archetypalBoxObject);
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

            EffectParameters effectParameters = new EffectParameters(unlitTexturedEffect,
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

            EffectParameters effectParameters = new EffectParameters(unlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject primitiveObject = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            objectManager.Add(primitiveObject);
        }

        private void InitSkybox()
        {
            //back
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            //  primitiveObject.StatusType = StatusType.Off; //Experiment of the effect of StatusType
            primitiveObject.ID = "sky back";
            primitiveObject.EffectParameters.Texture = backSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            objectManager.Add(primitiveObject);

            //left
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "left back";
            primitiveObject.EffectParameters.Texture = leftSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //right
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky right";
            primitiveObject.EffectParameters.Texture = rightSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //top
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky top";
            primitiveObject.EffectParameters.Texture = topSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, worldScale / 2.0f, 0);
            objectManager.Add(primitiveObject);

            //to do...front
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky front";
            primitiveObject.EffectParameters.Texture = frontSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            objectManager.Add(primitiveObject);
        }

        private void InitGround()
        {
            //grass
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "grass";
            primitiveObject.EffectParameters.Texture = grass;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, 90, 0);
            objectManager.Add(primitiveObject);
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            graphics.ApplyChanges();

            //set screen centre based on resolution
            new Vector2(width / 2, height / 2);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState {CullMode = CullMode.None};
            graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp
            };
            graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            //set blending
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //set screen centre for use when centering mouse
            new Vector2(width / 2, height / 2);
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.Escape))
            {
                Exit();
            }

            if (keyboardManager.IsFirstKeyPress(Keys.C))
            {
                cameraManager.CycleActiveCamera();
            }

            RaycastTests();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #endregion Update & Draw

        private void InitGrid()
        {
            Grid grid = new Grid(new Transform3D(new Vector3(0, 0, 0), -Vector3.UnitZ, Vector3.UnitY),
                new TileFactory(keyboardManager, objectManager, Content, modelEffect));
            grid.GenerateGrid(@"Game\LevelFiles\LevelTest2.json");

            List<DrawnActor3D> playerAll = objectManager.FindAll(actor3D => actor3D.ActorType == ActorType.Player);
            if (playerAll.Count > 0)
            {
                RotationAroundActor rotationAroundActor =
                    (RotationAroundActor) cameraManager.ActiveCamera.ControllerList[0];
                rotationAroundActor.Target = playerAll[0];
            }
        }

        private void RaycastTests()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.G))
            {
                ModelObject o = (ModelObject) this.archetypalBoxWireframe.Clone();
                o.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1));
                o.Transform3D = new Transform3D(Vector3.Up * 5, -Vector3.Forward, Vector3.Up);
                objectManager.Add(o);

                o = (ModelObject) o.Clone();
                o.Transform3D.Translation = new Vector3(5, 5, 0);
                objectManager.Add(o);
            }

            if (this.keyboardManager.IsFirstKeyPress(Keys.Space))
            {
                List<Raycaster.HitResult> hit = Raycaster.RaycastAll(new Vector3(0, 5, -5), new Vector3(0, 0, 1),
                    objectManager.FindAll(a => a != null));

                Debug.WriteLine("NEW HIT : MULTI");

                Debug.WriteLine("List size : " + hit.Count);

                foreach (Raycaster.HitResult result in hit)
                {
                    Debug.WriteLine("DISTANCE : " + result.distance + " ,ACTOR:" + result.actor);
                }

                hit.Sort((result, hitResult) => (int) (result.distance - hitResult.distance));

                hit = Raycaster.RaycastAll(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    objectManager.FindAll(a => a != null));

                Debug.WriteLine("NEW HIT : MULTI");

                Debug.WriteLine("List size : " + hit.Count);

                hit.Sort((result, hitResult) => (int) (result.distance - hitResult.distance));

                foreach (Raycaster.HitResult result in hit)
                {
                    Debug.WriteLine("DISTANCE : " + result.distance + " ,ACTOR:" + result.actor);
                }

                Debug.WriteLine("NEW HIT : SINGLE");

                Raycaster.HitResult hitSingle = Raycaster.Raycast(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    objectManager.FindAll(a => a != null));

                Debug.WriteLine("DISTANCE : " + hitSingle.distance + " ,ACTOR:" + hitSingle.actor);
            }
        }
    }
}