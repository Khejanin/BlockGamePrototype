﻿using GDGame.Game.Tiles;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Game.Controllers
{
    public class RotationComponent : IController
    {
        private CubePlayer parent;
        private Vector3 rightRotatePoint;
        private Vector3 leftRotatePoint;
        private Vector3 forwardRotatePoint;
        private Vector3 backwardRotatePoint;

        public object Clone()
        {
            return new RotationComponent();
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as CubePlayer;
        }

        public ControllerType GetControllerType()
        {
            throw new System.NotImplementedException();
        }

        public void SetRotatePoint(Vector3 direction)
        {
            UpdateRotatePoints();

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
                throw new System.ArgumentException("Invalid direction!");

            parent.RotatePoint = rotatePoint;
            foreach (AttachableTile tile in parent.AttachedTiles)
            {
                tile.RotatePoint = rotatePoint;
            }
        }

        public void UpdateRotatePoints()
        {
            //Set all rotation points to the edges of the player cube
            rightRotatePoint = parent.Transform3D.Translation + new Vector3(.5f, -.5f, 0);
            leftRotatePoint = parent.Transform3D.Translation + new Vector3(-.5f, -.5f, 0);
            forwardRotatePoint = parent.Transform3D.Translation + new Vector3(0, -.5f, -.5f);
            backwardRotatePoint = parent.Transform3D.Translation + new Vector3(0, -.5f, .5f);

            //Loops through attached tiles to update the rotation points
            foreach (AttachableTile tile in parent.AttachedTiles)
            {
                Vector3 playerPos = parent.Transform3D.Translation;
                Vector3 tilePos = tile.Transform3D.Translation;

                //Update right rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X > rightRotatePoint.X || tilePos.Y < rightRotatePoint.Y)
                    rightRotatePoint = tilePos + new Vector3(.5f, -.5f, 0);

                //Update left rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.X < leftRotatePoint.X || tilePos.Y < leftRotatePoint.Y)
                    leftRotatePoint = tilePos + new Vector3(-.5f, -.5f, 0);

                //Update forward rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z < forwardRotatePoint.Z || tilePos.Y < forwardRotatePoint.Y)
                    forwardRotatePoint = tilePos + new Vector3(0, -.5f, -.5f);

                //Update back rotate point
                if (tilePos.Y <= playerPos.Y && tilePos.Z > backwardRotatePoint.Z || tilePos.Y < backwardRotatePoint.Y)
                    backwardRotatePoint = tilePos + new Vector3(0, -.5f, .5f);
            }
        }
    }
}