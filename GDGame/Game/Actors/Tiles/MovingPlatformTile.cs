using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Interfaces;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     The moving platform is the Tile representing the platform movement
    /// </summary>
    public class MovingPlatformTile : PathMoveTile, IActivatable
    {
        #region Constructors

        public MovingPlatformTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking, ETileType tileType) : base(id,
            actorType, statusType,
            transform, effectParameters, model, isBlocking, tileType)
        {
        }

        #endregion

        #region Public Method

        public void Activate()
        {
        }

        public new object Clone()
        {
            MovingPlatformTile platform = new MovingPlatformTile("clone - " + ID, ActorType, StatusType,
                Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking, TileType);

            platform.ControllerList.AddRange(GetControllerListClone());

            return platform;
        }

        public void Deactivate()
        {
        }

        public void ToggleActivation()
        {
        }

        #endregion
    }
}