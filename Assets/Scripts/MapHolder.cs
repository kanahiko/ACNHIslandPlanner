using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    public static int[,] buildingsInfluence;

    public static void StartMapHolder()
    {
        width = mapPrefab.width;
        height = mapPrefab.height;
        
        decorationsTiles = new DecorationTiles[width, height];
        tiles = new MapTile[width, height];
        treeInfluence = new int[width, height];
        buildingsInfluence = new int[width,height];

        mapPrefab.StartPrefab();
    }
}
[System.Serializable]
public class MinimapDecorationsDictionary : SerializableDictionaryBase<DecorationType, Image> { }

public class DecorationTiles : IDisposable
{
    public Transform decorationBackground;
    public DecorationType type;

    public bool isHorizontal;
    public GameObject mainTile;
    public MeshRenderer mainTileRenderer;
    public Transform[] quarters;
    public bool[] isLinked;

    //to mark which tiles are not empty and where to start to make them empty
    public UniqueBuilding building;

    public int variation;

    public DecorationTiles()
    {
        quarters = new Transform[4];
        isLinked = new bool[4];
        decorationBackground = new GameObject("DecorationBase").transform;
        decorationBackground.parent = MapHolder.decorationsParent;
    }
    public DecorationTiles(DecorationType type)
    {
        this.type = type;
        quarters = new Transform[4];
        isLinked = new bool[4];
        decorationBackground = new GameObject("DecorationBase").transform;
        decorationBackground.transform.parent = MapHolder.decorationsParent;
    }

    public void AddMainTile(GameObject mainTile)
    {
        this.mainTile = mainTile;
        mainTile.transform.localPosition = Vector3.zero;
        mainTile.transform.localRotation = Quaternion.identity;
        mainTileRenderer = this.mainTile.GetComponent<MeshRenderer>();
    }

    public void Dispose()
    {
    }

    public void GoToLimbo()
    {
        decorationBackground.parent = MapHolder.limboDecorationsParent;
        decorationBackground.position = Util.cullingPosition;
        if (mainTile)
        {
            mainTileRenderer.enabled = false;
            //mainTile.SetActive(false);
        }

        if (building != null)
        {
            building.GoToLimbo();
        }
        /*for (int i = 0; i < 4; i++)
        {
            quarters[i] = null;
        }*/
    }
    public void ReturnFromLimbo()
    {
        //decorationBackground.parent = MapHolder.decorationsParent;
        if (mainTile)
        {
            mainTileRenderer.enabled = true;
            //mainTile.SetActive(true);
        }
    }
}

public class UniqueBuilding
{
    public DecorationType type;
    public DecorationTiles tile;

    public int startingColumn = -1;
    public int startingRow = -1;


    public Vector3Int size;
    public GameObject model;

    public UniqueBuilding(GameObject model, DecorationType type, Vector3Int size)
    {
        this.model = model;
        this.type = type;
        this.size = size;

        tile = new DecorationTiles(DecorationType.Building);
        tile.AddMainTile(model);
        tile.building = this;
        tile.GoToLimbo();
        model.transform.SetParent(tile.decorationBackground,false);
    }

    public void GoToLimbo()
    {
        startingColumn = -1;
        startingRow = -1;
    }
}