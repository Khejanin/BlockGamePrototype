using System;

namespace GDGame.Utilities
{
    public class Smoother
    {
        #region Enums

        public enum SmoothingMethod
        {
            Smooth,
            Accelerate,
            Decelerate,
            Back,
            Linear
        }

        #endregion

        #region Public Method

        //This is a bit of a replacement of the curves, small but powerful and quite simple
        public static float SmoothValue(SmoothingMethod method, float x)
        {
            switch (method)
            {
                case SmoothingMethod.Linear:
                    return x;
                case SmoothingMethod.Smooth:
                    return x * x * (3.0f - 2.0f * x);
                case SmoothingMethod.Accelerate:
                    return x * x;
                case SmoothingMethod.Decelerate:
                    x = 1.0f - x;
                    x *= x;
                    return 1.0f - x;
                case SmoothingMethod.Back:
                    x = 1.0f - x;
                    x = (float) (x * x * x - x * Math.Sin(x * Math.PI));
                    return 1.0f - x;
            }

            return x;
        }

        #endregion
    }
}