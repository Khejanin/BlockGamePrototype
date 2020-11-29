using GDGame.Component;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    class FallingTile : PathMoveTile, IActivatable
    {
        private Vector3 currentPos;

        public FallingTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<ActivatorEventInfo>(HandleActivatorEvent);
            base.InitializeTile();
        }

        private void HandleActivatorEvent(ActivatorEventInfo info)
        {
            Activate();
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
        }

        public void Activate()
        {
            Fall();
        }

        public void Fall()
        {
            this.Transform3D.TranslateBy(new Vector3(0, 0, (this.currentPos.Z - 1)));
            this.Transform3D.TranslateBy(new Vector3(0, 0, (this.currentPos.Z - 1)));
            this.Deactivate();
        }

        protected override void MoveToNextPoint()
        {
        }

        public void ToggleActivation()
        {
        }

        public void Deactivate()
        {

        }

        public new object Clone()
        {
            FallingTile platform = new FallingTile("clone - " + ID, ActorType, StatusType,
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
    }
}
