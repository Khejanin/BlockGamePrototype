using GDLibrary;
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
            {
                this.playerMove = temp.CreateInstance();
                this.playerMove.Volume = (float)0.25;
            }
            SoundEffect temp2 = soundManager.FindSound(attachSFX).GetSfx();
            if (temp2 != null)
            {
                this.playerAttach = temp2.CreateInstance();
            }
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
            {
                HandleKeyboardInput(gameTime);
            }
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            HandlePlayerMovement();
        }

        private void HandlePlayerMovement()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Space))
            {
                if (this.playerAttach != null)
                {
                    playerAttach.Play();
                }
            }

            if (player.IsMoving)
            {
                //Vector3 moveDir = Vector3.Zero;
                if (this.keyboardManager.IsFirstKeyPress(Keys.Up) || this.keyboardManager.IsFirstKeyPress(Keys.Down) || this.keyboardManager.IsFirstKeyPress(Keys.Left) || this.keyboardManager.IsFirstKeyPress(Keys.Right))
                {
                    if (this.playerMove != null)
                    {
                        playerMove.Play();
                    }

                    //Vector3 moveDir = Vector3.Zero;
                    //Vector3 start = player.Transform3D.Translation;
                    //Vector3 dest = player.Transform3D.Translation + moveDir;
                    //if (moveDir != Vector3.Zero)
                    //{
                    //    //Play valid move
                    //}
                    //else
                    //{
                    //    //Play invalid move
                    //}
                }
            }
        }
    }
}