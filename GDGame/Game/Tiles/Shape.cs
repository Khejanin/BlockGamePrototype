using GD_Library;
using System.Collections.Generic;

namespace GDLibrary
{
    public class Shape : Actor3D
    {
        private List<AttachableTile> attachableTiles;

        public List<AttachableTile> AttachableTiles { get => attachableTiles; set => attachableTiles = value; }

        public Shape(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType, transform3D)
        {
            this.AttachableTiles = new List<AttachableTile>();
        }

        public void AddTile(AttachableTile tile)
        {
            this.AttachableTiles.Add(tile);
        }
    }
}
