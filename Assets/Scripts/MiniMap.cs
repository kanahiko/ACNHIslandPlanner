using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiniMap
{
    private static Texture2D miniMapTexture;

    public static Action<Texture2D> UpdateMiniMap;
    
    public static void CreateMiniMap()
    {
        var mapPrefab = MapHolder.mapPrefab;
        if (miniMapTexture == null)
        {
            miniMapTexture = new Texture2D(MapHolder.width*3 + mapPrefab.miniMapOffset.x * 2 , MapHolder.height*3 + mapPrefab.miniMapOffset.y * 2, TextureFormat.ARGB32, false , true);

            int fullHeight = MapHolder.height * 3 + mapPrefab.miniMapOffset.y * 2;
            int fullWidth = MapHolder.width * 3 + mapPrefab.miniMapOffset.x * 2;
            int halfWidth = fullWidth / 2;
            int halfHeight = fullHeight / 2;
            for (int i = 0; i <= mapPrefab.miniMapOffset.y; i++)
            {
                for (int j = 0; j <= halfWidth; j++)
                {
                    //Debug.Log($"p1={j} {i} p2={j} {fullWidth - i} p3={fullHeight-j} {i} p4={fullHeight-j} {fullWidth - i}");
                    miniMapTexture.SetPixel(j, i, Color.white);
                    miniMapTexture.SetPixel(j, fullHeight - i, Color.white);
                    miniMapTexture.SetPixel(fullWidth-j, i, Color.white);
                    miniMapTexture.SetPixel(fullWidth-j, fullHeight - i, Color.white);
                }
            }

            for (int i = mapPrefab.miniMapOffset.y; i <= halfHeight; i++)
            {
                for (int j = 0; j <= mapPrefab.miniMapOffset.x; j++)
                {
                    miniMapTexture.SetPixel(j, i, Color.white);
                    miniMapTexture.SetPixel(j, fullHeight - i, Color.white);
                    miniMapTexture.SetPixel(fullWidth-j, i, Color.white);
                    miniMapTexture.SetPixel(fullWidth-j, fullHeight - i, Color.white);
                }
            }
        }
 
        TileType type;
        int elevation;
        Color color;
        Color secondaryColor = Color.white;
        int rotation = -1;
        
        for (int i = 0; i < MapHolder.height; i++)
        {
            for (int j = 0; j < MapHolder.width; j++)
            {
                
                type = MapHolder.grid[i * MapHolder.width + j];
                elevation = MapHolder.tiles[j, i].elevation;
                rotation = -1;
                //if (previousTypes)
                if (type == TileType.CliffDiagonal || type == TileType.Cliff || type == TileType.Land)
                {
                    color = mapPrefab.elevationColors[elevation];
                    if (type == TileType.CliffDiagonal)
                    {
                        rotation = MapHolder.tiles[j, i].diagonaCliffRotation;
                        secondaryColor = mapPrefab.elevationColors[elevation-1];
                    }
                    
                }
                else
                {
                    color = mapPrefab.tileTypeColorDictionary[type];
                    if (type == TileType.PathCurve)
                    {
                        rotation = Util.SubstractRotation(MapHolder.tiles[j, i].diagonalPathRotation,2);
                        secondaryColor = mapPrefab.elevationColors[elevation];
                    }
                    else
                    {
                        if (type == TileType.WaterDiagonal)
                        {
                            rotation = MapHolder.tiles[j, i].diagonaWaterRotation;
                            secondaryColor = mapPrefab.elevationColors[elevation];
                        }
                    }
                }
                
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        miniMapTexture.SetPixel(j*3+l + mapPrefab.miniMapOffset.x, (MapHolder.height * 3) - (i*3+k)  + mapPrefab.miniMapOffset.y - 1, CheckCanSecondaryColor(rotation,l,k) ? secondaryColor : color);
                    }
                }
            }
        }
        
        // Apply all SetPixel calls
        miniMapTexture.Apply();

        UpdateMiniMap?.Invoke(miniMapTexture);
    }

    static bool CheckCanSecondaryColor(int rotation, int l, int k)
    {
        if (rotation != -1)
        {
            if (l == 1)
            {
                if (((rotation >= 2) && k == 0) ||
                    ((rotation <= 1) && k == 2))
                {
                    return true;
                }
            }
            else
            {
                if ((((rotation == 1 || rotation == 2) && l == 0) ||
                     ((rotation == 0 || rotation == 3) && l == 2)) && 
                    (((rotation >= 2) && k != 2) ||
                     ((rotation <= 1) && k != 0)))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public static void ChangeMiniMap(int column, int row)
    {
        TileType type = MapHolder.grid[row * MapHolder.width + column];
        Color color;
        Color secondaryColor = Color.white;

        int rotation = -1;
        int elevation = MapHolder.tiles[column, row].elevation;
        if (type == TileType.CliffDiagonal || type == TileType.Cliff || type == TileType.Land)
        {
            color = MapHolder.mapPrefab.elevationColors[MapHolder.tiles[column,row].elevation];
            if (type == TileType.CliffDiagonal)
            {
                rotation = MapHolder.tiles[column, row].diagonaCliffRotation;
                secondaryColor = MapHolder.mapPrefab.elevationColors[elevation-1];
            }
                    
        }
        else
        {
            color = MapHolder.mapPrefab.tileTypeColorDictionary[type];
            if (type == TileType.PathCurve)
            {
                rotation = Util.SubstractRotation(MapHolder.tiles[column, row].diagonalPathRotation,2);
                secondaryColor = MapHolder.mapPrefab.elevationColors[elevation];
            }
            else
            {
                if (type == TileType.WaterDiagonal)
                {
                    rotation = MapHolder.tiles[column, row].diagonaWaterRotation;
                    secondaryColor = MapHolder.mapPrefab.elevationColors[elevation];
                }
            }
        }

        for (int k = 0; k < 3; k++)
        {
            for (int l = 0; l < 3; l++)
            {
                miniMapTexture.SetPixel(column*3+l + MapHolder.mapPrefab.miniMapOffset.x, (MapHolder.height * 3) - (row*3+k) + MapHolder.mapPrefab.miniMapOffset.y  - 1, CheckCanSecondaryColor(rotation,l,k) ? secondaryColor : color);
            }
        }
        
        // Apply all SetPixel calls
        miniMapTexture.Apply();
        
        UpdateMiniMap?.Invoke(miniMapTexture);
    }
    
    public static void ChangeMiniMap(HashSet<Vector2Int> coordinates)
    {
        var mapPrefab = MapHolder.mapPrefab;
        Color color;
        Color secondaryColor = Color.white;

        int rotation = -1;
        int elevation;
        foreach (var coordinate in coordinates)
        {
            TileType type = MapHolder.grid[coordinate.y * MapHolder.width + coordinate.x];
            elevation = MapHolder.tiles[coordinate.x, coordinate.y].elevation;
            rotation = -1;
            if (type == TileType.CliffDiagonal || type == TileType.Cliff || type == TileType.Land)
            {
                color = MapHolder.mapPrefab.elevationColors[MapHolder.tiles[coordinate.x, coordinate.y].elevation];
                if (type == TileType.CliffDiagonal)
                {
                    rotation = MapHolder.tiles[coordinate.x, coordinate.y].diagonaCliffRotation;
                    secondaryColor = mapPrefab.elevationColors[elevation-1];
                }

            }
            else
            {
                color = MapHolder.mapPrefab.tileTypeColorDictionary[type];
                if (type == TileType.PathCurve)
                {
                    rotation = Util.SubstractRotation(MapHolder.tiles[coordinate.x, coordinate.y].diagonalPathRotation,2);
                    secondaryColor = mapPrefab.elevationColors[elevation];
                }
                else
                {
                    if (type == TileType.WaterDiagonal)
                    {
                        rotation = MapHolder.tiles[coordinate.x, coordinate.y].diagonaWaterRotation;
                        secondaryColor = mapPrefab.elevationColors[elevation];
                    }
                }
            }

            for (int k = 0; k < 3; k++)
            {
                for (int l = 0; l < 3; l++)
                {
                    miniMapTexture.SetPixel(coordinate.x*3+l  + MapHolder.mapPrefab.miniMapOffset.x, (MapHolder.height * 3) - (coordinate.y*3+k)  + MapHolder.mapPrefab.miniMapOffset.y  - 1, CheckCanSecondaryColor(rotation,l,k) ? secondaryColor : color);
                    //miniMapTexture.SetPixel(l, MapHolder.height - k, color);
                    //miniMapTexture.SetPixel(coordinate.x, MapHolder.height - coordinate.y, color);
                }
            }
            //miniMapTexture.SetPixel(coordinate.x, MapHolder.height - coordinate.y, color);
        }
        
        // Apply all SetPixel calls
        miniMapTexture.Apply();
        UpdateMiniMap?.Invoke(miniMapTexture);
    }
}
