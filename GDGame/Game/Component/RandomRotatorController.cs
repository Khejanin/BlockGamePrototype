using System;
using GDGame.Utilities;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using SharpDX;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GDGame.Component
{
    public class RandomRotatorController : Controller
    {
        private Vector3 rotation;
        
        public RandomRotatorController(string id, ControllerType controllerType) : base(id, controllerType)
        {
            SharpDX.Vector3 vec = RandomUtil.NextVector3(MathHelperFunctions.rnd,SharpDX.Vector3.Zero, SharpDX.Vector3.One);
            rotation = new Vector3(vec.X,vec.Y,vec.Z);
        }
    }
}