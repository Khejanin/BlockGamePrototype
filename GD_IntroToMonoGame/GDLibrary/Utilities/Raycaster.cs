using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class Raycaster
    {
        public static List<Actor3D> Raycast(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects)
        {
            List<Actor3D> hit = new List<Actor3D>();
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                bool found = false;
                if (drawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                                                        as PrimitiveColliderController;

                    if (pcc != null && ray.Intersects(pcc.GetBounds()) != null)
                    {
                        hit.Add(drawnActor3D);
                        found = true;
                    }
                }
                else if (drawnActor3D is ModelObject)
                {
                    ModelColliderController mcc = drawnActor3D.ControllerList.Find(c => c is ModelColliderController) 
                                                        as ModelColliderController;
                    //Check if any part of the model is hit, if it is, the model is hit.
                    if (mcc != null && mcc.GetBounds().Find(sphere => ray.Intersects(sphere) != null) != null)
                    {
                        hit.Add(drawnActor3D);
                        found = true;
                    }
                }

                if (!found)
                {
                    CustomBoxColliderController cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                                                            as CustomBoxColliderController;

                    if (cbcc != null && ray.Intersects(cbcc.GetBounds()) != null) hit.Add(drawnActor3D);
                }
            }

            return hit;
        }
        
        public static void Raycast(Vector3 position, Vector3 direction, List<DrawnActor3D> AllObjects, ref List<DrawnActor3D> hit)
        {
            Ray ray = new Ray(position, direction);
            foreach (DrawnActor3D drawnActor3D in AllObjects)
            {
                bool found = false;
                if (drawnActor3D is PrimitiveObject)
                {
                    PrimitiveColliderController pcc = drawnActor3D.ControllerList.Find(c => c is PrimitiveColliderController) 
                        as PrimitiveColliderController;

                    if (pcc != null && ray.Intersects(pcc.GetBounds()) != null)
                    {
                        hit.Add(drawnActor3D);
                        found = true;
                    }
                }
                else if (drawnActor3D is ModelObject)
                {
                    ModelColliderController mcc = drawnActor3D.ControllerList.Find(c => c is ModelColliderController) 
                        as ModelColliderController;
                    //Check if any part of the model is hit, if it is, the model is hit.
                    if (mcc != null && mcc.GetBounds().Find(sphere => ray.Intersects(sphere) != null) != null)
                    {
                        hit.Add(drawnActor3D);
                        found = true;
                    }
                }

                if (!found)
                {
                    CustomBoxColliderController cbcc = drawnActor3D.ControllerList.Find(c => c is CustomBoxColliderController) 
                        as CustomBoxColliderController;

                    if (cbcc != null && ray.Intersects(cbcc.GetBounds()) != null) hit.Add(drawnActor3D);
                }
            }
        }
    }
}