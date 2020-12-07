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
    /// Component that 
    /// </summary>
    public class PlayerDeathComponent : Controller, ICloneable
    {
        private Tile parent;

        public PlayerDeathComponent(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as Tile;
        }

        public new object Clone()
        {
            PlayerDeathComponent playerDeathComponent = new PlayerDeathComponent(ID, ControllerType);
            playerDeathComponent.InitEventListeners();
            return playerDeathComponent;
        }

        private void InitEventListeners()
        {
            EventManager.RegisterListener<TileEventInfo>(HandleEvent);
        }

        private void HandleEvent(TileEventInfo tileEventInfo)
        {
            switch (tileEventInfo.Type)
            {
                case TileEventType.PlayerKill:
                    if (tileEventInfo.IsEasy)
                    {
                        parent?.Respawn();
                        if (parent is MovableTile movableTile)
                        {
                            movableTile.IsMoving = false;
                        }
                    }
                    else
                    {
                        if (parent is PlayerTile)
                        {
                            EventManager.FireEvent(new TileEventInfo{Type = TileEventType.Reset, IsEasy = tileEventInfo.IsEasy});
                        }
                    }
                    break;
            }
        }
    }
}