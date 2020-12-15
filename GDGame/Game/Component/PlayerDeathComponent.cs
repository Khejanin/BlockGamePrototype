using System;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Component
{
    /// <summary>
    ///     Component that
    /// </summary>
    public class PlayerDeathComponent : Controller, ICloneable, IDisposable
    {
        #region Private variables

        private Tile parent;

        #endregion

        #region Constructors

        public PlayerDeathComponent(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        #endregion

        #region Initialization

        private void InitEventListeners()
        {
            EventManager.RegisterListener<TileEventInfo>(HandleEvent);
        }

        #endregion

        #region Override Method

        public override void Dispose()
        {
            EventManager.UnregisterListener<TileEventInfo>(HandleEvent);
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as Tile;
        }

        #endregion

        #region Public Method

        public new object Clone()
        {
            PlayerDeathComponent playerDeathComponent = new PlayerDeathComponent(ID, ControllerType);
            playerDeathComponent.InitEventListeners();
            return playerDeathComponent;
        }

        #endregion

        #region Events

        private void HandleEvent(TileEventInfo tileEventInfo)
        {
            switch (tileEventInfo.Type)
            {
                case TileEventType.PlayerKill:
                    if (tileEventInfo.IsEasy)
                    {
                        parent?.Respawn();
                        if (parent is MovableTile movableTile) movableTile.IsMoving = false;
                    }
                    else
                    {
                        if (parent is PlayerTile)
                            EventManager.FireEvent(new TileEventInfo
                                {Type = TileEventType.Reset, IsEasy = tileEventInfo.IsEasy});
                    }

                    break;
            }
        }

        #endregion
    }
}