using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
{
    public class EventManager : GameComponent
    {
        private static Dictionary<Type, Dictionary<int, EventListener>> EventListeners;
        private static Queue<EventInfo> EventsToTrigger;
        
        public EventManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            EventListeners = new Dictionary<Type, Dictionary<int, EventListener>>();
            EventsToTrigger = new Queue<EventInfo>();
        }
        
        public static void RegisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);

            if (!EventListeners.ContainsKey(eventType) || EventListeners[eventType] == null)
                EventListeners[eventType] = new Dictionary<int, EventListener>();

            EventListeners[eventType][listener.GetHashCode()] = ei => { listener((T) ei); };
        }

        public static void UnregisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);

            if (EventListeners != null)
                if (EventListeners.ContainsKey(eventType) && EventListeners[eventType] != null)
                    EventListeners[eventType].Remove(listener.GetHashCode());
        }

        public static void FireEvent(EventInfo eventInfo)
        {
            EventsToTrigger.Enqueue(eventInfo);
        }

        public override void Update(GameTime gameTime)
        {
            while (EventsToTrigger.Count != 0)
            {
                ProcessEvent(EventsToTrigger.Dequeue());
            }
            
            base.Update(gameTime);
        }

        private void ProcessEvent(EventInfo eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();
            if (EventListeners == null || !EventListeners.ContainsKey(trueEventInfoClass))
                // No one is listening, we are done.
                return;

            foreach (var el in EventListeners[trueEventInfoClass].Values) el(eventInfo);
        }

        private delegate void EventListener(EventInfo ei);
    }
}