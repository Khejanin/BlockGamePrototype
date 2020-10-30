using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class GridTile : ModelObject
    {
        public ETileType tileType;

        public GridTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {

        }

        public virtual void Initialize()
        {

        }

        public void SetPosition(Vector3 position)
        {
            transform3D.Translation = position;
        }
    }
}
