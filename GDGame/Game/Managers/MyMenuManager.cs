using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Managers
{
    public class MyMenuManager : MenuManager
    {
        #region Private variables

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;

        #endregion

        #region Constructors

        public MyMenuManager(Game main, StatusType statusType, SpriteBatch spriteBatch, MouseManager mouseManager, KeyboardManager keyboardManager) : base(main, statusType,
            spriteBatch)
        {
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
        }

        #endregion

        #region Override Methode

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
                StatusType = eventData.EventActionType switch
                {
                    EventActionType.OnPause => StatusType.Drawn | StatusType.Update,
                    EventActionType.OnPlay => StatusType.Off,
                    _ => StatusType
                };
        }

        protected override void HandleInput(GameTime gameTime)
        {
            HandleMouse(gameTime);
            HandleKeyboard(gameTime);
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.M))
                EventDispatcher.Publish(StatusType == StatusType.Off
                    ? new EventData(EventCategoryType.Menu, EventActionType.OnPause, null)
                    : new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));

            base.HandleKeyboard(gameTime);
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            foreach (DrawnActor2D actor in ActiveList)
                if (actor is UIButtonObject buttonObject)
                    if (buttonObject.Transform2D.Bounds.Contains(mouseManager.Bounds))
                        if (mouseManager.IsLeftButtonClickedOnce())
                            HandleClickedButton(gameTime, buttonObject);

            base.HandleMouse(gameTime);
        }

        #endregion

        #region Events

        private void HandleClickedButton(GameTime gameTime, Actor uIButtonObject)
        {
            switch (uIButtonObject.ID)
            {
                case "Play":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    break;

                case "Options":
                    SetScene("Options");
                    break;

                case "Resume":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    break;

                case "Back":
                    SetScene("MainMenu");
                    break;

                case "Quit":
                    Game.Exit();
                    break;
            }
        }

        #endregion
    }
}