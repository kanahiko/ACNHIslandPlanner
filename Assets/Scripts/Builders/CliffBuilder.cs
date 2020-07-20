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
    
    public static void CreateCliffDiagonal(MapTile tile)
    {
        if (tile.backgroundType == TilePrefabType.CliffDiagonal)
        {
            tile.backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90 * tile.diagonalRotation, 0);
            return;
        }
        byte rotation = tile.diagonalRotation;

        tile.HardErase();
        tile.RemoveCliffs();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.CliffDiagonal], tile.colliderObject.transform);
        tile.backgroundTile.transform.localPosition = Vector3.zero;
        tile.backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0,90*rotation,0);
        tile.backgroundType = TilePrefabType.CliffDiagonal;
        tile.diagonalRotation = rotation;
    }
    public static void RebuildCliffDiagonal(MapTile tile)
    {
        byte rotation = tile.diagonalRotation;

        tile.SoftErase();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.CliffDiagonal], tile.colliderObject.transform);
        tile.backgroundTile.transform.localPosition = Vector3.zero;
        tile.backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90 * rotation, 0);
        tile.backgroundType = TilePrefabType.CliffDiagonal;
    }
    public static void CreateCliffSides(int column, int row, MapTile tile)
    {
        int elevation = tile.elevation;
        bool isWater = tile.backgroundType == TilePrefabType.Water;
        
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

                if (tile.cliffSides[i] == null || tile.cliffSidesType[i] != cliffIndex)
                {
                    tile.RemoveCliff(i);
                    tile.cliffSidesType[i] = (byte)cliffIndex;
                    tile.cliffSides[i] = GameObject.Instantiate(MapHolder.mapPrefab.cliffSidePrefabs[cliffIndex], MapHolder.tiles[column, row].backgroundTile.transform);
                }
                tile.cliffSides[i].transform.localPosition = Util.halfOffset;
                tile.cliffSides[i].transform.localRotation = Quaternion.Euler(0, 90 * i, 0);
            }
            else
            {
                if (tile.cliffSides[i] != null)
                {
                    tile.RemoveCliff(i);
                }
            }
        }
    }

    public static void CreateCliffSides(MapTile tile)
    {
        tile.RemoveCliff();
        for (int i = 0; i < 4; i++)
        {
            if (tile.cliffSidesType[i] != 255) 
            {
                tile.cliffSides[i] = GameObject.Instantiate(MapHolder.mapPrefab.cliffSidePrefabs[tile.cliffSidesType[i]], tile.backgroundTile.transform);
                tile.cliffSides[i].transform.localPosition = Util.halfOffset;
                tile.cliffSides[i].transform.localRotation = Quaternion.Euler(0, 90 * i, 0);
            }
        }
    }

    public static void ChangeElevation(MapTile tile)
    {
        tile.SetElevation(MapHolder.elevationLevels[tile.elevation]);
        tile.SetPosition();
    }
}
 