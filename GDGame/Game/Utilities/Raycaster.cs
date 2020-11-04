using System;
using System.Collections.Generic;
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
        public class HitResult : IComparable<HitResult>
        {
            public float distance;
            public Vector3 hitPosition;
            public Actor3D actor;

            public int CompareTo(HitResult other)
            {
                return distance.CompareTo(other.distance);
            }
        }
        
        #region Pass by Reference Definitions

        public static void Raycast(Vector3 position, Vector3 direction, ref List<HitResult> hit, float maxDist = float.MaxValue, List<Actor3D> ignoreList = null)
        {
            if(maxDist <= 0) throw new ArgumentException("You can't set a max cast distance to zero or negative!");
            
            List<DrawnActor3D> AllObjects = ObjectManager.GetAllObjects();
            if(ignoreList != null) AllObjects.RemoveAll(actor3D => ignoreList.Contains(actor3D));
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                float? dist = -1f;

                if(drawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                        as PrimitiveColliderController;
                    CustomBoxColliderController cbcc = null;
                    if(pcc == null)  
                        cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                            as CustomBoxColliderController;

                    bool pccCheck = pcc != null && (dist = ray.Intersects(pcc.GetBounds())) != null;
                    bool cbccCheck = cbcc != null && (dist = ray.Intersects(cbcc.GetBounds())) != null;
                    
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
        }

        #endregion
        
        #region Normal Definitions

        public static HitResult Raycast(Vector3 position, Vector3 direction,float maxDist = float.MaxValue,List<Actor3D> ignoreList = null)
        {
            List<HitResult> all = RaycastAll(position, direction, maxDist,ignoreList);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }
        
        public static List<HitResult> RaycastAll(Vector3 position, Vector3 direction,float maxDist = float.MaxValue,List<Actor3D> ignoreList = null)
        {
            List<HitResult> result = new List<HitResult>();
            Raycast(position, direction, ref result,maxDist,ignoreList);
            return result;
        }

        #endregion

        #region Actor Specific Definitions

        public static List<HitResult> PlayerCastAll(this CubePlayer player,List<Vector3> initialPositions, List<Vector3> endPositions)
        {
            List<HitResult> hit = new List<HitResult>();

            List<Actor3D> ignore = new List<Actor3D>();
            ignore.AddRange(player.AttachedTiles);
            ignore.Add(player);

            for (int i = 0; i < initialPositions.Count; i++)
            {
                Vector3 maxDist = endPositions[i] - initialPositions[i];
                Vector3 dir = Vector3.Normalize(maxDist);
                hit.AddRange(RaycastAll(initialPositions[i],dir,maxDist.Length(),ignore));
            }
            
            return hit;
        }

        public static HitResult Raycast(this DrawnActor3D callingDrawnActor3D, Vector3 position, Vector3 direction, bool ignoreSelf,float maxDist = Single.MaxValue)
        {
            List<HitResult> all = RaycastAll(callingDrawnActor3D,position, direction,ignoreSelf,maxDist);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }

        
        
        public static List<HitResult> RaycastAll(this DrawnActor3D callingDrawnActor3D,Vector3 position, Vector3 direction, bool ignoreSelf,float maxDist = Single.MaxValue)
        {
            List<Actor3D> ignoreList = new List<Actor3D>();
            if(ignoreSelf) ignoreList.Add(callingDrawnActor3D);

            return RaycastAll(position, direction, maxDist,ignoreList);
        }

        #endregion

    }
}