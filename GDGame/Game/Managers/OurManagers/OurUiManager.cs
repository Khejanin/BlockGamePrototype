using System;
using GDGame.Actors;
using GDGame.Constants;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;

namespace GDGame.Managers
{
    /// <summary>
    /// Class that sets up everything the UI needs for our Scene.
    /// </summary>
    public class OurUiManager : PausableDrawableGameComponent
    {
        #region Private variables

        private PlayerTile playerTile;
        private Coffee coffee;

        #endregion

        #region Constructors

        public OurUiManager(Main main, StatusType statusType) : base(main, statusType)
        {
            InitEventListeners();
            Main = main;
        }

        public void SetCoffee(Coffee coffee)
        {
            this.coffee = coffee;
        }

        private Main Main { get; }

        #endregion

        #region Properties, Indexers

        #endregion

        #region Initialization

        private void InitEventListeners()
        {
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
            EventManager.RegisterListener<OptionsEventInfo>(HandleOptionsEvent);
        }

        public void InitGameUi()
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
            
            uiTextureObject = ((UITextureObject) Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Drawn | StatusType.Update;
                Texture2D texture = Main.Textures["optionsButton"];
                uiTextureObject.ID = "Alarm";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(Constants.GameConstants.ScreenWidth*0.8f, GameConstants.ScreenHeight * 0.9f);
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
        
        #endregion

        #region Methods

        protected override void ApplyUpdate(GameTime gameTime)
        {
            UITextObject uiTextObject;
            if (coffee != null)
            {
                if ((uiTextObject =
                    Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText") as UITextObject) != null)
                {
                    if (coffee.TimeLeft == -1) uiTextObject.Text = "Coffee not rising yet!";
                    else
                    {
                        uiTextObject.Text = "Time Left: " + coffee.TimeLeft.ToString("0.00");
                    }
                    uiTextObject.Transform2D.Origin = new Vector2(
                        Main.Fonts["Arial"].MeasureString(uiTextObject.Text).X / 2,
                        Main.Fonts["Arial"].MeasureString(uiTextObject.Text).Y / 2);
                }
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