using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class MovingPlatformTile : PathMoveTile
    {
        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            base.InitializeTile();
        }

        private void HandlePlayerEvent(PlayerEventInfo info)
        {
            if (info.type == Enums.PlayerEventType.Move)
            {
                MoveToNextPoint();
            }
        }

        protected override void MoveToNextPoint()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, .5f);

            if (hit != null && hit.actor is PlayerTile)
                EventManager.FireEvent(new PlayerEventInfo() { type = Enums.PlayerEventType.Die });
        }

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

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
