using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class Raycaster
    {
        public struct HitResult : IComparable<HitResult>
        {
            public float distance;
            public Vector3 hitPosition;
            public Actor3D actor;

            public int CompareTo(HitResult other)
            {
                return distance.CompareTo(other.distance);
            }
        }

        public static HitResult Raycast(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects)
        {
            List<HitResult> all = RaycastAll(position, direction, AllObjects);
            all.Sort();
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
    }
}