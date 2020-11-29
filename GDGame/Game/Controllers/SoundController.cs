﻿using System;
using GDGame.Actors;
using GDGame.Managers;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Controllers
{
    internal class SoundController : Controller, ICloneable
    {
        #region 05. Private variables

        private KeyboardManager keyboardManager;
        private string moveSfx, attachSfx;
        private SoundEffectInstance playerMove, playerAttach;
        private PlayerTile playerTile;
        private SoundManager soundManager;

        #endregion

        #region 06. Constructors

        public SoundController(string id, ControllerType controllerType, KeyboardManager keyboardManager,
            SoundManager soundManager, string moveSfx, string attachSfx) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.soundManager = soundManager;
            this.moveSfx = moveSfx;
            this.attachSfx = attachSfx;

            SoundEffect temp = soundManager.FindSound(this.moveSfx).GetSfx();
            if (temp != null)
                playerMove = temp.CreateInstance();

            SoundEffect temp2 = soundManager.FindSound(this.attachSfx).GetSfx();
            if (temp2 != null)
                playerAttach = temp2.CreateInstance();
        }

        #endregion

        #region 09. Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            playerTile ??= actor as PlayerTile;
            if (keyboardManager.IsKeyPressed())
                HandleKeyboardInput();
        }

        #endregion

        #region 11. Methods

        public new object Clone()
        {
            return new SoundController(ID, ControllerType, keyboardManager, soundManager, moveSfx, attachSfx);
        }

        #endregion

        #region 12. Events

        private void HandleKeyboardInput()
        {
            HandlePlayerMovement();
        }

        private void HandlePlayerMovement()
        {
            if (keyboardManager.IsFirstKeyPress(Keys.Space) && playerTile.IsAttached)
                playerAttach?.Play();


            if (playerTile.IsMoving)
                if (playerMove != null && keyboardManager.IsFirstKeyPress(Keys.Up) ||
                    keyboardManager.IsFirstKeyPress(Keys.Down)
                    || keyboardManager.IsFirstKeyPress(Keys.Left) ||
                    keyboardManager.IsFirstKeyPress(Keys.Right))
                    playerMove?.Play();
        }

        #endregion
    }
}