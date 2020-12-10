using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UiBlinkingController : Controller
    {
        private UITextureObject parent;
        private bool active;
        private bool isVisible;
        private float cooldown;
        private float currentCD;
        
        public UiBlinkingController(string id, ControllerType controllerType, float cooldown) : base(id, controllerType)
        {
            this.cooldown = cooldown;
            EventManager.RegisterListener<CoffeeEventInfo>(HandleCoffeeEventInfo);
        }

        private void HandleCoffeeEventInfo(CoffeeEventInfo obj)
        {
            if (obj.coffeeEventType == CoffeeEventType.CoffeeStartMoving)
            {
                active = true;
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as UITextureObject;
            if (active)
            {
                if (currentCD < cooldown) currentCD += gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    if (isVisible)
                    {
                        SetInvisible();
                    }
                    else
                    {
                        SetVisible();
                    }

                    currentCD = 0;
                }
            }
        }

        private void SetInvisible()
        {
            parent.StatusType = StatusType.Update;
            isVisible = false;
        }

        private void SetVisible()
        {
            parent.StatusType = StatusType.Drawn | StatusType.Update;
            isVisible = true;
        }
    }
}