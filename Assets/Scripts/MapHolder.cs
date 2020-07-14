using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    public static Dictionary<DecorationType, List<UniqueBuilding>> uniqueBuildings;

    public static void StartMapHolder()
    {
        width = mapPrefab.width;
        height = mapPrefab.height;
        
        decorationsTiles = new DecorationTiles[width, height];
        tiles = new MapTile[width, height];
        
        mapPrefab.StartPrefab();
        uniqueBuildings = new Dictionary<DecorationType, List<UniqueBuilding>>();

        foreach (var building in mapPrefab.maxCount)
        {
            if (mapPrefab.decorationsPrefabDictionary.ContainsKey(building.Key))
            {
                List<UniqueBuilding> list = new List<UniqueBuilding>();
                for (int i = 0; i < building.Value; i++)
                {
                    list.Add(new UniqueBuilding(GameObject.Instantiate(mapPrefab.decorationsPrefabDictionary[building.Key], limboDecorationsParent),
                        building.Key, mapPrefab.decorationsSizeDictionary[building.Key]));
                }
            }
        }
    }

    public static UniqueBuilding FindAvailiableBuilding(DecorationType type)
    {
        if (uniqueBuildings.ContainsKey(type))
        {
            foreach(var building in uniqueBuildings[type])
            {
                if (building.beginningOfChain == null)
                {
                    return building;
                }
            }
        }

        return null;
    }
}

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
        decorationBackground.position = Util.cullingPosition;
        if (mainTile)
        {
            mainTileRenderer.enabled = false;
            //mainTile.SetActive(false);
        }
        /*for (int i = 0; i < 4; i++)
        {
            quarters[i] = null;
        }*/
    }
    public void ReturnFromLimbo()
    {
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
    public DecorationTiles beginningOfChain;


    public Vector2Int size;
    public GameObject model;

    public UniqueBuilding(GameObject model, DecorationType type, Vector2Int size)
    {
        this.model = model;
        model.transform.position = Util.cullingPosition;
        //model.SetActive(false);
        this.type = type;
        this.size = size;
    }
}