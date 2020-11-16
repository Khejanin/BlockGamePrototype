using System.Collections.Generic;
using GDGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;

namespace GDGame.Tiles
{
    public class Shape : Actor3D
    {
        private List<AttachableTile> attachableTiles;

        public List<AttachableTile> AttachableTiles { get => attachableTiles; set => attachableTiles = value; }

        public Shape(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType, transform3D)
        {
            AttachableTiles = new List<AttachableTile>();
        }

        public void AddTile(AttachableTile tile)
        {
            AttachableTiles.Add(tile);
        }
    }
}
