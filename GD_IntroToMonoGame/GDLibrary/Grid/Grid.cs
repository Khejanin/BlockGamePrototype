using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GDLibrary
{
    public class Grid
    {
        private Transform3D transform;
        private TileFactory tileFactory;
        private GridTile[,,] grid;
        private static Dictionary<int, Shape> shapes;

        public Grid(Transform3D transform, TileFactory tileFactory)
        {
            this.transform = transform;
            this.tileFactory = tileFactory;
            shapes = new Dictionary<int, Shape>();
        }

        public void GenerateGrid(string levelFilePath)
        {
            string jsonString = "";

            if (File.Exists(levelFilePath))
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(levelFilePath)))
                {
                    jsonString = reader.ReadToEnd();
                }
            }
            else
                throw new FileNotFoundException("The level file with the path: " + levelFilePath + " was not found!");

            LevelData data = LevelDataConverter.ConvertJsonToLevelData(jsonString);
            grid = new GridTile[(int)data.gridSize.X, (int)data.gridSize.Y, (int)data.gridSize.Z];
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
                            if(tile != null) tile.SetPosition(pos + transform.Translation);
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

            CreateShapes(data, this.grid);
        }

        private void CreateShapes(LevelData data, GridTile[,,] grid)
        {
            foreach (var shapesKey in data.shapes.Keys)
            {
                //Transform newParent = new GameObject("Shape" + shapesKey + "Parent").transform;
                Shape newShape = this.tileFactory.CreateShape();
                foreach (var shape in data.shapes[shapesKey])
                {
                    //grid[(int)shape.x, (int)shape.y, (int)shape.z].transform.SetParent(newParent);
                    newShape.AddTile(grid[(int)shape.X, (int)shape.Y, (int)shape.Z]);
                }
                shapes.Add((int)shapesKey, newShape);
            }
        }

        public static Shape GetShapeById(int id)
        {
            if(shapes.ContainsKey(id)) 
            {
                return shapes[id];
            }

            return null;
        }
    }
}
