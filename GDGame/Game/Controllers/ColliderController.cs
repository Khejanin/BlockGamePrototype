using GDGame.Enums;
using GDGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace GDGame.Controllers
{
    public abstract class ColliderController : IDrawnController
    {
        public ColliderType ColliderType { get; private set; }
        protected ColliderShape colliderShape;
        protected bool drawDebug;
        protected Transform3D parentTransform;

        protected ColliderController(ColliderShape colliderShape, ColliderType colliderType = ColliderType.Blocking)
        {
            this.colliderShape = colliderShape;
            ColliderType = colliderType;
        }

        public abstract object Clone();

        public abstract void Update(GameTime gameTime, IActor actor);

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        public abstract void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }

    public class CustomBoxColliderController : ColliderController
    {
        private Actor3D parent;

        private float scale;

        public CustomBoxColliderController(ColliderShape colliderShape, float scale,
            ColliderType colliderType = ColliderType.Blocking) : base(colliderShape, colliderType)
        {
            this.scale = scale;
        }

        public override object Clone()
        {
            return new CustomBoxColliderController(colliderShape, scale, ColliderType);
        }

        public BoundingBox GetBounds()
        {
            BoundingBox box = new BoundingBox(new Vector3(0f), new Vector3(0));
            if (parent != null)
            {
                Vector3 min = parent.Transform3D.Translation + new Vector3(-parent.Transform3D.Scale.X,
                    -parent.Transform3D.Scale.Y, -parent.Transform3D.Scale.Z) / 2.0f * scale;
                Vector3 max = parent.Transform3D.Translation + new Vector3(parent.Transform3D.Scale.X,
                    parent.Transform3D.Scale.Y, parent.Transform3D.Scale.Z) / 2.0f * scale;
                box = new BoundingBox(min, max);
            }


            return box;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (Actor3D) actor;
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }

    public class PrimitiveColliderController : ColliderController
    {
        private PrimitiveObject parent;

        public PrimitiveColliderController(ColliderShape colliderShape) : base(colliderShape)
        {
        }

        public override object Clone()
        {
            return new PrimitiveColliderController(colliderShape);
        }

        public BoundingBox GetBounds()
        {
            return parent.GetDrawnBoundingBox();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (PrimitiveObject) actor;
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }

/*
    public class ModelColliderController : ColliderController
    {
        private ModelObject parent;

        public ModelColliderController(ColliderShape colliderShape) : base(colliderShape)
        {
        }

        public override object Clone()
        {
            return new ModelColliderController(colliderShape);
        }

        public List<BoundingSphere> GetBounds()
        {
            return parent.GetBounds();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (ModelObject) actor;
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }
    */
}