using System;
using System.Collections.Generic;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    /// <summary>
    ///     When an object wants to Call a Function in x seconds it can use this class, it pauses with the game.
    ///     Initially only the TransformAnimationManager provided timed callbacks, as I don't want anyone to use the
    ///     TransformAnimationManager for just the Callback functionality, this class exists.
    /// </summary>
    public class TimeManager : PausableGameComponent
    {
        #region Static Fields and Constants

        private static Dictionary<string, Timer> _currentTimers = new Dictionary<string, Timer>();
        private static Dictionary<string, Timer> _timersToAdd = new Dictionary<string, Timer>();
        private static List<string> _timersToRemove = new List<string>();

        #endregion

        #region Constructors

        public TimeManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
        }

        #endregion

        #region Override Method

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
                foreach (KeyValuePair<string, Timer> timer in _timersToAdd)
                    _currentTimers.Add(timer.Key, timer.Value);

                _timersToAdd.Clear();
            }

            foreach (KeyValuePair<string, Timer> pair in _currentTimers)
                if (pair.Value.Tick(gameTime))
                    _timersToRemove.Add(pair.Key);

            base.ApplyUpdate(gameTime);
        }

        #endregion

        #region Public Method

        //Static function everyone can call to add a callback in x seconds.
        public static void CallFunctionInSeconds(string referenceId, Action action, float seconds)
        {
            if (!_currentTimers.ContainsKey(referenceId))
                _timersToAdd.Add(referenceId, new Timer(action, seconds));
        }

        //Pause a timer by its reference id
        public static void PauseTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _currentTimers[referenceId].pause = true;
        }

        //Remove a timer by its reference id
        public static void RemoveTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _timersToRemove.Add(referenceId);
        }

        //Resume a timer by it reference id
        public static void ResumeTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _currentTimers[referenceId].pause = false;
        }

        #endregion

        #region Nested Types

        /// <summary>
        ///     Timer struct for data and ticking.
        /// </summary>
        private class Timer
        {
            #region Public variables

            public bool pause;

            #endregion

            #region Private variables

            private float currentSeconds;
            private float delayInSeconds;
            private Action toCall;

            #endregion

            #region Constructors

            public Timer(Action toCall, float delayInSeconds)
            {
                this.toCall = toCall;
                this.delayInSeconds = delayInSeconds;
                currentSeconds = 0;
            }

            #endregion

            #region Public Method

            public bool Tick(GameTime gameTime)
            {
                if (pause) return false;

                currentSeconds += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (currentSeconds >= delayInSeconds)
                {
                    toCall.Invoke();
                    return true;
                }

                return false;
            }

            #endregion
        }

        #endregion
    }
}