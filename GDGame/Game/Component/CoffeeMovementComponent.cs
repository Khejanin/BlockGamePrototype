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
    public class CoffeeMovementComponent : PathMovementComponent
    {
        private Coffee coffeeParent;
        
        public CoffeeMovementComponent(string id, ControllerType controllerType, ActivationType activationType, float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType, timePercent, smoothingMethod)
        {
        }

        protected override void MoveToNextPoint()
        {
            Vector3 nextPoint = NextPathPoint();
            
            parent.MoveTo(new AnimationEventData()
            {
                callback = PointReached,
                destination = nextPoint,
                isRelative = false,
                maxTime = (int) (coffeeParent ?? parent as Coffee).CoffeeInfo[currentPositionIndex].TimeInMs
            });
        }

        protected void PointReached()
        {
            
        }
        
    }
}