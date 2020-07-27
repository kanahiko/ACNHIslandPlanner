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
    public static Dictionary<int, Image> minimapActivePins;
    
    static Vector3 hidePosition = Vector3.back;
    
    public static int pixelSize = 3;

    public static Vector2 ratio;
    public static Vector2 miniMapPosition;

    public static Image cameraPosition;
    
    public static void CreateMiniMap()
    {
        var mapPrefab = MapHolder.mapPrefab;
        if (miniMapTexture == null)
        {
            miniMapTexture = new Texture2D(MapHolder.width*3 + mapPrefab.miniMapOffset.x * 2 , MapHolder.height*3 + mapPrefab.miniMapOffset.y * 2, TextureFormat.ARGB32, false , true);
            miniMapTexture.filterMode = FilterMode.Point;
            int fullHeight = MapHolder.height * pixelSize + mapPrefab.miniMapOffset.y * 2;
            int fullWidth = MapHolder.width * pixelSize + mapPrefab.miniMapOffset.x * 2;
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
        
        for (int row = 0; row < MapHolder.height; row++)
        {
            for (int column = 0; column < MapHolder.width; column++)
            {
                ChangeMiniMap(column, row, mapPrefab);
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
        minimapActivePins = new Dictionary<int, Image>();
        foreach (var type in MapHolder.mapPrefab.maxCount)
        {
            if (minimapPinsDictionary.ContainsKey(type.Key))
            {
                int count = type.Value - 1;
                List<Image> images = new List<Image>();
                images.Add(minimapPinsDictionary[type.Key]);

                if (hidePosition == Vector3.back)
                {
                    hidePosition = minimapPinsDictionary[type.Key].transform.localPosition;
                }
                while (count != 0)
                {
                    Image image = GameObject.Instantiate(minimapPinsDictionary[type.Key], minimapPinsDictionary[type.Key].transform.parent);
                    image.transform.localPosition = hidePosition;
                    images.Add(image);
                    count--;
                }

                minimapPins.Add(type.Key, images);
                minimapInactivePins.Add(type.Key, images);
            }
        }
        cameraPosition.transform.SetAsLastSibling();
    }

    static bool CheckCanSecondaryColor(int rotation, int column, int row)
    {
        if (rotation != 255)
        {
            if (column == 1)
            {
                if (((rotation >= 2) && row == 0) ||
                    ((rotation <= 1) && row == 2))
                {
                    return true;
                }
            }
            else
            {
                if ((((rotation == 1 || rotation == 2) && column == 0) ||
                     ((rotation == 0 || rotation == 3) && column == 2)) && 
                    (((rotation >= 2) && row != 2) ||
                     ((rotation <= 1) && row != 0)))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public static void ChangeMiniMap(int column, int row, MapPrefabs mapPrefab)
    {
        TileType type = MapHolder.tiles[column, row].type;
        byte elevation = MapHolder.tiles[column, row].elevation;
        byte rotation = 255;
        Color color = Color.white;
        Color secondaryColor = Color.white;

        DecorationType decorationType;

        if (MapHolder.decorationsTiles[column, row] != null &&
            MapHolder.decorationsTiles[column, row].type == DecorationType.Incline)
        {
            decorationType = MapHolder.decorationsTiles[column, row].type;
            color = mapPrefab.inclineColors[0];
            secondaryColor = mapPrefab.inclineColors[1];
        }
        else
        {
            decorationType = DecorationType.Null;
            if (type == TileType.CliffDiagonal || type == TileType.Cliff || type == TileType.Land)
            {
                color = MapHolder.mapPrefab.elevationColors[MapHolder.tiles[column, row].elevation];
                if (type == TileType.CliffDiagonal)
                {
                    rotation = MapHolder.tiles[column, row].diagonalRotation;
                    secondaryColor = mapPrefab.elevationColors[elevation - 1];
                }
    
            }
            else
            {
                color = MapHolder.mapPrefab.tileTypeColorDictionary[type];
                if (type == TileType.PathCurve)
                {
                    rotation = Util.SubstractRotation(MapHolder.tiles[column, row].diagonalRotation, 2);
                    secondaryColor = mapPrefab.elevationColors[elevation];
                }
                else
                {
                    if (type == TileType.WaterDiagonal)
                    {
                        rotation = MapHolder.tiles[column, row].diagonalRotation;
                        secondaryColor = mapPrefab.elevationColors[elevation];
                    }
                    else
                    {
                        if (type == TileType.SandDiagonal)
                        {
                            rotation = MapHolder.tiles[column, row].diagonalRotation;
                            if (Util.CoordinateExists(column + Util.oppositeCornerForSand[rotation].x, row + Util.oppositeCornerForSand[rotation].y) &&
                                MapHolder.tiles[column + Util.oppositeCornerForSand[rotation].x, row + Util.oppositeCornerForSand[rotation].y].type != TileType.Sea)
                            {
                                secondaryColor = mapPrefab.elevationColors[0];
                            }
                            else
                            {
                                secondaryColor = mapPrefab.tileTypeColorDictionary[TileType.Sea];
                            }
                            rotation = Util.SubstractRotation(rotation, 2);
                        }
    
                        if (type == TileType.SeaDiagonal)
                        {
                            rotation = MapHolder.tiles[column, row].diagonalRotation;
                            secondaryColor = mapPrefab.elevationColors[0];
                            rotation = Util.SubstractRotation(rotation, 2);
                        }
                    }
                }
            }
        }
        

        for (int rowPixelAdd = 0; rowPixelAdd < pixelSize; rowPixelAdd++)
        {
            for (int columnPixelAdd = 0; columnPixelAdd < pixelSize; columnPixelAdd++)
            {
                bool isSecondaryColor = false;
                if (decorationType == DecorationType.Null)
                {
                    isSecondaryColor = CheckCanSecondaryColor(rotation, columnPixelAdd, rowPixelAdd);
                }
                else
                {
                    isSecondaryColor = InclineSecondaryColor(column, row, columnPixelAdd, rowPixelAdd);
                }

                
                miniMapTexture.SetPixel(column * pixelSize + columnPixelAdd + MapHolder.mapPrefab.miniMapOffset.x,
                    (MapHolder.height * pixelSize) - (row * pixelSize + rowPixelAdd) + MapHolder.mapPrefab.miniMapOffset.y - 1,
                    isSecondaryColor ? secondaryColor : color);
            }
        }
        
    }

    static  bool[] inclineColors = new bool[]
    {
        false,true,false, true,false,true, false,true,false, false,false,false
    };
    
    static bool InclineSecondaryColor(int column, int row, int columnPixelAdd, int rowPixelAdd)
    {
        DecorationTiles tile = MapHolder.decorationsTiles[column, row];

        return (tile.rotation == 0 || tile.rotation == 2) && inclineColors[Mathf.Abs(tile.startingRow - row)*pixelSize + rowPixelAdd] ||
               (tile.rotation == 1 || tile.rotation == 3) && inclineColors[Mathf.Abs(tile.startingColumn - column)*pixelSize + columnPixelAdd];
    }
        
    public static void ChangeMiniMap(HashSet<Vector2Int> coordinates)
    {
        var mapPrefab = MapHolder.mapPrefab;
        foreach (var coordinate in coordinates)
        {
            ChangeMiniMap(coordinate.x, coordinate.y, mapPrefab);
        }
        
        // Apply all SetPixel calls
        miniMapTexture.Apply();
        UpdateMiniMap?.Invoke(miniMapTexture);
    }

    public static void CreateBuilding(int column, int row, int sizeX, int sizeY, DecorationType type)
    {
        bool isAdd = MapHolder.decorationsTiles[column, row] != null;
        if (type == DecorationType.Plaza)
        {
            CreateBuildingBackground(column, row, sizeX, sizeY, isAdd);
        }
        miniMapTexture.Apply();
        UpdateMiniMap?.Invoke(miniMapTexture);

        PutPin(column, row, sizeX, sizeY, type, isAdd);
    }

    static void CreateBuildingBackground(int column, int row, int sizeX, int sizeY, bool isAdd)
    {
        Color color = MapHolder.mapPrefab.plazaColor;
        Color secondaryColor = MapHolder.mapPrefab.elevationColors[MapHolder.tiles[column, row].elevation];
        
        for (int rowIndex = 0; rowIndex < sizeY; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < sizeX; columnIndex++)
                {
                    if (!isAdd)
                    {
                        for (int rowPixelAdd = 0; rowPixelAdd < 3; rowPixelAdd++)
                        {
                            for (int columnPixelAdd = 0; columnPixelAdd < 3; columnPixelAdd++)
                            {
                                miniMapTexture.SetPixel((column + columnIndex) * pixelSize + columnPixelAdd + MapHolder.mapPrefab.miniMapOffset.x,
                                    (MapHolder.height * pixelSize) - ((row - rowIndex) * pixelSize + rowPixelAdd) + MapHolder.mapPrefab.miniMapOffset.y - 1,
                                    secondaryColor);
                            }
                        }

                        continue;
                    }

                    int rotation = -1;
                    if (columnIndex == 0)
                    {
                        rotation = 2;
                    }
                    else
                    {
                        if (columnIndex + 1 == sizeX)
                        {
                            rotation = 3;
                        }
                    }

                    if (rotation != -1)
                    {
                        if (rowIndex == 0)
                        {
                            rotation = rotation == 2 ? 1 : 0;
                        }
                        else
                        {
                            if (rowIndex + 1 == sizeY)
                            {
                                rotation = rotation == 2 ? 2 : 3;
                            }
                            else
                            {
                                rotation = -1;
                            }
                        }
                    }

                    for (int rowPixelAdd = 0; rowPixelAdd < 3; rowPixelAdd++)
                    {
                        for (int columnPixelAdd = 0; columnPixelAdd < 3; columnPixelAdd++)
                        {
                            miniMapTexture.SetPixel((column + columnIndex) * pixelSize + columnPixelAdd + MapHolder.mapPrefab.miniMapOffset.x,
                                (MapHolder.height * pixelSize) - ((row - rowIndex) * pixelSize + rowPixelAdd) + MapHolder.mapPrefab.miniMapOffset.y - 1,
                                CheckCanSecondaryColor(rotation, columnPixelAdd, rowPixelAdd) ? secondaryColor : color);
                        }
                    }

                }
            }
    }

    public static void PutPin(int column, int row, int sizeX, int sizeY, DecorationType type, bool isAdd)
    {
        if (type == DecorationType.Rock)
        {
            return;
        }
        if (isAdd)
        {
            
            if (minimapInactivePins.ContainsKey(type))
            {
                Image pin = minimapInactivePins[type][minimapInactivePins[type].Count - 1];
                minimapInactivePins[type].RemoveAt(minimapInactivePins[type].Count - 1);

                int newColumn = sizeX / 2;
                newColumn += column + (sizeX % 2 == 0 ? -1 : 0);
                
                //Debug.Log($"{column} {newColumn} {row}");

                Vector2 position = new Vector2();
                position.x = (newColumn * pixelSize + MapHolder.mapPrefab.miniMapOffset.x + pixelSize * 0.5f) * ratio.x;
                position.y = -(row * pixelSize + MapHolder.mapPrefab.miniMapOffset.y + pixelSize) * ratio.y;   
                
                pin.transform.localPosition = position;
                
                minimapActivePins.Add(column +  row *MapHolder.width, pin);
            }
        }
        else
        {
            //Debug.Log($"{column} {row}");
            Image pin = minimapActivePins[column +  row * MapHolder.width];
            
            pin.transform.localPosition = hidePosition;

            minimapInactivePins[type].Add(pin);
            minimapActivePins.Remove(column +  row *MapHolder.width);
        }
    }

    public static void PutBridgePin(int column, int row, int rotation, int sizeY, bool isAdd)
    {
        if (isAdd)
        {
            if (minimapInactivePins.ContainsKey(DecorationType.Bridge))
            {
                Image pin = minimapInactivePins[DecorationType.Bridge][minimapInactivePins[DecorationType.Bridge].Count - 1];
                minimapInactivePins[DecorationType.Bridge].RemoveAt(minimapInactivePins[DecorationType.Bridge].Count - 1);

                Vector2 position = new Vector2();
                position.x = (column * pixelSize + MapHolder.mapPrefab.miniMapOffset.x + pixelSize * 0.5f) * ratio.x;
                position.y = -(row * pixelSize + MapHolder.mapPrefab.miniMapOffset.y + pixelSize) * ratio.y;
                Quaternion rotate = Quaternion.identity;
                switch (rotation)
                {
                    case 0:
                        position.x += 0.5f;
                        position.y += 3.4f;
                        break;
                    case 1:
                        position.x -= 7;
                        position.y += 11;

                        if (MapHolder.tiles[column, row].type == TileType.WaterDiagonal)
                        {
                            position.x -= 2;
                            position.y -= 2;
                        }
                        rotate = Quaternion.Euler(0, 0, -45);


                break;
                    case 2:
                        position.x += 1.6f;
                        position.y += 11.6f;
                        rotate = Quaternion.Euler(0, 0, -90);
                        break;
                    case 3:
                        position.x += (-1 + 1);
                        position.y += (5 - 1);

                        if (MapHolder.tiles[column, row].type == TileType.WaterDiagonal)
                        {
                            position.x += 2;
                            position.y -= 2;
                        }
                        rotate = Quaternion.Euler(0, 0, 45);
                        break;
                }

                pin.transform.localPosition = position;
                pin.transform.localRotation = rotate;
                Vector2 size = pin.rectTransform.sizeDelta;
                switch (sizeY)
                {
                    case 3:
                        if (rotation != 0 && rotation != 2)
                        {
                            size.y = 36;
                        }
                        else
                        {
                            size.y = 26.25f;
                        }
                        break;
                    case 4:
                        size.y = 35;
                        break;
                    case 5:
                        size.y = 43.75f;
                        break;
                    case 6:
                        size.y = 42;
                        break;
                    case 7:
                        size.y = 48;
                        break;
                }
                pin.rectTransform.sizeDelta = size;

            minimapActivePins.Add(column + row * MapHolder.width, pin);
            }
        }
        else
        {
            Image pin = minimapActivePins[column + row * MapHolder.width];

            pin.transform.localPosition = hidePosition;

            minimapInactivePins[DecorationType.Bridge].Add(pin);
            minimapActivePins.Remove(column + row * MapHolder.width);
        }
    }

    public static void ChangeCameraPosition(Vector2 newPosition, float scroll)
    {
        Vector2 position = new Vector2();
        position.x = (newPosition.x * pixelSize + MapHolder.mapPrefab.miniMapOffset.x) * ratio.x;
        position.y = -(newPosition.y * pixelSize + MapHolder.mapPrefab.miniMapOffset.y) * ratio.y;
        cameraPosition.transform.localPosition = position;
        
        //cameraPosition.transform.localScale = Vector3.one * (Mathf.Log(scroll) + 1);
    }
    public static void RebuildMap()
    {
        var mapPrefab = MapHolder.mapPrefab;
        for (int i = 0; i < mapPrefab.height; i++)
        {
            for (int j = 0; j < mapPrefab.width; j++)
            {
                ChangeMiniMap(j,i, mapPrefab);
            }
        }

        // Apply all SetPixel calls
        miniMapTexture.Apply();
        UpdateMiniMap?.Invoke(miniMapTexture);
    }
}
