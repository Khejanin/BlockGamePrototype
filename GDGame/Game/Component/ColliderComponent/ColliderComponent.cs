using System;
using GDGame.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class ColliderComponent : Controller, ICloneable
    {
        protected OurCollidableObject parent;
        private bool handleIsSet;
        private CollisionCallbackFn handleCollision;

        public ColliderComponent(string id, ControllerType controllerType, CollisionCallbackFn handleCollision) : base(id, controllerType)
        {
            handleIsSet = false;
            this.handleCollision = handleCollision;
        }

        private void InitEventListeners()
        {
            parent.Body.CollisionSkin.callbackFn += handleCollision;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as OurCollidableObject;
            if (handleIsSet == false && parent != null)
            {
                InitEventListeners();
                handleIsSet = true;
            }
        }

        public new object Clone()
        {
            return new ColliderComponent(ID, ControllerType, handleCollision);
        }
    }
}