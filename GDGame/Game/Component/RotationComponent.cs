using System;
using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    public class RotationComponent : Controller, ICloneable
    {
        #region Private variables

        private Vector3 backwardRotatePoint;
        private Vector3 forwardRotatePoint;
        private Vector3 leftRotatePoint;
        private Vector3 rightRotatePoint;

        #endregion

        #region Constructors

        public RotationComponent(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new RotationComponent(ID, ControllerType);
        }

        public void SetRotatePoint(Vector3 direction, MovableTile actor)
        {
            MovableTile movableTile = actor;
            UpdateRotatePoints(movableTile);

            Vector3 rotatePoint;

            if (direction == Vector3.UnitX)
                rotatePoint = rightRotatePoint;
            else if (direction == -Vector3.UnitX)
                rotatePoint = leftRotatePoint;
            else if (direction == -Vector3.UnitZ)
                rotatePoint = forwardRotatePoint;
            else if (direction == Vector3.UnitZ)
                rotatePoint = backwardRotatePoint;
            else
                throw new ArgumentException("Invalid direction!");

            movableTile.RotatePoint = rotatePoint;

            if (!(movableTile is PlayerTile player)) return;
            foreach (AttachableTile tile in player.AttachedTiles) tile.RotatePoint = rotatePoint;
        }

        private void UpdateRotatePoints(Actor3D movableTile)
        {
            //Set all rotation points to the edges of the player cube
            rightRotatePoint = movableTile.Transform3D.Translation + new Vector3(.5f, -.5f, 0);
            leftRotatePoint = movableTile.Transform3D.Translation + new Vector3(-.5f, -.5f, 0);
            forwardRotatePoint = movableTile.Transform3D.Translation + new Vector3(0, -.5f, -.5f);
            backwardRotatePoint = movableTile.Transform3D.Translation + new Vector3(0, -.5f, .5f);

            if (!(movableTile is PlayerTile player)) return;

            //Loops through attached tiles to update the rotation points
            foreach (AttachableTile tile in player.AttachedTiles)
            {
                Vector3 playerPos = player.Transform3D.Translation;
                Vector3 tilePos = tile.Transform3D.Translation;

                //Update right rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X > rightRotatePoint.X || tilePos.Y < rightRotatePoint.Y)
                    //if(tile.Raycast(tilePos, Vector3.Right, true, 1f) == null)
                    rightRotatePoint = tilePos + new Vector3(.5f, -.5f, 0);

                //Update left rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X < leftRotatePoint.X || tilePos.Y < leftRotatePoint.Y)
                    //if(tile.Raycast(tilePos, Vector3.Left, true, 1f) == null)
                    leftRotatePoint = tilePos + new Vector3(-.5f, -.5f, 0);

                //Update forward rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z < forwardRotatePoint.Z || tilePos.Y < forwardRotatePoint.Y)
                    //if(tile.Raycast(tilePos, Vector3.Forward, true, 1f) == null)
                    forwardRotatePoint = tilePos + new Vector3(0, -.5f, -.5f);

                //Update back rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z > backwardRotatePoint.Z || tilePos.Y < backwardRotatePoint.Y)
                    //if(tile.Raycast(tilePos, Vector3.Backward, true, 1f) == null)
                    backwardRotatePoint = tilePos + new Vector3(0, -.5f, .5f);
            }
        }

        #endregion
    }
}