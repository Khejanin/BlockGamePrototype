using System.Collections.Generic;
using GDGame.Constants;
using GDGame.Controllers;
using GDGame.Game.Handlers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    /// <summary>
    ///     Class that sets up everything the UI needs for our Scene.
    /// </summary>
    public class GameUi
    {
        #region Private variables

        private Main main;

        #endregion

        #region Constructors

        public GameUi(Main main)
        {
            this.main = main;
        }

        #endregion

        #region Initialization

        public void InitGameUi()
        {
            float screenHeight = main.GraphicsDevice.Viewport.Height;
            float screenWidth = main.GraphicsDevice.Viewport.Width;

            if (((UITextureObject) main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = main.Textures["TopBar"];
                uiTextureObject.ID = "TopBar";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = main.ScreenCentre;
                main.UiManager.Add(uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug1";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(332, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                main.UiManager.Add(uiTextureObject);
            }


            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug2";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(508, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                main.UiManager.Add(uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Texture2D texture = main.Textures["Mug-Collected"];
                uiTextureObject.ID = "Mug3";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(332 + (508 - 332) * 2, 130);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                main.UiManager.Add(uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Update;
                Texture2D texture = main.Textures["CoffeeRisingWarning"];
                uiTextureObject.ID = "Alarm";
                uiTextureObject.Texture = texture;
                Vector2 offset = new Vector2(GameConstants.ScreenWidth * 0.85f, GameConstants.ScreenHeight * 0.75f);
                uiTextureObject.Transform2D.Origin = new Vector2(0, 0);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                uiTextureObject.Transform2D.Scale = Vector2.One * .5f;
                main.UiManager.Add(uiTextureObject);
                uiTextureObject.ControllerList.Add(new UiBlinkingController("uiBlinkingC", ControllerType.Ui, 700));
                uiTextureObject.EventHandlerList.Add(new UiCoffeeWarningEventHandler(EventCategoryType.UI,
                    uiTextureObject, main.Textures["CoffeeDangerWarning"]));
            }

            if (((UITextObject) main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Moves: 0";
                uiTextObject.ID = "MoveText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(main.Fonts["Arial"].MeasureString(text).X / 2,
                    main.Fonts["Arial"].MeasureString(text).Y / 2);

                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 75;
                Vector2 xPosition = -Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                main.UiManager.Add(uiTextObject);
                uiTextObject.EventHandlerList.Add(new UiMovesEventHandler(EventCategoryType.UI, uiTextObject));
            }

            uiTextObject = ((UITextObject) main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Time : 00:00:00";
                uiTextObject.ID = "TimeText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(main.Fonts["Arial"].MeasureString(text).X / 2,
                    main.Fonts["Arial"].MeasureString(text).Y / 2);
                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 60;
                Vector2 xPosition = Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                main.UiManager.Add(uiTextObject);
                uiTextObject.ControllerList.Add(new UiTimeController("TimeController", ControllerType.Ui));
            }

            uiTextureObject = ((UITextureObject) main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture = main.Textures["PressSpace"];
                uiTextureObject.ID = "PressSpace";
                uiTextureObject.Texture = texture;
                Vector2 offset = main.ScreenCentre - Vector2.UnitY * 100;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = offset;
                uiTextureObject.Transform2D.Scale *= 0.7f;
                uiTextureObject.StatusType = StatusType.Update;
                main.UiManager.Add(uiTextureObject);
                List<Texture2D> texture2Ds = new List<Texture2D>
                {
                    texture,
                    main.Textures["PressSpace02"],
                    main.Textures["PressSpace03"],
                    main.Textures["PressSpace03"],
                    main.Textures["PressSpace03"]
                };
                uiTextureObject.ControllerList.Add(new UiTextureChanger("UTC", ControllerType.Ui, texture2Ds, 200));
                uiTextureObject.ControllerList.Add(new UiToggleVisibilityController("TVC", ControllerType.Ui,
                    main.ObjectManager));
            }

            if (((UIButtonObject) main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.ID = "ResumeButton";
                uiButtonObject.Text = "Resume";
                uiButtonObject.Transform2D.Translation = main.ScreenCentre;
                uiButtonObject.StatusType = StatusType.Off;
                main.UiManager.Add(uiButtonObject);
            }
        }

        #endregion
    }
}