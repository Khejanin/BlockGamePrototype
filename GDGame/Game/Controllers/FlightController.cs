using System;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Controllers
{
    public class FlightController : Controller, ICloneable
    {
        #region Private variables

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private float moveSpeed, strafeSpeed, rotationSpeed;

        #endregion

        #region Constructors

        public FlightController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            float moveSpeed,
            float strafeSpeed, float rotationSpeed) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (actor is Actor3D parent)
            {
                HandleKeyboardInput(gameTime, parent);
                HandleMouseInput(gameTime, parent);
            }
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new FirstPersonController(ID, ControllerType, keyboardManager,
                mouseManager, moveSpeed, strafeSpeed, rotationSpeed);
        }

        #endregion

        #region Events

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            Vector3 moveVector = Vector3.Zero;

            if (keyboardManager.IsKeyDown(Keys.W))
                moveVector = parent.Transform3D.Look * moveSpeed;
            else if (keyboardManager.IsKeyDown(Keys.S)) moveVector = -1 * parent.Transform3D.Look * moveSpeed;

            if (keyboardManager.IsKeyDown(Keys.A))
                moveVector -= parent.Transform3D.Right * strafeSpeed;
            else if (keyboardManager.IsKeyDown(Keys.D)) moveVector += parent.Transform3D.Right * strafeSpeed;

            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = mouseManager.GetDeltaFromCentre(new Vector2(512, 384));
            mouseDelta *= rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0) parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));
        }

        #endregion
    }
}