﻿using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Controllers
{
    public class PlayerController : Controller
    {
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private CameraManager<Camera3D> cameraManager;

        public PlayerController(string id, ControllerType controllerType, KeyboardManager keyboardManager, GamePadManager gamePadManager, CameraManager<Camera3D> cameraManager) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.cameraManager = cameraManager;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (keyboardManager.IsKeyPressed())
                HandleKeyboardInput(actor as PlayerTile);
        }

        private void HandleKeyboardInput(PlayerTile actor)
        {
            HandlePlayerMovement(actor);
        }

        private void HandlePlayerMovement(PlayerTile playerTile)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.Space) && !playerTile.IsAttached) playerTile.Attach();
            else if (!keyboardManager.IsKeyDown(Keys.Space) && keyboardManager.IsStateChanged() && playerTile.IsAttached) playerTile.Detach();

            if (!playerTile.IsMoving)
            {
                Vector3 moveDir = Vector3.Zero;
                Vector3 look = cameraManager[cameraManager.ActiveCameraIndex].Transform3D.Look;
                look = Math.Abs(look.X) > Math.Abs(look.Z) ? new Vector3(look.X < 0 ? -1 : 1, 0, 0) : new Vector3(0, 0, look.Z < 0 ? -1 : 1);

                Vector3 right = cameraManager[cameraManager.ActiveCameraIndex].Transform3D.Right;
                right = Math.Abs(right.X) > Math.Abs(right.Z) ? new Vector3(right.X < 0 ? -1 : 1, 0, 0) : new Vector3(0, 0, right.Z < 0 ? -1 : 1);

                if (keyboardManager.IsKeyDown(Keys.Up) || keyboardManager.IsKeyDown(Keys.W)) moveDir = look;
                else if (keyboardManager.IsKeyDown(Keys.Down) || keyboardManager.IsKeyDown(Keys.S)) moveDir = -look;
                if (keyboardManager.IsKeyDown(Keys.Left) || keyboardManager.IsKeyDown(Keys.A)) moveDir = -right;
                else if (keyboardManager.IsKeyDown(Keys.Right) || keyboardManager.IsKeyDown(Keys.D)) moveDir = right;

                if (moveDir != Vector3.Zero)
                {
                    EventManager.FireEvent(new MovementEvent {type = MovementType.OnMove, tile = playerTile, direction = moveDir, onMoveEnd = playerTile.OnMoveEnd});
                }
            }
        }

        public bool IsMoveValid(PlayerTile playerTile, Quaternion rotationToApply, Vector3 rotatePoint, Vector3 playerTargetPos, Vector3 offset)
        {
            List<Vector3> initials = playerTile.AttachedTiles.Select(i => i.Transform3D.Translation).ToList();
            initials.Insert(0, playerTile.Transform3D.Translation);
            List<Vector3> ends = playerTile.AttachedTiles.Select(i => i.CalculateTargetPosition(rotatePoint, rotationToApply)).ToList();
            ends.Insert(0, playerTargetPos);
            List<Raycaster.HitResult> results = new List<Raycaster.HitResult>();
            List<Raycaster.FloorHitResult> floorHitResults = new List<Raycaster.FloorHitResult>();
            playerTile.PlayerCastAll(offset, initials, ends, ref results, ref floorHitResults);
            return results.Count == 0 && floorHitResults.Count > 0;
        }

        public override object Clone()
        {
            return new PlayerController(Id, ControllerType, keyboardManager, gamePadManager, cameraManager);
        }
    }
}