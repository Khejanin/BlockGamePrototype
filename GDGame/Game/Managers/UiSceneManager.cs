using System;
using GDGame.Actors;
using GDGame.Constants;
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
        #region Private variables

        private bool optionsShown = true;

        private PlayerTile playerTile;

        #endregion

        #region Constructors

        public UiSceneManager(Scene scene)
        {
            Scene = scene;
            InitEventListeners();
        }

        #endregion

        #region Properties, Indexers

        private Scene Scene { get; }

        #endregion

        #region Initialization

        private void InitEndUi()
        {
            string text = "You won!!! Press ESC to close the Game!";
            Vector2 origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);
            Integer2 dimensions = new Integer2(Scene.Main.Fonts["Arial"].MeasureString(text));
            Transform2D transform2D = new Transform2D(Scene.Main.ScreenCentre, 0, Vector2.One, origin, dimensions);

            UITextObject uITextObject = new UITextObject("WinText", ActorType.UIText, StatusType.Drawn, transform2D,
                Color.Wheat, 0, SpriteEffects.None, text,
                Scene.Main.Fonts["Arial"]);
            Scene.Main.UiManager.Add(uITextObject);
        }

        private void InitEventListeners()
        {
            EventManager.RegisterListener<DataManagerEvent>(HandleDataManagerEvent);
        }

        private void InitGameUi()
        {
            float screenHeight = Scene.Main.GraphicsDevice.Viewport.Height;
            float screenWidth = Scene.Main.GraphicsDevice.Viewport.Width;

            if (((UITextureObject) Scene.Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = Scene.Main.Textures["TopBar"];
                uiTextureObject.ID = "TopBar";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Scene.Main.ScreenCentre;
                Scene.Main.UiManager.Add(uiTextureObject);
            }


            if (((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Moves: 0";
                uiTextObject.ID = "MoveText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);

                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 75;
                Vector2 xPosition = -Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = Scene.Main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                Scene.Main.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Time : 00:00:00";
                uiTextObject.ID = "TimeText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);
                Vector2 yPosition = -Vector2.UnitY * screenHeight / 2 + Vector2.UnitY * 60;
                Vector2 xPosition = Vector2.UnitX * screenWidth / 5 - Vector2.UnitX * 15;
                uiTextObject.Transform2D.Translation = Scene.Main.ScreenCentre + yPosition + xPosition;
                uiTextObject.Color = GameConstants.colorGold;
                Scene.Main.UiManager.Add(uiTextObject);
            }

            uiTextObject = ((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                string text = "Hold Space To Attach";
                uiTextObject.ID = "AttachToolTipText";
                uiTextObject.Text = text;
                uiTextObject.StatusType = StatusType.Off;
                Scene.Main.UiManager.Add(uiTextObject);
            }

            uiTextureObject = ((UITextureObject) Scene.Main.UiArchetypes["texture"]).Clone() as UITextureObject;
            if (uiTextureObject != null)
            {
                Texture2D texture = Scene.Main.Textures["options"];
                uiTextureObject.ID = "PauseBackground";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Scene.Main.ScreenCentre;
                uiTextureObject.StatusType = StatusType.Off;
                Scene.Main.UiManager.Add(uiTextureObject);
            }

            if (((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.ID = "ResumeButton";
                uiButtonObject.Text = "Resume";
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre;
                uiButtonObject.StatusType = StatusType.Off;
                Scene.Main.UiManager.Add(uiButtonObject);
            }
        }

        private void InitInfoUi()
        {
            if (((UITextureObject) Scene.Main.UiArchetypes["texture"]).Clone() is UITextureObject uiTextureObject)
            {
                Texture2D texture = Scene.Main.Textures["Tutorial"];
                uiTextureObject.ID = "TutorialTexture";
                uiTextureObject.Texture = texture;
                uiTextureObject.Transform2D.Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                uiTextureObject.SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                uiTextureObject.Transform2D.Translation = Scene.Main.ScreenCentre;
                Scene.Main.UiManager.Add(uiTextureObject);
            }

            if (((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                string text = "Press SPACEBAR to continue!";
                uiTextObject.ID = "ContinueText";
                uiTextObject.Text = text;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation = Scene.Main.ScreenCentre + Vector2.UnitY * 300;
                Scene.Main.UiManager.Add(uiTextObject);
            }
        }

        private void InitMenuUi()
        {
            string text = "Play";
            if (((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre - Vector2.UnitY * 120;
                Scene.Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            text = "Options";
            uiButtonObject = ((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre;
                Scene.Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            text = "Quit";
            uiButtonObject = ((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre + Vector2.UnitY * 120;
                Scene.Main.MenuManager.Add("MainMenu", uiButtonObject);
            }

            if (((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() is UITextObject uiTextObject)
            {
                text = "B_Logic";
                uiTextObject.ID = "B_Logic";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.SaddleBrown;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation =
                    new Vector2(Scene.Main.ScreenCentre.X - 275, Scene.Main.ScreenCentre.Y - 25);
                Scene.Main.MenuManager.Add("MainMenu", uiTextObject);
            }


            uiTextObject = ((UITextObject) Scene.Main.UiArchetypes["text"]).Clone() as UITextObject;
            if (uiTextObject != null)
            {
                text = "Caffeine Edition!";
                uiTextObject.ID = "Caffeine Edition!";
                uiTextObject.Text = text;
                uiTextObject.Color = Color.SaddleBrown;
                uiTextObject.Transform2D.Origin = new Vector2(Scene.Main.Fonts["Arial"].MeasureString(text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(text).Y / 2);
                uiTextObject.Transform2D.Translation =
                    new Vector2(Scene.Main.ScreenCentre.X - 275, Scene.Main.ScreenCentre.Y - 50);
                Scene.Main.MenuManager.Add("MainMenu", uiTextObject);
            }
        }

        private void InitOptionsUi()
        {
            string text = "Resume";
            if (((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() is UIButtonObject uiButtonObject)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre - Vector2.UnitY * 75;
                Scene.Main.MenuManager.Add("Options", uiButtonObject);
            }

            text = "Back";
            uiButtonObject = ((UIButtonObject) Scene.Main.UiArchetypes["button"]).Clone() as UIButtonObject;
            if (uiButtonObject != null)
            {
                uiButtonObject.Text = text;
                uiButtonObject.ID = text;
                uiButtonObject.Transform2D.Translation = Scene.Main.ScreenCentre + Vector2.UnitY * 75;
                Scene.Main.MenuManager.Add("Options", uiButtonObject);
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
                    InitGameUi();
                    break;
                case SceneType.Info:
                    InitInfoUi();
                    break;
            }

            InitMenuUi();
            InitOptionsUi();
            Scene.Main.MenuManager.SetScene("MainMenu");
        }

        #endregion

        #region Methods

        private void AnimateButton(UIButtonObject uiButtonObject, float f)
        {
            uiButtonObject.Transform2D.Scale = Vector2.One * f;
            uiButtonObject.TextOffset = new Vector2(
                uiButtonObject.Transform2D.Bounds.Width / 2f -
                Scene.Main.Fonts["Arial"].MeasureString(uiButtonObject.Text).X / 2,
                uiButtonObject.Transform2D.Bounds.Height / 2f -
                Scene.Main.Fonts["Arial"].MeasureString(uiButtonObject.Text).Y / 2);
        }

        private void ToggleOptionsMenu()
        {
            optionsShown = !optionsShown;
            UITextureObject uiTextureObject =
                Scene.Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "PauseBackground") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
                uiTextureObject.StatusType = StatusType.Drawn;
            else if (uiTextureObject != null) uiTextureObject.StatusType = StatusType.Off;

            uiTextureObject =
                Scene.Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "ResumeButton") as UITextureObject;
            if (uiTextureObject != null && optionsShown)
            {
                uiTextureObject.StatusType = StatusType.Drawn | StatusType.Update;
                Scene.Main.MouseManager.MouseVisible = optionsShown;
            }
            else if (uiTextureObject != null)
            {
                uiTextureObject.StatusType = StatusType.Off;
                Scene.Main.MouseManager.MouseVisible = optionsShown;
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
                    UpdateMenuUi();
                    break;
                case SceneType.End:
                    UpdateEndUi();
                    break;
                case SceneType.Info:
                    UpdateInfoUi();
                    break;
                case SceneType.Options:
                    UpdateOptionsUi();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateEndUi()
        {
        }

        private void UpdateGameUi(GameTime gameTime)
        {
            if (Scene.Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "TimeText") is UITextObject uiTextObject
            )
            {
                uiTextObject.Text = "Time: " + gameTime.TotalGameTime.Hours % 24 + ":" +
                                    gameTime.TotalGameTime.Minutes % 60 + ":" + gameTime.TotalGameTime.Seconds % 60;
                uiTextObject.Transform2D.Origin = new Vector2(
                    Scene.Main.Fonts["Arial"].MeasureString(uiTextObject.Text).X / 2,
                    Scene.Main.Fonts["Arial"].MeasureString(uiTextObject.Text).Y / 2);
            }

            uiTextObject =
                Scene.Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "AttachToolTipText") as UITextObject;
            playerTile ??=
                Scene.Main.ObjectManager.OpaqueList.Find(actor3D => actor3D.ID == "clone - Player") as PlayerTile;
            if (uiTextObject != null && playerTile != null && playerTile.AttachCandidates.Count > 0)
                uiTextObject.StatusType = StatusType.Drawn;
            else if (uiTextObject != null) uiTextObject.StatusType = StatusType.Off;

            if (Scene.Main.KeyboardManager.IsFirstKeyPress(Keys.O))
                ToggleOptionsMenu();
        }

        private void UpdateInfoUi()
        {
        }

        private void UpdateMenuUi()
        {
            foreach (DrawnActor2D drawnActor2D in Scene.Main.UiManager.UIObjectList)
                if (drawnActor2D is UIButtonObject uiButtonObject)
                {
                    if (uiButtonObject.Transform2D.Bounds.Contains(Scene.Main.MouseManager.Bounds))
                    {
                        AnimateButton(uiButtonObject, 1.1f);

                        if (Scene.Main.MouseManager.IsLeftButtonClickedOnce())
                            switch (uiButtonObject.ID)
                            {
                                case "Play":
                                    Scene.Main.SceneManager.NextScene();
                                    break;
                                case "Options":
                                    Scene.Main.SceneManager.OptionsSwitchScene();
                                    break;
                                case "Quit":
                                    Scene.Main.Exit();
                                    break;
                            }
                    }
                    else
                    {
                        AnimateButton(uiButtonObject, 1);
                    }
                }
        }

        private void UpdateOptionsUi()
        {
            foreach (DrawnActor2D drawnActor2D in Scene.Main.UiManager.UIObjectList)
                if (drawnActor2D is UIButtonObject uiButtonObject)
                {
                    if (uiButtonObject.Transform2D.Bounds.Contains(Scene.Main.MouseManager.Bounds))
                    {
                        AnimateButton(uiButtonObject, 1.1f);

                        if (Scene.Main.MouseManager.IsLeftButtonClickedOnce())
                            switch (uiButtonObject.ID)
                            {
                                case "Resume":
                                    Scene.Main.SceneManager.MenuSwitchScene();
                                    break;
                                case "Back":
                                    Scene.Main.SceneManager.MenuSwitchScene();
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
            if (Scene.Main.UiManager.UIObjectList.Find(actor2D => actor2D.ID == "NumberOfMovesText") is UITextObject
                uiTextObject)
                uiTextObject.Text = Scene.Main.LevelDataManager.CurrentMovesCount.ToString();
        }

        #endregion
    }
}