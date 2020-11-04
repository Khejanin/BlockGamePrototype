using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GDGame.Game.Tiles;
using GDLibrary.Managers;
using GDGame.Game.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;

namespace GDGame.Game.Factory
{
    public class TileFactory
    {
        private ObjectManager objectManager;

        private Dictionary<string, DrawnActor3D> drawnActors;

        public TileFactory(ObjectManager objectManager, Dictionary<string, DrawnActor3D> drawnActors)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
        }

        public GridTile CreateTile(ETileType type)
        {
            GridTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(),
                ETileType.Static => CreateStatic(),
                ETileType.Attachable => CreateAttachable(),
                _ => null
            };

            if (tile != null) tile.TileType = type;
            return tile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private GridTile CreateStatic()
        {
            StaticTile staticTile = (StaticTile) drawnActors["StaticTile"];
            staticTile = staticTile.Clone() as StaticTile;
            objectManager.Add(staticTile);
            return staticTile;
        }

        private GridTile CreateAttachable()
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private GridTile CreatePlayer()
        {
            CubePlayer player = (CubePlayer) drawnActors["PlayerBlock"];
            player = player.Clone() as CubePlayer;
            objectManager.Add(player);
            return player;
        }
    }
}