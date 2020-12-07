using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{

    public struct AnimationEventData
    {
        public AnimationEventType type;
        public Actor3D actor;
        public int maxTime;
        public Vector3 destination;
        public LoopMethod loopMethod;
        public Smoother.SmoothingMethod smoothing;
        public bool isRelative;
        public Body body;
        public Action callback;
        public bool resetAferDone;
    }

    public enum AnimationEventType
    {
        Add,
        RemoveAllOfActor,
        RemoveAllOfActorAndInvokeCallbacks
    }
    
    public abstract class AnimationEvent : EventInfo
    {
        private AnimationEventType type;
        private int currentTime;
        private int maxTime;
        private int step = 1;
        private Vector3 lastPoint = Vector3.Zero;
        private Vector3 destination;
        private LoopMethod loopMethod;
        private Smoother.SmoothingMethod smoothing;
        
        protected Actor3D actor;
        protected Vector3 start;
        protected Process process;
        protected bool isRelative;
        protected Body body;
        protected Action callback;
        protected bool resetAfterDone;

        protected AnimationEvent(AnimationEventData animationEventData)
        {
            this.actor = animationEventData.actor;
            this.destination = animationEventData.destination;
            this.maxTime = animationEventData.maxTime;
            this.smoothing = animationEventData.smoothing;
            this.loopMethod = animationEventData.loopMethod;
            this.isRelative = animationEventData.isRelative;
            this.body = animationEventData.body;
            this.callback = animationEventData.callback;
            this.resetAfterDone = animationEventData.resetAferDone;
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

                if (done)
                {
                    if (resetAfterDone) 
                        process(target => start);
                    callback?.Invoke();
                }

                return done;
            }

            return false;
        }

        //We can manipulate/access values inside of the list without making them publicly accessible
        public Action<List<AnimationEvent>> PerformListOperation()
        {
            if (type == AnimationEventType.Add) return list => list.Add(this);
            if (actor != null)
            {
                if (type == AnimationEventType.RemoveAllOfActor)
                {
                    return list =>
                    {
                        list.RemoveAll(list => Equals(list.actor, this.actor));
                    };
                }

                if (type == AnimationEventType.RemoveAllOfActorAndInvokeCallbacks)
                {
                    return list =>
                    {
                        List<AnimationEvent> animationEvents = list.FindAll(list => Equals(list.actor, this.actor));
                        foreach (AnimationEvent animationEvent in animationEvents)
                        {
                            animationEvent.callback.Invoke();
                            list.Remove(animationEvent);
                        }
                    };
                }
            }
            
            return null;
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

    public class AnimateCustomEvent : AnimationEvent
    {
        public AnimateCustomEvent(AnimationEventData animationEventData) : base(animationEventData)
        {
            this.process = process;
        }
        
    }

    public class MovementEvent : AnimationEvent
    {

        public MovementEvent(AnimationEventData animationEventData) : base(animationEventData)
        {
            start = this.actor.Transform3D.Translation;
            process = ApplyAnimation;
        }

        private void ApplyAnimation(FinalOperation finalOperation)
        {
            Vector3 pos = finalOperation.Invoke(actor.Transform3D.Translation);
            if (body != null)
            {
                Tile tile = body.ExternalData as Tile;
                tile?.SetTranslation(pos);
            }
            else actor.Transform3D.Translation = pos;
        }
    }

    public class ScaleEvent : AnimationEvent
    {
        public ScaleEvent(AnimationEventData animationEventData) : base(animationEventData)
        {
            start = this.actor.Transform3D.Scale;
            process = ApplyAnimation;
        }

        protected void ApplyAnimation(FinalOperation finalOperation)
        {
            actor.Transform3D.Scale = finalOperation.Invoke(actor.Transform3D.Scale);
        }
    }

    public class RotationEvent : AnimationEvent
    {
        
        public RotationEvent(AnimationEventData animationEventData) : base(animationEventData)
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
        private List<AnimationEvent> movementInformationList = new List<AnimationEvent>();

        public TransformAnimationManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game,
            statusType)
        {
            EventManager.RegisterListener<MovementEvent>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<ScaleEvent>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<RotationEvent>(HandleAnimationInformationEvent);
        }

        private void HandleAnimationInformationEvent(AnimationEvent @event)
        {
            @event.PerformListOperation().Invoke(movementInformationList);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            for (int i = 0; i < movementInformationList.Count; i++)
                if (movementInformationList[i].Tick(gameTime))
                    movementInformationList.RemoveAt(i--);
        }
    }
}