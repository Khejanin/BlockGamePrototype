using System;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Handlers
{
    public class UiOptionsEvent : EventHandler, IDisposable
    {
        #region Private variables

        private Texture2D textureEasy, textureHard;
        private UIButtonObject uiButtonObject;

        #endregion

        #region Constructors

        public UiOptionsEvent(EventCategoryType eventCategoryType, IActor parent, Texture2D textureEasy,
            Texture2D textureHard) : base(eventCategoryType, parent)
        {
            uiButtonObject = parent as UIButtonObject;
            this.textureEasy = textureEasy;
            this.textureHard = textureHard;
            EventManager.RegisterListener<OptionsEventInfo>(HandleEvent);
        }

        #endregion

        #region Events

        private void HandleEvent(OptionsEventInfo optionsEventInfo)
        {
            switch (optionsEventInfo.Type)
            {
                case OptionsType.Toggle:
                    if (uiButtonObject != null)
                    {
                        uiButtonObject.Text = uiButtonObject.Text.Equals("Easy") ? "Hard" : "Easy";
                        uiButtonObject.Texture = uiButtonObject.Text.Equals("Easy") ? textureEasy : textureHard;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public override void Dispose()
        {
            textureEasy?.Dispose();
            textureHard?.Dispose();
            EventManager.UnregisterListener<OptionsEventInfo>(HandleEvent);
        }
    }
}