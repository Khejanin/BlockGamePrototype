using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class CubePlayer : GridTile
    {

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Move(Vector3 direction)
        {
            transform3D.Translation += direction;
        }
    }
}
