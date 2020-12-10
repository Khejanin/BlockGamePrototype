using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Controllers
{
    public class UiCoffeeWarningTextureSwitcher : Controller
    {
        private UITextureObject parent;
        private Texture2D initialTexture;
        private Texture2D textureToSwitch;
        
        public UiCoffeeWarningTextureSwitcher(string id, ControllerType controllerType,Texture2D textureTo) : base(id, controllerType)
        {
            textureToSwitch = textureTo;
            EventManager.RegisterListener<CoffeeEventInfo>(HandleCoffeeEventInfo);
        }

        private void HandleCoffeeEventInfo(CoffeeEventInfo obj)
        {
            switch (obj.coffeeEventType)
            {
                case CoffeeEventType.CoffeeDanger:
                    parent.Texture = textureToSwitch;
                    break;
                case CoffeeEventType.CoffeeDangerStop:
                    parent.Texture = initialTexture;
                    break;
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (parent == null)
            {
                parent = actor as UITextureObject;
                initialTexture = parent.Texture;
            }
        }
    }
}