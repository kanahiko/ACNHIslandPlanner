using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CliffBuilder
{
    public static MapPrefabs mapPrefab;
    
    public static void CreateCliffDiagonal(int column, int row)
    {
        if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.CliffDiagonal)
        {
                return;
        }
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        int[,] corners = Util.GetElevationCorners(column, row);
        int rotation = 0;
        for (int i = 0; i < 4; i++)
        {
                rotation = i;
                if (corners[0, 1] == MapHolder.tiles[column, row].elevation && corners[1, 0] == MapHolder.tiles[column, row].elevation)
                {
                        break;
                }
                corners = Util.RotateMatrix(corners);

        }

        MapHolder.tiles[column, row].HardErase();
        MapHolder.tiles[column, row].RemoveCliffs();
        MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(mapPrefab.lookUpTilePrefab[TilePrefabType.CliffDiagonal], MapHolder.elevationLevels[MapHolder.tiles[column, row].elevation]);
        MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
        MapHolder.tiles[column, row].backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0,90*rotation,0);
        MapHolder.tiles[column, row].backgroundType = TilePrefabType.CliffDiagonal;
    }
    
    public static void CreateCliffSides(int column, int row)
    {
        Debug.Log($"!!!!!");
        int elevation = MapHolder.tiles[column, row].elevation;
        bool isWater = MapHolder.tiles[column, row].backgroundType == TilePrefabType.Water;
        
        //checks all 4 sides using cross offset coordinates
        for (int i = 0; i < 4; i++)
        {
            if (!(column + Util.indexOffsetCross[i].y >= 0 && column + Util.indexOffsetCross[i].y < MapHolder.width && 
                row + Util.indexOffsetCross[i].x >= 0 && row + Util.indexOffsetCross[i].x < MapHolder.height))
            {
                continue;
            }
            
            //adds cliff if only current tile elevation bigger that side tile elevation
            if (elevation > MapHolder.tiles[column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x].elevation)
            {
                int cliffIndex = 4;
                if (isWater)
                {
                    Debug.Log($"start {column} {row} {cliffIndex} { Util.indexOffsetCross[i]}");
                    cliffIndex = 0;
                    
                    //uses offset cross coordinates in reverse (x -> y & y - >x) to check side tiles
                    if (row + Util.indexOffsetCross[i].y >= 0 && row + Util.indexOffsetCross[i].y < MapHolder.height &&
                        column + Util.indexOffsetCross[i].x >= 0 && column + Util.indexOffsetCross[i].x < MapHolder.width)
                    {
                        //if side left tile has water
                        if (MapHolder.grid[column + Util.indexOffsetCross[i].x + MapHolder.width * (row + Util.indexOffsetCross[i].y)] == TileType.Water)
                        {
                            cliffIndex += 1;
                        }
                    }
                    
                    if (row - Util.indexOffsetCross[i].y >= 0 && row - Util.indexOffsetCross[i].y < MapHolder.height &&
                        column - Util.indexOffsetCross[i].x >= 0 && column - Util.indexOffsetCross[i].x < MapHolder.width)
                    {
                        //if side right tile has water
                        if (MapHolder.grid[column -  Util.indexOffsetCross[i].x+ MapHolder.width * (row - Util.indexOffsetCross[i].y)] == TileType.Water)
                        {
                            cliffIndex += 2;
                        }
                    }
                }

                if (MapHolder.tiles[column, row].cliffSides[i] == null || MapHolder.tiles[column, row].cliffSidesType[i] != cliffIndex)
                {
                    MapHolder.tiles[column, row].RemoveCliff(i);
                    MapHolder.tiles[column, row].cliffSidesType[i] = cliffIndex;
                    MapHolder.tiles[column, row].cliffSides[i] = GameObject.Instantiate(mapPrefab.cliffSidePrefabs[cliffIndex], MapHolder.tiles[column, row].backgroundTile.transform);
                }
                MapHolder.tiles[column, row].cliffSides[i].transform.localPosition = Util.halfOffset;
                MapHolder.tiles[column, row].cliffSides[i].transform.localRotation = Quaternion.Euler(0, 90 * i, 0);
            }
            else
            {
                if (MapHolder.tiles[column, row].cliffSides[i] != null)
                {
                    MapHolder.tiles[column, row].RemoveCliff(i);
                }
            }
        }
    }

}
 