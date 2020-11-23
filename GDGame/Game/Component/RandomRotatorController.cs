using System;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
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

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D actor3D = actor as Actor3D;
            actor3D.Transform3D.RotateBy(rotation * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}