using System.Collections.Generic;
using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;

namespace GDGame.Tiles
{
    public class Shape : Actor3D
    {
        private List<MovableTile> attachableTiles;

        public List<MovableTile> AttachableTiles { get => attachableTiles; set => attachableTiles = value; }

        public Shape(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType, transform3D)
        {
            AttachableTiles = new List<MovableTile>();
        }

        public void AddTile(MovableTile tile)
        {
            AttachableTiles.Add(tile);
        }
    }
}
