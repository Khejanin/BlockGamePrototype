﻿using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Enums;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Factory
{
    public class TileFactory
    {
        #region 05. Private variables

        private readonly Dictionary<string, DrawnActor3D> drawnActors;
        private readonly ObjectManager objectManager;
        private readonly ContentDictionary<Texture2D> textures;

        #endregion

        #region 06. Constructors

        public TileFactory(ObjectManager objectManager, Dictionary<string, DrawnActor3D> drawnActors, ContentDictionary<Texture2D> textures)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
            this.textures = textures;
        }

        #endregion

        #region 11. Methods

        private BasicTile CreateAttachable()
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
            if (attachableTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = attachableTile;

            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private BasicTile CreateButton()
        {
            ButtonTile button = (ButtonTile) drawnActors["ButtonTile"];
            button = button.Clone() as ButtonTile;
            objectManager.Add(button);
            return button;
        }

        private BasicTile CreateCheckpoint()
        {
            CheckpointTile checkpoint = (CheckpointTile) drawnActors["CheckpointTile"];
            checkpoint = checkpoint.Clone() as CheckpointTile;
            objectManager.Add(checkpoint);
            return checkpoint;
        }

        private BasicTile CreateEnemy()
        {
            EnemyTile enemy = (EnemyTile) drawnActors["EnemyTile"];
            enemy = enemy.Clone() as EnemyTile;
            if (enemy?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = enemy;
            objectManager.Add(enemy);
            return enemy;
        }

        private BasicTile CreateGoal()
        {
            GoalTile goal = (GoalTile) drawnActors["GoalTile"];
            goal = goal.Clone() as GoalTile;
            objectManager.Add(goal);
            return goal;
        }

        private BasicTile CreateMovingPlatform()
        {
            MovingPlatformTile platform = (MovingPlatformTile) drawnActors["MovingPlatformTile"];
            platform = platform.Clone() as MovingPlatformTile;
            if (platform?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = platform;
            objectManager.Add(platform);
            return platform;
        }

        private BasicTile CreatePickup()
        {
            PickupTile pickupTile = (PickupTile) drawnActors["StarPickupTile"];
            pickupTile = pickupTile.Clone() as PickupTile;
            objectManager.Add(pickupTile);
            return pickupTile;
        }

        private BasicTile CreatePlayer()
        {
            PlayerTile playerTile = (PlayerTile) drawnActors["PlayerBlock"];
            playerTile = playerTile.Clone() as PlayerTile;
            if (playerTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = playerTile;

            objectManager.Add(playerTile);
            return playerTile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private BasicTile CreateSpike()
        {
            SpikeTile spikeTile = (SpikeTile) drawnActors["SpikeTile"];
            spikeTile = spikeTile.Clone() as SpikeTile;
            objectManager.Add(spikeTile);
            return spikeTile;
        }

        private BasicTile CreateStatic(BasicTile.EStaticTileType tileType)
        {
            BasicTile staticTile = null;

            string texStringType = "";
            string texStringTiling = "";
            int randomN = MathHelperFunctions.Rnd.Next(0, 100);
            if (randomN > 70 && randomN < 90) texStringTiling = "4x";
            else if (randomN > 90) texStringTiling = "8x";

            switch (tileType)
            {
                case BasicTile.EStaticTileType.Chocolate:
                    texStringType = "Chocolate";
                    break;
                case BasicTile.EStaticTileType.WhiteChocolate:
                    texStringType = "WhiteChocolate";
                    break;
                case BasicTile.EStaticTileType.DarkChocolate:
                    texStringType = "DarkChocolate";
                    break;
                case BasicTile.EStaticTileType.Plates:
                    staticTile = ((BasicTile) drawnActors["PlateStackTile"]).Clone() as BasicTile;
                    break;
            }

            if (texStringType != "")
            {
                staticTile = ((BasicTile) drawnActors["StaticTile"]).Clone() as BasicTile;
                if (staticTile != null) staticTile.EffectParameters.Texture = textures[texStringType + texStringTiling];
            }

            objectManager.Add(staticTile);
            return staticTile;
        }

        public BasicTile CreateTile(ETileType type, BasicTile.EStaticTileType staticTileType)
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
                ETileType.Star => CreatePickup(),
                ETileType.Checkpoint => CreateCheckpoint(),
                _ => null
            };

            return tile;
        }

        #endregion
    }
}