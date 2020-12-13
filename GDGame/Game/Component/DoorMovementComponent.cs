using GDGame.Controllers;
using GDGame.Utilities;
using GDLibrary.Enums;

namespace GDGame.Component
{
    public class DoorMovementComponent : PathMovementComponent
    {
        #region Constructors

        public DoorMovementComponent(string id, ControllerType controllerType, ActivationType activationType,
            float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType,
            timePercent, smoothingMethod)
        {
        }

        #endregion

        #region Override Method

        protected override void OnActivated()
        {
            MoveToNextPoint();
        }

        protected override void OnDeactivated()
        {
            MoveToNextPoint();
        }

        #endregion
    }
}