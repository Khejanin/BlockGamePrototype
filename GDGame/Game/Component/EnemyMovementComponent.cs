using System;
using GDGame.Controllers;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class EnemyMovementComponent : PathMovementComponent, ICloneable
    {
        public EnemyMovementComponent(string id, ControllerType controllerType, ActivationType activationType,
            float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType,
            timePercent, smoothingMethod)
        {
        }

        protected override void MoveToNextPoint()
        {
            base.MoveToNextPoint();

            parent.MoveTo(new AnimationEventData()
            {
                isRelative = true, destination = Vector3.Up,
                maxTime = movementTime / 2,
                smoothing = Smoother.SmoothingMethod.Accelerate, loopMethod = LoopMethod.PingPongOnce,
                body = parent.Body
            });

            parent.ScaleTo(new AnimationEventData()
            {
                isRelative = false, destination = Vector3.One * 0.5f,
                maxTime = movementTime / 2,
                smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PingPongOnce
            });

            parent.RotateTo(new AnimationEventData()
            {
                isRelative = true, destination = Vector3.Up * 360,
                maxTime = movementTime,
                smoothing = Smoother.SmoothingMethod.Smooth
            });
        }

        public new object Clone()
        {
            EnemyMovementComponent enemyMovementComponent =
                new EnemyMovementComponent(ID, ControllerType, activationType, timePercent, smoothingMethod);
            enemyMovementComponent.OnClone();
            return enemyMovementComponent;
        }
    }
}