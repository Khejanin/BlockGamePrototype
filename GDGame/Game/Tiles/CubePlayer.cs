using System;
using GDGame.Game.Enums;
using GDGame.Game.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GDGame.Game.UI;
using GDLibrary.Actors;
using GDLibrary.Interfaces;
using static GDGame.Game.Utilities.Raycaster;

namespace GDGame.Game.Tiles
{
    public class CubePlayer : GridTile, ICloneable
    {
        private float movementTime = .3f;
        private float currentMovementTime;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRotQ;
        private Quaternion endRotQ;
        private bool isMoving;
        private bool isAttached;

        private Vector3 leftRotatePoint;
        private Vector3 rightRotatePoint;
        private Vector3 forwardRotatePoint;
        private Vector3 backwardRotatePoint;
        
        private Text2D text2D;
        private SpriteFont font;

        List<AttachableTile> attachedTiles;
        List<Shape> attachCandidates;
        
        private Curve1D curve1D;

        public List<AttachableTile> AttachedTiles => attachedTiles;
        public bool IsMoving => isMoving;
        public bool IsAttached => isAttached;

        struct PlayerSurroundCheck
        {
            public EDirection dir;
            public HitResult hit;
        }

        public CubePlayer(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, SpriteFont font)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            this.font = font;
            attachedTiles = new List<AttachableTile>();
            attachCandidates = new List<Shape>();
            leftRotatePoint = rightRotatePoint = forwardRotatePoint = backwardRotatePoint = transform.Translation;
            
            curve1D = new Curve1D(CurveLoopType.Cycle);
            curve1D.Add(1,0);
            curve1D.Add(0,movementTime);
        }

        public void Attach()
        {
            if (attachCandidates.Count == 0) return;

            foreach (AttachableTile tile in attachCandidates.SelectMany(shape => shape.AttachableTiles))
            {
                attachedTiles.Add(tile);
                tile.EffectParameters.DiffuseColor = Color.Green;
            }

            isAttached = true;
        }

        public void Detach()
        {
            foreach (AttachableTile tile in attachedTiles)
            {
                tile.EffectParameters.DiffuseColor = Color.White;
            }
            attachedTiles.Clear();
            isAttached = false;
        }
        
        private Vector3 diff;

        /// <summary>
        /// This method will change the Player's state to moving if he's not already moving and calculates how the player will move.
        /// </summary>
        /// <param name="direction"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Move(Vector3 direction)
        {
            if (!isMoving)
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


                Vector3 offset = Transform3D.Translation - rotatePoint; //offset between the player and the point to rotate around
                Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Cross(direction, Vector3.Up), MathHelper.ToRadians(-90));   //The rotation to apply
                Vector3 translation = Vector3.Transform(offset, rot);   //Rotate around the offset point

                //Start and End Rotation --> Will be lerped between
                startRotQ = Transform3D.Rotation;
                endRotQ = rot * startRotQ;

                //Start and End Position --> Will be lerped between
                startPos = Transform3D.Translation;
                endPos = Transform3D.Translation + translation - offset;
                diff = endPos - startPos;

                if (IsMoveValid(rot, rotatePoint, endPos))
                {
                    //Calculate movement for each attached tile
                    foreach (AttachableTile tile in attachedTiles)
                        tile.Move(direction, rotatePoint);

                    //Set animation time and movement flag
                    currentMovementTime = movementTime;
                    isMoving = true;
                }
            }
        }

        private bool IsMoveValid(Quaternion rotationToApply, Vector3 rotatePoint, Vector3 playerTargetPos)
        {
            List<Vector3> initials = attachedTiles.Select(i => i.Transform3D.Translation).ToList();
            initials.Insert(0, Transform3D.Translation);
            List<Vector3> ends = attachedTiles.Select(i => i.CalculateTargetPosition(rotatePoint, rotationToApply)).ToList();
            ends.Insert(0, playerTargetPos);
            List<HitResult> results = new List<HitResult>();
            List<FloorHitResult> floorHitResults = new List<FloorHitResult>();
            Raycaster.PlayerCastAll(this, initials, ends,ref results,ref floorHitResults);
            return results.Count == 0 && floorHitResults.Count > 0;
        }
        

        /// <summary>
        /// Is in charge of the Animation for when the Player Moves
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isMoving)
            {
                if (currentMovementTime <= 0)
                {
                    isMoving = false;
                    currentMovementTime = 0;
                    UpdateAttachCandidates(); //remove this later
                }

                Quaternion rot = Quaternion.Slerp(startRotQ, endRotQ, 1 - currentMovementTime / movementTime);
                float currentStep = curve1D.Evaluate(currentMovementTime*1000, 5);
                Vector3 trans = startPos + diff * currentStep;
                //Vector3 trans2 = Vector3.Lerp(startPos, endPos, 1 - currentMovementTime / movementTime);
                Transform3D.Rotation = rot;
                Transform3D.Translation = trans;

                currentMovementTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            text2D = attachCandidates.Count > 0 ? new Text2D("Hold Space to attach", font) : null;
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            base.Draw(gameTime, camera, graphicsDevice);
            text2D?.Draw(gameTime, graphicsDevice);
        }

        public void UpdateRotatePoints()
        {
            //Set all rotation points to the edges of the player cube
            rightRotatePoint = Transform3D.Translation + new Vector3(.5f, -.5f, 0);
            leftRotatePoint = Transform3D.Translation + new Vector3(-.5f, -.5f, 0);
            forwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, -.5f);
            backwardRotatePoint = Transform3D.Translation + new Vector3(0, -.5f, .5f);

            //Loops through attached tiles to update the rotation points
            foreach (AttachableTile tile in attachedTiles)
            {
                Vector3 playerPos = Transform3D.Translation;
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

        public void UpdateAttachCandidates()
        {
            attachCandidates.Clear();

            foreach (PlayerSurroundCheck check in CheckSurroundings())
                if (check.hit?.actor is AttachableTile tile)
                    attachCandidates.Add(tile.Shape);
        }

        private List<PlayerSurroundCheck> CheckSurroundings()
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();

            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Right, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Right, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Forward, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Up, true, 1f);
            result.Add(surroundCheck);

            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Up, true, 1f);
            result.Add(surroundCheck);

            return result;
        }

        public new object Clone()
        {
            CubePlayer cubePlayer = new CubePlayer("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, font);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    cubePlayer.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return cubePlayer;
        }
    }
}
