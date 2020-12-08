using System;
using GDGame.Actors;
using GDGame.Constants;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    /// <summary>
    /// Class that sets up everything the UI needs for our Scene.
    /// </summary>
    public class UiSceneManager
    {
        #region Private variables

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

        public void InitUi()
        {
            InitMenuUi();
            InitOptionsUi();
            InitEndUi();
            InitGameUi();
            InitInfoUi();
            Main.MenuManager.SetScene("MainMenu");
        }

        private void InitEventListeners()
        {
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
            EventManager.RegisterListener<OptionsEventInfo>(HandleOptionsEvent);
        }

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

            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture2D = Main.Textures["Sad"];
                uiTextureObject.Texture = texture2D;
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                uiTextureObject.Transform2D.Origin = new Vector2(texture2D.Width / 2f, texture2D.Height / 2f);
                uiTextureObject.Transform2D.Translation = Main.ScreenCentre - Vector2.UnitX * 250;
                Main.MenuManager.Add("LoseScreen", uiTextureObject);
            }

            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                string text = "Continue";
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre;
                Main.MenuManager.Add("LoseScreen", uiButtonObject);
            }
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
                Main.UiManager.Add(uiTextureObject);
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
                Main.UiManager.Add(uiTextureObject);
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
                Main.UiManager.Add(uiTextureObject);
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
                Main.UiManager.Add(uiTextureObject);
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
                Main.UiManager.Add(uiTextObject);
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
                Main.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Hold Space To Attach";
                uiTextObject.ID = "AttachToolTipText";
                uiTextObject.Text = text;
                uiTextObject.StatusType = StatusType.Off;
                Main.UiManager.Add(uiTextObject);
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
                Main.UiManager.Add(uiTextureObject);
            }

            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.ID = "ResumeButton";
                uiButtonObject.Text = "Resume";
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre;
                uiButtonObject.StatusType = StatusType.Off;
                Main.UiManager.Add(uiButtonObject);
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
                Main.MenuManager.Add("Info", uiTextureObject);
            }
            
            if (((UIButtonObject) Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                string text = "Back";
                uiButtonObject.ID = text;
                uiButtonObject.Text = text;
                uiButtonObject.Transform2D.Translation = Main.ScreenCentre + Vector2.UnitY * 400;
                Main.MenuManager.Add("Info", uiButtonObject);
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

                Texture2D texture2D = Main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    Main.ScreenCentre - Vector2.UnitY * 125 + Vector2.UnitX * 100, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                Main.MenuManager.Add("Options", uiButtonObject);
            }
            
            text = "Controls";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;

                Texture2D texture2D = Main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    Main.ScreenCentre - Vector2.UnitY * 300 + Vector2.UnitX * 400, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                Main.MenuManager.Add("Options", uiButtonObject);
            }

            //Back Button
            text = "Difficulty";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = "Easy";
                uiButtonObject.ID = text;

                Texture2D texture2D = Main.Textures["GreenSticker"];
                uiButtonObject.Texture = texture2D;
                Vector2 position = Main.ScreenCentre + Vector2.UnitX * 350;
                uiButtonObject.Transform2D = new Transform2D(
                    position, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                Main.MenuManager.Add("Options", uiButtonObject);
            }

            //Back Button
            text = "Back";
            uiButtonObject = ((UIButtonObject) Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;

                Texture2D texture2D = Main.Textures["YellowSticker"];
                uiButtonObject.Texture = texture2D;
                uiButtonObject.Transform2D = new Transform2D(
                    Main.ScreenCentre + Vector2.UnitY * 125 + Vector2.UnitX * 100, 0, Vector2.One,
                    new Vector2(texture2D.Width / 2f, texture2D.Height / 2f),
                    new Integer2(texture2D.Width, texture2D.Height));
                uiButtonObject.SourceRectangle = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
                Main.MenuManager.Add("Options", uiButtonObject);
            }
        }

        #endregion

        #region Methods

        private void UpdateGameUi(GameTime gameTime)
        {
            if (Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText") is UITextObject uiTextObject)
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
                        UIButtonObject options)
                    {
                        options.Text = options.Text.Equals("Easy") ? "Hard" : "Easy";
                        options.Texture = options.Text.Equals("Easy")
                            ? Main.Textures["GreenSticker"]
                            : Main.Textures["RedSticker"];
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}