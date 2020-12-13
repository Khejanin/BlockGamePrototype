using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Constants;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
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
        private int id;
        private bool mode;

        #endregion

        #region Constructors

        public TileFactory(OurObjectManager objectManager, Dictionary<string, OurDrawnActor3D> drawnActors,
            ContentDictionary<Texture2D> textures, bool mode)
        {
            this.objectManager = objectManager;
            this.drawnActors = drawnActors;
            this.textures = textures;
            this.mode = mode;
        }

        #endregion

        #region Public Method

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        public Tile CreateTile(Vector3 position, ETileType type, Tile.StaticTileType staticTileType)
        {
            Tile tile = type switch
            {
                ETileType.Player => CreatePlayer(position),
                ETileType.Static => CreateStatic(position, staticTileType),
                ETileType.Attachable => CreateAttachable(position),
                ETileType.Win => CreateTile("GoalTile", position),
                ETileType.Enemy => CreateEnemy(position),
                ETileType.Button => CreateButton(position),
                ETileType.MovingPlatform => CreateMovingPlatform(position),
                ETileType.Spike => CreateTile("SpikeTile", position),
                ETileType.Star => CreatePickup(position),
                ETileType.Checkpoint => mode ? CreateTile("CheckpointTile", position, 2) : null,
                ETileType.Door => CreateDoorTile(position),
                _ => null
            };

            return tile;
        }

        #endregion

        #region Private Method

        private Tile CreateAttachable(Vector3 position)
        {
            AttachableTile attachableTile = (AttachableTile) drawnActors["AttachableBlock"];
            attachableTile = attachableTile.Clone() as AttachableTile;
            attachableTile?.InitializeCollision(position);
            if (attachableTile != null) attachableTile.ID += id++;
            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private Tile CreateButton(Vector3 position)
        {
            ActivatableTile activatable = (ActivatableTile) drawnActors["ButtonTile"];
            activatable = activatable.Clone() as ActivatableTile;
            activatable?.InitializeCollision(position, 0.7f);
            if (activatable != null) activatable.ID += id++;
            objectManager.Add(activatable);
            return activatable;
        }

        private Tile CreateDoorTile(Vector3 position)
        {
            PathMoveTile doorTile = (PathMoveTile) drawnActors["DoorTile"];
            doorTile = doorTile.Clone() as PathMoveTile;
            doorTile?.InitializeCollision(position, 0.9f);
            if (doorTile != null) doorTile.ID += id++;
            objectManager.Add(doorTile);
            return doorTile;
        }

        private Tile CreateEnemy(Vector3 position)
        {
            PathMoveTile enemy = (PathMoveTile) drawnActors["EnemyTile"];
            enemy = enemy.Clone() as PathMoveTile;
            enemy?.InitializeCollision(position, 0.7f);
            if (enemy != null) enemy.ID += id++;
            objectManager.Add(enemy);
            return enemy;
        }

        private Tile CreateMovingPlatform(Vector3 position)
        {
            MovingPlatformTile platform = (MovingPlatformTile) drawnActors["MovingPlatformTile"];
            platform = platform.Clone() as MovingPlatformTile;
            platform?.InitializeCollision(position, 0.9f);
            if (platform != null) platform.ID += id++;
            objectManager.Add(platform);
            return platform;
        }

        private Tile CreatePickup(Vector3 position)
        {
            Tile pickupTile = (Tile) drawnActors["StarPickupTile"];
            pickupTile = pickupTile.Clone() as Tile;
            pickupTile?.InitializeCollision(position, 0.9f);
            if (pickupTile != null) pickupTile.ID += id++;
            if (pickupTile != null)
            {
                pickupTile.ScaleTo(new AnimationEventData
                {
                    isRelative = true, destination = new Vector3(-0.2f, -0.2f, -0.2f),
                    maxTime = (int) (GameConstants.MovementCooldown * 1000),
                    smoothing = Smoother.SmoothingMethod.Accelerate
                });

                pickupTile.ScaleTo(new AnimationEventData
                {
                    isRelative = true, destination = new Vector3(-0.3f, -0.3f, -0.3f),
                    maxTime = (int) (GameConstants.MovementCooldown * 1000) * 10,
                    smoothing = Smoother.SmoothingMethod.Accelerate, loopMethod = LoopMethod.PingPongLoop
                });

                pickupTile.MoveTo(new AnimationEventData
                {
                    isRelative = true, destination = new Vector3(0, 0.5f, 0),
                    maxTime = (int) (GameConstants.MovementCooldown * 1000) * 5,
                    smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PingPongLoop
                });

                pickupTile.RotateTo(new AnimationEventData
                {
                    isRelative = true, destination = new Vector3(0, 0, 40),
                    maxTime = (int) (GameConstants.MovementCooldown * 1000) * 10,
                    smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PingPongLoop
                });

                pickupTile.RotateTo(new AnimationEventData
                {
                    isRelative = true, destination = new Vector3(0, 360, 0),
                    maxTime = (int) (GameConstants.MovementCooldown * 1000) * 30,
                    smoothing = Smoother.SmoothingMethod.Smooth, loopMethod = LoopMethod.PingPongLoop
                });
            }

            objectManager.Add(pickupTile);
            return pickupTile;
        }

        private Tile CreatePlayer(Vector3 position)
        {
            PlayerTile playerTile = (PlayerTile) drawnActors["PlayerBlock"];
            playerTile = playerTile.Clone() as PlayerTile;
            playerTile?.InitializeCollision(position);
            if (playerTile != null) playerTile.ID += id++;
            objectManager.Add(playerTile);
            return playerTile;
        }

        private Tile CreateStatic(Vector3 position, Tile.StaticTileType tileType)
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
                case Tile.StaticTileType.Chocolate:
                    texStringType = "Chocolate";
                    break;
                case Tile.StaticTileType.WhiteChocolate:
                    texStringType = "WhiteChocolate";
                    break;
                case Tile.StaticTileType.DarkChocolate:
                    texStringType = "DarkChocolate";
                    break;
                case Tile.StaticTileType.Plates:
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

            staticTile?.InitializeCollision(position, 0.9f);
            if (staticTile != null) staticTile.ID += id++;
            objectManager.Add(staticTile);
            return staticTile;
        }

        private Tile CreateTile(string actor, Vector3 position, float factor = 0.8f)
        {
            Tile tile = (Tile) drawnActors[actor];
            tile = tile.Clone() as Tile;
            tile?.InitializeCollision(position, factor);
            if (tile != null) tile.ID += id++;
            objectManager.Add(tile);
            return tile;
        }

        #endregion
    }
}