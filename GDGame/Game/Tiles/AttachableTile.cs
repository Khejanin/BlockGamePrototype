using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GD_Library
{
    public class AttachableTile : GridTile
    {
        public AttachableTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public void Move(Vector3 direction, Vector3 rotatePoint)
        {

        }
    }
}
