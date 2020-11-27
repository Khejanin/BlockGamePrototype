using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class AttachableTile : MovableTile
    {
        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType, statusType,
            transform, effectParameters, model, tileType)
        {
        }

        public void OnMoveEnd()
        {
            CheckCollision(this.Raycast(Transform3D.Translation, Vector3.Down, true, 0.5f, false));
            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Down, true, 0.5f, false);
            if (hit?.actor is SpikeTile)
                System.Diagnostics.Debug.WriteLine(ID + " is ded!");
        }

        private void CheckCollision(Raycaster.HitResult hit)
        {
            if (hit?.actor == null) return;

            switch (hit.actor)
            {
                case SpikeTile spikeTile:
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
            AttachableTile enemyTile = new AttachableTile("clone - " + ID , ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);

            if (ControllerList != null)
            {
                foreach (Controller controller in ControllerList)
                {
                    enemyTile.ControllerList.Add(controller.Clone() as Controller);
                }
            }

            return enemyTile;
        }
    }
}