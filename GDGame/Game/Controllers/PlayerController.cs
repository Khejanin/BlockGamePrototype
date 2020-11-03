using GDGame.Game.Tiles;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class PlayerController : IController
    {
        KeyboardManager keyboardManager;
        CubePlayer player;

        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;
        private Vector3 offset;

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
            if(this.keyboardManager.IsKeyPressed())
                HandleKeyboardInput(gameTime);
        }

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            HandlePlayerMovement();

            //parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private void HandlePlayerMovement()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Space))
                player.Attach();
            else if (this.keyboardManager.IsFirstKeyRelease(Keys.Space))
                player.Detach();

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
                        //if(gridPositionResult.floorTile.TileType == ETileType.Win) Debug.WriteLine("YOU WON THE GAME! WOO");
                        //player.Move(moveDir);
                        AttemptMove(moveDir);
                        Grid.MoveTo(start, dest);
                       // Debug.WriteLine("PLAYER MOVED FROM : " + start + " TO : " + dest);
                    }
                }
            }
        }

        private void AttemptMove(Vector3 direction)
        {
            if (!isMoving)
            {
                player.UpdateRotatePoints();

                if (direction == Vector3.UnitX)
                    offset = player.RightRotatePoint;
                else if (direction == -Vector3.UnitX)
                    offset = player.LeftRotatePoint;
                else if (direction == -Vector3.UnitZ)
                    offset = player.ForwardRotatePoint;
                else if (direction == Vector3.UnitZ)
                    offset = player.BackwardRotatePoint;
                else
                    throw new System.ArgumentException("Invalid direction!");

                //Move the player
                Move(direction, player.Transform3D);

                //Move the attached tiles
                foreach (AttachableTile tile in player.AttachedTiles)
                    Move(direction, tile.Transform3D);

                currentMovementTime = movementTime;
                isMoving = true;
            }
        }

        private void Move(Vector3 direction, Transform3D tileTrans)
        {
            offset -= tileTrans.Translation;

            startRotQ = tileTrans.Rotation;
            endRotQ = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, -Vector3.Up), MathHelper.ToRadians(90)) * startRotQ;

            offset = Vector3.Transform(-offset, endRotQ * Quaternion.Inverse(startRotQ));
            startPos = tileTrans.Translation;
            endPos = tileTrans.Translation + direction + offset;
        }

        public object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
