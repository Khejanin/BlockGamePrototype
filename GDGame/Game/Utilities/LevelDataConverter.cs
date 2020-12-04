using System;
using System.Collections.Generic;
using GDGame.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Utilities
{
    public struct CoffeeInfo
    {
        public float Y;
        public float TimeInMs;
        public float SetBackY;
    }

    [Serializable]
    public struct LevelData
    {
        public Vector3 gridSize;
        public Vector3 tileSize;
        public ETileType[,,] gridValues;
        public Transform3DCurve startCameraCurve;
        public List<CoffeeInfo> coffeeInfo;
        public Dictionary<double, List<Vector3>> shapes;
        public Dictionary<Vector3, List<Vector3>> movingTilePaths;
        public Dictionary<Vector3, int> activatorTargets;
    }

    public static class LevelDataConverter
    {
        #region Methods

        public static LevelData ConvertJsonToLevelData(string jsonLevelString)
        {
            JSONObject json = JSONObject.Parse(jsonLevelString);
            JSONObject gridSizeJSON = json.GetObject("GridSize");
            JSONObject tileSizeJSON = json.GetObject("TileSize");

            Vector3 gridSize = new Vector3((float) gridSizeJSON.GetNumber("X"), (float) gridSizeJSON.GetNumber("Y"),
                (float) gridSizeJSON.GetNumber("Z"));
            Vector3 tileSize = new Vector3((float) tileSizeJSON.GetNumber("X"), (float) tileSizeJSON.GetNumber("Y"),
                (float) tileSizeJSON.GetNumber("Z"));

            LevelData data = new LevelData
            {
                gridSize = gridSize,
                tileSize = tileSize,
                gridValues = new ETileType[(int) gridSize.X, (int) gridSize.Y, (int) gridSize.Z],
                startCameraCurve = new Transform3DCurve(CurveLoopType.Oscillate),
                coffeeInfo = new List<CoffeeInfo>(),
                shapes = new Dictionary<double, List<Vector3>>(),
                movingTilePaths = new Dictionary<Vector3, List<Vector3>>(),
                activatorTargets = new Dictionary<Vector3, int>()
            };

            //Set Camera Curve points
            JSONArray cameraInfoArray = json.GetArray("StartCameraInfo");
            foreach (JSONValue value in cameraInfoArray)
            {
                JSONObject obj = value.Obj;
                Vector3 translation = new Vector3((int)obj.GetNumber("X"), (int)obj.GetNumber("Y"), (int)obj.GetNumber("Z"));
                Vector3 look = new Vector3((int)obj.GetNumber("LookX"), (int)obj.GetNumber("LookY"), (int)obj.GetNumber("LookZ"));
                Vector3 up = new Vector3((int)obj.GetNumber("UpX"), (int)obj.GetNumber("UpY"), (int)obj.GetNumber("UpZ"));
                int time = (int)obj.GetNumber("MS");
                data.startCameraCurve.Add(translation, look, up, time);
            }

            //Ser Coffee Rising information
            JSONArray coffeeInfoArray = json.GetArray("CoffeeInfo");
            foreach (JSONValue value in coffeeInfoArray)
            {
                JSONObject obj = value.Obj;
                float y = (float)obj.GetNumber("Y");
                float time = (float) obj.GetNumber("MS");
                float setBackY = (float) obj.GetNumber("SetBackY");
                data.coffeeInfo.Add(new CoffeeInfo { Y = y, TimeInMs = time, SetBackY = setBackY });
            }

            //populate Grid values
            JSONArray jsonX = json.GetArray("Values");

            for (int x = 0; x < (int) gridSize.X; x++)
            {
                JSONArray jsonY = jsonX[x].Array;
                for (int y = 0; y < gridSize.Y; y++)
                {
                    JSONArray jsonZ = jsonY[y].Array;
                    for (int z = 0; z < gridSize.Z; z++)
                        if (jsonZ[z].Obj == null)
                        {
                            data.gridValues[x, y, z] = ETileType.None;
                        }
                        else
                        {
                            JSONObject obj = jsonZ[z].Obj;
                            data.gridValues[x, y, z] = (ETileType) obj.GetNumber("TileType");

                            //check if part of shape and store it separately
                            double shapeId = obj.GetNumber("ShapeId");
                            if (shapeId != -1d)
                                if (data.shapes.ContainsKey(shapeId))
                                    data.shapes[shapeId].Add(new Vector3(x, y, z));
                                else
                                    data.shapes.Add(shapeId, new List<Vector3> {new Vector3(x, y, z)});

                            //check if path moving tile and add paths to data
                            if (data.gridValues[x, y, z] == ETileType.Enemy || data.gridValues[x, y, z] == ETileType.MovingPlatform)
                            {
                                JSONArray path = obj.GetArray("Path");
                                List<Vector3> pathPositions = new List<Vector3>();

                                for (int i = 0; i < path.Length; i++)
                                {
                                    JSONObject pathObj = path[i].Obj;
                                    pathPositions.Add(new Vector3((int) pathObj["X"].Number, (int) pathObj["Y"].Number, (int) pathObj["Z"].Number));
                                }

                                data.movingTilePaths.Add(new Vector3(x, y, z), pathPositions);
                            }

                            //check if button and add targets to data
                            if (data.gridValues[x, y, z] == ETileType.Button)
                                //JSONArray targets = obj.GetArray("TargetGridPositions");
                                //List<Vector3> targetGridPositions = targets.Select(t => t.Obj).Select(pathObj => new Vector3((int)pathObj["X"].Number, (int)pathObj["Y"].Number, (int)pathObj["Z"].Number)).ToList();
                                data.activatorTargets.Add(new Vector3(x, y, z), (int) obj.GetNumber("ActivatorId"));
                        }
                }
            }

            return data;
        }

        #endregion
    }
}