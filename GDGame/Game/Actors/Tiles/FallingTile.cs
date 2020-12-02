using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Interfaces;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    internal class FallingTile : PathMoveTile, IActivatable
    {
        #region Private variables

        private Vector3 currentPos;

        #endregion

        #region Constructors

        public FallingTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, OurEffectParameters effectParameters, Model model, ETileType tileType) : base(
            id, actorType, statusType, transform, effectParameters, model, tileType)
        {
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
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
        }

        #endregion

        #region Methods

        public void Activate()
        {
            Fall();
        }

        public new object Clone()
        {
            FallingTile platform = new FallingTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    platform.ControllerList.Add(controller.Clone() as IController);
            return platform;
        }

        public void Deactivate()
        {
        }

        public void Fall()
        {
            Transform3D.TranslateBy(new Vector3(0, 0, currentPos.Z - 1));
            Transform3D.TranslateBy(new Vector3(0, 0, currentPos.Z - 1));
            Deactivate();
        }

        public void ToggleActivation()
        {
        }

        #endregion

        #region Events

        private void HandleActivatorEvent(ActivatorEventInfo info)
        {
            Activate();
        }

        #endregion
    }
}