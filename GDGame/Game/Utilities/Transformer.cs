using GDGame.EventSystem;
using GDGame.Managers;
using GDLibrary.Actors;
using MovementEvent = GDGame.Managers.MovementEvent;

namespace GDGame.Utilities
{
    public static class Transformer
    {
        #region Public Method

        public static void MoveTo(this Actor3D actor3D, AnimationEventData animationEventData)
        {
            if (animationEventData.actor == null) animationEventData.actor = actor3D;
            EventManager.FireEvent(new MovementEvent(animationEventData));
        }

        public static void RemoveAllAnimations(this Actor3D actor3D)
        {
            EventManager.FireEvent(
                new MovementEvent(new AnimationEventData {type = AnimationEventType.RemoveAllOfActor}));
        }

        public static void RotateTo(this Actor3D actor3D, AnimationEventData animationEventData)
        {
            if (animationEventData.actor == null) animationEventData.actor = actor3D;
            EventManager.FireEvent(new RotationEvent(animationEventData));
        }

        public static void ScaleTo(this Actor3D actor3D, AnimationEventData animationEventData)
        {
            if (animationEventData.actor == null) animationEventData.actor = actor3D;
            EventManager.FireEvent(new ScaleEvent(animationEventData));
        }

        #endregion
    }
}