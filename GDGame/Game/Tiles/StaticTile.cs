using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class StaticTile : GridTile
    {
        public StaticTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void TranslateBy(Vector3 translation)
        {
            Transform3D.TranslateBy(translation);
        }
    }
}