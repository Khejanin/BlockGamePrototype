using GDGame.Enums;
using GDGame.Interfaces;
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
        public ColliderType ColliderType { get; private set; }
        protected ColliderShape colliderShape;
        protected bool drawDebug;
        protected Transform3D parentTransform;

        protected ColliderController(string id, ControllerType controllerType, ColliderShape colliderShape,
            ColliderType colliderType = ColliderType.Blocking) : base(id, controllerType)
        {
            this.colliderShape = colliderShape;
            ColliderType = colliderType;
        }

        public override object Clone()
        {
            return base.Clone();
        }

        public abstract void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }

    public class CustomBoxColliderController : ColliderController
    {
        private float scale;

        public CustomBoxColliderController(string id, ControllerType controllerType, ColliderShape colliderShape, float scale, ColliderType colliderType = ColliderType.Blocking) : base(id, controllerType, colliderShape, colliderType)
        {
            this.scale = scale;
        }

        public override object Clone()
        {
            return new CustomBoxColliderController(Id, ControllerType, colliderShape, scale, ColliderType);
        }

        public BoundingBox GetBounds(Actor3D parent)
        {
            Vector3 min = parent.Transform3D.Translation + new Vector3(-parent.Transform3D.Scale.X, -parent.Transform3D.Scale.Y, -parent.Transform3D.Scale.Z) / 2.0f * scale;
            Vector3 max = parent.Transform3D.Translation + new Vector3(parent.Transform3D.Scale.X, parent.Transform3D.Scale.Y, parent.Transform3D.Scale.Z) / 2.0f * scale;
            BoundingBox box = new BoundingBox(min, max);
            return box;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }

    public class PrimitiveColliderController : ColliderController
    {
        private PrimitiveColliderController(string id, ControllerType controllerType, ColliderShape colliderShape, ColliderType colliderType = ColliderType.Blocking) : base(id, controllerType, colliderShape, colliderType)
        {
        }

        public override object Clone()
        {
            return new PrimitiveColliderController(Id, ControllerType, colliderShape, ColliderType);
        }

        public BoundingBox GetBounds(PrimitiveObject parent)
        {
            return parent.GetDrawnBoundingBox();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }
}