using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class EnemyTile : PathMoveTile
    {
        #region Constructors

        public EnemyTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking, ETileType tileType) : base(id,
            actorType, statusType, transform, effectParameters, model, isBlocking, tileType)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            EnemyTile enemyTile = new EnemyTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking,
                TileType);
            enemyTile.ControllerList.AddRange(GetControllerListClone());

            return enemyTile;
        }

        #endregion
    }
}