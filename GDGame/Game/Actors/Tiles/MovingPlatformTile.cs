using System;
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
    public class MovingPlatformTile : PathMoveTile, IActivatable, ICloneable
    {
        #region 05. Private variables

        private Vector3 currentPos;
        private Vector3 endPos;

        private bool isActivated;

        #endregion

        #region 06. Constructors

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType,
            Vector3 endPos) : base(id, actorType, statusType, transform, effectParameters, model, tileType)
        {
            currentPos = transform.Translation;
            this.endPos = endPos;
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

        #endregion

        #region 11. Methods

        public void Activate()
        {
            //System.Diagnostics.Debug.WriteLine("Moving Platform activate (doesn't work yet)");
            isActivated = true;
            if (isActivated) MovePlatform();
        }

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model,
                TileType, endPos);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    platform.ControllerList.Add(controller.Clone() as IController);

            return platform;
        }

        public void Deactivate()
        {
            isActivated = false;
        }

        public bool IsActive()
        {
            return isActivated;
        }

        private void MovePlatform()
        {
            bool max = currentPos.X >= endPos.X;

            if (!max)
                currentPos.X++;
            else
                currentPos.X--;

            Transform3D.TranslateBy(new Vector3(currentPos.X, 0, 0));
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