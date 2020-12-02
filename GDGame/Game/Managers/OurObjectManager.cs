using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Actors;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    public class OurObjectManager : PausableGameComponent
    {
        #region Fields

        private List<OurDrawnActor3D> opaqueList;

        private List<OurDrawnActor3D> transparentList;

        private List<Actor3D> allActors;
        
        private bool isDirty;

        #endregion Fields

        #region Properties

        public List<OurDrawnActor3D> TransparentList => transparentList;

        public List<OurDrawnActor3D> OpaqueList => opaqueList;


        public List<Actor3D> ActorList
        {
            get
            {
                if (isDirty)
                {
                    isDirty = false;
                    allActors = new List<Actor3D>();
                    allActors.AddRange(opaqueList);
                    allActors.AddRange(transparentList);
                }
                
                return allActors;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public OurObjectManager(Microsoft.Xna.Framework.Game game, StatusType statusType,
            int drawnActorListSize,int transparentActorListSize) : base(game, statusType)
        {
            opaqueList = new List<OurDrawnActor3D>(drawnActorListSize);
            transparentList = new List<OurDrawnActor3D>(transparentActorListSize);
            isDirty = true;
        }

        #region Handle Events

        protected override void SubscribeToEvents()
        {
            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add more ObjectManager specfic subscriptions here...
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);

            //call base method to subscribe to menu event
            base.SubscribeToEvents();
        }

        protected override void HandleEvent(EventData eventData)
        {
            //if this event relates to adding, removing, changing an object
            if (eventData.EventCategoryType == EventCategoryType.Object)
            {
                HandleObjectCategoryEvent(eventData);
            }
            else if (eventData.EventCategoryType == EventCategoryType.Player)
            {
                HandlePlayerCategoryEvent(eventData);
            }

            //pass event to base (in case it is a menu event)
            base.HandleEvent(eventData);
        }

        private void HandlePlayerCategoryEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnWin)
            {
                //gets params and add win animation
            }
        }

        private void HandleObjectCategoryEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnRemoveActor)
            {
                OurDrawnActor3D removeObject = eventData.Parameters[0] as OurDrawnActor3D;
                opaqueList.Remove(removeObject);
            }
            else if (eventData.EventActionType == EventActionType.OnAddActor)
            {
                OurModelObject modelObject = eventData.Parameters[0] as OurModelObject;
                if (modelObject != null)
                {
                    Add(modelObject);
                }
            }
        }

        #endregion Handle Events

        /// <summary>
        /// Add the actor to the appropriate list based on actor transparency
        /// </summary>
        /// <param name="actor"></param>
        public void Add(OurDrawnActor3D actor)
        {
            if (actor.EffectParameters.GetAlpha() < 1)
            {
                transparentList.Add(actor);
            }
            else opaqueList.Add(actor);

            isDirty = true;
        }
        
        /// <summary>
        /// Remove the first instance of an actor corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify an actor</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool RemoveFirstIf(Predicate<OurDrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int position = -1;
            bool wasRemoved = false;

            position = opaqueList.FindIndex(predicate);  
            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                wasRemoved = true;
            }

            position = transparentList.FindIndex(predicate);
            if (position != -1)
            {
                transparentList.RemoveAt(position);
                wasRemoved = true;
            }

            if (wasRemoved) isDirty = true;
            
            return wasRemoved;
        }

        /// <summary>
        /// Remove all occurences of any actors corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify one or more actors</param>
        /// <returns>Count of the number of removed actors</returns>
        public int RemoveAll(Predicate<OurDrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int count = 0;
            count = opaqueList.RemoveAll(predicate);
            count += transparentList.RemoveAll(predicate);
            if (count > 0) isDirty = true;
            return count;
        }

        /// <summary>
        /// Called to update the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Update(GameTime)"/>
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (OurDrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }

            foreach (OurDrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }
        }

        #endregion Constructors & Core
    }
}