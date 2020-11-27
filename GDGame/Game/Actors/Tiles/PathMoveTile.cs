using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GDGame.Enums;

namespace GDGame.Actors
{
    public abstract class PathMoveTile : MovableTile
    {
        protected int pathDir = 1;

        public List<Vector3> path;
        public int currentPositionIndex;

        protected PathMoveTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType) : base(id, actorType,
            statusType, transform, effectParameters, model, tileType)
        {
            path = new List<Vector3>();
        }

        protected abstract void MoveToNextPoint();

        protected virtual void OnMoveEnd()
        {
            currentPositionIndex += pathDir;
        }

        protected Vector3 NextPathPoint()
        {
            if (currentPositionIndex + pathDir == path.Count || currentPositionIndex + pathDir == -1)
                pathDir *= -1;

            return path[currentPositionIndex + pathDir];
        }
    }
}