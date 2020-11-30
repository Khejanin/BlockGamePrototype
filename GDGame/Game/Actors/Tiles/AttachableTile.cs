using System.Diagnostics;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class AttachableTile : MovableTile
    {
        #region Constructors

        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType,
            transform, effectParameters, model, tileType)
        {
        }

        #endregion

        #region Methods

        private void CheckCollision(Raycaster.HitResult hit)
        {
            if (hit?.actor == null) return;

            switch (hit.actor)
            {
                case SpikeTile _:
                    EventManager.FireEvent(new PlayerEventInfo
                        {type = PlayerEventType.AttachedTileDie, attachedTile = this});
                    break;
                case CheckpointTile checkpointTile:
                    EventManager.FireEvent(new PlayerEventInfo
                        {type = PlayerEventType.SetCheckpoint, position = checkpointTile.Transform3D.Translation});
                    break;
                case ButtonTile buttonTile:
                    buttonTile.Activate();
                    break;
            }
        }

        public new object Clone()
        {
            AttachableTile enemyTile = new AttachableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    enemyTile.ControllerList.Add(controller.Clone() as IController);

            return enemyTile;
        }

        #endregion

        #region Events

        public void OnMoveEnd()
        {
            CheckCollision(RaycastManager.Instance.Raycast(this, Transform3D.Translation, Vector3.Down, true, 0.5f));
            Raycaster.HitResult hit = RaycastManager.Instance.Raycast(this, Transform3D.Translation, Vector3.Down, true, 0.5f);
            if (hit?.actor is SpikeTile)
                Debug.WriteLine(ID + " is ded!");
        }

        #endregion
    }
}