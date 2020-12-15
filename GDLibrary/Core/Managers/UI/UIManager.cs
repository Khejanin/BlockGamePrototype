using System;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class UIManager : PausableDrawableGameComponent, IDisposable
    {
        #region Fields
        private SpriteBatch spriteBatch;
        private List<DrawnActor2D> uiObjectList;
        #endregion Fields

        #region Properties
        public List<DrawnActor2D> UIObjectList
        {
            get
            {
                return uiObjectList;
            }
        }

        #endregion Properties
        public UIManager(Game game, StatusType statusType, SpriteBatch spriteBatch, int initialDrawSize) : base(game, statusType)
        {
            this.spriteBatch = spriteBatch;
            uiObjectList = new List<DrawnActor2D>(initialDrawSize);
        }

        public void Add(DrawnActor2D actor)
        {
            // if(!uiObjectList.Contains(actor))
            uiObjectList.Add(actor);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (DrawnActor2D actor in uiObjectList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            base.ApplyUpdate(gameTime);
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null);
            foreach (DrawnActor2D actor in uiObjectList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
            base.ApplyDraw(gameTime);
        }

        public new void Dispose()
        {
            for (int i = 0; i< uiObjectList.Count; i++)
            {
                uiObjectList[i].ControllerList.Dispose();
                uiObjectList[i].EventHandlerList.Dispose();
            }
            uiObjectList.Clear();
        }
    }
}