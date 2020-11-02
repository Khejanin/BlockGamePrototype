using System.Collections.Generic;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;

namespace GDGame.Game.Tiles
{
    public class Shape : Actor3D
    {
        private List<GridTile> tiles;

        public Shape(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType, transform3D)
        {
            this.tiles = new List<GridTile>();
        }

        public void AddTile(GridTile tile)
        {
            this.tiles.Add(tile);
        }
    }
}