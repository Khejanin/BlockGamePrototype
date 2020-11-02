using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class ObjectManager : DrawableGameComponent
    {
        #region Fields

        private CameraManager<Camera3D> cameraManager;
        private static List<DrawnActor3D> opaqueList;
        private static List<DrawnActor3D> transparentList;

        #endregion Fields

        #region Constructors & Core

        public ObjectManager(Game game,
          int initialOpaqueDrawSize, int initialTransparentDrawSize,
          CameraManager<Camera3D> cameraManager) : base(game)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);
        }

        public void Add(DrawnActor3D actor)
        {
            if (actor.EffectParameters.Alpha < 1)
            {
                transparentList.Add(actor);
            }
            else
            {
                opaqueList.Add(actor);
            }
        }

        public bool RemoveFirstIf(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            int position = opaqueList.FindIndex(predicate);

            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                return true;
            }

            return false;
        }

        public int RemoveAll(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            return opaqueList.RemoveAll(predicate);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                {
                    actor.Draw(gameTime,
                       cameraManager.ActiveCamera,
                        GraphicsDevice);
                }
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                {
                    actor.Draw(gameTime,
                       cameraManager.ActiveCamera,
                        GraphicsDevice);
                }
            }
        }

        #endregion Constructors & Core

        #region OurCode

        public static List<DrawnActor3D> GetAllObjects()
        {
            List<DrawnActor3D> result = new List<DrawnActor3D>();
            result.AddRange(opaqueList);
            result.AddRange(transparentList);
            return result;
        }

        #endregion

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> result = opaqueList.FindAll(predicate);
            result.AddRange(transparentList.FindAll(predicate));
            return result;
        }
    }
}