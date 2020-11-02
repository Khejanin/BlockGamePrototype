using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDLibrary.Controllers
{
    public class RailController : Controller
    {
        public RailController(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            //to do...
            base.Update(gameTime, actor);
        }
    }
}