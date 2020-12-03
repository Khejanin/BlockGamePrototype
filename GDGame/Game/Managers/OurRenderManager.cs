using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using BlendState = Microsoft.Xna.Framework.Graphics.BlendState;

namespace GDGame.Managers
{
    public class OurRenderManager : PausableDrawableGameComponent
    {
        private ScreenLayoutType screenLayoutType;
        private OurObjectManager objectManager;
        private CameraManager<Camera3D> cameraManager;

        public OurRenderManager(Microsoft.Xna.Framework.Game game, StatusType statusType,
            ScreenLayoutType screenLayoutType,
            OurObjectManager objectManager,
            CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.screenLayoutType = screenLayoutType;
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;
        }

        /// <summary>
        /// Called to draw the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Draw(GameTime)"/>
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.screenLayoutType == ScreenLayoutType.Single)
                DrawSingle(gameTime, cameraManager.ActiveCamera);
            else
                DrawMulti(gameTime);

            // base.ApplyDraw(gameTime);
        }

        private void DrawMulti(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawSingle(gameTime, cameraManager[i]);
            }
        }

        private void DrawSingle(GameTime gameTime, Camera3D activeCamera)
        {
            this.GraphicsDevice.Viewport = activeCamera.Viewport;

            foreach (OurDrawnActor3D actor in objectManager.OpaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime,activeCamera,GraphicsDevice);
            }
            
            this.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (OurDrawnActor3D actor in objectManager.TransparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, activeCamera, GraphicsDevice);
            }
            
            GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}