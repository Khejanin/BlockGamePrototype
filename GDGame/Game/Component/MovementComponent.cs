﻿using System;
using System.Linq;
using GDGame.Actors;
using GDGame.Controllers;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class MovementComponent : IController
    {
        private MovableTile parent;

        private int movementTime;
        private Curve1D curve1D;
        private Vector3 diff;
        private Quaternion identityQuaternion;
        private Quaternion rotationQuaternion;
        private Vector3 startPos;
        private Vector3 endPos;

        public MovementComponent(int movementTime, Curve1D curve1D)
        {
            this.movementTime = movementTime;
            identityQuaternion = Quaternion.Identity;
            this.curve1D = curve1D;
            this.curve1D.Add(1, 0);
            this.curve1D.Add(0, movementTime);
        }

        public object Clone()
        {
            return new MovementComponent(movementTime, new Curve1D(curve1D.CurveLookType));
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as MovableTile;

            if (parent != null && parent.IsMoving)
            {
                if (parent.CurrentMovementTime <= 0)
                {
                    parent.IsMoving = false;
                    parent.CurrentMovementTime = 0;
                }

                Quaternion quaternion = Quaternion.Slerp(identityQuaternion, rotationQuaternion,
                    1 - (float) parent.CurrentMovementTime / movementTime);
                float currentStep = curve1D.Evaluate(parent.CurrentMovementTime, 5);
                Vector3 trans = startPos + diff * currentStep;

                parent.Transform3D.RotationInDegrees =  MathHelperFunctions.QuaternionToEulerAngles(quaternion);
                parent.Transform3D.Translation = trans;
                parent.CurrentMovementTime -= (int) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Vector3 direction)
        {
            if (parent != null && !parent.IsMoving)
            {
                RotationComponent rotationComponent =
                    (RotationComponent) parent.ControllerList.Find(controller =>
                        controller.GetType() == typeof(RotationComponent));
                rotationComponent?.SetRotatePoint(direction);

                //offset between the player and the point to rotate around
                Vector3 offset = parent.Transform3D.Translation - parent.RotatePoint;
                //The rotation to apply
                rotationQuaternion =
                    Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));
                //Rotate around the offset point
                Vector3 translation = Vector3.Transform(offset, rotationQuaternion);

                //Start and End Position --> Will be lerped between
                startPos = parent.Transform3D.Translation;
                endPos = parent.Transform3D.Translation + translation - offset;
                diff = endPos - startPos;

                if (parent.ActorType == ActorType.Player)
                {
                    PlayerController playerController =
                        (PlayerController) parent.ControllerList.Find(controller =>
                            controller.GetType() == typeof(PlayerController));
                    if (playerController != null &&
                        playerController.IsMoveValid(rotationQuaternion, parent.RotatePoint, endPos, offset))
                    {
                        EventManager.FireEvent(new PlayerEventInfo { type = Enums.PlayerEventType.Move });
                        //Calculate movement for each attached tile
                        if (parent is PlayerTile player)
                            foreach (MovementComponent movementController in player.AttachedTiles.Select(tile =>
                                (MovementComponent) tile.ControllerList.Find(controller =>
                                    controller.GetType() == typeof(MovementComponent))))
                            {
                                movementController?.Move(direction);
                            }


                        //Set animation time and movement flag
                        parent.CurrentMovementTime = movementTime;
                        parent.IsMoving = true;
                    }
                }
                else
                {
                    parent.CurrentMovementTime = movementTime;
                    parent.IsMoving = true;
                }
            }
        }
    }
}