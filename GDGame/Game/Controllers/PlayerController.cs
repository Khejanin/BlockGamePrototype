using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Utilities;
using GDLibrary;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Controllers
{
    public class PlayerController : IController
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private PlayerTile playerTile;

        public PlayerController(KeyboardManager keyboardManager,GamePadManager gamePadManager)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            playerTile ??= (PlayerTile) actor;
            if (keyboardManager.IsKeyPressed())
                HandleKeyboardInput(gameTime);
        }

        public ControllerType GetControllerType()
        {
            throw new NotImplementedException();
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            HandlePlayerMovement();
        }

        private void HandlePlayerMovement()
        {
            if ((keyboardManager.IsFirstKeyPress(Keys.Space) || gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.RightTrigger)) && !playerTile.IsAttached)
                playerTile.Attach();
            else if ((!keyboardManager.IsKeyDown(Keys.Space) || !gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.RightTrigger)) && keyboardManager.IsStateChanged() &&
                     playerTile.IsAttached)
                playerTile.Detach();

            if (!playerTile.IsMoving)
            {
                Vector3 moveDir = Vector3.Zero;
                if (keyboardManager.IsKeyDown(Keys.Up) || gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.LeftThumbstickUp))
                    moveDir = -Vector3.UnitZ;
                else if (keyboardManager.IsKeyDown(Keys.Down)|| gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.LeftThumbstickDown))
                    moveDir = Vector3.UnitZ;

                if (keyboardManager.IsKeyDown(Keys.Left)|| gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.LeftThumbstickLeft))
                    moveDir = -Vector3.UnitX;
                else if (keyboardManager.IsKeyDown(Keys.Right)|| gamePadManager.IsButtonPressed(PlayerIndex.One,Buttons.LeftThumbstickRight))
                    moveDir = Vector3.UnitX;

                if (moveDir != Vector3.Zero)
                {
                    MovementComponent movementComponent = (MovementComponent) playerTile.ControllerList.Find(controller =>
                        controller.GetType() == typeof(MovementComponent));
                    movementComponent?.Move(moveDir);
                }
            }
        }

        public bool IsMoveValid(Quaternion rotationToApply, Vector3 rotatePoint, Vector3 playerTargetPos, Vector3 offset)
        {
            List<Vector3> initials = playerTile.AttachedTiles.Select(i => i.Transform3D.Translation).ToList();
            initials.Insert(0, playerTile.Transform3D.Translation);
            List<Vector3> ends = playerTile.AttachedTiles.Select(i => i.CalculateTargetPosition(rotatePoint, rotationToApply)).ToList();
            ends.Insert(0, playerTargetPos);
            List<Raycaster.HitResult> results = new List<Raycaster.HitResult>();
            List<Raycaster.FloorHitResult> floorHitResults = new List<Raycaster.FloorHitResult>();
            playerTile.PlayerCastAll(offset,initials, ends,ref results,ref floorHitResults);
            return results.Count == 0 && floorHitResults.Count > 0;
        }

        public object Clone()
        {
            return new PlayerController(keyboardManager,gamePadManager);
        }
    }
}