using System;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public class MathHelperFunctions
    {
        public static Vector3 QuaternionToEulerAngles(Quaternion quaternion)
        {
            Vector3 angles;

            double rollN = 2 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z);
            double rollD = 1 - 2 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);
            angles.X = MathHelper.ToDegrees((float) Math.Atan2(rollN, rollD));

            double pitchN = 2 * (quaternion.W * quaternion.Y - quaternion.Z * quaternion.X);
            angles.Y = MathHelper.ToDegrees((float) Math.Asin(pitchN));

            double yawN = 2 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            double yawD = 1 - 2 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            angles.Z = MathHelper.ToDegrees((float) Math.Atan2(yawN, yawD));
            
            return angles;
        }
    }
}