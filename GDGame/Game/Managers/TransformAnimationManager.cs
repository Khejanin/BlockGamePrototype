using System.Collections.Generic;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    public abstract class AnimationInformation : EventInfo
    {
        protected Actor3D actor;
        private int currentTime;
        private int maxTime;
        private int step = 1;
        protected Vector3 start;
        private Vector3 lastPoint = Vector3.Zero;
        private Vector3 destination;
        private LoopMethod loopMethod;
        private Smoother.SmoothingMethod smoothing;
        protected Process process;
        protected bool isRelative;
        protected Body body;

        protected AnimationInformation(Actor3D actor,bool isRelative,
            Vector3 destination, int maxTime, Smoother.SmoothingMethod smoothingMethod,
            LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null)
        {
            this.actor = actor;
            this.destination = destination;
            this.maxTime = maxTime;
            smoothing = smoothingMethod;
            this.loopMethod = loopMethod;
            this.isRelative = isRelative;
            this.body = body;
        }

        public bool Tick(GameTime gameTime)
        {
            if (actor != null && (actor.StatusType & StatusType.Update) == StatusType.Update)
            {
                bool done = Looper.Loop(loopMethod, ref currentTime, ref step, maxTime);

                float timePercent = (float) currentTime / maxTime;
                float smoothedPercent = Smoother.SmoothValue(smoothing, timePercent);

                Vector3 currentPoint = Vector3.Lerp(isRelative ? Vector3.Zero : start, destination, smoothedPercent);

                FinalOperation finalOperation = PerformOperation(currentPoint, lastPoint);

                process(finalOperation);
                
                lastPoint = currentPoint;

                currentTime += gameTime.ElapsedGameTime.Milliseconds * step;

                return done;
            }

            return false;
        }

        public delegate void Process(FinalOperation finalOperation);
        
        public delegate Vector3 FinalOperation(Vector3 target);

        private FinalOperation PerformOperation(Vector3 currentPoint, Vector3 lastPoint)
        {
            if (isRelative && loopMethod == LoopMethod.PingPongOnce)
            {
                return target => target + currentPoint;
            }
            
            if (isRelative)
            {
                return target => target + currentPoint - lastPoint;
            }
            
            return vector3 => currentPoint;
        }

        private Vector3 StandardLerp(Vector3 start, Vector3 dest, float percent)
        {
            return Vector3.Lerp(start, dest, percent);
        }
    }

    public class AnimateCustomInformation : AnimationInformation
    {
        public AnimateCustomInformation(Actor3D actor,bool isRelative, Process process, Vector3 destination, int maxTime,
            Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) :
            base(actor,isRelative, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            this.process = process;
        }
        
    }

    public class MovementInformation : AnimationInformation
    {

        public MovementInformation(Actor3D actor, Vector3 destination, int maxTime, bool isRelative,
            Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) :
            base(actor,isRelative, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            this.isRelative = isRelative;
            start = this.actor.Transform3D.Translation;
            process = ApplyAnimation;
        }

        private void ApplyAnimation(FinalOperation finalOperation)
        {
            actor.Transform3D.Translation = finalOperation.Invoke(actor.Transform3D.Translation);
            body?.MoveTo(actor.Transform3D.Translation, Matrix.Identity);
        }
    }

    public class ScaleInformation : AnimationInformation
    {
        public ScaleInformation(Actor3D actor,bool isRelative, Vector3 destination, int maxTime,
            Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) :
            base(actor,isRelative, destination, maxTime, smoothingMethod, loopMethod, body)
        {
            start = this.actor.Transform3D.Scale;
            process = ApplyAnimation;
        }

        protected void ApplyAnimation(FinalOperation finalOperation)
        {
            actor.Transform3D.Scale = finalOperation.Invoke(actor.Transform3D.Scale);
        }
    }

    public class RotationInformation : AnimationInformation
    {
        
        public RotationInformation(Actor3D actor,bool isRelative, Vector3 destination, int maxTime,
            Smoother.SmoothingMethod smoothingMethod, LoopMethod loopMethod = LoopMethod.PlayOnce, Body body = null) :
            base(actor, isRelative,destination, maxTime, smoothingMethod, loopMethod, body)
        {
            start = actor.Transform3D.RotationInDegrees;
            process = ApplyAnimation;
        }

        protected void ApplyAnimation(FinalOperation finalOperation)
        {
            actor.Transform3D.RotationInDegrees = finalOperation.Invoke(actor.Transform3D.RotationInDegrees);
        }
    }

    public class TransformAnimationManager : PausableGameComponent
    {
        private List<AnimationInformation> movementInformationList = new List<AnimationInformation>();

        public TransformAnimationManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game,
            statusType)
        {
            EventManager.RegisterListener<MovementInformation>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<ScaleInformation>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<RotationInformation>(HandleAnimationInformationEvent);
        }

        private void HandleAnimationInformationEvent(AnimationInformation information)
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