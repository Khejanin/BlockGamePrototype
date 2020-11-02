﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class PlayerController : IController
    {
        KeyboardManager keyboardManager;
        CubePlayer player;

        public PlayerController(KeyboardManager keyboardManager)
        {
            this.keyboardManager = keyboardManager;
        }

        public void Initialize(IActor actor)
        {
            this.player = actor as CubePlayer;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            HandleKeyboardInput(gameTime);
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            if (this.keyboardManager.IsKeyDown(Keys.Up))
                this.player.Move(-Vector3.UnitZ);
            else if (this.keyboardManager.IsKeyDown(Keys.Down))
                this.player.Move(Vector3.UnitZ);

            if (this.keyboardManager.IsKeyDown(Keys.Left))
                this.player.Move(-Vector3.UnitX);
            else if (this.keyboardManager.IsKeyDown(Keys.Right))
                this.player.Move(Vector3.UnitX);

            //parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        public object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
