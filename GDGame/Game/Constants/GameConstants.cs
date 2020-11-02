using Microsoft.Xna.Framework.Input;

namespace GDGame
{
    public class GameConstants
    {
        #region Common

        private static readonly float strafeSpeedMultiplier = 0.75f;
        public static readonly Keys[] KeysOne = { Keys.W, Keys.S, Keys.A, Keys.D };
        public static readonly Keys[] KeysTwo = { Keys.U, Keys.J, Keys.H, Keys.K };

        #endregion Common

        #region First Person Camera

        public static readonly float moveSpeed = 0.1f;
        public static readonly float strafeSpeed = strafeSpeedMultiplier * moveSpeed;
        public static readonly float rotateSpeed = 0.01f;

        #endregion First Person Camera

        #region Flight Camera

        public static readonly float flightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = strafeSpeedMultiplier * flightMoveSpeed;
        public static readonly float flightRotateSpeed = 0.01f;

        #endregion Flight Camera

        #region Security Camera

        private static readonly float angularSpeedMultiplier = 10;
        public static readonly float lowAngularSpeed = 10;
        public static readonly float mediumAngularSpeed = lowAngularSpeed * angularSpeedMultiplier;
        public static readonly float hiAngularSpeed = mediumAngularSpeed * angularSpeedMultiplier;

        #endregion Security Camera
    }
}