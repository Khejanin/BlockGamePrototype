using System;
using System.Collections.Generic;
using GDGame.Actors;
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
    public class PathMovementComponent : ActivatableController, IActivatable, ICloneable
    {
        private int currentPositionIndex;
        protected int movementTime;
        protected float timePercent;
        protected Smoother.SmoothingMethod smoothingMethod;
        private int pathDir = 1;
        private PathMoveTile pathMoveTileParent;

        private List<Vector3> Path
        {
            get
            {
                pathMoveTileParent ??= parent as PathMoveTile;
                return pathMoveTileParent?.Path;
            }
        }


        public PathMovementComponent(string id, ControllerType controllerType, ActivationType activationType,
            float timePercent, Smoother.SmoothingMethod smoothingMethod) : base(id, controllerType, activationType)
        {
            this.timePercent = timePercent;
            movementTime = (int) (Constants.GameConstants.MovementCooldown * timePercent * 1000);
            this.smoothingMethod = smoothingMethod;
        }

        #region Events

        protected override void OnClone()
        {
            base.OnClone();
            EventManager.RegisterListener<MovingTilesEventInfo>(OnMovingTileEvent);
        }
        
        private void OnMovingTileEvent(MovingTilesEventInfo obj)
        {
            if (active)
            {
                MoveToNextPoint();
            }
        }

        #endregion

        #region Pathing

        private Vector3 NextPathPoint()
        {
            if (currentPositionIndex + pathDir == Path.Count || currentPositionIndex + pathDir == -1)
                pathDir *= -1;

            return Path[currentPositionIndex += pathDir];
        }

        //There is a lot of magic in here
        protected virtual void MoveToNextPoint()
        {
            Vector3 next = NextPathPoint();
            parent.MoveTo(new AnimationEventData()
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

        #endregion

        #region Activation

        public new void Activate()
        {
            active = true;
        }

        protected override void OnActivated()
        {
        }

        public new void Deactivate()
        {
            active = false;
        }

        protected override void OnDeactivated()
        {
        }

        public new void ToggleActivation()
        {
            active = !active;
        }

        #endregion
        
        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= (Tile) actor;
        }

        public new object Clone()
        {
            PathMovementComponent pathMovementComponent = new PathMovementComponent(ID + " - clone", ControllerType, activationType,timePercent,smoothingMethod);
            pathMovementComponent.OnClone();
            return pathMovementComponent;
        }
    }
}