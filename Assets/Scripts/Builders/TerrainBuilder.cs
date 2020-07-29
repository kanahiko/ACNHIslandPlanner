using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainBuilder : MonoBehaviour
{
    ToolMode toolMode = ToolMode.None;

    int cliffConstructionElevationLevel = -1;

    HashSet<MapTile> ignoreRaycastTiles;

    Action<HashSet<Vector2Int>> ChangeMiniMap;
    
    HashSet<Vector2Int> changedCoordinates;
    
    // Start is called before the first frame update
    void Awake()
    {             
        ignoreRaycastTiles = new HashSet<MapTile>();
        changedCoordinates = new HashSet<Vector2Int>();
        ChangeMiniMap = MiniMap.ChangeMiniMap;
    }
    

    public void ChangeTile(ToolType type, ToolMode mode, int column, int row, byte variation)
    {
        //TODO building influence enum, switch to bool variables
        bool canPath = type == ToolType.PathPermit;
        if (canPath && MapHolder.decorationsTiles[column, row] != null)
        {
            if (MapHolder.buildingsInfluence[column, row] == BuildingInfluence.pathsOnly)
            {
                canPath = true;
            }
            if (MapHolder.decorationsTiles[column, row].type == DecorationType.Bridge)
            {
                canPath = MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land;
            }
        }

        if ((MapHolder.buildingsInfluence[column, row] == BuildingInfluence.fullInfluence ||
            type != ToolType.PathPermit && MapHolder.treeInfluence[column,row] > 0 || 
            MapHolder.decorationsTiles[column, row] != null && (!canPath)))
        {
            return;
        }
        

        if (mode == ToolMode.Add)
        {
            ChangeTile(type, column, row,variation);
        }
        else
        {
            RemoveTile(type, column, row);
        }
    }

    public void RebuildTile(MapTile tile)
    {
        /*bool yes = false;
        if (tile.elevation != 0)
        {
            yes = true;
            Debug.Log(tile.elevation);
        }*/
        switch (tile.type)
        {
            case TileType.Land:
                LandBuilder.RebuildLandTile(tile);
                break;
            case TileType.Water:
                WaterBuilder.RebuildWaterCorner(tile);
                break;
            case TileType.WaterDiagonal:
                WaterBuilder.RebuildDiagonalWater(tile);
                break;
            case TileType.Path:
                PathBuilder.RebuildPathCorner(tile);
                break;
            case TileType.PathCurve:
                PathBuilder.RebuildPathDiagonal(tile);
                break;
            case TileType.CliffDiagonal:
                CliffBuilder.RebuildCliffDiagonal(tile);
                break;
            case TileType.Sea:
                LandBuilder.RebuildSeaTile(tile);
                break;
            case TileType.SeaDiagonal:
                LandBuilder.RebuildSeaDiagonal(tile);
                break;
            case TileType.Sand:
                LandBuilder.RebuildSandTile(tile);
                break;
            case TileType.SandDiagonal:
                LandBuilder.RebuildSandDiagonal(tile);
                break;
        }
        /*if (yes)
        {
            Debug.Log("aaas " +tile.elevation);
        }*/
        if (tile.elevation > 0)
        {
            CliffBuilder.ChangeElevation(tile);
            CliffBuilder.CreateCliffSides(tile);
        }
    }

    void ChangeTile(ToolType type, int column, int row, byte variation)
    {
        changedCoordinates.Clear();
        TileType previousTileType = MapHolder.tiles[column,row].type;
        TileType newType = TileType.Null;
        switch (type)
        {
            case ToolType.SandPermit:
                if (!LandBuilder.CheckSandTile(column, row, previousTileType, ref toolMode))
                {
                    return;
                }
                newType = MapHolder.tiles[column, row].type;
                break;
            case ToolType.Waterscaping:
                if (!WaterBuilder.CheckWater(column,row,previousTileType,ref toolMode))
                {
                    return;
                }

                newType = MapHolder.tiles[column, row].type;
                break;
            case ToolType.PathPermit:
                if (!PathBuilder.CheckPath(column,row,previousTileType,ref toolMode,variation))
                {
                    return;
                }
                newType = MapHolder.tiles[column, row].type;
                break;
            case ToolType.CliffConstruction:
                
                if (CliffBuilder.CheckCliff(column,row,previousTileType,ref toolMode,ref cliffConstructionElevationLevel, out newType))
                {
                    //MapHolder.tiles[column, row].IgnoreElevation();
                    ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                }
                else
                {
                    return;
                }
                break;
        }
        changedCoordinates.Add(new Vector2Int(column, row));
        CreateTile(column, row, newType, variation);
        ChangeMiniMap?.Invoke(changedCoordinates);
    }

    

    void RemoveTile(ToolType type, int column, int row)
    {
        changedCoordinates.Clear();
        toolMode = ToolMode.Remove;
        
        TileType previousTileType = MapHolder.tiles[column, row].type;
        switch (type)
        {
            case ToolType.Waterscaping:
                if (previousTileType == TileType.Water || previousTileType == TileType.WaterDiagonal)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case ToolType.PathPermit:
                if (previousTileType == TileType.Path || previousTileType == TileType.PathCurve)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case ToolType.CliffConstruction:
                if (cliffConstructionElevationLevel == MapHolder.tiles[column, row].elevation && MapHolder.tiles[column, row].elevation > 0)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    MapHolder.tiles[column, row].elevation -= 1;
                    if (Util.CanRemoveCliff(column,row))
                    {
                        RemoveCliff(column, row, TileType.Cliff);
                        CreateTile(column, row, MapHolder.tiles[column, row].type);
                        
                        MapHolder.tiles[column, row].IgnoreElevation();
                        ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                        changedCoordinates.Add(new Vector2Int(column, row));
                    }
                    else
                    {
                        MapHolder.tiles[column, row].type = previousTileType;
                        MapHolder.tiles[column, row].elevation += 1;
                    }
                }
                break;
            case ToolType.SandPermit:
                if (MapHolder.tiles[column, row].type == TileType.Sand || MapHolder.tiles[column, row].type == TileType.SeaDiagonal || 
                    MapHolder.tiles[column,row].type == TileType.Land && MapHolder.tiles[column,row].elevation == 0 && 
                    Util.CoordinateOnBorderChunks(column,row))
                {
                    MapHolder.tiles[column, row].type = TileType.Sea;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                    
                }
                if (MapHolder.tiles[column, row].type == TileType.SandDiagonal)
                {
                    MapHolder.tiles[column, row].type = TileType.Sand;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
        }
        ChangeMiniMap?.Invoke(changedCoordinates);
    }

    void RemoveCliff(int column, int row, TileType type)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (Util.CoordinateExists(column + i,row +j))
                {
                    TileType influencee = MapHolder.tiles[column + i, row + j].type;
                    if (influencee == TileType.CliffDiagonal && !CliffBuilder.CheckCanCurveCliff(column + i, row + j))
                    {
                        MapHolder.tiles[column + i, row + j].type = TileType.Land;
                    }

                    if (influencee == TileType.Water || influencee == TileType.WaterDiagonal)
                    {
                        if ((MapHolder.tiles[column + i, row + j].elevation > 0 &&
                             !Util.CheckHalfSurroundedBySameElevation(column + i, row + j)))
                        {
                            MapHolder.tiles[column + i, row + j].type = TileType.Land;
                            changedCoordinates.Add(new Vector2Int(column + i, row + j));
                        }else
                        {
                            if (!WaterBuilder.CheckCanCurveWater(column + i, row + j))
                            {
                                MapHolder.tiles[column + i, row + j].type = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }
                    RedoTile(column + i, row + j);
                }
            }
        }

        //cliff can influence larger circle
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (i >= 1 && i <= -1 && !(j == -2 || j == -2))
                {
                    //already check this circle
                    continue;
                }

                if (Util.CoordinateExists(column + i,row +j))
                {
                    //int influenceeIndex = Util.GetIndex(column + i, row + j);
                    TileType influencee = MapHolder.tiles[column + i, row + j].type;

                    if (influencee == TileType.Water || influencee == TileType.WaterDiagonal)
                    {
                        if ((MapHolder.tiles[column + i, row + j].elevation > 0 &&
                             !Util.CheckHalfSurroundedBySameElevation(column + i, row + j)))
                        {
                            MapHolder.tiles[column + i, row + j].type = TileType.Land;
                            changedCoordinates.Add(new Vector2Int(column + i, row+j));
                        }
                        else
                        {
                            if (!WaterBuilder.CheckCanCurveWater(column + i, row + j))
                            {
                                MapHolder.tiles[column + i, row + j].type = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }

                    RedoTile(column + i, row + j);
                }
            }
        }
    }

    void CreateTile(int column, int row, TileType type, byte variation = 0)
    {
        MapHolder.tiles[column, row].variation = variation;
        MapHolder.tiles[column, row].curvedTileVariation = 255;
        RecalculateDiagonals(column, row, type);

        switch (type)
        {
            case TileType.Null:
                break;
            case TileType.Land:
                LandBuilder.CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Water:
            case TileType.WaterDiagonal:
                WaterBuilder.MakeWaterTile(column, row, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, MapHolder.tiles[column, row].elevation, variation);
                break;
            case TileType.Cliff:
                LandBuilder.CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.CliffDiagonal:
                CliffBuilder.CreateCliffDiagonal(MapHolder.tiles[column, row]);
                MapHolder.tiles[column, row].IgnoreElevation();
                ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                break;
            case TileType.Sand:
                LandBuilder.CreateSandTile(column,row);
                break;
            case TileType.SandDiagonal:
                LandBuilder.CreateDiagonalSand(column,row);
                break;
            case TileType.Sea:
                LandBuilder.CreateSeaTile(column,row);
                break;
            case TileType.SeaDiagonal:
                LandBuilder.CreateDiagonalSea(column,row);
                break;
        }
    }

    

    void RecalculateDiagonals(int column,int row, TileType type)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int variation = MapHolder.tiles[column, row].variation;
        TileType influencer = type;
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (Util.CoordinateExists(column + i,row +j))
                {
                    if (elevation > MapHolder.tiles[column + i,row + j].elevation)
                    {
                        influencer = TileType.Cliff;
                    }
                    else
                    {   if (elevation == MapHolder.tiles[column + i, row + j].elevation)
                        {
                            influencer = type;
                        }
                        else
                        {
                            influencer = TileType.Null;
                        }
                    }
                    if (!(i!=0 && j!=0) && influencer != TileType.Null  && 
                        influencer.CanInfluence(MapHolder.tiles[column + i, row + j].type, Util.GetRotation(i,j), 
                            MapHolder.tiles[column + i, row + j].GetDirection(),
                            variation == MapHolder.tiles[column + i,row + j].variation))
                    {
                        SwitchTileType(column + i, row + j);
                        changedCoordinates.Add(new Vector2Int(column+i, row+j));
                    }
                    RedoTile(column + i, row + j);
                }
            }
        }
    }

    void RedoTile(int column, int row)
    {
        switch (MapHolder.tiles[column, row].type)
        {
            case TileType.Land:
                LandBuilder.CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.WaterDiagonal:
            case TileType.Water:
                WaterBuilder.MakeWaterTile(column, row, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, MapHolder.tiles[column, row].elevation,MapHolder.tiles[column, row].variation);
                break;
            case TileType.Cliff:
            case TileType.CliffDiagonal:
                //MapHolder.tiles[index] = TileType.Cliff;
                break;
            case TileType.Sand:
                LandBuilder.CreateSandTile(column,row);
                break;
        }
    }

    void SwitchTileType(int column, int row)
    {
        switch (MapHolder.tiles[column, row].type)
        {
            case TileType.WaterDiagonal:
                MapHolder.tiles[column, row].type = TileType.Water;
                break;
            case TileType.PathCurve:
                MapHolder.tiles[column, row].type = TileType.Path;
                break;
            case TileType.CliffDiagonal:
                MapHolder.tiles[column, row].type = TileType.Land;
                break;
            case TileType.SandDiagonal:
                MapHolder.tiles[column, row].type = TileType.Sand;
                break;
            case TileType.SeaDiagonal:
                MapHolder.tiles[column, row].type = TileType.Land;
                break;
        }
    }

    
    public void StartConstruction(int column, int row)
    {
        toolMode = ToolMode.None;
        cliffConstructionElevationLevel = MapHolder.tiles[column,row].elevation;
    }

    public void EndConstruction()
    {
        toolMode = ToolMode.None;
        cliffConstructionElevationLevel = -1;
        foreach (var tile in ignoreRaycastTiles)
        {
            tile.IgnoreElevation();
        }
        
        ignoreRaycastTiles.Clear();
    }


}
