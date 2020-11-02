﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BlockGame.Scenes;

namespace GDLibrary
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect unlitTexturedEffect, unlitWireframeEffect;
        private CameraManager cameraManager;
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

        public GraphicsDeviceManager Graphics => _graphics;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public BasicEffect ModelEffect => modelEffect;
        public BasicEffect UnlitTexturedEffect => unlitTexturedEffect;
        public BasicEffect UnlitWireframeEffect => unlitWireframeEffect;
        public CameraManager CameraManager => cameraManager;
        public ObjectManager ObjectManager => objectManager;
        public KeyboardManager KeyboardManager => keyboardManager;
        public MouseManager MouseManager => mouseManager;
        public SpriteFont DebugFont => debugFont;
        public BasicEffect WireframeModelEffect => wireframeModelEffect;
        public RasterizerState WireframeRasterizerState => wireframeRasterizerState;
        public Vector2 ScreenCentre => screenCentre;

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

        #region Initialization - Managers, Cameras, Effects, Textures
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "My Amazing Game";

            //camera
            cameraManager = new CameraManager(this);
            Components.Add(this.cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);

            //mouse
            mouseManager = new MouseManager(this, false);
            Components.Add(this.mouseManager);

            InitManagers();
            InitFonts();
            InitEffect();

            InitGraphics(1024, 768);
           
            base.Initialize();

            currentScene = new MainScene(this);
            currentScene.Initialize();
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            _graphics.PreferredBackBufferWidth = width;
            this._graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            this._graphics.ApplyChanges();

            //set screen centre based on resolution
            screenCentre = new Vector2(width / 2, height / 2);

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

        protected void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitDebug();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion


        #region Update & Draw
        protected override void Update(GameTime gameTime)
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            base.Update(gameTime);
            currentScene.Update(gameTime);
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