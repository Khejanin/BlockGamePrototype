using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class MenuManager : PausableDrawableGameComponent
    {
        #region Constructors & Core
        public MenuManager(Game game, StatusType statusType)
           : base(game, statusType)
        {
        }

        public void Add(/*params*/)
        {
        }

        public bool Remove(/*params*/)
        {
            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
        }
        protected override void ApplyDraw(GameTime gameTime)
        {
        }
        #endregion Constructors & Core
    }
}