using System;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UiBlinkingController : Controller, IDisposable
    {
        #region Private variables

        private bool active;
        private float cooldown;
        private float currentCd;
        private bool isVisible;
        private UITextureObject parent;

        #endregion

        #region Constructors

        public UiBlinkingController(string id, ControllerType controllerType, float cooldown) : base(id, controllerType)
        {
            this.cooldown = cooldown;
            EventManager.RegisterListener<CoffeeEventInfo>(HandleCoffeeEventInfo);
        }

        #endregion

        #region Override Method

        public override void Dispose()
        {
            parent = null;
            EventManager.UnregisterListener<CoffeeEventInfo>(HandleCoffeeEventInfo);
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as UITextureObject;
            if (active)
            {
                if (currentCd < cooldown)
                {
                    currentCd += gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    if (isVisible)
                        SetInvisible();
                    else
                        SetVisible();

                    currentCd = 0;
                }
            }
        }

        #endregion

        #region Private Method

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

        #endregion

        #region Events

        private void HandleCoffeeEventInfo(CoffeeEventInfo obj)
        {
            if (obj.coffeeEventType == CoffeeEventType.CoffeeStartMoving) active = true;
        }

        #endregion
    }
}