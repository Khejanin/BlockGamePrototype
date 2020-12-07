using System.Collections.Generic;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    /// The PathMoveTile is an extension from the MovableTile. A PathMoveTile has a predefined path on which it moves.
    /// The PathMoveTile actually doesn't move itself, it merely defines the Path. The PathMovementComponent makes sure that it moves.
    /// </summary>
    public class PathMoveTile : MovableTile
    {
        #region Constructors

        public PathMoveTile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking, ETileType tileType) : base(id,
            actorType, statusType, transform, effectParameters, model, isBlocking, tileType)
        {
            Path = new List<Vector3>();
        }

        #endregion

        #region Properties, Indexers

        public List<Vector3> Path { get; }

        #endregion

        public new object Clone()
        {
            PathMoveTile pathMoveTile = new PathMoveTile(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking, TileType);
            
            pathMoveTile.ControllerList.AddRange(GetControllerListClone());
            return pathMoveTile;
        }
    }
}