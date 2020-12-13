using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Managers;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plane = JigLibX.Geometry.Plane;

namespace GDGame.Managers
{
    /// <summary>
    ///     Renders the collision skins of any collidable objects within the scene. We can disable this component for the
    ///     release.
    ///     Our Version for Our Actors.
    /// </summary>
    public class OurPhysicsDebugDrawer : PausableDrawableGameComponent
    {
        #region Private variables

        private OurDrawnActor3D actor;
        private BasicEffect basicEffect;

        private CameraManager<Camera3D> cameraManager;

        private OurObjectManager objectManager;
        private List<VertexPositionColor> vertexData;
        private VertexPositionColor[] wf;

        #endregion

        #region Constructors

        public OurPhysicsDebugDrawer(Microsoft.Xna.Framework.Game game, StatusType statusType,
            CameraManager<Camera3D> cameraManager,
            OurObjectManager objectManager)
            : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            this.objectManager = objectManager;
            vertexData = new List<VertexPositionColor>();
            basicEffect = new BasicEffect(game.GraphicsDevice);
        }

        #endregion

        #region Override Method

        protected override void ApplyDraw(GameTime gameTime)
        {
            //add the vertices for each and every drawn object (opaque or transparent) to the vertexData array for drawing
            ProcessAllDrawnObjects();

            //no vertices to draw - would happen if we forget to call DrawCollisionSkins() above or there were no drawn objects to see!
            if (vertexData.Count == 0) return;

            basicEffect.AmbientLightColor = Vector3.One;
            basicEffect.VertexColorEnabled = false;

            DrawCollisionSkin(cameraManager.ActiveCamera);

            vertexData.Clear();
        }

        #endregion

        #region Public Method

        public void AddCollisionSkinVertexData(OurCollidableObject collidableObject)
        {
            if (!collidableObject.Body.CollisionSkin.GetType().Equals(typeof(Plane)))
            {
                wf = collidableObject.Collision.GetLocalSkinWireframe();

                // if the collision skin was also added to the body
                // we have to transform the skin wireframe to the body space
                if (collidableObject.Body.CollisionSkin != null) collidableObject.Body.TransformWireframe(wf);

                AddVertexDataForShape(wf, Color.Red);
            }
        }

        public void AddVertexDataForShape(List<Vector3> shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, color));
                vertexData.Add(new VertexPositionColor(shape[0], color));
            }

            foreach (Vector3 p in shape) vertexData.Add(new VertexPositionColor(p, color));
        }

        public void AddVertexDataForShape(List<Vector3> shape, Color color, bool closed)
        {
            AddVertexDataForShape(shape, color);

            Vector3 v = shape[0];
            vertexData.Add(new VertexPositionColor(v, color));
        }

        public void AddVertexDataForShape(List<VertexPositionColor> shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, color));
                vertexData.Add(new VertexPositionColor(shape[0].Position, color));
            }

            foreach (VertexPositionColor vps in shape) vertexData.Add(vps);
        }

        public void AddVertexDataForShape(VertexPositionColor[] shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                if (shape.Length > 0)
                {
                    vertexData.Add(new VertexPositionColor(v, color));
                    vertexData.Add(new VertexPositionColor(shape[0].Position, color));
                }
            }

            foreach (VertexPositionColor vps in shape) vertexData.Add(vps);
        }

        public void AddVertexDataForShape(List<VertexPositionColor> shape, Color color, bool closed)
        {
            AddVertexDataForShape(shape, color);

            VertexPositionColor v = shape[0];
            vertexData.Add(v);
        }

        #endregion

        #region Private Method

        private void DrawCollisionSkin(Camera3D activeCamera)
        {
            Game.GraphicsDevice.Viewport = activeCamera.Viewport;
            basicEffect.View = activeCamera.View;
            basicEffect.Projection = activeCamera.Projection;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertexData.ToArray(), 0, vertexData.Count - 1);
        }

        //debug method to draw collision skins for collidable objects and zone objects
        private void ProcessAllDrawnObjects()
        {
            for (int i = 0; i < objectManager.OpaqueList.Count; i++)
            {
                actor = objectManager.OpaqueList[i];
                if (actor is Tile tile && tile.TileType != ETileType.Static)
                    AddCollisionSkinVertexData(actor as OurCollidableObject);
            }

            for (int i = 0; i < objectManager.TransparentList.Count; i++)
            {
                actor = objectManager.TransparentList[i];
                if (actor is OurCollidableObject)
                    AddCollisionSkinVertexData(actor as OurCollidableObject);
            }
        }

        #endregion
    }
}