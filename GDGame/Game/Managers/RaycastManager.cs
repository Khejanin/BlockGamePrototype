using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Utilities;
using GDLibrary.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    /// <summary>
    ///     Proxy that handles the Raycaster Calls for you so that you dont have to worry about the ObjectManager.
    ///     This class is only a shadow of its former glory as we only use Raycasts in one place in the Game now, the rest is
    ///     handled through Collisions.
    /// </summary>
    public class RaycastManager
    {
        #region Static Fields and Constants

        private static RaycastManager _raycastManagerInstance;

        #endregion

        #region Constructors

        private RaycastManager()
        {
        }

        #endregion

        #region Properties, Indexers

        public static RaycastManager Instance =>
            _raycastManagerInstance ?? (_raycastManagerInstance = new RaycastManager());

        public OurObjectManager ObjectManager { get; set; }

        #endregion

        #region Public Method

        public Raycaster.HitResult Raycast(Actor3D actor3D, Vector3 position, Vector3 direction, bool ignoreSelf,
            float maxDistance, bool onlyCheckBlocking = true)
        {
            return actor3D.Raycast(ObjectManager, position, direction, ignoreSelf, maxDistance, onlyCheckBlocking);
        }

        public void RaycastAll(PlayerTile playerTile, Vector3 offset, List<Vector3> initialPositions,
            List<Vector3> endPositions,
            ref List<Raycaster.HitResult> blockingObjectsResult,
            ref List<Raycaster.FloorHitResult> floorResult)
        {
            playerTile.PlayerCastAll(ObjectManager, offset, initialPositions, endPositions, ref blockingObjectsResult,
                ref floorResult);
        }

        #endregion
    }
}