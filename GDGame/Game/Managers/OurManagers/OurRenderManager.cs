using System.Linq;
using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    /// <summary>
    ///     Our version of the RenderManager working with OurObjectManager.
    /// </summary>
    public class OurRenderManager : PausableDrawableGameComponent
    {
        #region Private variables

        private CameraManager<Camera3D> cameraManager;
        private OurObjectManager objectManager;
        private RasterizerState rasterizerStateOpaque;
        private RasterizerState rasterizerStateTransparent;
        private ScreenLayoutType screenLayoutType;

        #endregion

        #region Constructors

        public OurRenderManager(Microsoft.Xna.Framework.Game game, StatusType statusType,
            ScreenLayoutType screenLayoutType,
            OurObjectManager objectManager,
            CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.screenLayoutType = screenLayoutType;
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;

            InitializeGraphics();
        }

        #endregion

        #region Initialization

        private void InitializeGraphics()
        {
            SamplerState samplerState = new SamplerState
            {
                AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror
            };
            Game.GraphicsDevice.SamplerStates[0] = samplerState;

            //opaque objects
            rasterizerStateOpaque = new RasterizerState {CullMode = CullMode.CullCounterClockwiseFace};

            //transparent objects
            rasterizerStateTransparent = new RasterizerState {CullMode = CullMode.None};
        }

        #endregion

        #region Override Method

        /// <summary>
        ///     Called to draw the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Draw(GameTime)" />
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyDraw(GameTime gameTime)
        {
            if (screenLayoutType == ScreenLayoutType.Single)
                DrawSingle(gameTime, cameraManager.ActiveCamera);
            else
                DrawMulti(gameTime);

            // base.ApplyDraw(gameTime);
        }

        #endregion

        #region Private Method

        private void DrawMulti(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++) DrawSingle(gameTime, cameraManager[i]);
        }

        private void DrawSingle(GameTime gameTime, Camera3D activeCamera)
        {
            GraphicsDevice.Viewport = activeCamera.Viewport;

            SetGraphicsStateObjects(true);
            foreach (OurDrawnActor3D actor in objectManager.OpaqueList.Where(actor =>
                (actor.StatusType & StatusType.Drawn) == StatusType.Drawn))
                actor.Draw(gameTime, activeCamera, GraphicsDevice);

            SetGraphicsStateObjects(false);
            foreach (OurDrawnActor3D actor in objectManager.TransparentList.Where(actor =>
                (actor.StatusType & StatusType.Drawn) == StatusType.Drawn))
                actor.Draw(gameTime, activeCamera, GraphicsDevice);
        }

        private void SetGraphicsStateObjects(bool isOpaque)
        {
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            if (isOpaque)
            {
                Game.GraphicsDevice.RasterizerState = rasterizerStateOpaque;
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            else
            {
                Game.GraphicsDevice.RasterizerState = rasterizerStateTransparent;
                Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }
        }

        #endregion
    }
}