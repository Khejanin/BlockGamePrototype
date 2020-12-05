using GDGame.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public abstract class ColliderComponent : Controller
    {
        protected OurCollidableObject parent;
        private bool handleIsSet;

        protected ColliderComponent(string id, ControllerType controllerType) : base(id, controllerType)
        {
            handleIsSet = false;
        }

        private void InitEventListeners()
        {
            parent.Body.CollisionSkin.callbackFn += HandleCollision;
        }

        protected abstract bool HandleCollision(CollisionSkin skin0, CollisionSkin skin1);

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as OurCollidableObject;
            if (handleIsSet == false && parent != null)
            {
                InitEventListeners();
                handleIsSet = true;
            }
        }
    }
}