﻿using System.Collections.Generic;
using GDGame.Actors;
using Microsoft.Xna.Framework;
using GDGame.Tiles;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using GDGame.Game.Actors.Tiles;
using GDGame.Managers;

namespace GDGame.Factory
{
    public class TileFactory
    {
        private OurObjectManager objectManager;

        private Dictionary<string, DrawnActor3D> drawnActors;

        public TileFactory(OurObjectManager objectManager, Dictionary<string, DrawnActor3D> drawnActors)
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
                ETileType.Enemy => CreateEnemy(),
                ETileType.Button => CreateButton(),
                _ => null
            };

            tile.InitializeTile();
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
    }
}