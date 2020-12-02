using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class ObjectManager : PausableDrawableGameComponent
    {
        #region Fields

        private CameraManager<Camera3D> cameraManager;
        private static List<DrawnActor3D> opaqueList;
        private static List<DrawnActor3D> transparentList;

        #endregion Fields

        #region Constructors & Core

        public ObjectManager(Game game, StatusType statusType,
          int initialOpaqueDrawSize, int initialTransparentDrawSize,
          CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);

            //        EventDispatcherV2.Subscribe(EventCategoryType.Menu, HandleMenuChanged);

            SubscribeToEvents();
        }

        protected override void SubscribeToEvents()
        {
            //menu
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);


            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add

            //transparency
        }

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                    this.StatusType = StatusType.Off;
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    this.StatusType = StatusType.Drawn | StatusType.Update;
            }
            else if (eventData.EventCategoryType == EventCategoryType.Object)
            {
                if (eventData.EventActionType == EventActionType.OnRemoveActor)
                {
                    DrawnActor3D removeObject = eventData.Parameters[0] as DrawnActor3D;

                    opaqueList.Remove(removeObject);

                }
            }
        }

        public void Add(DrawnActor3D actor)
        {
            if (actor.EffectParameters.GetAlpha() < 1)
            {
                transparentList.Add(actor);
            else
                opaqueList.Add(actor);
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

        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }
        }

        #endregion Constructors & Core

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> result = opaqueList.FindAll(predicate);
            result.AddRange(transparentList.FindAll(predicate));
            return result;
        }
        
        public static List<DrawnActor3D> GetAllObjects()
        {
            List<DrawnActor3D> result = new List<DrawnActor3D>();
            result.AddRange(opaqueList);
            result.AddRange(transparentList);
            return result;
        }
    }
}