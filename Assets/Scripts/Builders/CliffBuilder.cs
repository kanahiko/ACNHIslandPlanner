using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CliffBuilder
{
    public static bool CheckCliff(int column, int row, TileType previousTileType,ref ToolMode toolMode,ref int cliffConstructionElevationLevel, out TileType newType)
    {
        newType = TileType.Land;
        if (cliffConstructionElevationLevel != MapHolder.tiles[column, row].elevation || previousTileType == TileType.Sand || previousTileType == TileType.Sea)
        {
            return false;
        }
        if ( MapHolder.tiles[column, row].elevation > 0)
        {
            //if cliff diagonal
            if (MapHolder.tiles[column, row].type == TileType.CliffDiagonal)
            {
                if (Util.ToolModeChange(true, column, row, TileType.Land, ref toolMode))
                {
                    cliffConstructionElevationLevel -= 1;
                    return true;
                }
            }
            else
            {
                if (CheckCanCurveCliff(column, row, MapHolder.tiles[column,row]))
                {
                    if (toolMode == ToolMode.None)
                    {
                        toolMode = ToolMode.Add;
                        MapHolder.tiles[column, row].type = TileType.CliffDiagonal;
                        
                        cliffConstructionElevationLevel -= 1;

                        newType = TileType.CliffDiagonal;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Util.CheckSurroundedBySameElevation(column, row))
                    {
                        return false;
                    }
                    if (MapHolder.tiles[column, row].elevation < MapHolder.maxElevation)
                    {
                        if (Util.ToolModeChange(true, column, row, TileType.Land,ref toolMode))
                        {
                            MapHolder.tiles[column, row].elevation += 1;
                            newType = TileType.Cliff;

                            return true;
                        }
                    }
                }
            }

        }
        else
        {
            if (Util.ToolModeChange(true, column, row, TileType.Land, ref toolMode))
            {
                MapHolder.tiles[column, row].elevation += 1;
                newType = TileType.Cliff;

                return true;
            }
        }
        return false;
    }
    
    public static bool CheckCanCurveCliff(int column,int row, MapTile tile = null)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int[] elevations = new int[7];

        if (row + 1 < MapHolder.width)
        {
            elevations[1] = MapHolder.tiles[column, row + 1].elevation;
        }

        if (column - 1 >= 0)
        {
            elevations[2] = MapHolder.tiles[column - 1, row].elevation;
        }

        if (row - 1 >= 0 )
        {
            elevations[3] = MapHolder.tiles[column, row - 1].elevation;
        }

        if (column + 1 < MapHolder.height)
        {
            elevations[4] = MapHolder.tiles[column + 1, row].elevation;
        }

        elevations[5] = elevations[1];
        elevations[6] = elevations[2];
        elevations[0] = elevations[4];
        for (int i = 1; i < 5; i++)
        {
            if ((elevations[i] == elevation && elevations[i - 1] == elevation &&
                 elevations[i + 1] < elevation && elevations[i + 2] < elevation))
            {
                if (tile != null)
                {
                    tile.diagonalRotation = Util.SubstractRotation(i - 1,2);
                }
                return true;
            }
        }

        return false;
    }
    
    public static void CreateCliffDiagonal(int column, int row)
    {
        if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.CliffDiagonal)
        {
                return;
        }
        //int[,] corners = Util.GetElevationCorners(column, row);
        int rotation = MapHolder.tiles[column, row].diagonalRotation;
        /*for (int i = 0; i < 4; i++)
        {
                rotation = i;
                if (corners[0, 1] == MapHolder.tiles[column, row].elevation && corners[1, 0] == MapHolder.tiles[column, row].elevation)
                {
                        break;
                }
                corners = Util.RotateMatrix(corners);

        } */

        MapHolder.tiles[column, row].HardErase();
        MapHolder.tiles[column, row].RemoveCliffs();
        MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.CliffDiagonal],MapHolder.tiles[column,row].colliderObject.transform);//, MapHolder.elevationLevels[MapHolder.tiles[column, row].elevation]);
        MapHolder.tiles[column, row].backgroundTile.transform.localPosition = Vector3.zero;//new Vector3(column, 0, -row);
        MapHolder.tiles[column, row].backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0,90*rotation,0);
        MapHolder.tiles[column, row].backgroundType = TilePrefabType.CliffDiagonal;
    }
    
    public static void CreateCliffSides(int column, int row)
    {
        //MapHolder.tiles[column, row].diagonalRotation = -1;
        //Debug.Log($"!!!!!");
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
                    //Debug.Log($"start {column} {row} {cliffIndex} { Util.indexOffsetCross[i]}");
                    cliffIndex = 0;
                    
                    //uses offset cross coordinates in reverse (x -> y & y - >x) to check side tiles
                    if (Util.CoordinateExists(column+Util.indexOffsetCross[i].x,row + Util.indexOffsetCross[i].y))
                    {
                        //if side left tile has water
                        if (MapHolder.tiles[column + Util.indexOffsetCross[i].x , row + Util.indexOffsetCross[i].y].type == TileType.Water)
                        {
                            cliffIndex += Mathf.Abs(Util.indexOffsetCross[i].x) * 1 +Mathf.Abs(Util.indexOffsetCross[i].y)*2;
                        }
                    }
                    
                    if (Util.CoordinateExists(column-Util.indexOffsetCross[i].x,row - Util.indexOffsetCross[i].y))
                    {
                        //if side right tile has water
                        if (MapHolder.tiles[column -  Util.indexOffsetCross[i].x, row - Util.indexOffsetCross[i].y].type == TileType.Water)
                        {
                            cliffIndex += Mathf.Abs(Util.indexOffsetCross[i].x) * 2 +Mathf.Abs(Util.indexOffsetCross[i].y)*1;
                            //cliffIndex += 2;
                        }
                    }
                }

                if (MapHolder.tiles[column, row].cliffSides[i] == null || MapHolder.tiles[column, row].cliffSidesType[i] != cliffIndex)
                {
                    MapHolder.tiles[column, row].RemoveCliff(i);
                    MapHolder.tiles[column, row].cliffSidesType[i] = cliffIndex;
                    MapHolder.tiles[column, row].cliffSides[i] = GameObject.Instantiate(MapHolder.mapPrefab.cliffSidePrefabs[cliffIndex], MapHolder.tiles[column, row].backgroundTile.transform);
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
 