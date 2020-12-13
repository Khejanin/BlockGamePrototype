using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
{
    /// <summary>
    ///     Our EventSystem version that's not too different, it trades safety for convenience.
    /// </summary>
    public class EventManager : GameComponent
    {
        #region Delegates

        private delegate void EventListener(EventInfo ei);

        #endregion

        #region Static Fields and Constants

        //The first dictionary is to get the Events based on their Type, the second one based on the hash of the original function.
        //The hash is needed because we wrap the function before we add it, so we would not be able to unsubscribe without this.
        private static Dictionary<Type, Dictionary<int, EventListener>> _eventListeners;
        private static Queue<EventInfo> _eventsToTrigger;

        #endregion

        #region Constructors

        public EventManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            _eventListeners = new Dictionary<Type, Dictionary<int, EventListener>>();
            _eventsToTrigger = new Queue<EventInfo>();
        }

        #endregion

        #region Override Method

        public override void Update(GameTime gameTime)
        {
            while (_eventsToTrigger.Count != 0) ProcessEvent(_eventsToTrigger.Dequeue());

            base.Update(gameTime);
        }

        #endregion

        #region Public Method

        public static void FireEvent(EventInfo eventInfo)
        {
            if (!_eventsToTrigger.Contains(eventInfo)) _eventsToTrigger.Enqueue(eventInfo);
        }

        public static void RegisterListener<T>(Action<T> listener) where T : EventInfo
        {
            Type eventType = typeof(T);

            if (!_eventListeners.ContainsKey(eventType) || _eventListeners[eventType] == null)
                _eventListeners[eventType] = new Dictionary<int, EventListener>();

            _eventListeners[eventType][listener.GetHashCode()] = ei => { listener((T) ei); };
        }

        public static void UnregisterListener<T>(Action<T> listener) where T : EventInfo
        {
            Type eventType = typeof(T);

            if (_eventListeners != null)
                if (_eventListeners.ContainsKey(eventType) && _eventListeners[eventType] != null)
                    _eventListeners[eventType].Remove(listener.GetHashCode());
        }

        #endregion

        #region Private Method

        private void ProcessEvent(EventInfo eventInfo)
        {
            Type trueEventInfoClass = eventInfo.GetType();
            if (_eventListeners == null || !_eventListeners.ContainsKey(trueEventInfoClass))
                // No one is listening, we are done.
                return;

            foreach (EventListener el in _eventListeners[trueEventInfoClass].Values) el(eventInfo);
        }

        #endregion
    }
}