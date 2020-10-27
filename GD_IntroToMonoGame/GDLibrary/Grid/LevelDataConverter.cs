using System.Collections.Generic;
using Boomlagoon.JSON;
using Microsoft.Xna.Framework;

[System.Serializable]
public struct LevelData
{
    public Vector3 gridSize;
    public Vector3 tileSize;
    public ETileType[,,] gridValues;
    public Dictionary<double, List<Vector3>> shapes;
}

public static class LevelDataConverter
{
    public static LevelData ConvertJsonToLevelData(string jsonLevelString)
    {
        JSONObject json = JSONObject.Parse(jsonLevelString);
        JSONObject gridSizeJSON = json.GetObject("GridSize");
        JSONObject tileSizeJSON = json.GetObject("TileSize");

        Vector3 gridSize = new Vector3((float) gridSizeJSON.GetNumber("X"), (float) gridSizeJSON.GetNumber("Y"),
            (float) gridSizeJSON.GetNumber("Z"));
        Vector3 tileSize = new Vector3((float)tileSizeJSON.GetNumber("X"), (float)tileSizeJSON.GetNumber("Y"), 
            (float)tileSizeJSON.GetNumber("Z"));

        LevelData data = new LevelData
        {
            gridSize = gridSize,
            tileSize = tileSize,
            gridValues = new ETileType[(int)gridSize.X, (int)gridSize.Y, (int)gridSize.Z],
            shapes = new Dictionary<double, List<Vector3>>()
        };
        
        //populate Grid values
        JSONArray jsonX = json.GetArray("Values");

        for (int x = 0; x < gridSize.X; x++)
        {
            JSONArray jsonY = jsonX[x].Array;
            for (int y = 0; y < gridSize.Y; y++)
            {
                JSONArray jsonZ = jsonY[y].Array;
                for (int z = 0; z < gridSize.Z; z++)
                {
                    if (jsonZ[z].Obj == null)
                    {
                        data.gridValues[x, y, z] = ETileType.None;
                    }
                    else
                    {
                        JSONObject obj = jsonZ[z].Obj;
                        data.gridValues[x, y, z] = (ETileType) obj.GetNumber("TileType");
                        
                        //check if part of shape and store it seperately separately
                        double shapeId = obj.GetNumber("ShapeId");
                        if(shapeId != -1d)
                            if(data.shapes.ContainsKey(shapeId))
                                data.shapes[shapeId].Add(new Vector3(x, y, z));
                            else
                                data.shapes.Add(shapeId, new List<Vector3>() { new Vector3(x, y, z) });
                    }
                }
            }
        }

        return data;
    }
}