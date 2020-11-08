using System;
using System.Collections.Generic;

namespace GDGame.Game.EventSystem.Base
{
    public static class EventSystem
    {
        private static Dictionary<Type, Dictionary<int, EventListener>> _eventListeners;

        public static void RegisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);
            _eventListeners ??= new Dictionary<Type, Dictionary<int, EventListener>>();

            if (!_eventListeners.ContainsKey(eventType) || _eventListeners[eventType] == null)
                _eventListeners[eventType] = new Dictionary<int, EventListener>();

            _eventListeners[eventType][listener.GetHashCode()] = ei => { listener((T) ei); };
        }

        public static void UnregisterListener<T>(Action<T> listener) where T : EventInfo
        {
            var eventType = typeof(T);

            if (_eventListeners != null)
                if (_eventListeners.ContainsKey(eventType) && _eventListeners[eventType] != null)
                    _eventListeners[eventType].Remove(listener.GetHashCode());
        }

        public static void FireEvent(EventInfo eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();
            if (_eventListeners == null || !_eventListeners.ContainsKey(trueEventInfoClass))
                // No one is listening, we are done.
                return;

            foreach (var el in _eventListeners[trueEventInfoClass].Values) el(eventInfo);
        }

        private delegate void EventListener(EventInfo ei);
    }
}