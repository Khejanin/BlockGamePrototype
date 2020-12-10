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

        private static List<Timer> _timers = new List<Timer>();

        //Static function everyone can call to add a callback in x seconds.
        public static void CallFunctionInSeconds(Action action, float seconds)
        {
            _timers.Add(new Timer(action,seconds));
        }
        
        public TimeManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
        }
        
        protected override void ApplyUpdate(GameTime gameTime)
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                if (_timers[i].Tick(gameTime)) _timers.RemoveAt(i--);
            }
            base.ApplyUpdate(gameTime);
        }
    }
}