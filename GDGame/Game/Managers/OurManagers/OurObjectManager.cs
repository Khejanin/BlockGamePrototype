using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.EventSystem;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.Managers
{
    /// <summary>
    ///     Our Object Manager that is basically the same as the original ones but it uses OurDrawnActors.
    /// </summary>
    public class OurObjectManager : PausableGameComponent
    {
        #region Private variables

        private List<Actor3D> allActors;

        private bool isDirty;

        private List<OurDrawnActor3D> removeList;

        #endregion

        #region Constructors

        public OurObjectManager(Microsoft.Xna.Framework.Game game, StatusType statusType,
            int drawnActorListSize, int transparentActorListSize) : base(game, statusType)
        {
            OpaqueList = new List<OurDrawnActor3D>(drawnActorListSize);
            TransparentList = new List<OurDrawnActor3D>(transparentActorListSize);
            isDirty = true;
            removeList = new List<OurDrawnActor3D>();
            EventManager.RegisterListener<RemoveActorEvent>(OnActorRemove);
        }

        #endregion

        #region Properties, Indexers

        public List<Actor3D> ActorList
        {
            get
            {
                if (isDirty)
                {
                    isDirty = false;
                    allActors = new List<Actor3D>();
                    allActors.AddRange(OpaqueList);
                    allActors.AddRange(TransparentList);
                }

                return allActors;
            }
        }

        public List<OurDrawnActor3D> OpaqueList { get; }

        public List<OurDrawnActor3D> TransparentList { get; }

        #endregion

        #region Override Method

        /// <summary>
        ///     Called to update the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Update(GameTime)" />
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyUpdate(GameTime gameTime)
        {
            ApplyBatchRemove();

            foreach (OurDrawnActor3D actor in OpaqueList)
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);

            foreach (OurDrawnActor3D actor in TransparentList)
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
        }

        protected override void HandleEvent(EventData eventData)
        {
            //if this event relates to adding, removing, changing an object
            if (eventData.EventCategoryType == EventCategoryType.Object)
                HandleObjectCategoryEvent(eventData);
            else if (eventData.EventCategoryType == EventCategoryType.Player) HandlePlayerCategoryEvent(eventData);

            //pass event to base (in case it is a menu event)
            base.HandleEvent(eventData);
        }

        protected override void SubscribeToEvents()
        {
            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add more ObjectManager specfic subscriptions here...
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);

            //call base method to subscribe to menu event
            base.SubscribeToEvents();
        }

        #endregion

        #region Public Method

        /// <summary>
        ///     Add the actor to the appropriate list based on actor transparency
        /// </summary>
        /// <param name="actor"></param>
        public void Add(OurDrawnActor3D actor)
        {
            if (actor.EffectParameters.GetAlpha() < 1)
                TransparentList.Add(actor);
            else OpaqueList.Add(actor);

            isDirty = true;
        }

        /// <summary>
        ///     Remove all occurences of any actors corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify one or more actors</param>
        /// <returns>Count of the number of removed actors</returns>
        public int RemoveAll(Predicate<OurDrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int count = 0;
            count = OpaqueList.RemoveAll(predicate);
            count += TransparentList.RemoveAll(predicate);
            if (count > 0) isDirty = true;
            return count;
        }

        /// <summary>
        ///     Remove the first instance of an actor corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify an actor</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool RemoveFirstIf(Predicate<OurDrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int position = -1;
            bool wasRemoved = false;

            position = OpaqueList.FindIndex(predicate);
            if (position != -1)
            {
                OpaqueList.RemoveAt(position);
                wasRemoved = true;
            }

            position = TransparentList.FindIndex(predicate);
            if (position != -1)
            {
                TransparentList.RemoveAt(position);
                wasRemoved = true;
            }

            if (wasRemoved) isDirty = true;

            return wasRemoved;
        }

        #endregion

        #region Private Method

        private void ApplyBatchRemove()
        {
            foreach (OurDrawnActor3D actor in removeList)
                if (actor.EffectParameters.GetAlpha() < 1)
                    TransparentList.Remove(actor);
                else
                    OpaqueList.Remove(actor);

            removeList.Clear();
        }

        #endregion

        #region Events

        private void HandleObjectCategoryEvent(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnRemoveActor:
                {
                    OurDrawnActor3D removeObject = eventData.Parameters[0] as OurDrawnActor3D;
                    OpaqueList.Remove(removeObject);
                    break;
                }
                case EventActionType.OnAddActor:
                {
                    if (eventData.Parameters[0] is OurModelObject modelObject) Add(modelObject);
                    break;
                }
            }
        }

        private void HandlePlayerCategoryEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnWin)
            {
                //gets params and add win animation
            }
        }

        private void OnActorRemove(RemoveActorEvent obj)
        {
            if (obj.actor3D != null) removeList.Add((OurDrawnActor3D) obj.actor3D);
        }

        #endregion
    }
}