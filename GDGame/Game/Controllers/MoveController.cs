using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class MoveController : Controller
    {
        #region Private variables

        private Vector3 movementPerSecond;

        #endregion

        #region Constructors

        public MoveController(string id, ControllerType controllerType, Vector3 movementPerSecond) : base(id, controllerType)
        {
            this.movementPerSecond = movementPerSecond;
        }

        #endregion

        #region Override Methode

        public override bool Equals(object? obj)
        {
            return movementPerSecond == (obj as MoveController).movementPerSecond && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return movementPerSecond.GetHashCode() + base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString() + movementPerSecond;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D actor3D = actor as Actor3D;
            float elapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            Vector3 delta = elapsed * movementPerSecond;
            actor3D.Transform3D.TranslateBy(delta);
            base.Update(gameTime, actor);
        }

        #endregion
    }
}