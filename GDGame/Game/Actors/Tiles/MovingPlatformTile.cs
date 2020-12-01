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
        #region Private variables

        private Vector3 currentPos;
        private int dir; //-1 = X, 1 = Y, 0 = Z
        private Vector3 endPos;
        private bool isActivated;
        private bool max;
        private bool oppDir;
        private Vector3 starPos;
        private int tileMoves;

        #endregion

        #region Constructors

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType,
            int tileMoves, int dir) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            starPos = currentPos = Transform3D.Translation;
            this.dir = dir;
            this.tileMoves = tileMoves;

            if (dir == -1)
                endPos = new Vector3(starPos.X + tileMoves, starPos.Y, StartPos.Z);
            else if (dir == 1)
                endPos = new Vector3(starPos.X, starPos.Y + tileMoves, StartPos.Z);
            else
                endPos = new Vector3(starPos.X, starPos.Y, starPos.Z + tileMoves);

            oppDir = IsOppDir();
            max = false;
        }

        #endregion

        #region Initialization

        public override void InitializeTile()
        {
            EventManager.RegisterListener<ActivatorEventInfo>(HandleActivatorEvent);
            base.InitializeTile();
        }

        #endregion

        #region Override Methode

        protected override void MoveToNextPoint()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
        }

        #endregion

        #region Methods

        public void Activate()
        {
            //System.Diagnostics.Debug.WriteLine("Moving Platform activate (doesn't work yet)");
            isActivated = true;
            if (isActivated) MovePlatform();
        }

        public bool AtMaxPoint()
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

        public bool IsActive()
        {
            return isActivated;
        }

        public bool IsOppDir()
        {
            if (starPos.X > endPos.X || starPos.Y > endPos.Y || starPos.Z > endPos.Z) return true;
            return false;
        }

        public Vector3 MoveDown()
        {
            int move = -1;
            if (oppDir)
                move = 1;

            float a = currentPos.X, b = currentPos.Y, c = currentPos.Z;

            if (dir == -1) //X
                a += move;
            else if (dir == 1) //Y
                b += move;
            else //Z
                c += move;

            return new Vector3(a, b, c);
        }


        public void MovePlatform()
        {
            Vector3 b = new Vector3(0, 0, 0);
            max = AtMaxPoint();

            if (!max)
                b = MoveUp();
            else
                b = MoveDown();

            Transform3D.TranslateBy(b);
            currentPos = b;
        }

        public Vector3 MoveUp()
        {
            int move = 1;
            if (oppDir)
                move = -1;

            float a = currentPos.X, b = currentPos.Y, c = currentPos.Z;

            if (dir == -1) //X
                a += move;
            else if (dir == 1) //Y
                b += move;
            else //Z
                c += move;

            return new Vector3(a, b, c);
        }

        public void ToggleActivation()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }

        #endregion

        #region Events

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