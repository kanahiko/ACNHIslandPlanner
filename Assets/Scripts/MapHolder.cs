using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MapHolder
{
    public static int width=112;
    public static int height=96;

    public static Vector3 offset;

    public static TileType[] grid;

    public static MapTile[,] tiles;

    public static Transform parent;
}

public class MapTile
{
    public GameObject backgroundTile;

    public GameObject[] quarters;

    public TilePrefabType backgroundType;

    public TilePrefabType[] type;

    public bool isDirty;

    public MapTile(GameObject tile)
    {
        this.backgroundTile = tile;
        quarters = new GameObject[4];
        type = new TilePrefabType[4];
        isDirty = false;
    }

    public Vector2Int GetDirectionOfPath()
    {
        for (int i = 0; i < 4; i++)
        {
            if (quarters[i] != null && type[i] == TilePrefabType.PathCurved)
            {
                return new Vector2Int(Util.SubstractRotation(i, 2), Util.SubstractRotation(i,1));
            }
        }

        return new Vector2Int(-1,-1);
    }

    public void SoftErase()
    {
        isDirty = true;
    }
    public void HardErase()
    {
        isDirty = false;
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
        isDirty = false;
        for (int i = 0; i < 4; i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
                type[i] = TilePrefabType.Null;
            }
        }
    }
    public void EraseQuarters(int exception1, int exception2 = -1, int exception3 = -1)
    {
        isDirty = false;
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
}