using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using MovementEvent = GDGame.EventSystem.MovementEvent;

namespace GDGame.Component
{
    public class PlayerMovementComponent : Controller, ICloneable
    {
        #region Private variables

        private PlayerTile playerTile;

        private TileMovementComponent tileMovementComponent;

        #endregion

        #region Constructors

        public PlayerMovementComponent(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime, IActor actor)
        {
            playerTile ??= actor as PlayerTile;
            tileMovementComponent ??= (TileMovementComponent) playerTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            PlayerMovementComponent playerMovementComponent = new PlayerMovementComponent(ID, ControllerType);
            playerMovementComponent.EventListeners();
            return playerMovementComponent;
        }

        private void EventListeners()
        {
            EventManager.RegisterListener<MovementEvent>(HandleMovement);
        }

        private bool IsMoveValid(Quaternion rotationToApply, Vector3 rotatePoint, Vector3 playerTargetPos, Vector3 offset)
        {
            List<Vector3> initials = playerTile.AttachedTiles.Select(i => i.Transform3D.Translation).ToList();
            initials.Insert(0, playerTile.Transform3D.Translation);
            List<Vector3> ends = playerTile.AttachedTiles.Select(i => i.CalculateTargetPosition(rotatePoint, rotationToApply)).ToList();
            ends.Insert(0, playerTargetPos);
            List<Raycaster.HitResult> results = new List<Raycaster.HitResult>();
            List<Raycaster.FloorHitResult> floorHitResults = new List<Raycaster.FloorHitResult>();
            RaycastManager.Instance.RaycastAll(playerTile, offset, initials, ends, ref results, ref floorHitResults);
            return results.Count == 0 && floorHitResults.Count > 0;
        }

        private void Move(Vector3 direction)
        {
            if (playerTile.IsAlive)
            {
                playerTile.SetRotatePoint(direction);
                tileMovementComponent.CalculateEndPos(direction, out Vector3 endPos, out Quaternion q, out Vector3 o);
                if (IsMoveValid(q, playerTile.RotatePoint, endPos, o))
                {
                    tileMovementComponent.MoveTile();
                    foreach (TileMovementComponent movementComponent in playerTile.AttachedTiles.Select(attachedTile =>
                        attachedTile.ControllerList.Find(controller =>
                            controller.GetControllerType() == ControllerType.Movement) as TileMovementComponent))
                    {
                        movementComponent?.CalculateEndPos(direction, out endPos, out q, out o);
                        movementComponent?.MoveTile();
                    }

                    //Play player move sound
                    EventManager.FireEvent(new SoundEventInfo { soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.PlayerMove, emitterTransform = playerTile.Transform3D });
                }
            }
        }

        #endregion

        #region Events

        private void HandleMovement(MovementEvent movementEvent)
        {
            if (movementEvent.type == MovementType.OnMove) Move(movementEvent.direction);
        }

        #endregion
    }
}