﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Factory;
using GDGame.Tiles;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public class Grid
    {
        #region Static Fields and Constants

        private static Tile[,,] _grid;
        private static Dictionary<int, Shape> _shapes;

        #endregion

        #region Private variables

        private LevelData data;
        private TileFactory tileFactory;

        #endregion

        #region Constructors

        public Grid(TileFactory tileFactory)
        {
            this.tileFactory = tileFactory;
            _shapes = new Dictionary<int, Shape>();
        }

        #endregion

        #region Public Method

        public LevelData GenerateGrid(string levelFilePath)
        {
            string jsonString;
            List<Vector3> checkpoints = new List<Vector3>();

            if (File.Exists(levelFilePath))
            {
                using StreamReader reader = new StreamReader(File.OpenRead(levelFilePath));
                jsonString = reader.ReadToEnd();
            }
            else
            {
                throw new FileNotFoundException("The level file with the path: " + levelFilePath +
                                                " was not found! Remember to set Build Action to 'Content' and Copy to 'Copy always' in the file properties!");
            }

            data = LevelDataConverter.ConvertJsonToLevelData(jsonString);
            _grid = new Tile[(int) data.gridSize.X, (int) data.gridSize.Y, (int) data.gridSize.Z];
            Vector3 pos = Vector3.Zero;

            for (int x = 0; x < (int) data.gridSize.X; x++)
            {
                for (int y = 0; y < (int) data.gridSize.Y; y++)
                {
                    for (int z = 0; z < (int) data.gridSize.Z; z++)
                    {
                        if (data.gridValues[x, y, z] != ETileType.None)
                        {
                            //Super duper algorithm to determine what the current tile looks like!
                            Tile.StaticTileType staticTileType = Tile.StaticTileType.DarkChocolate;
                            int count = 0;

                            for (int i = -1; i <= 1; i += 2)
                            {
                                if (x + i >= 0 && x + i < (int) data.gridSize.X)
                                    count += data.gridValues[x + i, y, z] == ETileType.Static ? 1 : 0;
                                if (y + i >= 0 && y + i < (int) data.gridSize.Y)
                                    count += data.gridValues[x, y + i, z] == ETileType.Static ? 1 : 0;
                                if (z + i >= 0 && z + i < (int) data.gridSize.Z)
                                    count += data.gridValues[x, y, z + i] == ETileType.Static ? 1 : 0;
                            }


                            if (count > 4) staticTileType = Tile.StaticTileType.WhiteChocolate;
                            else if (count > 3) staticTileType = Tile.StaticTileType.Chocolate;

                            if (y == 0) staticTileType = Tile.StaticTileType.Chocolate;

                            Tile tile = tileFactory.CreateTile(pos + new Vector3(0, 0, data.gridSize.Z - 1),
                                data.gridValues[x, y, z], staticTileType);
                            tile?.InitializeTile();

                            //Giving the player a list of checkpoints, so that we can switch between them (for debug/demo purposes)
                            if (tile != null && tile.TileType == ETileType.Checkpoint)
                                checkpoints.Add(tile.Transform3D.Translation);

                            _grid[x, y, (int) data.gridSize.Z - 1 - z] = tile;
                        }
                        else
                        {
                            _grid[x, y, (int) data.gridSize.Z - 1 - z] = null;
                        }

                        pos.Z -= data.tileSize.Z; //MonoGames Forward is -UnitZ
                    }

                    pos.Z = 0;
                    pos.Y += data.tileSize.Y;
                }

                pos.Y = 0;
                pos.X += data.tileSize.X;
            }

            CreateShapes(data, _grid);
            SetPaths(data, _grid);
            SetActivatorIds(data, _grid);
            SetCollectibleIds(data, _grid);

            EventManager.FireEvent(new PlayerEventInfo
                {type = PlayerEventType.SetCheckpointList, checkpoints = checkpoints});

            return data;
        }

        public Vector3 GetGridBounds()
        {
            return data.gridSize;
        }

        #endregion

        #region Private Method

        private void CreateShapes(LevelData data, Tile[,,] grid)
        {
            foreach (double shapesKey in data.shapes.Keys)
            {
                Shape newShape = tileFactory.CreateShape();
                foreach (AttachableTile tile in data.shapes[shapesKey]
                    .Select(shape =>
                        grid[(int) shape.X, (int) shape.Y, (int) data.gridSize.Z - 1 - (int) shape.Z] as AttachableTile)
                )
                {
                    newShape.AddTile(tile);
                    if (tile != null) tile.Shape = newShape;
                }

                _shapes.Add((int) shapesKey, newShape);
            }
        }

        private void SetActivatorIds(LevelData data, Tile[,,] grid)
        {
            foreach (Vector3 targetKey in data.activatorTargets.Keys)
                grid[(int) targetKey.X, (int) targetKey.Y, (int) data.gridSize.Z - 1 - (int) targetKey.Z].activatorId =
                    data.activatorTargets[targetKey];
        }

        private void SetCollectibleIds(LevelData data, Tile[,,] grid)
        {
            foreach (Vector3 key in data.collectibles.Keys)
                grid[(int) key.X, (int) key.Y, (int) data.gridSize.Z - 1 - (int) key.Z].ID = data.collectibles[key];
        }

        private void SetPaths(LevelData data, Tile[,,] grid)
        {
            foreach (Vector3 pathTileKey in data.movingTilePaths.Keys)
            {
                PathMoveTile moveTile =
                    grid[(int) pathTileKey.X, (int) pathTileKey.Y,
                        (int) data.gridSize.Z - 1 - (int) pathTileKey.Z] as PathMoveTile;

                foreach (Vector3 pathPoint in data.movingTilePaths[pathTileKey].Select(tilePath =>
                    new Vector3(tilePath.X, tilePath.Y, data.gridSize.Z - 1 - tilePath.Z)))
                    moveTile?.Path.Add(pathPoint);

                if (moveTile?.Path.Count > 0)
                {
                }
            }
        }

        #endregion
    }
}