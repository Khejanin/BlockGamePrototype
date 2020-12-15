using System;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;

namespace GDGame.Game.Handlers
{
    public class UiMovesEventHandler : EventHandler, IDisposable
    {
        #region Private variables

        private UITextObject uiTextObject;

        #endregion

        #region Constructors

        public UiMovesEventHandler(EventCategoryType eventCategoryType, IActor parent) : base(eventCategoryType, parent)
        {
            uiTextObject = parent as UITextObject;
            EventManager.RegisterListener<DataManagerEvent>(HandleEvent);
        }

        #endregion

        #region Events

        private void HandleEvent(DataManagerEvent dataManagerEvent)
        {
            if (uiTextObject != null) uiTextObject.Text = "Moves: " + dataManagerEvent.CurrentMovesCount;
        }

        #endregion

        public override void Dispose()
        {
            EventManager.UnregisterListener<DataManagerEvent>(HandleEvent);
        }
    }
}