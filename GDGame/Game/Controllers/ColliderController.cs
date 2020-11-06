﻿using System.Collections.Generic;
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
        Cube,
        Sphere
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
            Vector3 min = parent.Transform3D.Translation + new Vector3(-parent.Transform3D.Scale.X,
                -parent.Transform3D.Scale.Y, -parent.Transform3D.Scale.Z) / 2.0f * scale;
            Vector3 max = parent.Transform3D.Translation + new Vector3(parent.Transform3D.Scale.X,
                parent.Transform3D.Scale.Y, parent.Transform3D.Scale.Z) / 2.0f * scale;

            BoundingBox box = new BoundingBox(min, max);

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

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (PrimitiveObject) actor;
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
            return new ModelColliderController(colliderType);
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
}