﻿using GDLibrary;
using GDGame.Game.Tiles;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using GDGame.Game.Managers;
using GDLibrary.Managers;

namespace GDGame.Game.Controllers
{
    internal class SoundController : IController
    {
        private KeyboardManager keyboardManager;
        private CubePlayer player;
        private SoundManager soundManager;
        private string moveSFX, attachSFX;
        private SoundEffectInstance playerMove, playerAttach;

        public SoundController(KeyboardManager keyboardManager, SoundManager soundManager, string moveSFX, string attachSFX)
        {
            this.keyboardManager = keyboardManager;
            this.soundManager = soundManager;
            this.moveSFX = moveSFX;
            this.attachSFX = attachSFX;

            SoundEffect temp = soundManager.FindSound(moveSFX).GetSfx();
            if (temp != null)
                this.playerMove = temp.CreateInstance();
                //this.playerMove.Volume = (float)0.4;

            SoundEffect temp2 = soundManager.FindSound(attachSFX).GetSfx();
            if (temp2 != null)
                this.playerAttach = temp2.CreateInstance();
        }

        public object Clone()
        {
            return new SoundController(keyboardManager, soundManager, moveSFX, attachSFX);
        }

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            player ??= actor as CubePlayer;
            if (this.keyboardManager.IsKeyPressed())
                HandleKeyboardInput(gameTime);
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            HandlePlayerMovement();
        }

        private void HandlePlayerMovement()
        {
            if (keyboardManager.IsFirstKeyPress(Keys.Space) && player.IsAttached && this.playerAttach != null)   
                    playerAttach.Play();


            if (player.IsMoving)
            {
                if (this.playerMove != null && this.keyboardManager.IsFirstKeyPress(Keys.Up) || this.keyboardManager.IsFirstKeyPress(Keys.Down) 
                    || this.keyboardManager.IsFirstKeyPress(Keys.Left) || this.keyboardManager.IsFirstKeyPress(Keys.Right))
                    playerMove.Play();
            }
        }
    }
}