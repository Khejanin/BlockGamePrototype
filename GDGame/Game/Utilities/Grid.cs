using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDGame.Actors;
using GDGame.Enums;
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

        #region Methods

        private void CreateShapes(LevelData data, Tile[,,] grid)
        {
            foreach (double shapesKey in data.shapes.Keys)
            {
                Shape newShape = tileFactory.CreateShape();
                foreach (AttachableTile tile in data.shapes[shapesKey].Select(shape => (grid[(int) shape.X, (int) shape.Y, (int) data.gridSize.Z - 1 - (int) shape.Z] as AttachableTile)))
                {
                    newShape.AddTile(tile);
                    if (tile != null) tile.Shape = newShape;
                }

                _shapes.Add((int) shapesKey, newShape);
            }
        }

        public void GenerateGrid(string levelFilePath)
        {
            string jsonString = "";

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

            for (int x = 0; x < data.gridSize.X; x++)
            {
                for (int y = 0; y < data.gridSize.Y; y++)
                {
                    for (int z = 0; z < data.gridSize.Z; z++)
                    {
                        if (data.gridValues[x, y, z] != ETileType.None)
                        {
                            //Super duper algorithm to determine what the current tile looks like!
                            Tile.EStaticTileType staticTileType = Tile.EStaticTileType.DarkChocolate;
                            int count = 0;

                            for (int i = -1; i <= 1; i += 2)
                            {
                                if (x + i >= 0 && x + i < data.gridSize.X)
                                    count += data.gridValues[x + i, y, z] == ETileType.Static ? 1 : 0;
                                if (y + i >= 0 && y + i < data.gridSize.Y)
                                    count += data.gridValues[x, y + i, z] == ETileType.Static ? 1 : 0;
                                if (z + i >= 0 && z + i < data.gridSize.Z)
                                    count += data.gridValues[x, y, z + i] == ETileType.Static ? 1 : 0;
                            }

                            if (y - 1 >= 0 && count == 1 && data.gridValues[x, y - 1, z] == ETileType.Static)
                            {
                                staticTileType = Tile.EStaticTileType.Plates;
                            }
                            else
                            {
                                if (count > 4) staticTileType = Tile.EStaticTileType.WhiteChocolate;
                                else if (count > 3) staticTileType = Tile.EStaticTileType.Chocolate;
                            }

                            Tile tile = tileFactory.CreateTile(pos + new Vector3(0, 0, data.gridSize.Z - 1), data.gridValues[x, y, z], staticTileType);
                            tile?.InitializeTile();

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
        }

        public Vector3 GetGridBounds()
        {
            return data.gridSize;
        }

        private void SetActivatorIds(LevelData data, Tile[,,] grid)
        {
            foreach (Vector3 targetKey in data.activatorTargets.Keys)
                //List<IActivatable> targets = new List<IActivatable>();
                //ButtonTile button = grid[(int)targetKey.X, (int)targetKey.Y,
                //    (int)data.gridSize.Z - 1 - (int)targetKey.Z] as ButtonTile;

                //foreach (var target in data.activatorTargets[buttonTargetKey])
                //    targets.Add(grid[(int)target.X, (int)target.Y, (int)target.Z] as IActivatable);

                //button.Targets = targets;

                grid[(int) targetKey.X, (int) targetKey.Y, (int) data.gridSize.Z - 1 - (int) targetKey.Z].activatorId =
                    data.activatorTargets[targetKey];
        }

        private void SetPaths(LevelData data, Tile[,,] grid)
        {
            foreach (Vector3 pathTileKey in data.movingTilePaths.Keys)
            {
                PathMoveTile moveTile =
                    grid[(int) pathTileKey.X, (int) pathTileKey.Y,
                        (int) data.gridSize.Z - 1 - (int) pathTileKey.Z] as PathMoveTile;

                foreach (Vector3 pathPoint in data.movingTilePaths[pathTileKey].Select(tilePath => new Vector3(tilePath.X, tilePath.Y, data.gridSize.Z - 1 - tilePath.Z)))
                {
                    moveTile?.path.Add(pathPoint);
                }

                if (moveTile?.path.Count > 0)
                    moveTile.currentPositionIndex = 0;
            }
        }

        #endregion
    }
}