using System.Collections.Generic;
using GDGame.Controllers;
using GDGame.Game.Actors;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using SharpDX.XInput;

namespace GDGame.Component
{
    /// <summary>
    /// Component that handles the Coffee Movement.
    /// </summary>
    public class CoffeeMovementComponent : PathMovementComponent
    {
        private Coffee coffeeParent;
        
        public CoffeeMovementComponent(string id, ControllerType controllerType, ActivationType activationType, float timePercent, Smoother.SmoothingMethod smoothingMethod,Coffee coffee) : base(id, controllerType, activationType, timePercent, smoothingMethod)
        {
            parent = coffee;
            coffeeParent = coffee;
            MoveToNextPoint();
        }

        /// <summary>
        /// Override implementation that invokes PointReached when done.
        /// Because we have custom timings for each point we want to reach we just handle the next MoveToNextPoint() as a callback.
        /// </summary>
        protected override void MoveToNextPoint()
        {
            Vector3 nextPoint = NextPathPoint();
            
            parent.MoveTo(new AnimationEventData()
            {
                callback = PointReached,
                destination = nextPoint,
                body = parent.Body,
                isRelative = false,
                maxTime = (int) coffeeParent.CoffeeInfo[currentPositionIndex].TimeInMs
            });
        }

        protected void PointReached()
        {
            MoveToNextPoint();
        }
        
    }
}