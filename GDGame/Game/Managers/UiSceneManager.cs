using System;
using GDGame.Actors;
using GDGame.Actors.Drawn;
using GDGame.EventSystem;
using GDGame.Scenes;
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
        #region 05. Private variables

        private bool optionsShown = true;

        private PlayerTile playerTile;

        #endregion

        #region 06. Constructors

        public UiSceneManager(Scene scene)
        {
            Scene = scene;
            InitEventListeners();
        }

        #endregion

        #region 07. Properties, Indexers

        private Scene Scene { get; }

        #endregion

        #region 08. Initialization

        private void InitEndUi()
        {
            string text = "You won!!! Press ESC to close the Game!";
            Vector2 origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
            Integer2 dimensions = new Integer2(Scene.Game.Fonts["Arial"].MeasureString(text));
            Transform2D transform2D = new Transform2D(Scene.Game.ScreenCentre, 0, Vector2.One, origin, dimensions);

            UITextObject uITextObject = new UITextObject("WinText", ActorType.UIText, StatusType.Drawn, transform2D, Color.Wheat, 0, SpriteEffects.None, text,
                Scene.Game.Fonts["Arial"]);
            Scene.Game.UiManager.Add(uITextObject);
        }

        private void InitEventListeners()
        {
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
        }

        private void InitInfoUi()
        {
            if (((UITextureObject) Scene.Game.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = Scene.Game.Textures["Tutorial"];
                uiTextureObject.ID = "TutorialTexture";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Scene.Game.ScreenCentre;
                Scene.Game.UiManager.Add(uiTextureObject);
            }

            if (((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Press SPACEBAR to continue!";
                uiTextObject.ID = "ContinueText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * 300;
                Scene.Game.UiManager.Add(uiTextObject);
            }
        }

        private void InitMainUi()
        {
            float screenHeight = Scene.Game.GraphicsDevice.Viewport.Height;
            float screenWidth = Scene.Game.GraphicsDevice.Viewport.Width;

            if (((UITextureObject) Scene.Game.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                float textureHeight = 50;
                Texture2D texture = Scene.Game.Textures["WhiteSquare"];
                uiTextureObject.ID = "BottomBar";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Scale = new Vector2(screenWidth, textureHeight);
                uiTextureObject.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - textureHeight / 2);
                Scene.Game.UiManager.Add(uiTextureObject);
            }

            uiTextureObject = ((UITextureObject) Scene.Game.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                float textureHeight = 100;
                float textureWidth = 120;
                Texture2D texture = Scene.Game.Textures["WhiteSquare"];
                uiTextureObject.ID = "BottomMiddleBar";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Scale = new Vector2(textureWidth, textureHeight);
                uiTextureObject.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - textureHeight / 2);
                Scene.Game.UiManager.Add(uiTextureObject);
            }


            if (((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Moves";
                uiTextObject.ID = "MoveText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - uiTextObject.Transform2D.Origin.Y - 55);
                Scene.Game.UiManager.Add(uiTextObject);
            }


            uiTextObject = ((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = Scene.SceneName;
                uiTextObject.ID = "SceneNameText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation =
                    Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - uiTextObject.Transform2D.Origin.Y - 5) - Vector2.UnitX * screenWidth / 4;
                Scene.Game.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Time : 00:00:00";
                uiTextObject.ID = "TimeText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation =
                    Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - uiTextObject.Transform2D.Origin.Y - 5) + Vector2.UnitX * screenWidth / 4;
                Scene.Game.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "0";
                uiTextObject.ID = "NumberOfMovesText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(text).X / 2, Scene.Game.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * (screenHeight / 2 - uiTextObject.Transform2D.Origin.Y - 5);
                Scene.Game.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Scene.Game.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Hold Space To Attach";
                uiTextObject.ID = "AttachToolTipText";
                uiTextObject.Text = text;
                uiTextObject.StatusType = StatusType.Off;
                Scene.Game.UiManager.Add(uiTextObject);
            }

            uiTextureObject = ((UITextureObject) Scene.Game.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture = Scene.Game.Textures["options"];
                uiTextureObject.ID = "PauseBackground";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Scene.Game.ScreenCentre;
                uiTextureObject.StatusType = StatusType.Off;
                Scene.Game.UiManager.Add(uiTextureObject);
            }

            if (((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() is UiButtonObject uiButtonObject)
            {
                uiButtonObject.ID = "ResumeButton";
                uiButtonObject.Text = "Resume";
                uiButtonObject.Transform2D.Translation = Scene.Game.ScreenCentre;
                uiButtonObject.StatusType = StatusType.Off;
                Scene.Game.UiManager.Add(uiButtonObject);
            }
        }

        private void InitMenuUi()
        {
            string text = "Play";
            if (((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() is UiButtonObject button)
            {
                button.Text = text;
                button.Transform2D.Translation = Scene.Game.ScreenCentre - Vector2.UnitY * 120;
                Scene.Game.UiManager.Add(button);
            }

            text = "Options";
            button = ((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() as UiButtonObject;
            if (button != null)
            {
                button.Text = text;
                button.Transform2D.Translation = Scene.Game.ScreenCentre;
                Scene.Game.UiManager.Add(button);
            }

            text = "Quit";
            button = ((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() as UiButtonObject;
            if (button != null)
            {
                button.Text = text;
                button.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * 120;
                Scene.Game.UiManager.Add(button);
            }
        }

        private void InitOptionsUi()
        {
            string text = "Resume";
            if (((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() is UiButtonObject button)
            {
                button.Text = text;
                button.Transform2D.Translation = Scene.Game.ScreenCentre - Vector2.UnitY * 75;
                Scene.Game.UiManager.Add(button);
            }

            text = "Quit";
            button = ((UiButtonObject) Scene.Game.UiArchetypes["button"]).Clone() as UiButtonObject;
            if (button != null)
            {
                button.Text = text;
                button.Transform2D.Translation = Scene.Game.ScreenCentre + Vector2.UnitY * 75;
                Scene.Game.UiManager.Add(button);
            }
        }

        public void InitUi()
        {
            switch (Scene.SceneType)
            {
                case SceneType.End:
                    InitEndUi();
                    break;
                case SceneType.Game:
                    InitMainUi();
                    break;
                case SceneType.Info:
                    InitInfoUi();
                    break;
                case SceneType.Menu:
                    InitMenuUi();
                    break;
                case SceneType.Options:
                    InitOptionsUi();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 11. Methods

        private void ToggleOptionsMenu()
        {
            optionsShown = !optionsShown;
            UITextureObject uiTextureObject = Scene.Game.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "PauseBackground") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
                uiTextureObject.StatusType = StatusType.Drawn;
            else if (uiTextureObject != null) uiTextureObject.StatusType = StatusType.Off;

            uiTextureObject = Scene.Game.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "ResumeButton") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
            {
                uiTextureObject.StatusType = StatusType.Drawn | StatusType.Update;
                Scene.Game.MouseManager.MouseVisible = optionsShown;
            }
            else if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Scene.Game.MouseManager.MouseVisible = optionsShown;
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (Scene.SceneType)
            {
                case SceneType.Game:
                    UpdateGameUi(gameTime);
                    break;
                case SceneType.Menu:
                    UpdateMenuUi(gameTime);
                    break;
                case SceneType.End:
                    UpdateEndUi(gameTime);
                    break;
                case SceneType.Info:
                    UpdateInfoUi(gameTime);
                    break;
                case SceneType.Options:
                    UpdateOptionsUi(gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateEndUi(GameTime gameTime)
        {
        }

        private void UpdateGameUi(GameTime gameTime)
        {
            if (Scene.Game.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText") is UITextObject uiTextObject)
            {
                uiTextObject.Text = gameTime.TotalGameTime.Hours % 24 + ":" + gameTime.TotalGameTime.Minutes % 60 + ":" + gameTime.TotalGameTime.Seconds % 60;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Game.Fonts["Arial"].MeasureString(uiTextObject.Text).X / 2,
                    Scene.Game.Fonts["Arial"].MeasureString(uiTextObject.Text).Y / 2);
            }

            uiTextObject = Scene.Game.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "AttachToolTipText") as UITextObject;
            playerTile ??= Scene.Game.ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player") as PlayerTile;
            if (uiTextObject != null && playerTile != null && playerTile.AttachCandidates.Count > 0)
                uiTextObject.StatusType = StatusType.Drawn;
            else if (uiTextObject != null) uiTextObject.StatusType = StatusType.Off;

            if (Scene.Game.KeyboardManager.IsFirstKeyPress(Keys.O))
                ToggleOptionsMenu();
        }

        private void UpdateInfoUi(GameTime gameTime)
        {
        }

        private void UpdateMenuUi(GameTime gameTime)
        {
        }

        private void UpdateOptionsUi(GameTime gameTime)
        {
        }

        #endregion

        #region 12. Events

        private void HandleDataManagerEvent(DataManagerEvent obj)
        {
            if (Scene.Game.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "NumberOfMovesText") is UITextObject uiTextObject)
                uiTextObject.Text = Scene.Game.LevelDataManager.CurrentMovesCount.ToString();
        }

        #endregion
    }
}