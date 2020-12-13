using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Constants;
using GDGame.Controllers;
using GDGame.EventSystem;
using GDGame.Interfaces;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    /// <summary>
    ///     Component that needs to be added to a PathMoveTile. It inherits from ActivatableController so can be activated in a
    ///     multitude of ways.
    ///     It uses the TransformAnimationManager to move the tile's transform.
    /// </summary>
    public class PathMovementComponent : ActivatableController, IActivatable, ICloneable
    {
        #region Private variables

        protected int currentPositionIndex;
        protected int movementTime;
        private int pathDir = 1;
        private PathMoveTile pathMoveTileParent;
        protected Smoother.SmoothingMethod smoothingMethod;
        protected float timePercent;

        #endregion

        #region Constructors

        public PathMovementComponent(string id, ControllerType controllerType, ActivationType activationType,
            float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType)
        {
            this.timePercent = timePercent;
            movementTime = (int) (GameConstants.MovementCooldown * timePercent * 1000);
            this.smoothingMethod = smoothingMethod;
        }

        #endregion

        #region Properties, Indexers

        private List<Vector3> Path
        {
            get
            {
                pathMoveTileParent ??= parent as PathMoveTile;
                return pathMoveTileParent?.Path;
            }
        }

        #endregion

        #region Override Method

        protected override void OnActivated()
        {
        }

        protected override void OnClone()
        {
            base.OnClone();
            EventManager.RegisterListener<MovingTilesEventInfo>(OnMovingTileEvent);
        }


        protected override void OnDeactivated()
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (Tile) actor;
        }

        #endregion

        #region Public Method

        public new object Clone()
        {
            PathMovementComponent pathMovementComponent = new PathMovementComponent(ID + " - clone", ControllerType,
                activationType, timePercent, smoothingMethod);
            pathMovementComponent.OnClone();
            return pathMovementComponent;
        }

        public new void ToggleActivation()
        {
            active = !active;
        }

        #endregion

        #region Private Method

        //There is a lot of magic in here
        protected virtual void MoveToNextPoint()
        {
            Vector3 next = NextPathPoint();
            parent.MoveTo(new AnimationEventData
            {
                isRelative = false, destination = next,
                maxTime = movementTime,
                smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PlayOnce,
                body = parent.Body
            });

            //@TODO Fix the look rotation
            /*
            Vector3 newLook = parent.Transform3D.Translation - next;
            double angle = Math.Acos((Vector3.Dot(newLook, parent.Transform3D.Look) / newLook.Length() * parent.Transform3D.Look.Length()));
            float floatAngle = MathHelper.ToDegrees((float) angle);
            floatAngle -= parent.Transform3D.RotationInDegrees.Y;
            if (floatAngle != 0)
            {
                parent.RotateTo(true,Vector3.Up*(floatAngle),movementTime,Smoother.SmoothingMethod.Smooth);
            }
            */
        }

        //Fetch the next point and go back if you've reached the end.
        protected Vector3 NextPathPoint()
        {
            if (currentPositionIndex + pathDir == Path.Count || currentPositionIndex + pathDir == -1)
                pathDir *= -1;

            return Path[currentPositionIndex += pathDir];
        }

        #endregion

        #region Events

        private void OnMovingTileEvent(MovingTilesEventInfo obj)
        {
            if (active) MoveToNextPoint();
        }

        #endregion
    }
}