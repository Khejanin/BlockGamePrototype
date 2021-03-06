﻿using System.Collections.Generic;
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

        private bool gameRunning;

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
            if (gameRunning)
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
            //bool if game started? 
            if (gameRunning)
            {
                // otherwise goes main menu on first trigger, then ingame menu on second press
                if (keyboardManager.IsKeyDown(Keys.Escape))
                    SetScene("GameOptions");

                if (keyboardManager.IsFirstKeyPress(Keys.Escape))
                {
                    EventManager.FireEvent(new SoundEventInfo
                    {
                        soundEventType = SoundEventType.PlaySfx,
                        sfxType = SfxType.MenuButtonClick,
                        category = SoundCategory.UI
                    });

                    EventDispatcher.Publish(StatusType == StatusType.Off
                        ? new EventData(EventCategoryType.Menu, EventActionType.OnPause, null)
                        : new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                }
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
                sfxType = SfxType.MenuButtonClick,
                category = SoundCategory.UI
            });

            switch (uIButtonObject.ID)
            {
                case "Play":
                    gameRunning = true;
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Start});
                    EventDispatcher.Publish(new EventData(EventCategoryType.UI, EventActionType.OnPause, null));
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

                case "QuitInstance":
                    //needs to kill game
                    gameRunning = false;
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    SetScene("MainMenu");
                    break;

                case "Back":
                    gameRunning = false;
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

                case "VolumeUp":
                    EventManager.FireEvent(new SoundEventInfo
                    {
                        soundEventType = SoundEventType.IncreaseVolume
                    });
                    break;

                case "VolumeDown":
                    EventManager.FireEvent(new SoundEventInfo
                    {
                        soundEventType = SoundEventType.DecreaseVolume
                    });
                    break;
            }
        }

        private bool HandleRequestIfGameRun()
        {
            return gameRunning;
        }

        #endregion
    }
}