﻿using System;
using GDGame.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    /// <summary>
    ///     Component that Handles collisions for OurCollidableObjects.
    ///     Inheritance is generally not needed as the callback is defined in the Constructor.
    /// </summary>
    public class ColliderComponent : Controller, ICloneable
    {
        #region Private variables

        private CollisionCallbackFn handleCollision;
        private bool handleIsSet;
        protected OurCollidableObject parent;

        #endregion

        #region Constructors

        public ColliderComponent(string id, ControllerType controllerType, CollisionCallbackFn handleCollision) : base(
            id, controllerType)
        {
            handleIsSet = false;
            this.handleCollision = handleCollision;
        }

        #endregion

        #region Initialization

        private void InitEventListeners()
        {
            parent.Body.CollisionSkin.callbackFn += handleCollision;
        }

        #endregion

        #region Override Method

        public override void Update(GameTime gameTime, IActor actor)
        {
            //Fetch the Parent in the update
            parent ??= actor as OurCollidableObject;
            if (handleIsSet == false && parent != null)
            {
                InitEventListeners();
                handleIsSet = true;
            }
        }

        #endregion

        #region Public Method

        public new object Clone()
        {
            return new ColliderComponent(ID, ControllerType, handleCollision);
        }

        #endregion
    }
}