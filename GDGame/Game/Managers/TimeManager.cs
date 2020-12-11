using System;
using System.Collections.Generic;
using System.Timers;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    /// <summary>
    /// When an object wants to Call a Function in x seconds it can use this class, it pauses with the game.
    /// Initially only the TransformAnimationManager provided timed callbacks, as I don't want anyone to use the TransformAnimationManager for just the Callback functionality, this class exists.
    /// </summary>
    public class TimeManager : PausableGameComponent
    {
        /// <summary>
        /// Timer struct for data and ticking.
        /// </summary>
        private class Timer
        {
            private Action toCall;
            private float delayInSeconds;
            private float currentSeconds;

            public Timer(Action toCall, float delayInSeconds)
            {
                this.toCall = toCall;
                this.delayInSeconds = delayInSeconds;
                currentSeconds = 0;
            }

            public bool Tick(GameTime gameTime)
            {
                currentSeconds += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (currentSeconds >= delayInSeconds)
                {
                    toCall.Invoke();
                    return true;
                }

                return false;
            }

        }

        private static Dictionary<string, Timer> _currentTimers = new Dictionary<string, Timer>();
        private static Dictionary<string, Timer> _timersToAdd = new Dictionary<string, Timer>();
        private static List<string> _timersToRemove = new List<string>();

        //Static function everyone can call to add a callback in x seconds.
        public static void CallFunctionInSeconds(string referenceId, Action action, float seconds)
        {
            if(!_currentTimers.ContainsKey(referenceId))
                _timersToAdd.Add(referenceId, new Timer(action, seconds));
        }

        //Remove a timer by its reference id
        public static void RemoveTimer(string referenceId)
        {
            if(_currentTimers.ContainsKey(referenceId))
                _timersToRemove.Add(referenceId);
        }
        
        public TimeManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
        }
        
        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (_timersToRemove.Count > 0)
            {
                foreach (string id in _timersToRemove)
                    _currentTimers.Remove(id);

                _timersToRemove.Clear();
            }

            if (_timersToAdd.Count > 0)
            {
                foreach (var timer in _timersToAdd)
                    _currentTimers.Add(timer.Key, timer.Value);

                _timersToAdd.Clear();
            }

            foreach (var pair in _currentTimers)
                if (pair.Value.Tick(gameTime))
                    _timersToRemove.Add(pair.Key);

            base.ApplyUpdate(gameTime);
        }
    }
}