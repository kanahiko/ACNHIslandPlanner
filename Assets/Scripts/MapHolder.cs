using RotaryHeart.Lib.SerializableDictionary;
using System;
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

    public static void Save()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(@"D:\test.testSave", FileMode.Create)))
        {
            //writer.Write(width);
            //writer.Write(height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    writer.Write((byte)tiles[j,i].type);
                    writer.Write((byte)tiles[j, i].backgroundType);
                    for (int k = 0; k < 4; k++)
                     {
                         writer.Write((byte)tiles[j, i].prefabType[k]);
                     }
                     for (int k = 0; k < 4; k++)
                     {
                         writer.Write(tiles[j, i].cliffSidesType[k]);
                     }
                     writer.Write(tiles[j, i].diagonalRotation);
                     writer.Write(tiles[j, i].variation);
                     writer.Write(tiles[j, i].elevation);
                     writer.Write(tiles[j, i].curvedTileVariation);
                }
            }

            List<DecorationTiles> decorationTilesSave = new List<DecorationTiles>();
            List<Vector2Int> decorationTilesCoordinates = new List<Vector2Int>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (decorationsTiles[j,i] != null)
                    {
                        decorationTilesSave.Add(decorationsTiles[j, i]);
                        decorationTilesCoordinates.Add(new Vector2Int(j, i));
                    }
                }
            }
            writer.Write(decorationTilesSave.Count);

            for (int i = 0; i < decorationTilesSave.Count; i++)
            {
                writer.Write(decorationTilesCoordinates[i].x);
                writer.Write(decorationTilesCoordinates[i].y);
                writer.Write((byte)decorationTilesSave[i].type);
                //will check with type
                //writer.Write(tile.building != null);
                if (decorationTilesSave[i].building != null)
                {
                    writer.Write((byte)decorationTilesSave[i].building.type);
                    writer.Write(decorationTilesSave[i].building.startingColumn);
                    writer.Write(decorationTilesSave[i].building.startingRow);
                }
                for (int j = 0; j < 4; j++)
                {
                    writer.Write((byte)decorationTilesSave[i].isLinked[j]);
                }
                writer.Write(decorationTilesSave[i].rotation);
                writer.Write(decorationTilesSave[i].variation);
                writer.Write(decorationTilesSave[i].size);
            }
        }
    }
    public static void ResetInfluence()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                treeInfluence[j, i] = 0;
                buildingsInfluence[j, i] = BuildingInfluence.noInfluence;
            }
        }
    }
    public static void Load()
    {
        Dictionary<Vector2Int, List<Vector2Int>> buildings = new Dictionary<Vector2Int, List<Vector2Int>>();
        List<PreDecorationTile> preDecorationTiles = new List<PreDecorationTile>();

        using (BinaryReader reader = new BinaryReader(File.Open(@"D:\test.testSave", FileMode.Open)))
        {
            //width = reader.ReadInt32();
            //height = reader.ReadInt32();

            for (int i = 0; i< height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[j, i].type = (TileType)reader.ReadByte();
                    tiles[j, i].backgroundType = (TilePrefabType)reader.ReadByte();

                    for (int k = 0; k < 4; k++)
                    {
                        tiles[j, i].prefabType[k] = (TilePrefabType)reader.ReadByte();
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        tiles[j, i].cliffSidesType[k] = reader.ReadByte();
                    }
                    tiles[j, i].diagonalRotation = reader.ReadByte();
                    tiles[j, i].variation = reader.ReadByte();
                    tiles[j, i].elevation = reader.ReadByte();
                    tiles[j, i].curvedTileVariation = reader.ReadByte();
                }
            }
            int decorationsCount = reader.ReadInt32();

            for (int i = 0; i < decorationsCount; i++)
            {
                PreDecorationTile tile = new PreDecorationTile();
                tile.coordinates = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());

                tile.type = (DecorationType)reader.ReadByte();

                if (tile.type == DecorationType.Building)
                {
                    tile.buildingType = (DecorationType)reader.ReadByte();
                    tile.startingCoords = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());

                    if (!buildings.ContainsKey(tile.startingCoords))
                    {
                        buildings.Add(tile.startingCoords, new List<Vector2Int>());
                    }

                    buildings[tile.startingCoords].Add(tile.coordinates);
                }

                for (int j = 0; j < 4; j++)
                {
                    tile.isLinked[j] = (FenceLinked)reader.ReadByte();
                }
                tile.rotation = reader.ReadInt32();
                tile.variation = reader.ReadByte();
                tile.size = reader.ReadInt32();

                preDecorationTiles.Add(tile);
            }
        }

        ResetInfluence();
        Controller.RebuildMap?.Invoke(buildings, preDecorationTiles);
        MiniMap.RebuildMap();
        //Rebuild entire map
        //goes to building controller
    }
}

public class PreDecorationTile
{
    public Vector2Int coordinates;
    public DecorationType type;

    public DecorationType buildingType;
    public Vector2Int startingCoords;

    public FenceLinked[] isLinked = new FenceLinked[4];
    public int rotation;
    public byte variation;
    public int size;
}

[System.Serializable]
public class MinimapDecorationsDictionary : SerializableDictionaryBase<DecorationType, Image> { }

public class DecorationTiles : IDisposable
{
    public Transform decorationBackground;
    public DecorationType type;

    //TODO:change
    //public bool isHorizontal;
    public GameObject mainTile;
    public MeshRenderer mainTileRenderer;
    public Transform[] quarters;
    public FenceLinked[] isLinked;

    public int rotation;
    public int size;

    //to mark which tiles are not empty and where to start to make them empty
    public UniqueBuilding building;

    public byte variation;

    public DecorationTiles()
    {
        quarters = new Transform[4];
        isLinked = new FenceLinked[4];
        decorationBackground = new GameObject("DecorationBase").transform;
        decorationBackground.parent = MapHolder.decorationsParent;
    }
    public DecorationTiles(DecorationType type)
    {
        this.type = type;
        quarters = new Transform[4];
        isLinked = new FenceLinked[4];
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