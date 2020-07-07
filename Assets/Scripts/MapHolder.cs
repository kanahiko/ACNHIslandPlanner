using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MapHolder
{
    public static TileType[] grid;

    public static MapTile[,] tiles;

}

public class MapTile
{
    public GameObject backgroundTile;

    public GameObject[] quarters;

    public MapTile(GameObject tile)
    {
        this.backgroundTile = tile;
        quarters = new GameObject[4];
    }
}