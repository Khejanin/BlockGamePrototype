﻿using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Managers;
using GDLibrary.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    /// <summary>
    ///     Class that performs Raycasts using the JigLib Library. It has a lot of definitions depending on the users choice.
    /// </summary>
    public static class Raycaster
    {
        #region Public Method

        public static void PlayerCastAll(this PlayerTile player, OurObjectManager objectManager, Vector3 offset,
            List<Vector3> initialPositions, List<Vector3> endPositions,
            ref List<HitResult> blockingObjectsResult, ref List<FloorHitResult> floorResult)
        {
            List<Actor3D> ignore = new List<Actor3D>();
            ignore.AddRange(player.AttachedTiles);
            ignore.Add(player);

            offset = new Vector3(offset.X % 1f, offset.Y % 1f, offset.Z % 1f) * 0.9f;

            for (int i = 0; i < initialPositions.Count; i++)
            {
                //Check if this block's trajectory is blocked by anything in its path
                Vector3 maxDist = endPositions[i] - initialPositions[i];
                Vector3 dir = Vector3.Normalize(maxDist);
                blockingObjectsResult.AddRange(RaycastAll(objectManager, initialPositions[i] + offset, dir,
                    maxDist.Length() + 0.1f, ignore));

                //If there's anything directly above this block and the block moves in Y, it's an illegal move
                if (dir.Y > 0)
                    blockingObjectsResult.AddRange(RaycastAll(objectManager, initialPositions[i], Vector3.Up, 1f,
                        ignore));

                //Check if this block will be on a floor tile after moving
                HitResult hit = Raycast(objectManager, endPositions[i], Vector3.Down, 1f, ignore);
                if (hit != null)
                    floorResult.Add(new FloorHitResult {hitResult = hit, actor3D = ignore[i]});
            }
        }

        public static HitResult Raycast(this Actor3D callingDrawnActor3D, OurObjectManager objectManager,
            Vector3 position, Vector3 direction, bool ignoreSelf,
            float maxDist = float.MaxValue, bool onlyCheckBlocking = true)
        {
            List<HitResult> all = RaycastAll(callingDrawnActor3D, objectManager, position, direction, ignoreSelf,
                maxDist, onlyCheckBlocking);
            all.Sort();

            return all.Count == 0 ? null : all[0];
        }

        #endregion

        #region Private Method

        private static void Raycast(OurObjectManager objectManager, Vector3 position, Vector3 direction,
            ref List<HitResult> hit, float maxDist = float.MaxValue,
            ICollection<Actor3D> ignoreList = null, bool onlyCheckBlocking = true)
        {
            if (maxDist <= 0) throw new ArgumentException("You can't set a max cast distance to zero or negative!");


            List<Actor3D> allObjects = new List<Actor3D>();
            allObjects.AddRange(objectManager.ActorList);
            if (ignoreList != null) allObjects.RemoveAll(ignoreList.Contains);
            Ray ray = new Ray(position, direction);
            foreach (Actor3D actor3D in allObjects)
                if (actor3D is OurCollidableObject collidableObject)
                    if (collidableObject.IsBlocking || !onlyCheckBlocking)
                    {
                        float? dist;
                        bool collidableObjectCheck =
                            (dist = ray.Intersects(collidableObject.Collision.WorldBoundingBox)) != null;
                        if (collidableObjectCheck && dist < maxDist)
                        {
                            HitResult result = new HitResult {actor = actor3D, distance = (float) dist};
                            hit.Add(result);
                        }
                    }
        }

        private static HitResult Raycast(OurObjectManager objectManager, Vector3 position, Vector3 direction,
            float maxDist = float.MaxValue, List<Actor3D> ignoreList = null,
            bool onlyCheckBlocking = true)
        {
            List<HitResult> all = RaycastAll(objectManager, position, direction, maxDist, ignoreList,
                onlyCheckBlocking);
            all.Sort();

            return all.Count == 0 ? null : all[0];
        }

        private static List<HitResult> RaycastAll(OurObjectManager objectManager, Vector3 position, Vector3 direction,
            float maxDist = float.MaxValue,
            List<Actor3D> ignoreList = null, bool onlyCheckBlocking = true)
        {
            List<HitResult> result = new List<HitResult>();
            Raycast(objectManager, position, direction, ref result, maxDist, ignoreList, onlyCheckBlocking);
            return result;
        }


        private static List<HitResult> RaycastAll(this Actor3D callingDrawnActor3D, OurObjectManager objectManager,
            Vector3 position, Vector3 direction, bool ignoreSelf,
            float maxDist = float.MaxValue, bool onlyCheckBlocking = true)
        {
            List<Actor3D> ignoreList = new List<Actor3D>();
            if (ignoreSelf) ignoreList.Add(callingDrawnActor3D);

            return RaycastAll(objectManager, position, direction, maxDist, ignoreList, onlyCheckBlocking);
        }

        #endregion

        #region Nested Types

        public struct FloorHitResult
        {
            public HitResult hitResult;
            public Actor3D actor3D;
        }

        public class HitResult : IComparable<HitResult>
        {
            #region Public variables

            public Actor3D actor;
            public float distance;

            #endregion

            #region Public Method

            public int CompareTo(HitResult other)
            {
                return distance.CompareTo(other.distance);
            }

            #endregion
        }

        #endregion
    }
}