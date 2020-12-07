using System.Collections.Generic;
using GDGame.Enums;
using GDGame.EventSystem;
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
        private bool isMenu = true;

        #endregion

        #region Constructors

        public MyMenuManager(Microsoft.Xna.Framework.Game main, StatusType statusType, SpriteBatch spriteBatch,
            MouseManager mouseManager, KeyboardManager keyboardManager) : base(
            main, statusType,
            spriteBatch)
        {
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
        }

        #endregion

        #region Properties, Indexers

        public List<DrawnActor2D> DrawnActor2D => ActiveList;

        #endregion

        #region Override Methode

        protected override void HandleInput(GameTime gameTime)
        {
            HandleMouse(gameTime);
            HandleKeyboard(gameTime);
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.M))
            {
                bool scene = isMenu ? SetScene("MainMenu") : SetScene("Game");
                isMenu = !isMenu;
            }
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            foreach (DrawnActor2D actor in ActiveList)
                if (actor is UIButtonObject buttonObject)
                    if (buttonObject.Transform2D.Bounds.Contains(mouseManager.Bounds))
                        if (mouseManager.IsLeftButtonClickedOnce())
                            HandleClickedButton(buttonObject);
        }

        #endregion

        #region Events

        private void HandleClickedButton(Actor uIButtonObject)
        {
            switch (uIButtonObject.ID)
            {
                case "Play":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Start});
                    break;

                case "Options":
                    SetScene("Options");
                    break;

                case "Resume":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Start});
                    break;

                case "Back":
                    SetScene("MainMenu");
                    break;

                case "Difficulty":
                    EventManager.FireEvent(new OptionsEventInfo {Type = OptionsType.Toggle, Id = "Difficulty"});
                    break;
                case "Quit":
                    Game.Exit();
                    break;
            }
        }

        #endregion
    }
}