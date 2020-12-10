using GDLibrary.Controllers;
using GDLibrary.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Controllers
{
    public class UiCoffeeWarningTextureSwitcher : Controller
    {
        private Texture2D textureToSwitch;
        
        public UiCoffeeWarningTextureSwitcher(string id, ControllerType controllerType,Texture2D textureTo) : base(id, controllerType)
        {
            textureToSwitch = textureTo;
        }
        
        
        
    }
}