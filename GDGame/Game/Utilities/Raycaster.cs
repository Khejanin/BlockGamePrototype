using System;
using System.Collections.Generic;
using GDGame.Game.Controllers;
using GDLibrary.Actors;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;

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

        #region Actor Specific Definitions

        public static HitResult Raycast(this DrawnActor3D callingDrawnActor3D, Vector3 position, Vector3 direction, bool ignoreSelf)
        {
            List<HitResult> all = RaycastAll(callingDrawnActor3D,position, direction,ignoreSelf);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }

        
        
        public static List<HitResult> RaycastAll(this DrawnActor3D callingDrawnActor3D,Vector3 position, Vector3 direction, bool ignoreSelf)
        {
            List<HitResult> hit = new List<HitResult>();
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D currentDrawnActor3D in ObjectManager.GetAllObjects())
            {
                float? dist;
                bool found = false;

                if(ignoreSelf && Equals(currentDrawnActor3D, callingDrawnActor3D)) continue;

                if (currentDrawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = currentDrawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                                                        as PrimitiveColliderController;

                    if (pcc != null && (dist = ray.Intersects(pcc.GetBounds())) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = currentDrawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }
                //Models don't work yet and aren't imporant, use the custom ones.
                /*else if (drawnActor3D is ModelObject)
                {
                    ModelColliderController mcc = drawnActor3D.ControllerList.Find(c => c is ModelColliderController) 
                                                        as ModelColliderController;

                    //Check if any part of the model is hit, if it is, the model is hit.
                    if (mcc != null && mcc.GetBounds().Find(sphere => (dist = ray.Intersects(sphere)) != null) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }*/

                if (!found)
                {
                    CustomBoxColliderController cbcc = currentDrawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                                                            as CustomBoxColliderController;

                    if (cbcc != null && (dist = ray.Intersects(cbcc.GetBounds())) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = currentDrawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                    }
                }
            }

            return hit;
        }

        #endregion

        #region Normal Definitions

        public static HitResult Raycast(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects)
        {
            List<HitResult> all = RaycastAll(position, direction, AllObjects);
            all.Sort();

            if (all.Count == 0)
                return null;

            return all[0];
        }
        
        public static List<HitResult> RaycastAll(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects)
        {
            List<HitResult> hit = new List<HitResult>();
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                float? dist;
                bool found = false;

                if (drawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                                                        as PrimitiveColliderController;

                    if (pcc != null && (dist = ray.Intersects(pcc.GetBounds())) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }
                //Models don't work yet and aren't imporant, use the custom ones.
                /*else if (drawnActor3D is ModelObject)
                {
                    ModelColliderController mcc = drawnActor3D.ControllerList.Find(c => c is ModelColliderController) 
                                                        as ModelColliderController;

                    //Check if any part of the model is hit, if it is, the model is hit.
                    if (mcc != null && mcc.GetBounds().Find(sphere => (dist = ray.Intersects(sphere)) != null) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }*/

                if (!found)
                {
                    CustomBoxColliderController cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                                                            as CustomBoxColliderController;

                    if (cbcc != null && (dist = ray.Intersects(cbcc.GetBounds())) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                    }
                }
            }

            return hit;
        }

        #endregion

        #region Pass by Reference Definitions

        public static void Raycast(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects, ref List<HitResult> hit)
        {
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                float? dist;
                bool found = false;
                
                if (drawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                                                        as PrimitiveColliderController;

                    if (pcc != null && (dist = ray.Intersects(pcc.GetBounds())) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }
                //Models don't work yet and aren't imporant, use the custom ones.
                /*else if (drawnActor3D is ModelObject)
                {
                    ModelColliderController mcc = drawnActor3D.ControllerList.Find(c => c is ModelColliderController) 
                                                        as ModelColliderController;

                    //Check if any part of the model is hit, if it is, the model is hit.
                    if (mcc != null && mcc.GetBounds().Find(sphere => (dist = ray.Intersects(sphere)) != null) != null)
                    {
                        HitResult result = new HitResult();
                        result.actor = drawnActor3D;
                        result.distance = (float)dist;
                        result.hitPosition = position + direction * result.distance;
                        hit.Add(result);
                        found = true;
                    }
                }*/

                if (!found)
                {
                    CustomBoxColliderController cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                                                            as CustomBoxColliderController;

                    if (cbcc != null && (dist = ray.Intersects(cbcc.GetBounds())) != null)
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
        
        
    }
}