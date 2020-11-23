﻿using System.Collections.Generic;
using GDGame.Actors;
using Microsoft.Xna.Framework;
using GDGame.Tiles;
using GDGame.Enums;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Factory
{
    public class TileFactory
    {
        private readonly ObjectManager objectManager;
        private readonly Dictionary<string, DrawnActor3D> drawnActors;
        private readonly Dictionary<string, Texture2D> textures;

        public TileFactory(ObjectManager objectManager, Dictionary<string, DrawnActor3D> drawnActors, Dictionary<string, Texture2D> textures)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
            this.textures = textures;
        }

        public BasicTile CreateTile(ETileType type,BasicTile.EStaticTileType staticTileType)
        {
            BasicTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(),
                ETileType.Static => CreateStatic(staticTileType),
                ETileType.Attachable => CreateAttachable(),
                ETileType.Win => CreateGoal(),
                ETileType.Enemy => CreateEnemy(),
                ETileType.Button => CreateButton(),
                ETileType.MovingPlatform => CreateMovingPlatform(),
                ETileType.Spike => CreateSpike(),
                ETileType.Star => CreateStarPickup(),
                _ => null
            };

            tile?.InitializeTile();
            return tile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private BasicTile CreateStatic(BasicTile.EStaticTileType tileType)
        {
            BasicTile staticTile = null;

            string texStringType = "";
            string texStringTiling = "";
            int randomN = MathHelperFunctions.rnd.Next(0, 100);
            if (randomN > 70 && randomN < 90) texStringTiling = "4x";
            else if (randomN > 90) texStringTiling = "8x";
            
            switch (tileType)
            {
                case BasicTile.EStaticTileType.Chocolate:
                    staticTile = ((BasicTile)drawnActors["StaticTile"]).Clone() as BasicTile;
                    break;
                case BasicTile.EStaticTileType.WhiteChocolate:
                    staticTile = ((BasicTile)drawnActors["WhiteChocolateTile"]).Clone() as BasicTile;
                    break;
                case BasicTile.EStaticTileType.DarkChocolate:
                    staticTile = ((BasicTile)drawnActors["DarkChocolateTile"]).Clone() as BasicTile;
                    break;
                case BasicTile.EStaticTileType.Plates:
                    staticTile = ((BasicTile)drawnActors["PlateStackTile"]).Clone() as BasicTile;
                    break;
            }
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

        private BasicTile CreateStarPickup()
        {
            StarPickupTile starPickupTile = (StarPickupTile)drawnActors["StarPickupTile"];
            starPickupTile = starPickupTile.Clone() as StarPickupTile;
            objectManager.Add(starPickupTile);
            return starPickupTile;
        }
    }
}