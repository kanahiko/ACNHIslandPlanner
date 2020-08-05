using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMapFromPicture : MonoBehaviour
{
    public CreateGrid gridCreator;

    public List<ColorToType> colors;
    public MapTileToColorDictionary convertColors;

    public RawImage testImage;
    public RawImage testAreaImage;

    public RectTransform currentArea;
    Vector2Int chunkSize;
    Vector2Int start;
    Vector2 tileSize;
    Vector2Int tile;
    public byte maxDeviation = 60;
    MapInfoHelper info;
    Texture2D test;
    Texture2D testArea;
    private void Awake()
    {
        gridCreator.CreateMap += Create;
    }

    Vector2Int currentIndex;
    public void Create(MapInfoHelper info)
    {
        this.info = info;
        test = new Texture2D(112, 96, TextureFormat.RGBA32, false);
        test.filterMode = FilterMode.Point;

        chunkSize = new Vector2Int(info.xChunkOffset, info.yChunkOffset);

        start = new Vector2Int(info.xOffsetMap + (info.xfirstChunkOffset - chunkSize.x),info.yOffsetMap + info.yMapSize - info.yfirstChunkOffset - chunkSize.y *5);

        tileSize = new Vector2(chunkSize.x / 16f,chunkSize.y / 16f);

        tile = new Vector2Int((int)tileSize.x, (int)tileSize.y);
        testArea = new Texture2D(tile.x, tile.y, TextureFormat.RGBA32, false);
        testArea.filterMode = FilterMode.Point;
        currentArea.sizeDelta = tileSize;

        currentIndex = new Vector2Int();
        DoTile(0, 0);
        test.Apply();
        testImage.texture = test;
        testArea.Apply();
        testAreaImage.texture = testArea;
        currentArea.position = new Vector2(start.x + (tileSize.x * currentIndex.x), start.y + (tileSize.y * currentIndex.y));
    }

    void DoTile(int x, int y)
    {
        int coordX = start.x + (int)(tileSize.x * x);
        int coordY = start.y + (int)(tileSize.y * y);
        int r = 0;
        int g = 0;
        int b = 0;
        Dictionary<MapTileT, int> countColor = new Dictionary<MapTileT, int>();
        for (int k = 0; k < tile.y; k++)
        {
            for (int l = 0; l < tile.x; l++)
            {
                Color32 pixelColor = info.mapImage.GetPixel(coordX, coordY);
                testArea.SetPixel(l, k, pixelColor);
                MapTileT tile = FindColor(pixelColor);
                if (countColor.ContainsKey(tile))
                {
                    countColor[tile] += 1;
                }
                else
                {
                    countColor.Add(tile, 1);
                }
                r += pixelColor.r;
                g += pixelColor.g;
                b += pixelColor.b;
            }
        }
        Color32 finalColor = FindMostUsedColor(countColor, x, y);

        test.SetPixel(x, y, finalColor);
    }

    public void Next()
    {
        currentIndex.x++;
        if (currentIndex.x >= 112)
        {
            currentIndex.x = 0;
            currentIndex.y++;
        }

        if (currentIndex.y >= 96)
        {
        }
        else
        {
            DoTile(currentIndex.x, currentIndex.y);
            currentArea.position = new Vector2(start.x + (tileSize.x * currentIndex.x), start.y + (tileSize.y * currentIndex.y));
        }
        test.Apply();
        testImage.texture = test;

        testArea.Apply();
        testAreaImage.texture = testArea;
    }
    public void Next(int value)
    {
        for (int i = 0; i < value; i++)
        {
            currentIndex.x++;
            if (currentIndex.x >= 112)
            {
                currentIndex.x = 0;
                currentIndex.y++;
            }

            if (currentIndex.y >= 96)
            {
            }
            else
            {
                DoTile(currentIndex.x, currentIndex.y);
                currentArea.position = new Vector2(start.x + (tileSize.x * currentIndex.x), start.y + (tileSize.y * currentIndex.y));
            }
        }
        test.Apply();
        testImage.texture = test;

        testArea.Apply();
        testAreaImage.texture = testArea;
    }

    public void Finish()
    {
        while (currentIndex.y< 96)
        {
            currentIndex.x++;
            if (currentIndex.x >= 112)
            {
                currentIndex.x = 0;
                currentIndex.y++;
            }

            if (currentIndex.y >= 96)
            {
            }
            else
            {
                DoTile(currentIndex.x, currentIndex.y);
            }
        }
        test.Apply();
        testImage.texture = test;
    }

    Color32 FindMostUsedColor(Dictionary<MapTileT, int> countColor, int x, int y)
    {
        MapTileT tile = new MapTileT();
        int uses = -1;
        foreach(var used in countColor)
        {
            if (uses < used.Value)
            {
                tile = used.Key;
                uses = used.Value;
            }
        }
        if (tile == MapTileT.Sea && 
            x >= 16 && x < 96 &&
            y >= 16 && y < 80)
        {
            return convertColors[MapTileT.Water];
        }

        if (tile == MapTileT.Path &&
            !(y >= 16 && y < 80))
        {

            return convertColors[MapTileT.Sea];
        }

        if (tile == MapTileT.Sand &&
            (x >= 16 && x < 96 &&
            y >= 16 && y < 80))
        {
            return convertColors[MapTileT.Path];
        }
        return convertColors[tile];
    }
    MapTileT FindColor(Color32 color)
    {
        Color32 closestColor = colors[0].color;
        int fitness = CheckColorFitness(color, closestColor);
        MapTileT tile = colors[0].type;
        for (int i=1;i< colors.Count; i++)
        {
            int newFitness = CheckColorFitness(color, colors[i].color);

            if (newFitness < fitness)
            {
                closestColor = colors[i].color;
                fitness = newFitness;
                tile = colors[i].type;
            }
        }

        return tile;
    }
    int CheckColorFitness(Color32 color, Color32 comparedColor)
    {
        return Mathf.Abs(color.r - comparedColor.r) + Mathf.Abs(color.g - comparedColor.g) + Mathf.Abs(color.b - comparedColor.b);
    }
    bool CheckColor(Color32 color, Color32 comparedColor)
    {
        return Mathf.Abs(color.r - comparedColor.r) + Mathf.Abs(color.g - comparedColor.g) + Mathf.Abs(color.b - comparedColor.b) < maxDeviation;
    }
}

public class MapInfoHelper
{
    public Texture2D mapImage;
    public int xMapSize;
    public int yMapSize;

    public int xOffsetMap; 
    public int yOffsetMap;
    public int xMapStart;
    public int yMapStart;
    public int xMapEnd; 
    public int yMapEnd;
    public int xfirstChunkOffset;
    public int yfirstChunkOffset;
    public int xChunkOffset; 
    public int yChunkOffset;
}

[System.Serializable]
public class ColorToType
{
    public Color32 color;
    public MapTileT type;
}

public enum MapTileT
{
    Sea=0, Sand = 1, Land0 = 2, Land1 = 3, Land2 = 4, Path = 5, Water = 6
}
[System.Serializable]
public class MapTileToColorDictionary : SerializableDictionaryBase<MapTileT, Color> { }
