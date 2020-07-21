using System;
using UnityEngine;

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

    public int startingColumn = -1;
    public int startingRow = -1;

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

        startingColumn = -1;
        startingRow = -1;
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

    //public int startingColumn = -1;
    //public int startingRow = -1;


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
        model.transform.SetParent(tile.decorationBackground, false);
    }

    public void GoToLimbo()
    {
        //startingColumn = -1;
        //startingRow = -1;
    }
}
