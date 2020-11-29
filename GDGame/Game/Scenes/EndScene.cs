using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scenes
{
    public class EndScene : Scene
    {
        #region 06. Constructors

        public EndScene(Main game, bool unloadsContent = false) : base(game, unloadsContent)
        {
            backgroundColor = Color.Black;
        }

        #endregion

        #region 08. Initialization

        public override void Initialize()
        {
            InitializeCamera();
            InitializeText();
        }

        private void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu_Camera", ActorType.Camera3D, StatusType.Update, new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up),
                Game.GlobalProjectionParameters, new Viewport(0, 0, 1024, 768));
            Game.CameraManager.Add(camera3D);
        }


        private void InitializeText()
        {
            string text = "You won!!! Press ESC to close the Game!";
            Vector2 origin = new Vector2(Game.Fonts["Arial"].MeasureString(text).X / 2, Game.Fonts["Arial"].MeasureString(text).Y / 2);
            Integer2 dimensions = new Integer2(Game.Fonts["Arial"].MeasureString(text));
            Transform2D transform2D = new Transform2D(Game.ScreenCentre, 0, Vector2.One, origin, dimensions);

            UITextObject uITextObject = new UITextObject("WinText", ActorType.UIText, StatusType.Drawn, transform2D, Color.Wheat, 0, SpriteEffects.None, text, Game.Fonts["Arial"]);
            Game.UiManager.Add(uITextObject);
        }

        #endregion

        #region 09. Override Methode

        protected override void DrawScene(GameTime gameTime)
        {
        }


        protected override void Terminate()
        {
            Game.UiManager.Dispose();
        }

        protected override void UpdateScene(GameTime gameTime)
        {
        }

        #endregion
    }
}