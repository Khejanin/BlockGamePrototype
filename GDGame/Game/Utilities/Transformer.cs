using GDGame.EventSystem;
using GDGame.Managers;
using GDLibrary.Parameters;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public static class TransforHelper
    {
        public static void MoveTo(this Transform3D transform3D, Vector3 target, int time,
            Smoother.SmoothingMethod method,LoopMethod loopMethod = LoopMethod.PlayOnce, Body parentBody = null)
        {
            EventManager.FireEvent(new MovementInformation(transform3D,target,time,method,loopMethod,parentBody));
        }
        
        public static void RotateTo(this Transform3D transform3D, Vector3 target, int time,
            Smoother.SmoothingMethod method,LoopMethod loopMethod = LoopMethod.PlayOnce, Body parentBody = null)
        {
            EventManager.FireEvent(new RotationInformation(transform3D,target,time,method,loopMethod,parentBody));
        }
        
        public static void ScaleTo(this Transform3D transform3D, Vector3 target, int time,
            Smoother.SmoothingMethod method,LoopMethod loopMethod = LoopMethod.PlayOnce, Body parentBody = null)
        {
            EventManager.FireEvent(new ScaleInformation(transform3D,target,time,method,loopMethod,parentBody));
        }
        
    }
}