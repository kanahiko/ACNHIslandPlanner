using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MapHolder
{
    public static int maxElevation = 3;

    public static int width;
    public static int height;

    public static Vector3 offset;

    public static TileType[] grid;

    public static MapTile[,] tiles;


    public static List<Transform> elevationLevels;
    public static MapPrefabs mapPrefab;
    
    //public static 
}

public class DecorationTiles
{
    public DecorationType type;

    //to mark which tiles are not empty and where to start to make them empty
    public DecorationTiles nextInChain;
    public DecorationTiles beginningOfChain;

    public int variation;
}