using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GDLibrary.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using BlockGame.Scenes;
using GDGame.Game.Controllers;
using GDGame.Game.Factory;
using GDGame.Game.Utilities;
using GDGame.Scenes;
using GDLibrary;
using GDLibrary.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;

namespace GDGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect unlitTexturedEffect, unlitWireframeEffect;
        private CameraManager<Camera3D> cameraManager;
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private BasicEffect modelEffect;
        private SpriteFont debugFont;
        private ModelObject archetypalBoxWireframe;
        private BasicEffect wireframeModelEffect;
        private RasterizerState wireframeRasterizerState;
        Vector2 screenCentre = Vector2.Zero;
        private Scene currentScene;
        private SoundManager soundManager;
        
        public GraphicsDeviceManager Graphics => _graphics;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public BasicEffect ModelEffect => modelEffect;
        public BasicEffect UnlitTexturedEffect => unlitTexturedEffect;
        public BasicEffect UnlitWireframeEffect => unlitWireframeEffect;
        public CameraManager<Camera3D> CameraManager => cameraManager;
        public ObjectManager ObjectManager => objectManager;
        public KeyboardManager KeyboardManager => keyboardManager;
        public MouseManager MouseManager => mouseManager;
        public SpriteFont DebugFont => debugFont;
        public BasicEffect WireframeModelEffect => wireframeModelEffect;
        public RasterizerState WireframeRasterizerState => wireframeRasterizerState;
        public Vector2 ScreenCentre => screenCentre;
        public SoundManager SoundManager => soundManager;

        //eventually we will remove this content
        private VertexPositionColorTexture[] vertices;
        private Texture2D backSky, leftSky, rightSky, frontSky, topSky, grass;
        private PrimitiveObject archetypalTexturedQuad;
        private float worldScale = 3000;
        PrimitiveObject primitiveObject = null;


        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region Initialization - Managers, Cameras, Effects, Textures, Audio
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "My Amazing Game";

            //camera
            cameraManager = new CameraManager<Camera3D>(this);
            Components.Add(this.cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);

            //mouse
            mouseManager = new MouseManager(this, false);
            Components.Add(this.mouseManager);

            //Sound
            this.soundManager = new SoundManager(this);
            Components.Add(this.soundManager);

            InitManagers();
            InitFonts();
            InitEffect();

            InitGraphics(1024, 768);

            base.Initialize();

            currentScene = new MainScene(this);
            currentScene.Initialize();
        }

        private void InitDebug()
        {
            /*Components.Add(new DebugDrawer(this, _spriteBatch, this.debugFont,
                this.cameraManager, this.objectManager));*/

        }

        private void InitFonts()
        {
            this.debugFont = Content.Load<SpriteFont>("Assets/Fonts/debug");
        }

        private void InitManagers()
        {
            this.objectManager = new ObjectManager(this, 6, 10, this.cameraManager);
            Components.Add(this.objectManager);
        }

        private void InitEffect()
        {
            //to do...
            this.unlitTexturedEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.unlitTexturedEffect.VertexColorEnabled = true; //otherwise we wont see RGB
            this.unlitTexturedEffect.TextureEnabled = true;

            //wireframe primitives with no lighting and no texture
            this.unlitWireframeEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.unlitWireframeEffect.VertexColorEnabled = true;

            //model effect
            //add a ModelObject
            this.modelEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.modelEffect.TextureEnabled = true;
            //this.modelEffect.LightingEnabled = true;
            //this.modelEffect.EnableDefaultLighting();

            this.wireframeModelEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.wireframeModelEffect.TextureEnabled = false;
            this.wireframeModelEffect.VertexColorEnabled = true;

            this.wireframeRasterizerState = new RasterizerState();
            this.wireframeRasterizerState.FillMode = FillMode.WireFrame;
        }


        #endregion

        #region Load and Unload Content

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitDebug();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

        private void InitGraphics(int width, int height)
        {
            //set resolution
            this._graphics.PreferredBackBufferWidth = width;
            this._graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            this._graphics.ApplyChanges();

            //set screen centre based on resolution
            this.screenCentre = new Vector2(width / 2, height / 2);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            this._graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            this._graphics.GraphicsDevice.SamplerStates[0] = samplerState;
        }


        #region Update & Draw
        protected override void Update(GameTime gameTime)
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
            currentScene.Update(gameTime);
        }

        private void RaycastTests()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.G))
            {
                ModelObject o = (ModelObject)this.archetypalBoxWireframe.Clone();
                o.ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube,1));
                o.Transform3D = new Transform3D(Vector3.Up * 5, -Vector3.Forward, Vector3.Up);
                objectManager.Add(o);

                o = (ModelObject)o.Clone();
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
                
                hit.Sort((result, hitResult) => (int)(result.distance - hitResult.distance));
                   
                hit = Raycaster.RaycastAll(new Vector3(-5, 5, 0), new Vector3(1, 0, 0),
                    objectManager.FindAll(a => a != null));
                   
                Debug.WriteLine("NEW HIT : MULTI");
                   
                Debug.WriteLine("List size : " + hit.Count);
                
                hit.Sort((result, hitResult) => (int)(result.distance - hitResult.distance));
                
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
    
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
            currentScene.Draw(gameTime);
        }

        #endregion
    }
}