using GDGame.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class ColliderComponent : Controller
    {
        protected OurCollidableObject parent;
        private bool handleIsSet;
        private CollisionCallbackFn doIt;

        public ColliderComponent(string id, ControllerType controllerType, CollisionCallbackFn doIt) : base(id, controllerType)
        {
            handleIsSet = false;
            this.doIt = doIt;
        }

        private void InitEventListeners()
        {
            parent.Body.CollisionSkin.callbackFn += doIt;
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
            return new ColliderComponent(ID, ControllerType, doIt);
        }
    }
}