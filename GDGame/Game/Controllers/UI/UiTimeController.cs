using GDGame.Game.Actors;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UiTimeController : Controller
    {
        #region Private variables

        private Coffee coffee;
        private UITextObject uiTextObject;

        #endregion

        #region Constructors

        public UiTimeController(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        #endregion

        #region Override Method

        public override void Update(GameTime gameTime, IActor actor)
        {
            uiTextObject ??= actor as UITextObject;
            if (coffee != null && uiTextObject != null)
            {
                if (coffee.TimeLeft == -1) uiTextObject.Text = "Coffee not rising yet!";
                else uiTextObject.Text = "Time Left: " + coffee.TimeLeft.ToString("0.00");

                uiTextObject.Transform2D.Origin = uiTextObject.SpriteFont.MeasureString(uiTextObject.Text) / 2;
            }
        }

        #endregion

        #region Public Method

        public void SetCoffee(Coffee coffee)
        {
            this.coffee = coffee;
        }

        #endregion
    }
}