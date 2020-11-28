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
    public class MovingPlatformTile : PathMoveTile, IActivatable
    {
        private bool isActivated;
        private Vector3 endPos;
        private Vector3 currentPos;
        private Vector3 starPos;
        private int maxMove;

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType, Vector3 endpos) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            this.starPos = this.currentPos = this.Transform3D.Translation;
            this.endPos = endpos;
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
                EffectParameters.Clone() as EffectParameters, Model, TileType, endPos);

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
            //System.Diagnostics.Debug.WriteLine("Moving Platform activate (doesn't work yet)");
            isActivated = true;
            if (isActivated)
            {
                movePlatform();
            }
        }

        public bool isActive()
        {
            return isActivated;
        }

        public void movePlatform()
        {
            bool max = false;

            if (this.currentPos.X <= this.starPos.X)
            {
                max = false;
            }

            if (this.currentPos.X >= this.endPos.X)
            {
                max = true;
            }

            if (!max)
                this.currentPos.X++;
            else
                this.currentPos.X--;

            this.Transform3D.TranslateBy(new Vector3(this.currentPos.X , 0, 0));

        }

        public void Deactivate()
        {
            System.Diagnostics.Debug.WriteLine("Moving Platform deactivate (doesn't work yet)");
            //this.Transform3D.TranslateBy(this.starPos);
            //this.currentPos = this.StartPos;
            isActivated = false;
        }

        public void ToggleActivation()
        {
            if(isActivated)
                Deactivate();
            else
                Activate();
        }
    }
}
