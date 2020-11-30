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
        private int dir; //-1 = X, 1 = Y, 0 = Z
        private Vector3 endPos;
        private int tileMoves;
        private bool isActivated;
        private Vector3 starPos;
        private bool oppDir;
        private bool max;

       
        #endregion

        #region 06. Constructors


        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType,
            int tileMoves, int dir) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            this.starPos = this.currentPos = Transform3D.Translation;
            this.dir = dir;
            this.tileMoves = tileMoves;

            if (dir == -1)
                this.endPos = new Vector3(starPos.X + tileMoves, starPos.Y, StartPos.Z);
            else if (dir == 1)
                this.endPos = new Vector3(starPos.X, starPos.Y + tileMoves, StartPos.Z);
            else
                this.endPos = new Vector3(this.starPos.X, this.starPos.Y, (this.starPos.Z + tileMoves));

            this.oppDir = isOppDir();
            this.max = false;
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
                EffectParameters.Clone() as EffectParameters, Model, TileType, tileMoves, dir);

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

        public Vector3 moveUp()
        {
            int move = 1;
            if (oppDir)
                move = -1;

            float a = this.currentPos.X, b = this.currentPos.Y, c = this.currentPos.Z;

            if (this.dir == -1) //X
                a += move;
            else if (this.dir == 1) //Y
                b += move;
            else //Z
                c += move;

            return new Vector3(a, b, c);
        }

        public bool atMaxPoint()
        {
            if (oppDir)
            {
                if (currentPos.X >= starPos.X || currentPos.Y >= starPos.Y || currentPos.Z >= starPos.Z)
                    max = false;

                if (currentPos.X <= endPos.X || currentPos.Y <= starPos.Y || currentPos.Z <= starPos.Z)
                    max = true;
            }
            else
            {
                if (currentPos.X <= starPos.X || currentPos.Y <= starPos.Y || currentPos.Z <= starPos.Z)
                    max = false;

                if (currentPos.X >= endPos.X || currentPos.Y >= starPos.Y || currentPos.Z >= starPos.Z)
                    max = true;
            }
            return false;
        }

        public bool isOppDir()
        {
            if ((this.starPos.X > this.endPos.X || this.starPos.Y > this.endPos.Y || this.starPos.Z > this.endPos.Z))
            {
                return true;
            }
            return false;
        }

        public Vector3 moveDown()
        {
            int move = -1;
            if (oppDir)
                move = 1;

            float a = this.currentPos.X, b = this.currentPos.Y, c = this.currentPos.Z;

            if (this.dir == -1) //X
                a += move;
            else if (this.dir == 1) //Y
                b += move;
            else //Z
                c += move;

            return new Vector3(a, b, c);
        }


        public void movePlatform()
        {
            Vector3 b = new Vector3(0, 0, 0);
            this.max = atMaxPoint();

            if (!max)
                b = moveUp();
            else
                b = moveDown();

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