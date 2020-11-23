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
        private readonly ObjectManager objectManager;
        private readonly Dictionary<string, DrawnActor3D> drawnActors;

        public TileFactory(ObjectManager objectManager, Dictionary<string, DrawnActor3D> drawnActors)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
        }

        public BasicTile CreateTile(ETileType type, bool test = false)
        {
            BasicTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(),
                ETileType.Static => CreateStatic(test),
                ETileType.Attachable => CreateAttachable(),
                ETileType.Win => CreateGoal(),
                ETileType.Enemy => CreateEnemy(),
                ETileType.Button => CreateButton(),
                ETileType.MovingPlatform => CreateMovingPlatform(),
                ETileType.Spike => CreateSpike(),
                ETileType.Star => CreatePickup(),
                ETileType.Checkpoint => CreateCheckpoint(),
                _ => null
            };

            return tile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private BasicTile CreateStatic(bool black)
        {
            BasicTile staticTile;
            if (black) staticTile = (BasicTile) drawnActors["StaticTile"];
            else staticTile = (BasicTile) drawnActors["WhiteChocolateTile"];
            staticTile = staticTile.Clone() as BasicTile;
            objectManager.Add(staticTile);
            return staticTile;
        }

        private BasicTile CreateAttachable()
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
            objectManager.Add(attachableTile);
            return attachableTile;
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

        private BasicTile CreateEnemy()
        {
            EnemyTile enemy = (EnemyTile)drawnActors["EnemyTile"];
            enemy = enemy.Clone() as EnemyTile;
            objectManager.Add(enemy);
            return enemy;
        }

        private BasicTile CreateButton()
        {
            ButtonTile button = (ButtonTile)drawnActors["ButtonTile"];
            button = button.Clone() as ButtonTile;
            objectManager.Add(button);
            return button;
        }

        private BasicTile CreateMovingPlatform()
        {
            MovingPlatformTile platform = (MovingPlatformTile)drawnActors["MovingPlatformTile"];
            platform = platform.Clone() as MovingPlatformTile;
            objectManager.Add(platform);
            return platform;
        }

        private BasicTile CreateSpike()
        {
            SpikeTile spikeTile = (SpikeTile)drawnActors["SpikeTile"];
            spikeTile = spikeTile.Clone() as SpikeTile;
            objectManager.Add(spikeTile);
            return spikeTile;
        }

        private BasicTile CreatePickup()
        {
            PickupTile pickupTile = (PickupTile)drawnActors["StarPickupTile"];
            pickupTile = pickupTile.Clone() as PickupTile;
            objectManager.Add(pickupTile);
            return pickupTile;
        }

        private BasicTile CreateCheckpoint()
        {
            CheckpointTile checkpoint = (CheckpointTile)drawnActors["CheckpointTile"];
            checkpoint = checkpoint.Clone() as CheckpointTile;
            objectManager.Add(checkpoint);
            return checkpoint;
        }
    }
}