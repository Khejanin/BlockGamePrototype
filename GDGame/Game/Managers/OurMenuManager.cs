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
    public class OurMenuManager : MenuManager
    {
        #region Private variables

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;

        #endregion

        #region Constructors

        public OurMenuManager(Microsoft.Xna.Framework.Game main, StatusType statusType, SpriteBatch spriteBatch,
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

        #region Override Method

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
            if ((StatusType & StatusType.Update) != 0)
            {
                HandleMouse(gameTime);
                HandleKeyboard(gameTime);
            }
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.M))
            {
                SetScene("GameOptions");
                EventDispatcher.Publish(StatusType == StatusType.Off
                    ? new EventData(EventCategoryType.Menu, EventActionType.OnPause, null)
                    : new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
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

            //play button sound 
            EventManager.FireEvent(new SoundEventInfo
            {
                soundEventType = SoundEventType.PlaySfx,
                sfxType = SfxType.MenuButtonClick
            });

            switch (uIButtonObject.ID)
            {
                case "Play":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Start});
                    break;

                case "Options":
                    SetScene("Options");
                    break;

                case "GameOptions":
                    SetScene("GameOptions");
                    break;

                case "Resume":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Resume});
                    break;

                case "Controls":
                    SetScene("Info");
                    break;

                case "GameControls":
                    SetScene("GameInfo");
                    break;

                case "BackToOptions":
                    SetScene("Options");
                    break;

                case "BackToGameOptions":
                    SetScene("GameOptions");
                    break;

                case "Back":
                    SetScene("MainMenu");
                    break;

                case "Continue":
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