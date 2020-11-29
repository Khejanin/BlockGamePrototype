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
        private int dir;

        //-1 = X, 1 = Y, 0 = Z

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType, int tileMoves, int dir) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            this.starPos = this.currentPos = this.Transform3D.Translation; 
            this.dir = dir;

            if(dir == -1)
                this.endPos = new Vector3((this.starPos.X + maxMove), this.starPos.Y, this.StartPos.Z);
            else if(dir == 1)
                this.endPos = new Vector3(this.starPos.X, (this.starPos.Y + maxMove), this.StartPos.Z);
            else
                this.endPos = new Vector3(this.starPos.X, this.starPos.Y, (this.starPos.Z + maxMove));
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
                EffectParameters.Clone() as EffectParameters, Model, TileType, maxMove, dir);

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
            int move = 0;
            Vector3 b = new Vector3(0, 0, 0);

            if (this.currentPos.X <= this.starPos.X || this.currentPos.Y <= this.starPos.Y || this.currentPos.Z <= this.starPos.Z)
                max = false;

            if (this.currentPos.X >= this.endPos.X || this.currentPos.Y >= this.starPos.Y || this.currentPos.Z >= this.starPos.Z)
                max = true;


            if (!max)
                move = 1;
            else
                move = -1;

            if(this.dir == -1) //X
            {
                b = new Vector3((this.currentPos.X + move), 0, 0);
            }
            else if(this.dir == 1) //Y
            {
                b = new Vector3(0, (this.currentPos.Y + move), 0);
            }
            else //Z
            {
                b = new Vector3(0, 0, (this.currentPos.Z + move));
            }

            this.Transform3D.TranslateBy(b);
            this.currentPos = b;

        }

        public void Deactivate()
        {
            System.Diagnostics.Debug.WriteLine("Moving Platform deactivate (doesn't work yet)");
            //this.Transform3D.TranslateBy(this.starPos);t
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
