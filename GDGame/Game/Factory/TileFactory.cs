using System.Collections.Generic;
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
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Factory
{
    public class TileFactory
    {
        #region Private variables

        private readonly Dictionary<string, OurDrawnActor3D> drawnActors;
        private readonly OurObjectManager objectManager;
        private readonly ContentDictionary<Texture2D> textures;

        #endregion

        #region Constructors

        public TileFactory(OurObjectManager objectManager, Dictionary<string, OurDrawnActor3D> drawnActors, ContentDictionary<Texture2D> textures)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
            this.textures = textures;
        }

        #endregion

        #region Methods

        private BasicTile CreateAttachable(Vector3 position)
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
            if (attachableTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = attachableTile;
            if (attachableTile != null)
            {
                attachableTile.Transform3D.Translation = position;
                attachableTile.AddPrimitive(new Box(attachableTile.Transform3D.Translation, Matrix.Identity, attachableTile.Transform3D.Scale),
                    new MaterialProperties(0.3f, 0.5f, 0.3f));
                attachableTile.Enable(false, 1);
            }

            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private BasicTile CreateButton(Vector3 position)
        {
            ButtonTile button = (ButtonTile) drawnActors["ButtonTile"];
            button = button.Clone() as ButtonTile;
            if (button != null)
            {
                button.Transform3D.Translation = position;
                button.AddPrimitive(new Box(button.Transform3D.Translation, Matrix.Identity, button.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                button.Enable(true, 1);
            }

            objectManager.Add(button);
            return button;
        }

        private BasicTile CreateCheckpoint(Vector3 position)
        {
            CheckpointTile checkpoint = (CheckpointTile) drawnActors["CheckpointTile"];
            checkpoint = checkpoint.Clone() as CheckpointTile;
            if (checkpoint != null)
            {
                checkpoint.Transform3D.Translation = position;
                checkpoint.AddPrimitive(new Box(checkpoint.Transform3D.Translation, Matrix.Identity, checkpoint.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                checkpoint.Enable(true, 1);
            }

            objectManager.Add(checkpoint);
            return checkpoint;
        }

        private BasicTile CreateEnemy(Vector3 position)
        {
            EnemyTile enemy = (EnemyTile) drawnActors["EnemyTile"];
            enemy = enemy.Clone() as EnemyTile;
            if (enemy?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = enemy;
            if (enemy != null)
            {
                enemy.Transform3D.Translation = position;
                enemy.AddPrimitive(new Box(enemy.Transform3D.Translation, Matrix.Identity, enemy.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                enemy.Enable(false, 1);
            }

            objectManager.Add(enemy);
            return enemy;
        }

        private BasicTile CreateFallingPlatform(Vector3 position)
        {
            FallingTile fallingTile = (FallingTile) drawnActors["FallingTile"];
            fallingTile = fallingTile.Clone() as FallingTile;
            if (fallingTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = fallingTile;
            if (fallingTile != null)
            {
                fallingTile.Transform3D.Translation = position;
                fallingTile.AddPrimitive(new Box(fallingTile.Transform3D.Translation, Matrix.Identity, fallingTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                fallingTile.Enable(true, 1);
            }

            objectManager.Add(fallingTile);
            return fallingTile;
        }

        private BasicTile CreateGoal(Vector3 position)
        {
            GoalTile goal = (GoalTile) drawnActors["GoalTile"];
            goal = goal.Clone() as GoalTile;
            if (goal != null)
            {
                goal.Transform3D.Translation = position;
                goal.AddPrimitive(new Box(goal.Transform3D.Translation, Matrix.Identity, goal.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                goal.Enable(true, 1);
            }

            objectManager.Add(goal);
            return goal;
        }

        private BasicTile CreateMovingPlatform(Vector3 position)
        {
            MovingPlatformTile platform = (MovingPlatformTile) drawnActors["MovingPlatformTile"];
            platform = platform.Clone() as MovingPlatformTile;
            if (platform?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = platform;
            if (platform != null)
            {
                platform.Transform3D.Translation = position;
                platform.AddPrimitive(new Box(platform.Transform3D.Translation, Matrix.Identity, platform.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                platform.Enable(true, 1);
            }

            objectManager.Add(platform);
            return platform;
        }

        private BasicTile CreatePickup(Vector3 position)
        {
            PickupTile pickupTile = (PickupTile) drawnActors["StarPickupTile"];
            pickupTile = pickupTile.Clone() as PickupTile;
            if (pickupTile != null)
            {
                pickupTile.Transform3D.Translation = position;
                pickupTile.AddPrimitive(new Box(pickupTile.Transform3D.Translation, Matrix.Identity, pickupTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                pickupTile.Enable(true, 1);
            }

            objectManager.Add(pickupTile);
            return pickupTile;
        }

        private BasicTile CreatePlayer(Vector3 position)
        {
            PlayerTile playerTile = (PlayerTile) drawnActors["PlayerBlock"];
            playerTile = playerTile.Clone() as PlayerTile;
            if (playerTile?.ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) is TileMovementComponent tileMovementComponent)
                tileMovementComponent.Tile = playerTile;
            if (playerTile != null)
            {
                playerTile.Transform3D.Translation = position;
                playerTile.AddPrimitive(new Box(playerTile.Transform3D.Translation, Matrix.Identity, playerTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                playerTile.Enable(false, 1);
            }

            objectManager.Add(playerTile);
            return playerTile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update, new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private BasicTile CreateSpike(Vector3 position)
        {
            SpikeTile spikeTile = (SpikeTile) drawnActors["SpikeTile"];
            spikeTile = spikeTile.Clone() as SpikeTile;
            if (spikeTile != null)
            {
                spikeTile.Transform3D.Translation = position;
                spikeTile.AddPrimitive(new Box(spikeTile.Transform3D.Translation, Matrix.Identity, spikeTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                spikeTile.Enable(true, 1);
            }

            objectManager.Add(spikeTile);
            return spikeTile;
        }

        private BasicTile CreateStatic(Vector3 position, BasicTile.EStaticTileType tileType)
        {
            BasicTile staticTile = null;

            string texStringType = "";
            string texStringTiling = "";
            int randomN = MathHelperFunctions.Rnd.Next(0, 100);
            if (randomN > 65 && randomN < 71) texStringTiling = "_choco";
            if (randomN > 60 && randomN < 66) texStringTiling = "_b_logic";
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
            }

            if (texStringType != "")
            {
                staticTile = ((BasicTile) drawnActors["StaticTile"]).Clone() as BasicTile;
                if (staticTile != null)
                {
                    NormalEffectParameters normalEffectParameters =
                        staticTile.EffectParameters as NormalEffectParameters;
                    normalEffectParameters.ColorTexture = textures[texStringType + texStringTiling];
                    normalEffectParameters.NormalTexture = textures["big-normalmap" + texStringTiling];
                } 
            }

            if (staticTile != null)
            {
                staticTile.Transform3D.Translation = position;
                staticTile.AddPrimitive(new Box(staticTile.Transform3D.Translation, Matrix.Identity, staticTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                staticTile.Enable(true, 1);
            }

            objectManager.Add(staticTile);
            return staticTile;
        }

        public BasicTile CreateTile(Vector3 position, ETileType type, BasicTile.EStaticTileType staticTileType)
        {
            BasicTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(position),
                ETileType.Static => CreateStatic(position, staticTileType),
                ETileType.Attachable => CreateAttachable(position),
                ETileType.Win => CreateGoal(position),
                ETileType.Enemy => CreateEnemy(position),
                ETileType.Button => CreateButton(position),
                ETileType.MovingPlatform => CreateMovingPlatform(position),
                ETileType.Spike => CreateSpike(position),
                ETileType.Star => CreatePickup(position),
                ETileType.Checkpoint => CreateCheckpoint(position),
                _ => null
            };

            return tile;
        }

        #endregion
    }
}