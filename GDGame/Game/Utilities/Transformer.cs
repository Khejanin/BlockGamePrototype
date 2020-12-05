using GDGame.EventSystem;
using GDGame.Managers;
using GDLibrary.Actors;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public static class Transformer
    {
        #region Methods

        public static void MoveTo(this Actor3D actor, bool isRelative, Vector3 target, int timeInMilliseconds,
            Smoother.SmoothingMethod method, LoopMethod loopMethod = LoopMethod.PlayOnce, Body parentBody = null)
        {
            EventManager.FireEvent(new MovementInformation(actor, target, timeInMilliseconds, isRelative, method,
                loopMethod, parentBody));
        }

        public static void RotateTo(this Actor3D actor, bool isRelative, Vector3 target, int timeInMilliseconds,
            Smoother.SmoothingMethod method, LoopMethod loopMethod = LoopMethod.PlayOnce)
        {
            EventManager.FireEvent(new RotationInformation(actor, isRelative, target, timeInMilliseconds, method,
                loopMethod));
        }

        public static void ScaleTo(this Actor3D actor, bool isRelative, Vector3 target, int timeInMilliseconds,
            Smoother.SmoothingMethod method, LoopMethod loopMethod = LoopMethod.PlayOnce)
        {
            EventManager.FireEvent(new ScaleInformation(actor, isRelative, target, timeInMilliseconds, method,
                loopMethod));
        }

        #endregion
    }
}