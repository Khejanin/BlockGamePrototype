using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GDGame.Game.Tiles;
using GDGame.Game.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Game.Controllers
{
    public class PlayerController : IController
    {
        private KeyboardManager keyboardManager;
        private CubePlayer player;

        public PlayerController(KeyboardManager keyboardManager)
        {
            this.keyboardManager = keyboardManager;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            player ??= (CubePlayer) actor;
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
            if (keyboardManager.IsFirstKeyPress(Keys.Space) && !player.IsAttached)
                player.Attach();
            else if (!keyboardManager.IsKeyDown(Keys.Space) && keyboardManager.IsStateChanged() &&
                     player.IsAttached)
                player.Detach();

            if (!player.IsMoving)
            {
                Vector3 moveDir = Vector3.Zero;
                if (keyboardManager.IsKeyDown(Keys.Up))
                    moveDir = -Vector3.UnitZ;
                else if (keyboardManager.IsKeyDown(Keys.Down))
                    moveDir = Vector3.UnitZ;

                if (keyboardManager.IsKeyDown(Keys.Left))
                    moveDir = -Vector3.UnitX;
                else if (keyboardManager.IsKeyDown(Keys.Right))
                    moveDir = Vector3.UnitX;

                if (moveDir != Vector3.Zero)
                {
                    MovementComponent movementComponent = (MovementComponent) player.ControllerList.Find(controller =>
                        controller.GetType() == typeof(MovementComponent));
                    movementComponent?.Move(moveDir);
                }
            }
        }

        public bool IsMoveValid(Quaternion rotationToApply, Vector3 rotatePoint, Vector3 playerTargetPos)
        {
            List<Vector3> initials = player.AttachedTiles.Select(i => i.Transform3D.Translation).ToList();
            initials.Insert(0, player.Transform3D.Translation);
            List<Vector3> ends = player.AttachedTiles.Select(i => i.CalculateTargetPosition(rotatePoint, rotationToApply)).ToList();
            ends.Insert(0, playerTargetPos);
            List<Raycaster.HitResult> results = new List<Raycaster.HitResult>();
            List<Raycaster.FloorHitResult> floorHitResults = new List<Raycaster.FloorHitResult>();
            player.PlayerCastAll(initials, ends,ref results,ref floorHitResults);
            return results.Count == 0 && floorHitResults.Count > 0;
        }

        public object Clone()
        {
            return new PlayerController(keyboardManager);
        }
    }
}