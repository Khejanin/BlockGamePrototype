using System;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public class MathHelperFunctions
    {
        #region 03. Static Fields and Constants

        private static Random _rnd;

        #endregion

        #region 07. Properties, Indexers

        public static Random Rnd => _rnd ?? new Random();

        #endregion

        #region 11. Methods

        public static float GetAngle(Vector3 forward, Vector3 look)
        {
            Vector3 noY = look * (Vector3.Forward + Vector3.Right);
            noY.Normalize();

            Vector3 cross = Vector3.Cross(forward, noY);
            double dot = Vector3.Dot(forward, noY);

            double angle = Math.Atan2(cross.Length(), dot);

            double test = Vector3.Dot(Vector3.Up, cross);
            if (test < 0.0f) angle = -angle;
            return (float) -(angle + Math.PI);
        }

        public static Vector3 QuaternionToEulerAngles(Quaternion q)
        {
            Vector3 pitchYawRoll = Vector3.Zero;
            q.Normalize();

            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;

            float roll;
            float pitch;
            float yaw;

            float test = q.W * q.Y - q.Z * q.X;

            if (test > 0.4999996)
            {
                yaw = (float) (-2 * Math.Atan2(q.X, q.W));
                pitch = (float) (Math.PI / 2);
                roll = 0;
            }
            else if (test < -0.4999996)
            {
                yaw = (float) (2 * Math.Atan2(q.X, q.W));
                pitch = (float) (-Math.PI / 2);
                roll = 0;
            }
            else
            {
                roll = (float) Math.Atan2(2f * (q.X * q.W + q.Y * q.Z), 1 - 2f * (sqx + sqy));
                pitch = (float) Math.Asin(2f * test);
                yaw = (float) Math.Atan2(2f * (q.X * q.Y + q.Z * q.W), 1 - 2f * (sqy + sqz));
            }


            pitchYawRoll.X = MathF.Round(MathHelper.ToDegrees(roll));
            pitchYawRoll.Y = MathF.Round(MathHelper.ToDegrees(pitch));
            pitchYawRoll.Z = MathF.Round(MathHelper.ToDegrees(yaw));
            return pitchYawRoll;
        }

        #endregion
    }
}