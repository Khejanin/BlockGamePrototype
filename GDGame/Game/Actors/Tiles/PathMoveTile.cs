using GDGame.Component;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDGame.Actors
{
    public abstract class PathMoveTile : MovableTile
    {
        protected TileMovementComponent movementComponent;
        protected int pathDir = 1;

        public List<Vector3> path;
        public int currentPositionIndex;

        public PathMoveTile(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model) : base(id, actorType, statusType, transform, effectParameters, model)
        {
            path = new List<Vector3>();
        }

        public override void InitializeTile()
        {
            movementComponent = (TileMovementComponent)ControllerList.Find(controller => controller is TileMovementComponent);
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
