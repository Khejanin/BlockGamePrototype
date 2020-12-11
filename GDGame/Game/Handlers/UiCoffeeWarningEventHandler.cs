using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Controllers
{
    public class UiCoffeeWarningEventHandler : EventHandler
    {
        #region Private variables

        private Texture2D initialTexture;
        private Texture2D textureToSwitch;
        private UITextureObject uiTextureObject;

        #endregion

        #region Constructors

        public UiCoffeeWarningEventHandler(EventCategoryType eventCategoryType, IActor parent,
            Texture2D textureToSwitch) : base(eventCategoryType, parent)
        {
            this.textureToSwitch = textureToSwitch;
            uiTextureObject = parent as UITextureObject;
            if (uiTextureObject != null) initialTexture = uiTextureObject.Texture;
            EventManager.RegisterListener<CoffeeEventInfo>(HandleEvent);
        }

        #endregion

        #region Events

        private void HandleEvent(CoffeeEventInfo coffeeEventInfo)
        {
            uiTextureObject.Texture = coffeeEventInfo.coffeeEventType switch
            {
                CoffeeEventType.CoffeeDanger => textureToSwitch,
                CoffeeEventType.CoffeeDangerStop => initialTexture,
                _ => uiTextureObject.Texture
            };
        }

        #endregion
    }
}