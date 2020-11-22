using GDGame.Actors;
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
        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public void OnMoveEnd()
        {
            Raycaster.HitResult hit = this.Raycast(Transform3D.Translation, Vector3.Down, true, 0.5f,false);
            if(hit?.actor is SpikeTile)
                System.Diagnostics.Debug.WriteLine(ID + " is ded!");
        }

        public new object Clone()
        {
            AttachableTile enemyTile = new AttachableTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);

            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    enemyTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return enemyTile;
        }
    }
}
