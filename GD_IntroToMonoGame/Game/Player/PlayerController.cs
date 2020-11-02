using System.Diagnostics;
using Microsoft.Xna.Framework;
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
            if(this.keyboardManager.IsStateChanged())
                HandleKeyboardInput(gameTime);
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            HandlePlayerMovement();

            //parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private void HandlePlayerMovement()
        {
            if (!player.IsMoving)
            {
                Vector3 moveDir = Vector3.Zero;
                if (this.keyboardManager.IsKeyDown(Keys.Up))
                    moveDir = -Vector3.UnitZ;
                else if (this.keyboardManager.IsKeyDown(Keys.Down))
                    moveDir = Vector3.UnitZ;

                if (this.keyboardManager.IsKeyDown(Keys.Left))
                    moveDir = -Vector3.UnitX;
                else if (this.keyboardManager.IsKeyDown(Keys.Right))
                    moveDir = Vector3.UnitX;

                Vector3 start = player.Transform3D.Translation;
                Vector3 dest = player.Transform3D.Translation + moveDir;

                if (moveDir != Vector3.Zero)
                {
                    Grid.GridPositionResult gridPositionResult = Grid.QueryMove(dest);
                    if (gridPositionResult.validMovePos
                    )
                    {
                        if(gridPositionResult.floorTile.TileType == ETileType.Win) Debug.WriteLine("YOU WON THE GAME! WOO");
                        player.Move(moveDir);
                        Grid.MoveTo(start, dest);
                        Debug.WriteLine("PLAYER MOVED FROM : " + start + " TO : " + dest);
                    }
                }
            }
        }

        public object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
