using System;
using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Game.Controllers;
using GDGame.Game.Tiles;
using GDLibrary.Actors;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1;

namespace GDGame.Game.Utilities
{
    public static class Raycaster
    {

        public struct FloorHitResult
        {
            public HitResult hitResult;
            public Actor3D actor3D;
        }
        #region Public Methods

        public static void PlayerCastAll(this CubePlayer player,Vector3 offset,List<Vector3> initialPositions, List<Vector3> endPositions,ref List<HitResult> blockingObjectsResult,ref List<FloorHitResult> floorResult)
        {
            List<Actor3D> ignore = new List<Actor3D>();
            ignore.AddRange(player.AttachedTiles);
            ignore.Add(player);

            offset = new Vector3(offset.X % 1f,offset.Y % 1f,offset.Z % 1f) * 0.9f;
            
            for (int i = 0; i < initialPositions.Count; i++)
            {
                //Check if this block's trajectory is blocked by anything in its path
                Vector3 maxDist = (endPositions[i]) - (initialPositions[i]);
                Vector3 dir = Vector3.Normalize(maxDist);
                blockingObjectsResult.AddRange(RaycastAll(initialPositions[i] + offset,dir,maxDist.Length(),ignore));

                //If there's anything directly above this block and the block moves in Y, it's an illegal move
                if(dir.Y > 0)
                    blockingObjectsResult.AddRange(RaycastAll(initialPositions[i],Vector3.Up,1f,ignore));
                
                //Check if this block will be on a floor tile after moving
                HitResult hit = Raycast(endPositions[i], Vector3.Down, 1f, ignore);
                if(hit != null)
                    floorResult.Add(new FloorHitResult(){hitResult = hit,actor3D = ignore[i]});
            }
        }

        public static void Raycast(Vector3 position, Vector3 direction, ref List<HitResult> hit, float maxDist = float.MaxValue,List<Actor3D> ignoreList = null, bool onlyCheckBlocking = true)
        {
            if(maxDist <= 0) throw new ArgumentException("You can't set a max cast distance to zero or negative!");
            
            List<DrawnActor3D> AllObjects = ObjectManager.GetAllObjects();
            if(ignoreList != null) AllObjects.RemoveAll(actor3D => ignoreList.Contains(actor3D));
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                float? dist = -1f;

                PrimitiveColliderController pcc =
                    drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController)
                        as PrimitiveColliderController;
                    
                CustomBoxColliderController cbcc = null;
                    if(pcc == null)
                        cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                            as CustomBoxColliderController;

                //@TODO Refactor this so it uses a bitmask corresponding to the ColliderType enum for checks

                bool pccCheck = pcc != null && (dist = ray.Intersects(pcc.GetBounds())) != null && (pcc.ColliderType == ColliderType.Blocking || !onlyCheckBlocking);
                bool cbccCheck = cbcc != null && (dist = ray.Intersects(cbcc.GetBounds())) != null && (cbcc.ColliderType == ColliderType.Blocking || !onlyCheckBlocking);

                if ((pccCheck || cbccCheck) && dist < maxDist)
                { 
                    HitResult result = new HitResult(); 
                    result.actor = drawnActor3D; 
                    result.distance = (float)dist; 
                    result.hitPosition = position + direction * result.distance; 
                    hit.Add(result);
                }
            }
        }

        public static HitResult Raycast(Vector3 position, Vector3 direction,float maxDist = float.MaxValue,List<Actor3D> ignoreList = null, bool onlyCheckBlocking = true)
        {
            List<HitResult> all = RaycastAll(position, direction, maxDist,ignoreList,onlyCheckBlocking);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }

        public static HitResult Raycast(this DrawnActor3D callingDrawnActor3D, Vector3 position, Vector3 direction, bool ignoreSelf,float maxDist = Single.MaxValue, bool onlyCheckBlocking = true)
        {
            List<HitResult> all = RaycastAll(callingDrawnActor3D,position, direction,ignoreSelf,maxDist,onlyCheckBlocking);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }

        public static List<HitResult> RaycastAll(Vector3 position, Vector3 direction,float maxDist = float.MaxValue,List<Actor3D> ignoreList = null, bool onlyCheckBlocking = true)
        {
            List<HitResult> result = new List<HitResult>();
            Raycast(position, direction, ref result,maxDist,ignoreList,onlyCheckBlocking);
            return result;
        }


        public static List<HitResult> RaycastAll(this DrawnActor3D callingDrawnActor3D,Vector3 position, Vector3 direction, bool ignoreSelf,float maxDist = Single.MaxValue, bool onlyCheckBlocking = true)
        {
            List<Actor3D> ignoreList = new List<Actor3D>();
            if(ignoreSelf) ignoreList.Add(callingDrawnActor3D);

            return RaycastAll(position, direction, maxDist,ignoreList,onlyCheckBlocking);
        }

        #endregion

        public class HitResult : IComparable<HitResult>
        {
            #region Public variables

            public Actor3D actor;
            public ColliderController collider;
            public float distance;
            public Vector3 hitPosition;

            #endregion

            #region Public Methods

            public int CompareTo(HitResult other)
            {
                return distance.CompareTo(other.distance);
            }

            #endregion
        }
    }
}