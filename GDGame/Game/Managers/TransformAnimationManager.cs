using System.Collections.Generic;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Parameters;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Managers
{

    public abstract class AnimationInformation : EventInfo
    {
        protected Transform3D transform3D;
        private int currentTime = 0;
        private int maxTime;
        private int step = 1;
        protected Vector3 start;
        private Vector3 destination;
        private LoopMethod loopMethod;
        private Smoother.SmoothingMethod smoothing;

        public AnimationInformation(Transform3D target, Vector3 destination,int maxTime, Smoother.SmoothingMethod smoothingMethod,LoopMethod loopMethod = LoopMethod.PlayOnce,Body body = null)
        {
            transform3D = target;
            this.destination = destination;
            this.maxTime = maxTime;
            this.smoothing = smoothingMethod;
            this.loopMethod = loopMethod;
        }

        public bool Tick(GameTime gameTime)
        {
            bool done = Looper.Loop(loopMethod, ref currentTime, ref step, maxTime);

            if (!done)
            {
                float timePercent = (float) currentTime / maxTime;
                float smoothedPercent = Smoother.SmoothValue(smoothing, timePercent);

                Vector3 currentPoint = Vector3.Lerp(start, destination, smoothedPercent);
                ApplyAnimation(currentPoint);

                currentTime += gameTime.ElapsedGameTime.Milliseconds;
            }

            return done;
        }

        protected abstract void ApplyAnimation(Vector3 target);
    }
    
    public class MovementInformation : AnimationInformation
    {
        private Body body;
        
        public MovementInformation(Transform3D target, Vector3 destination, int maxTime, Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) : base(target, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            start = transform3D.Translation;
            this.body = body;
        }

        protected override void ApplyAnimation(Vector3 target)
        {
            transform3D.Translation = target;
            if (body != null) body.MoveTo(target, Matrix.Identity);
        }
    }
    
    public class ScaleInformation : AnimationInformation
    {
        public ScaleInformation(Transform3D target, Vector3 destination, int maxTime, Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) : base(target, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            start = transform3D.Scale;
        }

        protected override void ApplyAnimation(Vector3 target)
        {
            transform3D.Scale = target;
        }
    }

    public class RotationInformation : AnimationInformation
    {
        public RotationInformation(Transform3D target, Vector3 destination, int maxTime, Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) : base(target, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            start = transform3D.RotationInDegrees;
        }

        protected override void ApplyAnimation(Vector3 target)
        {
            transform3D.RotateBy(target);
        }
    }
    
    public class TransformAnimationManager : PausableGameComponent
    {
        private List<MovementInformation> movementInformationList = new List<MovementInformation>();

        public TransformAnimationManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
            EventManager.RegisterListener<MovementInformation>(HandleMovementInformationEvent);
        }

        private void HandleMovementInformationEvent(MovementInformation information)
        {
            movementInformationList.Add(information);
        }
        
        protected override void ApplyUpdate(GameTime gameTime)
        {
            for (int i = 0; i < movementInformationList.Count; i++)
                if (movementInformationList[i].Tick(gameTime))
                    movementInformationList.RemoveAt(i--);
        }
    }
}