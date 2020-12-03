using System.Diagnostics;
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

        private bool canMove;
        private int index = 0;
        private int incrementor = 1;

        #endregion

        #region Constructors

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType,
            transform, effectParameters, model, tileType)
        {
        }

        #endregion

        #region Initialization

        public override void InitializeTile()
        {
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
        
        private void OnMovingTileEvent(MovingTilesEventInfo info)
        {
            if (canMove)
            {
                canMove = false;
                MoveToNextPoint();
            }
        }

    

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, TileType);
            
            platform.ControllerList.AddRange(GetControllerListClone());

            return platform;
        }
        
        public void Activate()
        {
            
        }

        public void Deactivate()
        {
            
        }

        public void ToggleActivation()
        {
            if(isActivated) Deactivate();
            else Activate();
        }

       
        #endregion

        #region Events

        private void HandleActivatorEvent(ActivatorEventInfo info)
        {
            if (info.id == activatorId)
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
        }

        #endregion
        
    }
}