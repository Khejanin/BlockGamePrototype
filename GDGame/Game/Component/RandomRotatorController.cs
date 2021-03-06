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
    /// <summary>
    ///     Controller that randomly rotates the object that you attach it to.
    /// </summary>
    public class RandomRotatorController : Controller
    {
        #region Private variables

        private Vector3 rotation;

        #endregion

        #region Constructors

        public RandomRotatorController(string id, ControllerType controllerType) : base(id, controllerType)
        {
            SharpDX.Vector3 vec = MathHelperFunctions.Rnd.NextVector3(SharpDX.Vector3.Zero, SharpDX.Vector3.One);
            rotation = new Vector3(vec.X, vec.Y, vec.Z);
        }

        #endregion

        #region Override Method

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D actor3D = actor as Actor3D;
            actor3D?.Transform3D.RotateBy(rotation * (float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        #endregion
    }
}