using GDGame.Controllers;
using GDGame.Utilities;
using GDLibrary.Enums;

namespace GDGame.Component
{
    public class DoorMovementComponent : PathMovementComponent
    {
        public DoorMovementComponent(string id, ControllerType controllerType, ActivationType activationType, float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType, timePercent, smoothingMethod)
        {
            
        }

        protected override void OnDeactivated()
        {
            MoveToNextPoint();
        }

        protected override void OnActivated()
        {
            MoveToNextPoint();
        }
    }
}
