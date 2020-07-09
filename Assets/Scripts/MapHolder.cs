using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MapHolder
{
    public static int width;
    public static int height;

    public static Vector3 offset;

    public static TileType[] grid;

    public static MapTile[,] tiles;

    //public static Transform parent;

    public static List<Transform> elevationLevels;
}

public class MapTile
{
    public GameObject backgroundTile;

    public GameObject[] quarters;
    public GameObject[] cliffSides;

    public TilePrefabType backgroundType;

    public TilePrefabType[] type;

    public int diagonalPathRotation = -1;

    //public bool isDirty;

    public int elevation;

    public MapTile(GameObject tile)
    {
        this.backgroundTile = tile;
        quarters = new GameObject[4];
        type = new TilePrefabType[4];
        cliffSides = new GameObject[4];
        diagonalPathRotation = -1;
        //isDirty = false;
    }
    public MapTile()
    {
        quarters = new GameObject[4];
        type = new TilePrefabType[4];
        cliffSides = new GameObject[4];
        diagonalPathRotation = -1;
        //isDirty = false;
    }

    public Vector2Int GetDirectionOfPath()
    {
        if (diagonalPathRotation != -1)
        {
            return new Vector2Int(Util.SubstractRotation(diagonalPathRotation, 2), Util.SubstractRotation(diagonalPathRotation, 1));
        }
        return new Vector2Int(-1, -1);
    }

    public void SoftErase()
    {
        //isDirty = true;
        //diagonalPathRotation = -1;
        Debug.Log("fuck1");
    }
    public void HardErase()
    {
        //isDirty = false;
        Debug.Log("fuck2");
        diagonalPathRotation = -1;
        GameObject.Destroy(backgroundTile);
        backgroundTile = null;
        for(int i=0;i<4;i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
                type[i] = TilePrefabType.Null;
            }
        }
    }
    public void EraseQuarters()
    {
        //isDirty = false;
        for (int i = 0; i < 4; i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
                type[i] = TilePrefabType.Null;
            }
        }
        Debug.Log("fuck3");
        diagonalPathRotation = -1;
    }
    public void EraseQuarters(int exception1, int exception2 = -1, int exception3 = -1)
    {
        Debug.Log("fuck4");
        //isDirty = false;
        for (int i = 0; i < 4; i++)
        {
            if (i != exception1 && i != exception2 && i != exception3)
            {
                if (quarters[i] != null)
                {
                    GameObject.Destroy(quarters[i]);
                    quarters[i] = null;
                    type[i] = TilePrefabType.Null;
                }
            }
        }
    }
    public void RemoveQuarter(int index)
    {
        if (quarters[index] != null)
        {
            GameObject.Destroy(quarters[index]);
            quarters[index] = null;
            type[index] = TilePrefabType.Null;
        }
    }

    public void RemoveCliff()
    {
        for (int i = 0; i < 4; i++)
        {
            if (cliffSides[i] != null)
            {
                GameObject.Destroy(cliffSides[i]);
                cliffSides[i] = null;
            }
        }

        elevation -= 1;
    }
}