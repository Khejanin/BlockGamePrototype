using GDGame.Controllers;
using GDGame.Game.Actors;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    /// <summary>
    ///     Component that handles the Coffee Movement.
    /// </summary>
    public class CoffeeMovementComponent : PathMovementComponent
    {
        #region Private variables

        private Coffee coffeeParent;
        private Vector3 destination;

        private bool isLowering;
        private float loweringSpeed;
        private float loweringTarget;
        private float speed;

        #endregion

        #region Constructors

        public CoffeeMovementComponent(string id, ControllerType controllerType, ActivationType activationType,
            float timePercent, Smoother.SmoothingMethod smoothingMethod, Coffee coffee) : base(id, controllerType,
            activationType, timePercent, smoothingMethod)
        {
            parent = coffee;
            coffeeParent = coffee;
        }

        #endregion

        #region Override Method

        /// <summary>
        ///     Override implementation that invokes PointReached when done.
        ///     Because we have custom timings for each point we want to reach we just handle the next MoveToNextPoint() as a
        ///     callback.
        /// </summary>
        protected override void MoveToNextPoint()
        {
            destination = NextPathPoint();
            speed = (destination.Y - parent.Transform3D.Translation.Y) /
                    coffeeParent.CoffeeInfo[currentPositionIndex].timeInMs;
        }

        protected override void OnActivated()
        {
            MoveToNextPoint();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (parent.Transform3D.Translation.Y != destination.Y)
            {
                float spd = speed;

                if (isLowering)
                {
                    spd = loweringSpeed;
                    if (parent.Transform3D.Translation.Y + speed < loweringTarget)
                    {
                        parent.SetTranslation(Vector3.Up * loweringTarget);
                        isLowering = false;
                    }
                    else
                    {
                        parent.SetTranslation(parent.Transform3D.Translation +
                                              Vector3.Up * spd * gameTime.ElapsedGameTime.Milliseconds);
                    }
                }
                else
                {
                    if (parent.Transform3D.Translation.Y + speed > destination.Y)
                        parent.SetTranslation(destination);
                    else
                        parent.SetTranslation(parent.Transform3D.Translation +
                                              Vector3.Up * spd * gameTime.ElapsedGameTime.Milliseconds);
                }
            }
            else
            {
                PointReached();
            }
        }

        #endregion

        #region Public Method

        public float GetTotalTimeLeft(Transform3D playerTransform)
        {
            return (playerTransform.Translation.Y + 0.5f - parent.Transform3D.Translation.Y) / speed;
        }

        public void StartLowering(float target, float speed)
        {
            isLowering = true;
            loweringTarget = target;
            loweringSpeed = speed;
        }

        #endregion

        #region Private Method

        private void PointReached()
        {
            MoveToNextPoint();
        }

        #endregion
    }
}