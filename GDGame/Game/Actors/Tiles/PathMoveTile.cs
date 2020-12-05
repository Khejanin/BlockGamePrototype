using System.Collections.Generic;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public abstract class PathMoveTile : MovableTile
    {
        #region Public variables

        public int currentPositionIndex;

        public List<Vector3> path;

        #endregion

        #region Private variables

        protected int pathDir = 1;

        #endregion

        #region Constructors

        protected PathMoveTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, OurEffectParameters effectParameters, Model model,
            bool isBlocking,ETileType tileType) : base(id, actorType,
            statusType, transform, effectParameters, model,isBlocking, tileType)
        {
            path = new List<Vector3>();
        }

        #endregion

        #region Methods

        protected Vector3 NextPathPoint()
        {
            if (currentPositionIndex + pathDir == path.Count || currentPositionIndex + pathDir == -1)
                pathDir *= -1;

            return path[currentPositionIndex + pathDir];
        }

        #endregion

        #region Events

        protected virtual void OnMoveEnd()
        {
            currentPositionIndex += pathDir;
        }

        #endregion
    }
}