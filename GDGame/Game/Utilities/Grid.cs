﻿using System;
using System.Collections.Generic;
using System.IO;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.Factory;
using GDGame.Game.Actors.Tiles;
using GDGame.Tiles;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GDGame.Utilities
{
    public class Grid
    {
        private TileFactory tileFactory;
        private static BasicTile[,,] _grid;
        private static Dictionary<int, Shape> _shapes;

        public Grid(TileFactory tileFactory)
        {
            this.tileFactory = tileFactory;
            _shapes = new Dictionary<int, Shape>();
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
                throw new FileNotFoundException("The level file with the path: " + levelFilePath + " was not found! Remember to set Build Action to 'Content' and Copy to 'Copy always' in the file properties!");

            LevelData data = LevelDataConverter.ConvertJsonToLevelData(jsonString);
            _grid = new BasicTile[(int)data.gridSize.X, (int)data.gridSize.Y, (int)data.gridSize.Z];
            Vector3 pos = Vector3.Zero;

            for (int x = 0; x < data.gridSize.X; x++)
            {
                for (int y = 0; y < data.gridSize.Y; y++)
                {
                    for (int z = 0; z < data.gridSize.Z; z++)
                    {
                        if (data.gridValues[x, y, z] != ETileType.None) 
                        {
                            BasicTile tile = tileFactory.CreateTile(data.gridValues[x, y, z]);
                            tile?.SetPosition(pos + new Vector3(0, 0, data.gridSize.Z - 1));
                            _grid[x, y, (int)data.gridSize.Z - 1 - z] = tile;
                        }
                        else
                            _grid[x, y, (int)data.gridSize.Z - 1 - z] = null;

                        pos.Z -= data.tileSize.Z; //MonoGames Forward is -UnitZ
                    }

                    pos.Z = 0;
                    pos.Y += data.tileSize.Y;
                }

                pos.Y = 0;
                pos.X += data.tileSize.X;
            }

            CreateShapes(data, _grid);
            SetEnemyPaths(data, _grid);
        }

        private void CreateShapes(LevelData data, BasicTile[,,] grid)
        {
            foreach (var shapesKey in data.shapes.Keys)
            {
                Shape newShape = this.tileFactory.CreateShape();
                foreach (Vector3 shape in data.shapes[shapesKey])
                {
                    MovableTile tile = grid[(int)shape.X, (int)shape.Y, (int)data.gridSize.Z - 1 - (int)shape.Z] as MovableTile;
                    newShape.AddTile(tile);
                    if (tile != null) tile.Shape = newShape;
                }
                _shapes.Add((int)shapesKey, newShape);
            }
        }

        private void SetEnemyPaths(LevelData data, BasicTile[,,] grid)
        {
            foreach (var enemyKey in data.enemyPaths.Keys)
            {
                EnemyTile enemy = grid[(int)enemyKey.X, (int)enemyKey.Y, (int)data.gridSize.Z - 1 - (int)enemyKey.Z] as EnemyTile;

                foreach (Vector3 enemyPath in data.enemyPaths[enemyKey])
                {
                    Vector3 pathPoint = new Vector3(enemyPath.X, enemyPath.Y, data.gridSize.Z - 1 - enemyPath.Z); 
                    enemy.path.Add(pathPoint);
                }

                if(enemy.path.Count > 0)
                    enemy.currentPositionIndex = 0;
            }
        }
    }
}
