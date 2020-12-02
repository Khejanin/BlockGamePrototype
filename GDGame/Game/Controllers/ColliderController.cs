using System;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace GDGame.Controllers
{
    public abstract class ColliderController : Controller
    {
        #region Private variables

        protected ColliderShape colliderShape;
        protected bool drawDebug;
        protected Transform3D parentTransform;

        #endregion

        #region Constructors

        protected ColliderController(string id, ControllerType controllerType, ColliderShape colliderShape,
            ColliderType colliderType = ColliderType.Blocking) : base(id, controllerType)
        {
            this.colliderShape = colliderShape;
            ColliderType = colliderType;
        }

        #endregion

        #region Properties, Indexers

        public ColliderType ColliderType { get; }

        #endregion

        #region Methods

        public abstract void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);

        #endregion
    }
    
    public class CustomBoxColliderController : ColliderController, ICloneable
    {
        #region Private variables

        private float scale;

        #endregion

        #region Constructors

        public CustomBoxColliderController(string id, ControllerType controllerType, ColliderShape colliderShape, float scale, ColliderType colliderType = ColliderType.Blocking) :
            base(id, controllerType, colliderShape, colliderType)
        {
            this.scale = scale;
        }

        #endregion

        #region Override Methode

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new CustomBoxColliderController(ID, ControllerType, colliderShape, scale, ColliderType);
        }

        public BoundingBox GetBounds(Actor3D parent)
        {
            Vector3 min = parent.Transform3D.Translation + new Vector3(-parent.Transform3D.Scale.X, -parent.Transform3D.Scale.Y, -parent.Transform3D.Scale.Z) / 2.0f * scale;
            Vector3 max = parent.Transform3D.Translation + new Vector3(parent.Transform3D.Scale.X, parent.Transform3D.Scale.Y, parent.Transform3D.Scale.Z) / 2.0f * scale;
            BoundingBox box = new BoundingBox(min, max);
            return box;
        }

        #endregion
    }

    
}