using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PathBuilder
{
    public static void CreatePath(int column, int row, int elevationLevel)
    {
        if (MapHolder.tiles[column,row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType != TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(
                    MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land]);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
                MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], MapHolder.tiles[column,row].colliderObject.transform));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
        }
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;

        TileType[,] corners = Util.CreateMatrix(column, row);

        if (MapHolder.tiles[column,row].type == TileType.PathCurve)
        {
            CreateCurvedPath(corners, column, row);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindCornerPath(corners, k, column, row);
                corners = Util.RotateMatrix(corners);
            }
            MapHolder.tiles[column, row].diagonalPathRotation = -1;
        }
        if (elevationLevel > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
        }
    }
    public static void FindCornerPath(TileType[,] corners, int rotation, int column, int row)
    {
        TilePrefabType type = TilePrefabType.PathFull;
        if (corners[0, 1] == TileType.Land)
        {
            if (corners[1, 0] == TileType.Land)
            {
                type = TilePrefabType.PathCorner;
            }
            else
            {
                type = TilePrefabType.PathSide;
            }
        }
        else
        {
            if (corners[1, 0] == TileType.Land)
            {
                type = TilePrefabType.PathSideRotated;
            }
            else
            {
                if (corners[0, 0] == TileType.Land)
                {
                    type = TilePrefabType.PathSmallCorner;
                }
            }
        }

        Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);

        GameObject prefab = MapHolder.mapPrefab.prefabDictionary[type];
        if (type == TilePrefabType.PathSideRotated)
        {
            rotate *= Quaternion.Euler(0, -90, 0);
        }

        if (MapHolder.tiles[column, row].prefabType[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);
               MapHolder.tiles[column, row].prefabType[rotation] = type;
            //creates corner and adds its reference to MapHolder
            if (prefab != null)
            {
                GameObject tile = GameObject.Instantiate(prefab, MapHolder.tiles[column, row].backgroundTile.transform);
                tile.transform.localPosition = Util.offset[rotation];
                tile.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = tile;
            }
        }
        else
        {
            MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = Util.offset[rotation];
            MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
        }

        //MapHolder.tiles[row * MapHolder.width + column] = TileType.Path;
    }

    public static void CreateCurvedPath(TileType[,] corners, int column, int row)
    {
        
        int rotation = MapHolder.tiles[column, row].diagonalPathRotation;

        int oppositeRotation = Util.SubstractRotation(rotation, 2);

        //rotates corners matrix for easier calculations
        for (int i = 0; i < rotation; i++)
        {
            corners = Util.RotateMatrix(corners);
        }

        //check which curved path needs to be in place
        int curvedTile = 0; //1  = only down| 2 = only right | 3 = both sides dont extend anwhere

        if (corners[1, 2] != TileType.Path && corners[1, 2] != TileType.PathCurve)
        {
            curvedTile += 1;
        }
        if (corners[2, 1] != TileType.Path && corners[2, 1] != TileType.PathCurve)
        {
            curvedTile += 2;
        }
        
        //useless to do it another way
        //TODO:add better prefab type check
        MapHolder.tiles[column, row].RemoveQuarters();

        //opposite tile cant be created if both sides are not path or curved path
        if (curvedTile == 0)
        {
            GameObject oppositePrefab = null;
            var oppositePrefabType = (corners[2, 2] != TileType.Path && corners[2, 2] != TileType.PathCurve) ? TilePrefabType.PathSmallCorner : TilePrefabType.PathFull;

            oppositePrefab = MapHolder.mapPrefab.prefabDictionary[oppositePrefabType];
            MapHolder.tiles[column, row].prefabType[oppositeRotation] = oppositePrefabType;
            //creates opposite corner and adds its reference to MapHolder
            GameObject oppositeTile = GameObject.Instantiate(oppositePrefab,MapHolder.tiles[column, row].backgroundTile.transform);

            oppositeTile.transform.localPosition = Util.offset[oppositeRotation];
            oppositeTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);
            MapHolder.tiles[column, row].quarters[oppositeRotation] = oppositeTile;
            
        }

        //creates curved corner and adds its reference to MapHolder
        GameObject tile = GameObject.Instantiate(MapHolder.mapPrefab.specialCurvedPath[curvedTile], MapHolder.tiles[column, row].backgroundTile.transform);
        tile.transform.localPosition = Util.halfOffset;
        tile.transform.localRotation = Quaternion.Euler(0, rotation * 90, 0); 
        
        MapHolder.tiles[column, row].quarters[rotation] = tile;
        MapHolder.tiles[column, row].prefabType[rotation] = TilePrefabType.PathCurved;
        Debug.Log($"path rotation {rotation}");
        MapHolder.tiles[column, row].diagonalPathRotation = rotation;
        //MapHolder.tiles[row * MapHolder.width + column] = TileType.PathCurve;
        //Debug.Log($"{MapHolder.tiles[column, row].GetDirectionOfPath()}");
        //MapHolder.tiles[column, row].diagonalPathRotation = rotation;


    }
}