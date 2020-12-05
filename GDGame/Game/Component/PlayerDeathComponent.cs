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
                    parent?.Die();
                    if (parent is MovableTile movableTile)
                    {
                        movableTile.IsMoving = false;
                    }
                    break;
            }
        }
    }
}