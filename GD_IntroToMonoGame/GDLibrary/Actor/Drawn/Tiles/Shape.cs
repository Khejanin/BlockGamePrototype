using GDLibrary;
using System.Collections.Generic;

namespace GDLibrary
{
    public class Shape : Actor3D
    {
        List<GridTile> tiles;

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
