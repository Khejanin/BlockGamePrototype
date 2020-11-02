using System.Collections.Generic;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace GDGame.Game.Controllers
{
    public enum ColliderType
    {
        Cube, Sphere
    }
    
    public abstract class ColliderController : IDrawnController
    {
        protected ColliderType colliderType;
        protected bool drawDebug;
        protected Transform3D parentTransform;

        protected ColliderController(ColliderType colliderType)
        {
            this.colliderType = colliderType;
        }

        public abstract object Clone();

        protected abstract void Init(IActor actor);

        public void Initialize(IActor actor)
        {
            Init(actor);
        }

        public abstract void Update(GameTime gameTime, IActor actor);
        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        public abstract void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }
    
    public class CustomBoxColliderController : ColliderController
    { 
        private DrawnActor3D parent;
        
        private float scale;
        
        public CustomBoxColliderController(ColliderType colliderType, float scale) : base(colliderType)
        {
            this.scale = scale;
        }

        public override object Clone()
        {
            return new CustomBoxColliderController(colliderType, scale);
        }

        public BoundingBox GetBounds()
        {
            List<Vector3> positions = new List<Vector3>();

            Vector3 min = parent.Transform3D.Translation + new Vector3(-parent.Transform3D.Scale.X,
                -parent.Transform3D.Scale.Y, -parent.Transform3D.Scale.Z)/2.0f * scale;
            Vector3 max = parent.Transform3D.Translation + new Vector3(parent.Transform3D.Scale.X,
                parent.Transform3D.Scale.Y, parent.Transform3D.Scale.Z)/2.0f * scale;

            BoundingBox box = new BoundingBox(min, max);

            return box;
        }

        protected override void Init(IActor actor)
        {
            parent = actor as DrawnActor3D;
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
        private PrimitiveObject parent;
        
        public PrimitiveColliderController(ColliderType colliderType) : base(colliderType)
        {
        }

        public override object Clone()
        {
            return new PrimitiveColliderController(colliderType);
        }

        public BoundingBox GetBounds()
        {
            return parent.GetDrawnBoundingBox();
        }

        protected override void Init(IActor actor)
        {
            parent = actor as PrimitiveObject;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }
    
    public class ModelColliderController : ColliderController
    {
        private ModelObject parent;
        
        public ModelColliderController(ColliderType colliderType) : base(colliderType)
        {
        }

        public override object Clone()
        {
            return new PrimitiveColliderController(colliderType);
        }

        public List<BoundingSphere> GetBounds()
        {
            return parent.GetBounds();
        }

        protected override void Init(IActor actor)
        {
            parent = actor as ModelObject;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }
    }
}