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
    /// <summary>
    /// Struct that stores everything the Animations need, the constructors got too overloaded so we resorted to making a big struct.
    /// </summary>
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

    /// <summary>
    /// Class that executes Animations on the Transforms of Actor3Ds. Transformer.cs in utilities is the one sending the Events.
    /// Used with our Type based EventManager.
    /// </summary>
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

        /// <summary>
        /// Makes all necessary calculations to figure out where the current point at this step of the animation is.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
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
                    return list => { list.RemoveAll(list => Equals(list.actor, this.actor)); };
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

        //Each Subclass will define how it processes (on what part of Transform3D it processes) the current Animation point.
        public delegate void Process(FinalOperation finalOperation);

        //The FinalOperation will be given to the Subclass so it can apply it to its current Scale/Transform/Anything and gets back the modified version.
        public delegate Vector3 FinalOperation(Vector3 target);

        //This method figures out based on whether the animation is relative and the loopmethod how the next animation point is supposed to be used and the result is given to the subclass.
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
    }

    /// <summary>
    /// Class that is supposed to have multiple actors subscribe to an animation so the calculations are only done once.
    /// It is not as performance increasing as anticipated and has a lot of flaws for the setup.
    /// </summary>
    public class GroupAnimationEvent : AnimationEvent
    {
        public string id;
        private List<Actor3D> actor3Ds;
        private List<Action> callbacks;

        private List<AnimationEvent> animationEvents;

        public GroupAnimationEvent(AnimationEventData data, List<AnimationEvent> animEvents, string id) : base(data)
        {
            animationEvents = animEvents;
            this.id = id;
            actor3Ds = new List<Actor3D>();
            callbacks = new List<Action>();
            actor3Ds.Add(data.actor);
            callbacks.Add(data.callback);
        }

        public void SubscribeActorToAnimation(GroupAnimationEvent info)
        {
            actor3Ds.Add(info.actor);
            callbacks.Add(info.callback);
        }

        public bool Tick(GameTime gameTime)
        {
            Vector3 offsetTranslation = actor3Ds[0].Transform3D.Translation;
            Vector3 offsetScale = actor3Ds[0].Transform3D.Scale;
            Vector3 offsetRotation = actor3Ds[0].Transform3D.RotationInDegrees;
            for (int i = 0; i < animationEvents.Count; i++)
            {
                if (animationEvents[i].Tick(gameTime)) animationEvents.RemoveAt(i--);
            }

            offsetTranslation = actor3Ds[0].Transform3D.Translation - offsetTranslation;
            offsetScale = actor3Ds[0].Transform3D.Scale - offsetScale;
            offsetRotation = actor3Ds[0].Transform3D.RotationInDegrees - offsetRotation;

            for (int i = 1; i < actor3Ds.Count; i++)
            {
                actor3Ds[i].Transform3D.Translation += offsetTranslation;
                actor3Ds[i].Transform3D.Scale += offsetScale;
                actor3Ds[i].Transform3D.RotationInDegrees += offsetRotation;
            }

            if (animationEvents.Count == 0)
            {
                foreach (Action action in callbacks)
                {
                    action.Invoke();
                }

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Unused class that doesn't work but would be desirable someday later.
    /// </summary>
    public class AnimateCustomEvent : AnimationEvent
    {
        public AnimateCustomEvent(AnimationEventData animationEventData) : base(animationEventData)
        {
            this.process = process;
        }
    }

    /// <summary>
    /// Class that Handles Translation Animations
    /// </summary>
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
    
    /// <summary>
    /// Class that Handles Scale Animations
    /// </summary>
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
    
    /// <summary>
    /// Class that Handles Rotation Animations
    /// </summary>
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

    /// <summary>
    /// Manager that keeps track of all Animations playing and removes them from his list when they're done playing.
    /// </summary>
    public class TransformAnimationManager : PausableGameComponent
    {
        private List<AnimationEvent> movementInformationList = new List<AnimationEvent>();

        private Dictionary<string, GroupAnimationEvent> groupAnimationEvents =
            new Dictionary<string, GroupAnimationEvent>();

        public TransformAnimationManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game,
            statusType)
        {
            EventManager.RegisterListener<MovementEvent>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<ScaleEvent>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<RotationEvent>(HandleAnimationInformationEvent);
            EventManager.RegisterListener<GroupAnimationEvent>(HandleGroupAnimationInformationEvent);
        }

        private void HandleAnimationInformationEvent(AnimationEvent @event)
        {
            @event.PerformListOperation().Invoke(movementInformationList);
        }

        private void HandleGroupAnimationInformationEvent(GroupAnimationEvent groupAnimationEvent)
        {
            if (!groupAnimationEvents.ContainsKey(groupAnimationEvent.id))
                groupAnimationEvents.Add(groupAnimationEvent.id, groupAnimationEvent);
            else
                groupAnimationEvents[groupAnimationEvent.id].SubscribeActorToAnimation(groupAnimationEvent);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            for (int i = 0; i < movementInformationList.Count; i++)
                if (movementInformationList[i].Tick(gameTime))
                    movementInformationList.RemoveAt(i--);

            foreach (KeyValuePair<string, GroupAnimationEvent> groupAnimationEvent in groupAnimationEvents)
                if (groupAnimationEvent.Value.Tick(gameTime))
                    groupAnimationEvents.Remove(groupAnimationEvent.Key);
        }
    }
}