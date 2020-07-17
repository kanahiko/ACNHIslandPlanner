using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class MiniMap
{
    private static Texture2D miniMapTexture;

    public static Action<Texture2D> UpdateMiniMap;

    public static Dictionary<DecorationType,List<Image>> minimapPins;
    public static Dictionary<DecorationType, List<Image>> minimapInactivePins;
    public static Dictionary<Vector2Int, Image> minimapActivePins;
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
                
                type = MapHolder.tiles[j,i].type;
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

    public static void CreatePins(MinimapDecorationsDictionary minimapPinsDictionary)
    {
        minimapPins = new Dictionary<DecorationType, List<Image>>();
        minimapInactivePins = new Dictionary<DecorationType, List<Image>>();
        minimapActivePins = new Dictionary<Vector2Int, Image>();

        foreach (var type in MapHolder.mapPrefab.maxCount)
        {
            if (minimapPinsDictionary.ContainsKey(type.Key))
            {
                int count = type.Value - 1;
                List<Image> images = new List<Image>();
                images.Add(minimapPinsDictionary[type.Key]);
                minimapPinsDictionary[type.Key].transform.localPosition = new Vector3(-100, -100);
                while (count != 0)
                {
                    Image image = GameObject.Instantiate(minimapPinsDictionary[type.Key], minimapPinsDictionary[type.Key].transform.parent);
                    image.transform.localPosition = new Vector3(-100, -100);
                    images.Add(image);
                    count--;
                }

                minimapPins.Add(type.Key, images);
                minimapInactivePins.Add(type.Key, images);
            }
        }
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
        TileType type = MapHolder.tiles[column, row].type;
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
            TileType type = MapHolder.tiles[ coordinate.x , coordinate.y].type;
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

    public static void CreateBuilding(int column, int row, int sizeX, int sizeY)
    {
        bool isAdd = MapHolder.decorationsTiles[column, row] != null;
        Color color = MapHolder.mapPrefab.plazaColor;
        Color secondaryColor = MapHolder.mapPrefab.elevationColors[MapHolder.tiles[column, row].elevation];
        for (int i=0;i<sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                if (!isAdd) 
                {
                    miniMapTexture.SetPixel(column * 3 + j + MapHolder.mapPrefab.miniMapOffset.x,
                      (MapHolder.height * 3) - (row * 3 - i) + MapHolder.mapPrefab.miniMapOffset.y - 1, secondaryColor);
                    continue;
                }
                int rotation = -1;
                if (j == 0)
                {
                    rotation =2;
                }
                else
                {
                    if (j + 1 == sizeX)
                    {
                        rotation = 3;
                    }
                }
                if (rotation != -1)
                {
                    if (i == 0)
                    {
                        rotation = rotation == 2 ? 0 : 1;
                    }
                    else
                    {
                        if (i + 1 == sizeY)
                        {
                            rotation = rotation == 2 ? 3 : 2;
                        }
                        else
                        {
                            rotation = -1;
                        }
                    }
                }

                miniMapTexture.SetPixel(column * 3 + j + MapHolder.mapPrefab.miniMapOffset.x, 
                    (MapHolder.height * 3) - (row * 3 - i) + MapHolder.mapPrefab.miniMapOffset.y - 1, 
                    CheckCanSecondaryColor(rotation, j, i) ? secondaryColor : color);

            }
        }
    }

    static void PutPin(int column, int row, int sizeX, int sizeY, DecorationType type, bool isAdd)
    {
        if (isAdd)
        {
            if (minimapInactivePins.ContainsKey(type))
            {
                Image pin = minimapInactivePins[type][minimapInactivePins[type].Count - 1];
                minimapInactivePins[type].RemoveAt(minimapInactivePins[type].Count - 1);

                int newColumn = column / 2;
                newColumn += column % 2 == 0 ? -1 : 0;

                minimapActivePins.Add(new Vector2Int(newColumn, row), pin);
            }
        }
        else
        {
            int newColumn = column / 2;
            newColumn += column % 2 == 0 ? -1 : 0;


            minimapActivePins.Remove(new Vector2Int(newColumn, row));
        }
    }
}
