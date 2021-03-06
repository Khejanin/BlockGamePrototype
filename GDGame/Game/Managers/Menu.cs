﻿using GDGame.Constants;
using GDGame.Game.Handlers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    public class Menu
    {
        #region Private variables

        private Main main;

        #endregion

        #region Constructors

        public Menu(Main main)
        {
            this.main = main;
        }

        #endregion

        #region Initialization

        private void InitEndUi()
        {
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = main.Textures["EndScreen"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("LoseScreen", uiTextureObject);
            }

            if (((UITextObject) main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "You have been dissolved";
                uiTextObject.ID = "LoseText";
                uiTextObject.Text = text;
                uiTextObject.Color = GameConstants.colorGold;
                uiTextObject.Transform2D.Origin = new Vector2(main.Fonts["Arial"].MeasureString(text).X / 2,
                    main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = main.ScreenCentre - Vector2.UnitY * 250;
                main.MenuManager.Add("LoseScreen", uiTextObject);
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture2D = main.Textures["Sad"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre - Vector2.UnitX * 250;
                main.MenuManager.Add("LoseScreen", uiTextureObject);
            }

            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                string text = "Continue";
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("LoseScreen", uiButtonObject);
            }
        }

        private void InitEndWinUi()
        {
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = main.Textures["EndScreen"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("WinScreen", uiTextureObject);
            }

            if (((UITextObject) main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "A winner is you :)";
                uiTextObject.ID = "WinText";
                uiTextObject.Text = text;
                uiTextObject.Color = GameConstants.colorGold;
                uiTextObject.Transform2D.Origin = new Vector2(main.Fonts["Arial"].MeasureString(text).X / 2,
                    main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = main.ScreenCentre - Vector2.UnitY * 250;
                main.MenuManager.Add("WinScreen", uiTextObject);
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture2D = main.Textures["Happy"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre - Vector2.UnitX * 250;
                main.MenuManager.Add("WinScreen", uiTextureObject);
            }

            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                string text = "Continue";
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("WinScreen", uiButtonObject);
            }
        }

        private void InitGameOptionsUi()
        {
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = main.Textures["Options"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("GameOptions", uiTextureObject);
            }

            //Resume Button
            string text = "Resume";
            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;

                Texture2D texture2D = main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * 125 + Vector2.UnitX * 100, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("GameOptions", uiButtonObject);
            }

            text = "Controls";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = "GameControls";

                Texture2D texture2D = main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * 300 + Vector2.UnitX * 400, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("GameOptions", uiButtonObject);
            }

            text = "Quit";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = "QuitInstance";

                Texture2D texture2D = main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * -150 + Vector2.UnitX * 400, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("GameOptions", uiButtonObject);
            }

            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
                VolumeButtons("GameOptions");
        }

        private void InitInfoUi()
        {
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = main.Textures["Tutorial"];
                uiTextureObject.ID = "TutorialTexture";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("Info", uiTextureObject);
            }

            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                string text = "Back";
                uiButtonObject.ID = "BackToOptions";
                uiButtonObject.Text = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre + Vector2.UnitY * 400;
                main.MenuManager.Add("Info", uiButtonObject);
            }
        }

        private void InitMenuUi()
        {
            //Background
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = main.Textures["Menu"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("MainMenu", uiTextureObject);
            }

            Vector2 xPosition = Vector2.UnitX * 500;

            //Play Button
            string text = "Play";
            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre + xPosition - Vector2.UnitY * 120;
                main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            //Options Button
            text = "Options";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre + xPosition;
                main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            //Quit Button
            text = "Quit";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = main.ScreenCentre + xPosition + Vector2.UnitY * 120;
                main.MenuManager.Add("MainMenu", uiButtonObject);
            }
        }

        private void InitOptionsUi()
        {
            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture2D = main.Textures["Options"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.MenuManager.Add("Options", uiTextureObject);
            }

            string text = "Controls";
            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;

                Texture2D texture2D = main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * 300 + Vector2.UnitX * 400, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("Options", uiButtonObject);
            }

            //Back Button
            text = "Difficulty";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = "Easy";
                uiButtonObject.ID = text;

                Texture2D texture2D = main.Textures["GreenSticker"];
                uiButtonObject.Texture = texture2D;
                Vector2 position = main.ScreenCentre + Vector2.UnitX * 350;
                uiButtonObject.Transform2D = new Transform2D(
                    position, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("Options", uiButtonObject);
                uiButtonObject.EventHandlerList.Add(new UiOptionsEvent(EventCategoryType.Menu, uiButtonObject,
                    texture2D, main.Textures["RedSticker"]));
            }

            //Back Button
            text = "Back";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;

                Texture2D texture2D = main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre + Vector2.UnitY * 125 + Vector2.UnitX * 100, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add("Options", uiButtonObject);
            }

            VolumeButtons("Options");
        }

        public void InitUi()
        {
            InitMenuUi();
            InitOptionsUi();
            InitEndUi();
            InitEndWinUi();
            InitInfoUi();
            InitGameOptionsUi();
            main.MenuManager.SetScene("MainMenu");
        }

        #endregion

        #region Private Method

        private void VolumeButtons(string menu)
        {
            string text;
            if (((UITextObject) main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                text = "Volume";
                uiTextObject.ID = "LoseText";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.Black;
                uiTextObject.Transform2D.Origin = new Vector2(main.Fonts["Arial"].MeasureString(text).X / 2,
                    main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = main.ScreenCentre - Vector2.UnitY * -280 + Vector2.UnitX * 500;
                main.MenuManager.Add(menu, uiTextObject);
            }

            text = "-";
            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = "VolumeDown";

                Texture2D texture2D = main.Textures["RedSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * -400 + Vector2.UnitX * 400, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add(menu, uiButtonObject);
            }

            text = "+";
            uiButtonObject = ((UIButtonObject) main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = "VolumeUp";

                Texture2D texture2D = main.Textures["GreenSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    main.ScreenCentre - Vector2.UnitY * -400 + Vector2.UnitX * 600, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiButtonObject.TextColor = Color.Black;
                main.MenuManager.Add(menu, uiButtonObject);
            }
        }

        #endregion
    }
}