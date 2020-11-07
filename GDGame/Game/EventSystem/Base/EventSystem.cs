using System;
using System.Collections.Generic;

namespace GDGame.Game.EventSystem.Base
{
    public static class EventSystem
    {
        private static Dictionary<Type, Dictionary<int, EventListener>> eventListeners;

        public static void RegisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);
            if (eventListeners == null) eventListeners = new Dictionary<Type, Dictionary<int, EventListener>>();

            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null)
                eventListeners[eventType] = new Dictionary<int, EventListener>();

            eventListeners[eventType][listener.GetHashCode()] = ei => { listener((T) ei); };
        }

        public static void UnregisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);

            if (eventListeners != null)
                if (eventListeners.ContainsKey(eventType) && eventListeners[eventType] != null)
                    eventListeners[eventType].Remove(listener.GetHashCode());
        }

        public static void FireEvent(EventInfo eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();
            if (eventListeners == null || !eventListeners.ContainsKey(trueEventInfoClass))
                // No one is listening, we are done.
                return;

            foreach (var el in eventListeners[trueEventInfoClass].Values) el(eventInfo);
        }

        private delegate void EventListener(EventInfo ei);
    }
}