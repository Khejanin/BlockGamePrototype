using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.Controllers.CameraControllers
{
    public class RotationAroundActor : Controller
    {
        #region Fields

        private KeyboardManager keyboardManager;
        private float angle;
        private Actor3D target;

        public Actor3D Target
        {
            get => target;
            set => target = value;
        }

        #endregion

        public RotationAroundActor(string id, ControllerType controllerType, KeyboardManager keyboardManager,
            float angle, Actor3D target = null) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.angle = angle;
            this.target = target;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (actor is Actor3D parent)
            {
                HandleKeyboardInput(gameTime, parent);
            }
        }

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            if (keyboardManager.IsKeyDown(Keys.Q))
            {
                parent.Transform3D.Translation =
                    Vector3.Transform(parent.Transform3D.Translation - target.Transform3D.Translation,
                        Matrix.CreateFromAxisAngle(Vector3.Up, angle)) + target.Transform3D.Translation;
            }

            if (keyboardManager.IsKeyDown(Keys.E))
            {
                parent.Transform3D.Translation =
                    Vector3.Transform(parent.Transform3D.Translation - target.Transform3D.Translation,
                        Matrix.CreateFromAxisAngle(Vector3.Up, -angle)) + target.Transform3D.Translation;
            }

            if(target == null) return;
            Vector3 view = target.Transform3D.Translation - parent.Transform3D.Translation;
            
            Vector3 rotationAxis = Vector3.Cross(parent.Transform3D.Look, view);
            
            parent.Transform3D.RotateBy(rotationAxis);
            
            parent.Transform3D.Up = Vector3.Up;
        }
    }
}