using System;
using System.Collections.Generic;
using System.IO;
using GDGame.Game.Enums;
using GDGame.Game.Factory;
using GDGame.Game.Tiles;
using GDLibrary.Parameters;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GDLibrary
{
    public class Grid
    {
        private Transform3D transform;
        private TileFactory tileFactory;
        private static GridTile[,,] grid;
        private static Dictionary<int, Shape> shapes;

        public Grid(Transform3D transform, TileFactory tileFactory)
        {
            this.transform = transform;
            this.tileFactory = tileFactory;
            shapes = new Dictionary<int, Shape>();
        }

        public void GenerateGrid(string levelFilePath)
        {
            string jsonString;

            if (File.Exists(levelFilePath))
            {
                using StreamReader reader = new StreamReader(File.OpenRead(levelFilePath));
                jsonString = reader.ReadToEnd();
            }
            else
                throw new FileNotFoundException("The level file with the path: " + levelFilePath + " was not found!");

            LevelData data = LevelDataConverter.ConvertJsonToLevelData(jsonString);
            grid = new GridTile[(int) data.gridSize.X, (int) data.gridSize.Y, (int) data.gridSize.Z];
            Vector3 pos = Vector3.Zero;

            for (int x = 0; x < data.gridSize.X; x++)
            {
                for (int y = 0; y < data.gridSize.Y; y++)
                {
                    for (int z = 0; z < data.gridSize.Z; z++)
                    {
                        if (data.gridValues[x, y, z] != ETileType.None)
                        {
                            GridTile tile = tileFactory.CreateTile(data.gridValues[x, y, z]);
                            tile?.SetPosition(pos + transform.Translation);
                            grid[x, y, z] = tile;
                        }
                        else
                            grid[x, y, z] = null;

                        pos.Z += data.tileSize.Z;
                    }

                    pos.Z = 0;
                    pos.Y += data.tileSize.Y;
                }

                pos.Y = 0;
                pos.X += data.tileSize.X;
            }

            CreateShapes(data, grid);
        }

        private void CreateShapes(LevelData data, GridTile[,,] grid)
        {
            foreach (var shapesKey in data.shapes.Keys)
            {
                //Transform newParent = new GameObject("Shape" + shapesKey + "Parent").transform;
                Shape newShape = this.tileFactory.CreateShape();
                foreach ((float x, float y, float z) in data.shapes[shapesKey])
                {
                    //grid[(int)shape.x, (int)shape.y, (int)shape.z].transform.SetParent(newParent);
                    newShape.AddTile(grid[(int) x, (int) y, (int) z]);
                }

                shapes.Add((int) shapesKey, newShape);
            }
        }

        public static void MoveTo(Vector3 start, Vector3 dest)
        {
            grid[(int) dest.X, (int) dest.Y, (int) dest.Z] = grid[(int) start.X, (int) start.Y, (int) start.Z];
            grid[(int) start.X, (int) start.Y, (int) start.Z] = null;
        }

        public struct GridPositionResult
        {
            public Vector3 pos;
            public GridTile floorTile;
            public GridTile positionTile;
            public bool validMovePos;
        }

        public static GridPositionResult QueryMove(Vector3 pos)
        {
            try
            {
                int x = (int) pos.X;
                int y = (int) pos.Y;
                int z = (int) pos.Z;

                GridTile floorTile = grid[x, y - 1, z];
                GridTile destinationTile = grid[x, y, z];

                GridPositionResult gpr = new GridPositionResult();
                gpr.pos = pos;
                gpr.floorTile = floorTile;
                gpr.positionTile = destinationTile;

                bool hasFloor = floorTile != null;
                bool validDest = destinationTile == null || destinationTile.CanMoveInto;
                if (hasFloor && validDest)
                {
                    gpr.validMovePos = true;
                    return gpr;
                }
                else
                {
                    gpr.validMovePos = false;
                    return gpr;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                return new GridPositionResult() {validMovePos = false, pos = pos};
            }
        }

        public static bool CanMove(Vector3 pos)
        {
            return CanMove((int) pos.X, (int) pos.Y, (int) pos.Z);
        }

        public static bool CanMove(int x, int y, int z)
        {
            try
            {
                bool hasFloor = grid[x, y - 1, z] != null;
                bool isFree = grid[x, y, z] == null;

                return hasFloor && isFree;
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
        }

        public static Shape GetShapeById(int id)
        {
            if (shapes.ContainsKey(id))
            {
                return shapes[id];
            }

            return null;
        }

        public GridTile[,,] GetGrid()
        {
            return grid;
        }
    }
}