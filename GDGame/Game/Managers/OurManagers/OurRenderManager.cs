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
    public class OurRenderManager : PausableDrawableGameComponent
    {
        #region Private variables

        private CameraManager<Camera3D> cameraManager;
        private OurObjectManager objectManager;
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
        }

        #endregion

        #region Override Methode

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

        #region Methods

        private void DrawMulti(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++) DrawSingle(gameTime, cameraManager[i]);
        }

        private void DrawSingle(GameTime gameTime, Camera3D activeCamera)
        {
            GraphicsDevice.Viewport = activeCamera.Viewport;

            foreach (OurDrawnActor3D actor in objectManager.OpaqueList.Where(actor =>
                (actor.StatusType & StatusType.Drawn) == StatusType.Drawn))
                actor.Draw(gameTime, activeCamera, GraphicsDevice);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (OurDrawnActor3D actor in objectManager.TransparentList.Where(actor =>
                (actor.StatusType & StatusType.Drawn) == StatusType.Drawn))
                actor.Draw(gameTime, activeCamera, GraphicsDevice);

            GraphicsDevice.BlendState = BlendState.Opaque;
        }

        #endregion
    }
}