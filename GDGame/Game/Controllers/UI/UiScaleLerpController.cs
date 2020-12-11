using System;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UiScaleLerpController : Controller, ICloneable
    {
        #region Private variables

        private MouseManager mouseManager;
        private TrigonometricParameters trigonometricParameters;

        #endregion

        #region Constructors

        public UiScaleLerpController(string id, ControllerType controllerType, MouseManager mouseManager, TrigonometricParameters trigonometricParameters) : base(id,
            controllerType)
        {
            this.mouseManager = mouseManager;
            this.trigonometricParameters = trigonometricParameters;
        }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (actor is UIButtonObject drawnActor)
            {
                if (drawnActor.Transform2D.Bounds.Contains(mouseManager.Bounds))
                {
                    float lerpFactor = (float) (trigonometricParameters.MaxAmplitude *
                                                Math.Sin(MathHelper.ToRadians((float) (trigonometricParameters.AngularSpeed * gameTime.TotalGameTime.TotalMilliseconds +
                                                                                       trigonometricParameters.PhaseAngleInDegrees))));
                    drawnActor.Transform2D.Scale = drawnActor.Transform2D.OriginalScale + lerpFactor * drawnActor.Transform2D.OriginalScale;
                    drawnActor.TextScale = drawnActor.Transform2D.OriginalScale + lerpFactor * drawnActor.Transform2D.OriginalScale;
                }
                else
                {
                    drawnActor.Transform2D.Scale = Vector2.One;
                    drawnActor.TextScale = Vector2.One;
                }
            }
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new UiScaleLerpController(ID, ControllerType, mouseManager, trigonometricParameters.Clone() as TrigonometricParameters);
        }

        #endregion
    }
}