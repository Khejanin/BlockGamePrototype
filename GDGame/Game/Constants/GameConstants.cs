﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Constants
{
    public class GameConstants
    {
        #region Static Fields and Constants

        private static readonly float StrafeSpeedMultiplier = 0.75f;
        public static readonly Keys[] keysOne = {Keys.W, Keys.S, Keys.A, Keys.D};
        public static readonly Keys[] keysTwo = {Keys.U, Keys.J, Keys.H, Keys.K};

        public static readonly int screenWidth = 1024;
        public static readonly int screenHeight = 768;

        private const float MoveSpeed = 0.1f;
        public static readonly float strafeSpeed = StrafeSpeedMultiplier * MoveSpeed;
        public static readonly float rotateSpeed = 0.01f;

        private const float FlightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = StrafeSpeedMultiplier * FlightMoveSpeed;
        public static readonly float flightRotateSpeed = 0.01f;

        private static readonly float AngularSpeedMultiplier = 10;
        private const float LowAngularSpeed = 10;
        private static readonly float MediumAngularSpeed = LowAngularSpeed * AngularSpeedMultiplier;
        public static readonly float hiAngularSpeed = MediumAngularSpeed * AngularSpeedMultiplier;

        public const float MovementCooldown = 2f;

        public static readonly Color colorGold = new Color(138, 99, 39, 255);

        #endregion
    }
}