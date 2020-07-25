using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class MapHolder
{
    public static Transform decorationsParent;
    public static Transform limboDecorationsParent;

    public static int maxElevation = 3;

    public static int width;
    public static int height;

    public static Vector3 offset;

    //public static TileType[,] grid;
    public static MapTile[,] tiles;

    public static DecorationTiles[,] decorationsTiles;

    public static List<Transform> elevationLevels;
    public static MapPrefabs mapPrefab;

    public static int[,] treeInfluence;
    public static BuildingInfluence[,] buildingsInfluence;

    public static bool isDirty = false;
    
    public static void StartMapHolder()
    {
        width = mapPrefab.width;
        height = mapPrefab.height;
        
        decorationsTiles = new DecorationTiles[width, height];
        tiles = new MapTile[width, height];
        treeInfluence = new int[width, height];
        buildingsInfluence = new BuildingInfluence[width,height];

        mapPrefab.StartPrefab();
    }

    
}

[System.Serializable]
public class MinimapDecorationsDictionary : SerializableDictionaryBase<DecorationType, Image> { }

