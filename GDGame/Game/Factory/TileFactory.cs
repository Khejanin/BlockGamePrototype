using System.Collections.Generic;
using GDGame.Actors;
using Microsoft.Xna.Framework;
using GDGame.Tiles;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Parameters;

namespace GDGame.Factory
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

        public BasicTile CreateTile(ETileType type)
        {
            BasicTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(),
                ETileType.Static => CreateStatic(),
                ETileType.Attachable => CreateAttachable(),
                ETileType.Win => CreateGoal(),
                _ => null
            };

            if (tile != null)
            {
            }

            return tile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private BasicTile CreateStatic()
        {
            BasicTile staticTile = (BasicTile) drawnActors["StaticTile"];
            staticTile = staticTile.Clone() as BasicTile;
            objectManager.Add(staticTile);
            return staticTile;
        }

        private BasicTile CreateAttachable()
        {
            MovableTile movableTile = (MovableTile) drawnActors["AttachableBlock"];
            movableTile = movableTile.Clone() as MovableTile;
            objectManager.Add(movableTile);
            return movableTile;
        }

        private BasicTile CreatePlayer()
        {
            PlayerTile playerTile = (PlayerTile) drawnActors["PlayerBlock"];
            playerTile = playerTile.Clone() as PlayerTile;
            objectManager.Add(playerTile);
            return playerTile;
        }

        private BasicTile CreateGoal()
        {
            GoalTile goal = (GoalTile)drawnActors["GoalTile"];
            goal = goal.Clone() as GoalTile;
            objectManager.Add(goal);
            return goal;
        }
    }
}