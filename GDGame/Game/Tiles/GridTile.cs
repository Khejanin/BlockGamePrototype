using GDGame.Game.Controllers;
using GDGame.Game.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class GridTile : ModelObject
    {
        private ETileType tileType;
        private Shape shape;
        private bool canMoveInto;

        public bool CanMoveInto => canMoveInto;

        public Shape Shape { get; set; }
        public ETileType TileType { get; set; }

        public GridTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            ControllerList.Add(new CustomBoxColliderController(ColliderType.Cube, 1f));
        }

        public virtual void SetPosition(Vector3 position)
        {
            Transform3D.Translation = position;
        }
    }
}