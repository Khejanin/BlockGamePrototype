using System.Diagnostics;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class MovingPlatformTile : PathMoveTile, IActivatable
    {
        #region 05. Private variables

        private Vector3 currentPos;
        private int dir;
        private Vector3 endPos;
        private bool isActivated;
        private int maxMove;
        private Vector3 starPos;

        #endregion

        #region 06. Constructors

        //-1 = X, 1 = Y, 0 = Z

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType,
            int tileMoves, int dir) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            starPos = currentPos = Transform3D.Translation;
            this.dir = dir;

            if (dir == -1)
                endPos = new Vector3(starPos.X + maxMove, starPos.Y, StartPos.Z);
            else if (dir == 1)
                endPos = new Vector3(starPos.X, starPos.Y + maxMove, StartPos.Z);
            else
                endPos = new Vector3(starPos.X, starPos.Y, starPos.Z + maxMove);
        }

        #endregion

        #region 08. Initialization

        public override void InitializeTile()
        {
            EventManager.RegisterListener<ActivatorEventInfo>(HandleActivatorEvent);
            base.InitializeTile();
        }

        #endregion

        #region 09. Override Methode

        protected override void MoveToNextPoint()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
        }

        #endregion

        #region 11. Methods

        public void Activate()
        {
            //System.Diagnostics.Debug.WriteLine("Moving Platform activate (doesn't work yet)");
            isActivated = true;
            if (isActivated) movePlatform();
        }

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType, maxMove, dir);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    platform.ControllerList.Add(controller.Clone() as IController);

            return platform;
        }

        public void Deactivate()
        {
            Debug.WriteLine("Moving Platform deactivate (doesn't work yet)");
            //this.Transform3D.TranslateBy(this.starPos);t
            //this.currentPos = this.StartPos;
            isActivated = false;
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

            if (currentPos.X <= starPos.X || currentPos.Y <= starPos.Y || currentPos.Z <= starPos.Z)
                max = false;

            if (currentPos.X >= endPos.X || currentPos.Y >= starPos.Y || currentPos.Z >= starPos.Z)
                max = true;


            if (!max)
                move = 1;
            else
                move = -1;

            if (dir == -1) //X
                b = new Vector3(currentPos.X + move, 0, 0);
            else if (dir == 1) //Y
                b = new Vector3(0, currentPos.Y + move, 0);
            else //Z
                b = new Vector3(0, 0, currentPos.Z + move);

            Transform3D.TranslateBy(b);
            currentPos = b;
        }

        public void ToggleActivation()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }

        #endregion

        #region 12. Events

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

        #endregion
    }
}