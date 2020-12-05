using System;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Enums;
using JigLibX.Collision;

namespace GDGame.Component
{
    public class HostileColliderHandler : ColliderComponent, ICloneable
    {
        public HostileColliderHandler(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        protected override bool HandleCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (parent.Equals(skin0.Owner.ExternalData as OurCollidableObject))
            {
                if (skin1.Owner.ExternalData is Tile collide)
                {
                    switch (collide.TileType)
                    {
                            
                        case ETileType.Attachable:
                            EventManager.FireEvent(new TileEventInfo
                                {Type = TileEventType.AttachableKill, TileId = collide.ID});
                            break;
                        case ETileType.Player:
                            EventManager.FireEvent(new TileEventInfo
                                {Type = TileEventType.PlayerKill, TileId = collide.ID});
                            break;
                    }
                }
            }

            return true;
        }

        public new object Clone()
        {
            return new HostileColliderHandler(ID, ControllerType);
        }
    }
}