using System.Collections.Generic;
using System.Diagnostics;
using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlockGame.Scenes
{
    public class MainScene : Scene
    {
        private ModelObject archetypalBoxWireframe;
        private VertexPositionColorTexture[] vertices;
        private Texture2D grass;
        private PrimitiveObject archetypalTexturedQuad;
        
        private Texture2D backSky;
        private Texture2D leftSky;
        private Texture2D rightSky;
        private Texture2D frontSky;
        private Texture2D topSky;

        
        public MainScene(Main game) : base(game)
        {
        }
        
        public override void Initialize()
        {
            InitCameras3D();
            InitTextures();
            InitDrawnContent();
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
        }
        
        private void InitTextures()
        {
            //step 1 - texture
            this.backSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            this.leftSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            this.rightSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            this.frontSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            this.topSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");

            this.grass
                = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
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
            camera3D.ControllerList.Add(new FirstPersonCameraController(
                KeyboardManager, MouseManager,
                GDConstants.moveSpeed, GDConstants.strafeSpeed, GDConstants.rotateSpeed));
            CameraManager.Add(camera3D);

            #endregion

            #region Camera - Flight

            transform3D = new Transform3D(new Vector3(0, 10, 10),
                new Vector3(0, 0, -1),
                Vector3.UnitY);

            camera3D = new Camera3D("flight person",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);

            //attach a controller
            camera3D.ControllerList.Add(new FlightCameraController(
                KeyboardManager, MouseManager,
                GDConstants.moveSpeed, GDConstants.strafeSpeed, GDConstants.rotateSpeed));
            CameraManager.Add(camera3D);

            #endregion

            #region Camera - Security

            transform3D = new Transform3D(new Vector3(10, 10, 50),
                new Vector3(0, 0, -1),
                Vector3.UnitY);

            camera3D = new Camera3D("security",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);

            camera3D.ControllerList.Add(new PanController(new Vector3(1, 1, 0),
                30, GDConstants.mediumAngularSpeed, 0));
            CameraManager.Add(camera3D);

            #endregion

            #region Camera - Giant

            transform3D = new Transform3D(new Vector3(0, 250, 100),
                new Vector3(0, -1, -1), //look
                new Vector3(0, 1, -1)); //up

            CameraManager.Add(new Camera3D("giant looking down 1st person",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen));
            CameraManager.Add(camera3D);

            #endregion

            CameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitGrid()
        {
            Grid grid = new Grid(new Transform3D(new Vector3(0, 0, 0), -Vector3.UnitZ, Vector3.UnitY), new TileFactory(game.KeyboardManager, game.ObjectManager, game.Content, game.ModelEffect));
            grid.GenerateGrid(@"GDLibrary\Grid\LevelFiles\LevelTest2.json");
        }

        private void InitStaticModels()
        {
            //transform
            Transform3D transform3D = new Transform3D(Vector3.Up,
                                Vector3.Zero,       //rotation
                                Vector3.One,        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(game.ModelEffect,
                Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/box2");

            //model object
            /*ModelObject archetypalBoxObject = new ModelObject("car", ActorType.Player,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, model);
            this.objectManager.Add(archetypalBoxObject);*/
            
            EffectParameters wireframeEffectParameters =
                new EffectParameters(game.ModelEffect, null, Color.White, 1);
            
            
            archetypalBoxWireframe = new ModelObject("original wireframe box mesh",
            ActorType.Helper, StatusType.Update | StatusType.Drawn , transform3D, wireframeEffectParameters,model,game.WireframeRasterizerState);

            ObjectManager.Add(archetypalBoxWireframe);
        }

        private void InitVertices()
        {
            this.vertices
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
                this.grass, /*bug*/ Color.White, 1);

            IVertexData vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices, PrimitiveType.TriangleStrip, 2);

            this.archetypalTexturedQuad = new PrimitiveObject("original texture quad",
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

        //private void InitSkybox()
        //{ 
        //    //back
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //  //  primitiveObject.StatusType = StatusType.Off; //Experiment of the effect of StatusType
        //    primitiveObject.ID = "sky back";
        //    primitiveObject.EffectParameters.Texture = this.backSky;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
        //    primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
        //    this.objectManager.Add(primitiveObject);

        //    //left
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //    primitiveObject.ID = "left back";
        //    primitiveObject.EffectParameters.Texture = this.leftSky;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
        //    primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
        //    primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
        //    this.objectManager.Add(primitiveObject);

        //    //right
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //    primitiveObject.ID = "sky right";
        //    primitiveObject.EffectParameters.Texture = this.rightSky;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
        //    primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
        //    primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
        //    this.objectManager.Add(primitiveObject);

             
        //    //top
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //    primitiveObject.ID = "sky top";
        //    primitiveObject.EffectParameters.Texture = this.topSky;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
        //    primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, 0);
        //    primitiveObject.Transform3D.Translation = new Vector3(0 ,worldScale / 2.0f, 0);
        //    this.objectManager.Add(primitiveObject);

        //    //to do...front
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //    primitiveObject.ID = "sky front";
        //    primitiveObject.EffectParameters.Texture = this.frontSky;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
        //    primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
        //    primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
        //    this.objectManager.Add(primitiveObject);

        //}

        //private void InitGround()
        //{
        //    //grass
        //    primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
        //    primitiveObject.ID = "grass";
        //    primitiveObject.EffectParameters.Texture = this.grass;
        //    primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
        //    primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, 90, 0);
        //    this.objectManager.Add(primitiveObject);
        //}

        
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.C))
            {
                CameraManager.CycleActiveCamera();
                // this.cameraManager.ActiveCameraIndex++;
            }

            //use g and space
            RaycastTests();
        }

        public override void Draw(GameTime gameTime)
        {
        }
        
         private void RaycastTests()
        {
            if (KeyboardManager.IsFirstKeyPress(Keys.G))
            {
                ModelObject o = (ModelObject)this.archetypalBoxWireframe.Clone();
                o.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube,1));
                o.Transform3D = new Transform3D(Vector3.Up * 5, -Vector3.Forward, Vector3.Up);
                ObjectManager.Add(o);

                o = (ModelObject)o.Clone();
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
                
                hit.Sort((result, hitResult) => (int)(result.distance - hitResult.distance));
                   
                hit = Raycaster.RaycastAll(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    ObjectManager.FindAll(a => a != null));
                   
                Debug.WriteLine("NEW HIT : MULTI");
                   
                Debug.WriteLine("List size : " + hit.Count);
                
                hit.Sort((result, hitResult) => (int)(result.distance - hitResult.distance));
                
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
         
    }
}