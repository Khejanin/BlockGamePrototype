using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
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

        private Tile CreateAttachable(Vector3 position)
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
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

        private Tile CreateButton(Vector3 position)
        {
            ActivatableTile activatable = (ActivatableTile) drawnActors["ButtonTile"];
            activatable = activatable.Clone() as ActivatableTile;
            if (activatable != null)
            {
                activatable.Transform3D.Translation = position;
                activatable.AddPrimitive(new Box(activatable.Transform3D.Translation, Matrix.Identity, activatable.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                activatable.Enable(true, 1);
            }

            objectManager.Add(activatable);
            return activatable;
        }

        private Tile CreateCheckpoint(Vector3 position)
        {
            Tile checkpoint = (Tile) drawnActors["CheckpointTile"];
            checkpoint = checkpoint.Clone() as Tile;
            if (checkpoint != null)
            {
                checkpoint.Transform3D.Translation = position;
                checkpoint.AddPrimitive(new Box(checkpoint.Transform3D.Translation, Matrix.Identity, checkpoint.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                checkpoint.Enable(true, 1);
            }

            objectManager.Add(checkpoint);
            return checkpoint;
        }

        private Tile CreateEnemy(Vector3 position)
        {
            EnemyTile enemy = (EnemyTile) drawnActors["EnemyTile"];
            enemy = enemy.Clone() as EnemyTile;
            if (enemy != null)
            {
                enemy.Transform3D.Translation = position;
                enemy.AddPrimitive(new Box(enemy.Transform3D.Translation, Matrix.Identity, enemy.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                enemy.Enable(false, 1);
            }

            objectManager.Add(enemy);
            return enemy;
        }

        private Tile CreateFallingPlatform(Vector3 position)
        {
            FallingTile fallingTile = (FallingTile) drawnActors["FallingTile"];
            fallingTile = fallingTile.Clone() as FallingTile;
            if (fallingTile != null)
            {
                fallingTile.Transform3D.Translation = position;
                fallingTile.AddPrimitive(new Box(fallingTile.Transform3D.Translation, Matrix.Identity, fallingTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                fallingTile.Enable(true, 1);
            }

            objectManager.Add(fallingTile);
            return fallingTile;
        }

        private Tile CreateGoal(Vector3 position)
        {
            Tile goal = (Tile) drawnActors["GoalTile"];
            goal = goal.Clone() as Tile;
            if (goal != null)
            {
                goal.Transform3D.Translation = position;
                goal.AddPrimitive(new Box(goal.Transform3D.Translation, Matrix.Identity, goal.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                goal.Enable(true, 1);
            }

            objectManager.Add(goal);
            return goal;
        }

        private Tile CreateMovingPlatform(Vector3 position)
        {
            MovingPlatformTile platform = (MovingPlatformTile) drawnActors["MovingPlatformTile"];
            platform = platform.Clone() as MovingPlatformTile;
            if (platform != null)
            {
                platform.Transform3D.Translation = position;
                platform.AddPrimitive(new Box(platform.Transform3D.Translation, Matrix.Identity, platform.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                platform.Enable(true, 1);
            }

            objectManager.Add(platform);
            return platform;
        }

        private Tile CreatePickup(Vector3 position)
        {
            Tile pickupTile = (Tile) drawnActors["StarPickupTile"];
            pickupTile = pickupTile.Clone() as Tile;
            if (pickupTile != null)
            {
                pickupTile.Transform3D.Translation = position;
                pickupTile.AddPrimitive(new Box(pickupTile.Transform3D.Translation, Matrix.Identity, pickupTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                pickupTile.Enable(true, 1);
                pickupTile.ScaleTo(true,new Vector3(-0.2f,-0.2f,-0.2f),(int) (Constants.GameConstants.MOVEMENT_COOLDOWN*1000),Smoother.SmoothingMethod.Accelerate,LoopMethod.PlayOnce);
                pickupTile.ScaleTo(true,new Vector3(-0.3f,-0.3f,-0.3f),(int) (Constants.GameConstants.MOVEMENT_COOLDOWN*1000)*10,Smoother.SmoothingMethod.Decelerate,LoopMethod.PingPongLoop);
                pickupTile.MoveTo(true,new Vector3(0,0.5f,0),(int) (Constants.GameConstants.MOVEMENT_COOLDOWN*1000)*5,Smoother.SmoothingMethod.Smooth,LoopMethod.PingPongLoop);
                pickupTile.RotateTo(true, new Vector3(0,0,40), (int) (Constants.GameConstants.MOVEMENT_COOLDOWN*1000)*10,Smoother.SmoothingMethod.Smooth,LoopMethod.PingPongLoop);
                pickupTile.RotateTo(true,new Vector3(0,360,0),(int) (Constants.GameConstants.MOVEMENT_COOLDOWN*1000)*30,Smoother.SmoothingMethod.Smooth,LoopMethod.PingPongLoop);
            }

            objectManager.Add(pickupTile);
            return pickupTile;
        }

        private Tile CreatePlayer(Vector3 position)
        {
            PlayerTile playerTile = (PlayerTile) drawnActors["PlayerBlock"];
            playerTile = playerTile.Clone() as PlayerTile;
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

        private Tile CreateSpike(Vector3 position)
        {
            Tile spikeTile = (Tile) drawnActors["SpikeTile"];
            spikeTile = spikeTile.Clone() as Tile;
            if (spikeTile != null)
            {
                spikeTile.Transform3D.Translation = position;
                spikeTile.AddPrimitive(new Box(spikeTile.Transform3D.Translation, Matrix.Identity, spikeTile.Transform3D.Scale), new MaterialProperties(0.3f, 0.5f, 0.3f));
                spikeTile.Enable(true, 1);
            }

            objectManager.Add(spikeTile);
            return spikeTile;
        }

        private Tile CreateStatic(Vector3 position, Tile.EStaticTileType tileType)
        {
            Tile staticTile = null;

            string texStringType = "";
            string texStringTiling = "";
            int randomN = MathHelperFunctions.Rnd.Next(0, 100);
            if (randomN > 65 && randomN < 71) texStringTiling = "_choco";
            if (randomN > 60 && randomN < 66) texStringTiling = "_b_logic";
            if (randomN > 70 && randomN < 90) texStringTiling = "4x";
            else if (randomN > 90) texStringTiling = "8x";

            switch (tileType)
            {
                case Tile.EStaticTileType.Chocolate:
                    texStringType = "Chocolate";
                    break;
                case Tile.EStaticTileType.WhiteChocolate:
                    texStringType = "WhiteChocolate";
                    break;
                case Tile.EStaticTileType.DarkChocolate:
                    texStringType = "DarkChocolate";
                    break;
                case Tile.EStaticTileType.Plates:
                    staticTile = ((Tile) drawnActors["PlateStackTile"]).Clone() as Tile;
                    break;
            }

            if (texStringType != "")
            {
                staticTile = ((Tile) drawnActors["StaticTile"]).Clone() as Tile;
                if (staticTile?.EffectParameters is NormalEffectParameters normalEffectParameters)
                {
                    normalEffectParameters.ColorTexture = textures[texStringType + texStringTiling];
                    normalEffectParameters.NormalTexture = textures["big-normalmap" + texStringTiling];
                }
            }

            if (staticTile != null)
            {
                staticTile.Transform3D.Translation = position;
                if (tileType != Tile.EStaticTileType.WhiteChocolate)
                {
                    staticTile.AddPrimitive(
                        //*0.99f Saves you around 200 FPS
                        new Box(staticTile.Transform3D.Translation, Matrix.Identity, staticTile.Transform3D.Scale*0.99f),
                        new MaterialProperties(0.3f, 0.5f, 0.3f));
                    staticTile.Enable(true, 1);
                }
            }

            objectManager.Add(staticTile);
            return staticTile;
        }

        public Tile CreateTile(Vector3 position, ETileType type, Tile.EStaticTileType staticTileType)
        {
            Tile tile = type switch
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