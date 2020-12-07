using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Controllers
{
    /// <summary>
    /// Player Camera Controller that rotates around the Player Actor.
    /// </summary>
    public class RotationAroundActor : Controller
    {
        #region Private variables

        private float angle = -15;
        private float distance;
        private float elevationAngle;

        private KeyboardManager keyboardManager;
        private IActor target;

        #endregion

        #region Constructors

        public RotationAroundActor(string id, ControllerType controllerType, KeyboardManager keyboardManager, float elevationAngle, float distance) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.elevationAngle = elevationAngle;
            this.distance = distance;
        }

        #endregion

        #region Properties, Indexers

        public IActor Target
        {
            set => target = value;
        }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            HandelInput();
            if (actor is Actor3D parent && target is Actor3D target3D)
            {
                Vector3 targetToCamera = Vector3.Transform(target3D.Transform3D.Look,
                    Matrix.CreateFromAxisAngle(target3D.Transform3D.Right, MathHelper.ToRadians(elevationAngle)));
                targetToCamera = Vector3.Transform(targetToCamera,
                    Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(angle)));

                targetToCamera.Normalize();

                parent.Transform3D.Translation = target3D.Transform3D.Translation + targetToCamera * distance;
                parent.Transform3D.Look = -targetToCamera;
            }
        }

        #endregion

        #region Methods

        private void HandelInput()
        {
            if (keyboardManager.IsKeyDown(Keys.Q))
                angle -= 1;
            else if (keyboardManager.IsKeyDown(Keys.E))
                angle += 1;

            angle %= 360;
        }

        #endregion
    }
}