﻿using GDGame.Actors;
using GDGame.Constants;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Managers
{
    public class UiSceneManager
    {
        #region Private variables

        private bool optionsShown = true;

        private PlayerTile playerTile;

        #endregion

        #region Constructors

        public UiSceneManager(Main main)
        {
            InitEventListeners();
            Main = main;
        }

        private Main Main { get; }

        #endregion

        #region Properties, Indexers

        #endregion

        #region Initialization

        private void InitEndUi()
        {
            
            if (((UITextureObject) Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = Main.Textures["EndScreen"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                Main.MenuManager.Add("LoseScreen", uiTextureObject);
            }

            if (((UITextObject) Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "You have been dissolved";
                uiTextObject.ID = "LoseText";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.Yellow;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitY * 250;
                Main.MenuManager.Add("LoseScreen", uiTextObject);
            }
            uiTextureObject = ((UITextureObject)Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture2D = Main.Textures["Sad"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitX * 250;
                Main.MenuManager.Add("LoseScreen", uiTextureObject);
            }
        }

        private void InitEventListeners()
        {
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
            EventManager.RegisterListener<OptionsEventInfo>(HandleOptionsEvent);
        }

        private void InitGameUi()
        {
            float screenHeight = Main.GraphicsDevice.Viewport.Height;
            float screenWidth = Main.GraphicsDevice.Viewport.Width;

            if (((UITextureObject) Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = Main.Textures["TopBar"];
                uiTextureObject.ID = "TopBar";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                Main.MenuManager.Add("Game", uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = Main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug1";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(332, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                Main.MenuManager.Add("Game", uiTextureObject);
            }


            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = Main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug2";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(508, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                Main.MenuManager.Add("Game", uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = Main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug3";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(332 + (508 - 332) * 2, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                Main.MenuManager.Add("Game", uiTextureObject);
            }


            if (((UITextObject) Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Moves: 0";
                uiTextObject.ID = "MoveText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);

                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 75;
                Vector2 xPosition = -Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = Main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                Main.MenuManager.Add("Game", uiTextObject);
            }

            uiTextObject = ((UITextObject) Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Time : 00:00:00";
                uiTextObject.ID = "TimeText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);
                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 60;
                Vector2 xPosition = Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = Main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                Main.MenuManager.Add("Game", uiTextObject);
            }

            uiTextObject = ((UITextObject) Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Hold Space To Attach";
                uiTextObject.ID = "AttachToolTipText";
                uiTextObject.Text = text;
                uiTextObject.StatusType = StatusType.Off;
                Main.MenuManager.Add("Game", uiTextObject);
            }

            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture = Main.Textures["options"];
                uiTextureObject.ID = "PauseBackground";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                uiTextureObject.StatusType = StatusType.Off;
                Main.MenuManager.Add("Game", uiTextureObject);
            }

            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.ID = "ResumeButton";
                uiButtonObject.Text = "Resume";
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre;
                uiButtonObject.StatusType = StatusType.Off;
                Main.MenuManager.Add("Game", uiButtonObject);
            }
        }

        private void InitInfoUi()
        {
            if (((UITextureObject) Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = Main.Textures["Tutorial"];
                uiTextureObject.ID = "TutorialTexture";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                Main.UiManager.Add(uiTextureObject);
            }

            
            if (((UITextObject) Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Press SPACEBAR to continue!";
                uiTextObject.ID = "ContinueText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Main.ScreenCentre + Vector2.UnitY * 300;
                Main.UiManager.Add(uiTextObject);
            }
        }

        private void InitMenuUi()
        {
            //Background
            if (((UITextureObject) Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = Main.Textures["Menu"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                Main.MenuManager.Add("MainMenu", uiTextureObject);
            }


            Vector2 xPosition = Vector2.UnitX * 500;

            //Play Button
            string text = "Play";
            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + xPosition - Vector2.UnitY * 120;
                Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            //Options Button
            text = "Options";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + xPosition;
                Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            //Quit Button
            text = "Quit";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + xPosition + Vector2.UnitY * 120;
                Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            //Game Name
            if (((UITextObject) Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                text = "B_Logic";
                uiTextObject.ID = "B_Logic";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.SaddleBrown;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitX * 525 - Vector2.UnitY * 50;
                Main.MenuManager.Add("MainMenu", uiTextObject);
            }

            uiTextObject = ((UITextObject) Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                text = "Caffeine Edition!";
                uiTextObject.ID = "Caffeine Edition!";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.SaddleBrown;
                uiTextObject.Transform2D.Origin = new Vector2(Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitX * 525;
                Main.MenuManager.Add("MainMenu", uiTextObject);
            }
        }

        private void InitOptionsUi()
        {
            if (((UITextureObject) Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = Main.Textures["Options"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre;
                Main.MenuManager.Add("Options", uiTextureObject);
            }

            //Resume Button
            string text = "Resume";
            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitY * 125;
                Main.MenuManager.Add("Options", uiButtonObject);
            }

            //Back Button
            text = "Difficulty";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = "Easy";
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + Vector2.UnitY * 0;
                Main.MenuManager.Add("Options", uiButtonObject);
            }

            //Back Button
            text = "Back";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + Vector2.UnitY * 125;
                Main.MenuManager.Add("Options", uiButtonObject);
            }
        }

        public void InitUi()
        {
            InitMenuUi();
            InitOptionsUi();
            InitEndUi();
            InitGameUi();
            Main.MenuManager.SetScene("MainMenu");
        }

        #endregion

        #region Methods

        private void AnimateButton(UIButtonObject uiButtonObject, float f)
        {
            uiButtonObject.Transform2D.Scale = Vector2.One * f;
            uiButtonObject.TextOffset = new Vector2(
                uiButtonObject.Transform2D.Bounds.Width / 2f -
                Main.Fonts["Arial"].MeasureString(uiButtonObject.Text).X / 2,
                uiButtonObject.Transform2D.Bounds.Height / 2f -
                Main.Fonts["Arial"].MeasureString(uiButtonObject.Text).Y / 2);
        }

        private void ToggleOptionsMenu()
        {
            optionsShown = !optionsShown;
            UITextureObject uiTextureObject =
                Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "PauseBackground") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
                uiTextureObject.StatusType = StatusType.Drawn;
            else if (uiTextureObject != null) uiTextureObject.StatusType = StatusType.Off;

            uiTextureObject =
                Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "ResumeButton") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
            {
                uiTextureObject.StatusType = StatusType.Drawn | StatusType.Update;
                Main.MouseManager.MouseVisible = optionsShown;
            }
            else if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Main.MouseManager.MouseVisible = optionsShown;
            }
        }

        private void UpdateGameUi(GameTime gameTime)
        {
            if (Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText") is UITextObject uiTextObject
            )
            {
                uiTextObject.Text = "Time: " + gameTime.TotalGameTime.Hours % 24 + ":" +
                                    gameTime.TotalGameTime.Minutes % 60 + ":" + gameTime.TotalGameTime.Seconds % 60;
                uiTextObject.Transform2D.Origin = new Vector2(
                    Main.Fonts["Arial"].MeasureString(uiTextObject.Text).X / 2,
                    Main.Fonts["Arial"].MeasureString(uiTextObject.Text).Y / 2);
            }

            uiTextObject =
                Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "AttachToolTipText") as UITextObject;
            playerTile ??=
                Main.ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player") as PlayerTile;
            if (uiTextObject != null && playerTile != null && playerTile.AttachCandidates.Count > 0)
                uiTextObject.StatusType = StatusType.Drawn;
            else if (uiTextObject != null) uiTextObject.StatusType = StatusType.Off;

            if (Main.KeyboardManager.IsFirstKeyPress(Keys.O))
                ToggleOptionsMenu();
        }

        private void UpdateMenuUi()
        {
            foreach (DrawnActor2D drawnActor2D in Main.UiManager.UIObjectList)
                if (drawnActor2D is UIButtonObject uiButtonObject)
                {
                    if (uiButtonObject.Transform2D.Bounds.Contains(Main.MouseManager.Bounds))
                    {
                        AnimateButton(uiButtonObject, 1.1f);

                        if (Main.MouseManager.IsLeftButtonClickedOnce())
                            switch (uiButtonObject.ID)
                            {
                                case "Play":
                                    break;
                                case "Options":
                                    break;
                                case "Quit":
                                    Main.Exit();
                                    break;
                            }
                    }
                    else
                    {
                        AnimateButton(uiButtonObject, 1);
                    }
                }
        }

        #endregion

        #region Events

        private void HandleDataManagerEvent(DataManagerEvent obj)
        {
            if (Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "NumberOfMovesText") is UITextObject
                uiTextObject)
                uiTextObject.Text = Main.LevelDataManager.CurrentMovesCount.ToString();
        }

        private void HandleOptionsEvent(OptionsEventInfo optionsEventInfo)
        {
            switch (optionsEventInfo.Type)
            {
                case OptionsType.Toggle:
                    if (Main.MenuManager.DrawnActor2D.Find(actor2D => actor2D.ID == optionsEventInfo.Id) is
                        UIButtonObject options) options.Text = options.Text.Equals("Easy") ? "Difficult" : "Easy";
                    break;
            }
        }

        #endregion
    }
}