using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class MovingPlatformTile : PathMoveTile, IActivatable
    {
        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<ActivatorEventInfo>(HandleActivatorEvent);
            base.InitializeTile();
        }

        private void HandleActivatorEvent(ActivatorEventInfo info)
        {
            switch (info.type)
            {
                case ActivatorEventType.Activate:
                    Activate();
                    break;
                case ActivatorEventType.Deactivate:
                    Deactivate();
                    break;
            }
        }

        protected override void MoveToNextPoint()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
        }

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    platform.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return platform;
        }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public void ToggleActivation()
        {
            throw new System.NotImplementedException();
        }
    }
}
