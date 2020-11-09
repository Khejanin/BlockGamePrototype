using Microsoft.Xna.Framework.Input;

namespace GDGame.Constants
{
    public class GameConstants
    {
        #region Common

        private static readonly float StrafeSpeedMultiplier = 0.75f;
        public static readonly Keys[] KeysOne = { Keys.W, Keys.S, Keys.A, Keys.D };
        public static readonly Keys[] KeysTwo = { Keys.U, Keys.J, Keys.H, Keys.K };
       
        public static readonly int screenWidth = 1024;
        public static readonly int screenHeight = 768;

        #endregion Common

        #region First Person Camera

        public static readonly float moveSpeed = 0.1f;
        public static readonly float strafeSpeed = StrafeSpeedMultiplier * moveSpeed;
        public static readonly float rotateSpeed = 0.01f;

        #endregion First Person Camera

        #region Flight Camera

        public static readonly float flightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = StrafeSpeedMultiplier * flightMoveSpeed;
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