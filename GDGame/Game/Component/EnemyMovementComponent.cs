using System;
using GDGame.Controllers;
using GDGame.Utilities;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class EnemyMovementComponent : PathMovementComponent, ICloneable
    {
        public EnemyMovementComponent(string id, ControllerType controllerType, ActivationType activationType, float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType, timePercent, smoothingMethod)
        {
        }

        protected override void MoveToNextPoint()
        {
            base.MoveToNextPoint();
            parent.MoveTo(true,Vector3.Up, movementTime/2,Smoother.SmoothingMethod.Accelerate,LoopMethod.PingPongOnce);
            parent.ScaleTo(false,Vector3.One*0.5f,movementTime/2,Smoother.SmoothingMethod.Smooth,LoopMethod.PingPongOnce);
            parent.RotateTo(true,Vector3.Up* 360,movementTime,Smoother.SmoothingMethod.Smooth);
        }

        public new object Clone()
        {
            EnemyMovementComponent enemyMovementComponent = new EnemyMovementComponent(ID,ControllerType,activationType,timePercent,smoothingMethod);
            enemyMovementComponent.OnClone();
            return enemyMovementComponent;
        }
    }
}