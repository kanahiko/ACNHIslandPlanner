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
    
    public void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                CreateLandTile(column ,row);
            }
        }
    }
    
    bool ToolModeChange(bool isAdd, int column, int row, TileType type)
    {
        //Debug.Log($"{toolMode} {isAdd} {index} {type}  willdo?");
        if (toolMode == ToolMode.None)
        {
            toolMode = isAdd? ToolMode.Add : ToolMode.Remove;
            MapHolder.tiles[column, row].type = type;
            return true;
        }
        else
        {
            if ((toolMode == ToolMode.Add && isAdd) || (toolMode == ToolMode.Remove && !isAdd))
            {
                MapHolder.tiles[column, row].type = type;
                return true;
            }
        }
        return false;
    }

    public void ChangeTile(ToolType type, ToolMode mode, int column, int row)
    {
        Debug.Log($"{MapHolder.buildingsInfluence[column, row]} {MapHolder.buildingsInfluence[column, row] == 2} " +
                  $"{type != ToolType.PathPermit && MapHolder.treeInfluence[column,row] > 0} " +
                  $"{MapHolder.decorationsTiles[column, row] != null} { MapHolder.buildingsInfluence[column, row] == 1} {type != ToolType.PathPermit}");
        if ((MapHolder.buildingsInfluence[column, row] == 2 ||
            type != ToolType.PathPermit && MapHolder.treeInfluence[column,row] > 0 || 
            MapHolder.decorationsTiles[column, row] != null && (MapHolder.buildingsInfluence[column, row] == 0 ||
             MapHolder.buildingsInfluence[column, row] == 1 && type != ToolType.PathPermit)))
        {
            return;
        }

        TileType tileType = TileType.Water;
        switch (type)
        {
            case ToolType.CliffConstruction:
                tileType = TileType.Cliff;
                break;
            case ToolType.PathPermit:
                tileType = TileType.Path;
                break;
        }

        if (mode == ToolMode.Add)
        {
            ChangeTile(tileType, column, row);
        }
        else
        {
            RemoveTile(tileType, column, row);
        }
    }

    void ChangeTile(TileType type, int column, int row)
    {
        changedCoordinates.Clear();
       // int index = row * MapHolder.width + column;
        TileType previousTileType = MapHolder.tiles[column,row].type;
        
        switch (type)
        {
            case TileType.Land:
                MapHolder.tiles[column, row].type = TileType.Land;
                break;
            case TileType.Water:
                if (previousTileType == TileType.CliffDiagonal)
                {
                    return;
                }

                if (MapHolder.tiles[column, row].elevation > 0 && !Util.CheckHalfSurroundedBySameElevation(column,row))
                {
                    return;
                }
                
                if (previousTileType == TileType.Water)
                {
                    if (CheckCanCurveWater(column, row))
                    {
                        if (toolMode == ToolMode.None)
                        {
                            toolMode = ToolMode.Add;
                            MapHolder.tiles[column, row].type = TileType.WaterDiagonal;
                        }
                        else
                        {
                            ToolModeChange(true, column, row, TileType.Water);
                            //return;
                        }
                    }
                    else
                    {
                        if (!ToolModeChange(false, column, row, TileType.Land))
                        {
                            return;
                        }

                    }
                }
                else
                {
                    if (previousTileType== TileType.WaterDiagonal)
                    {
                        if (!ToolModeChange(false, column, row, TileType.Land))
                        {
                            ToolModeChange(true, column, row, TileType.Water);
                        }
                    }
                    else
                    {
                        if (!ToolModeChange(true, column, row, TileType.Water))
                        {
                            return;
                        }
                    }
                }
                changedCoordinates.Add(new Vector2Int(column, row));
                CreateTile(column, row, MapHolder.tiles[column, row].type);
                break;
            case TileType.Path:
                if (previousTileType == TileType.CliffDiagonal)
                {
                    return;
                }
                if (previousTileType == TileType.Path)
                {
                    if (CheckCanCurvePath(column, row, MapHolder.tiles[column,row]))
                    {
                        if (toolMode == ToolMode.None)
                        {
                            toolMode = ToolMode.Add;
                            MapHolder.tiles[column, row].type = TileType.PathCurve;
                        }
                        else
                        {
                            ToolModeChange(true, column, row, TileType.Path);
                            //return;
                        }
                    }
                    else
                    {
                        ToolModeChange(false, column, row, TileType.Land);
                    }
                }
                else
                {
                    if (previousTileType == TileType.PathCurve)
                    {
                        ToolModeChange(false, column, row, TileType.Land);
                    }
                    else
                    {
                        ToolModeChange(true, column, row, TileType.Path);
                    }
                }
                changedCoordinates.Add(new Vector2Int(column, row));
                CreateTile(column, row, MapHolder.tiles[column, row].type);
                break;
            case TileType.Cliff:
                //Debug.Log($"{cliffConstructionElevationLevel} {MapHolder.tiles[column, row].elevation}");
                if (cliffConstructionElevationLevel != MapHolder.tiles[column, row].elevation)
                {
                    return;
                }
                if ( MapHolder.tiles[column, row].elevation > 0)
                {
                    if (MapHolder.tiles[column, row].type == TileType.CliffDiagonal)
                    {
                        ToolModeChange(true, column, row, TileType.Land);
                        cliffConstructionElevationLevel -= 1;
                    }
                    else
                    {
                        if (CheckCanCurveCliff(column, row))
                        {
                            if (toolMode == ToolMode.None)
                            {
                                toolMode = ToolMode.Add;
                                MapHolder.tiles[column, row].type = TileType.CliffDiagonal;
                                
                                CreateTile(column, row, TileType.CliffDiagonal);
                                MapHolder.tiles[column, row].IgnoreElevation();
                                //MapHolder.tiles[column, row].colliderObject.layer = ignoreRaycastLayer;
                                ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                                
                                cliffConstructionElevationLevel -= 1;
                            }
                            else
                            {
                                return;
                            }
                            changedCoordinates.Add(new Vector2Int(column, row));
                            ChangeMiniMap?.Invoke(changedCoordinates);
                            return;
                        }
                        else
                        {
                            if (!Util.CheckSurroundedBySameElevation(column, row))
                            {
                                return;
                            }
                            if (MapHolder.tiles[column, row].elevation < MapHolder.maxElevation)
                            {
                                ToolModeChange(true, column, row, TileType.Land);
                                MapHolder.tiles[column, row].elevation += 1;
                                MapHolder.tiles[column, row].IgnoreElevation();
                                //MapHolder.tiles[column, row].colliderObject.layer = ignoreRaycastLayer;
                                ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                            }
                        }
                    }

                }
                else
                {
                    ToolModeChange(true, column, row, TileType.Land);
                    MapHolder.tiles[column, row].elevation += 1;
                    MapHolder.tiles[column, row].IgnoreElevation();
                    //MapHolder.tiles[column, row].colliderObject.layer = ignoreRaycastLayer;
                    ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                }
                changedCoordinates.Add(new Vector2Int(column, row));
                CreateTile(column, row, TileType.Cliff);
                break;
        }
        ChangeMiniMap?.Invoke(changedCoordinates);
    }

    void RemoveTile(TileType type, int column, int row)
    {
        changedCoordinates.Clear();
        toolMode = ToolMode.Remove;
        //int index = row * MapHolder.width + column;
        TileType previousTileType = MapHolder.tiles[column, row].type;
        switch (type)
        {
            case TileType.Land:
                //MapHolder.tiles[index] = TileType.Land;
                break;
            case TileType.Water:
                if (previousTileType == TileType.Water || previousTileType == TileType.WaterDiagonal)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case TileType.Path:
                if (previousTileType == TileType.Path || previousTileType == TileType.Path)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    CreateTile(column, row, MapHolder.tiles[column, row].type);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case TileType.Cliff:
                if (cliffConstructionElevationLevel == MapHolder.tiles[column, row].elevation && MapHolder.tiles[column, row].elevation > 0)
                {
                    MapHolder.tiles[column, row].type = TileType.Land;
                    MapHolder.tiles[column, row].elevation -= 1;
                    if (Util.CanRemoveCliff(column,row))
                    {
                        RemoveCliff(column, row, TileType.Cliff);
                        CreateTile(column, row, MapHolder.tiles[column, row].type);
                        MapHolder.tiles[column, row].IgnoreElevation();
                        //MapHolder.tiles[column, row].colliderObject.layer = ignoreRaycastLayer;
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
        }
        ChangeMiniMap?.Invoke(changedCoordinates);
    }

    void RemoveCliff(int column, int row, TileType type)
    {
        TileType influencer = type;
        int elevation = MapHolder.tiles[column, row].elevation;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if ( column + i >= 0 && column + i < MapHolder.width &&
                     row + j >= 0 && row + j < MapHolder.height)
                {
                    //int influenceeIndex = Util.GetIndex(column + i, row + j);
                    TileType influencee = MapHolder.tiles[column + i, row + j].type;
                    if (influencer == TileType.CliffDiagonal && CheckCanCurveCliff(column, row))
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
                            if (!CheckCanCurveWater(column + i, row + j))
                            {
                                MapHolder.tiles[column + i, row + j].type = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }
                    RedoTile(column + i, row + j, MapHolder.tiles[column + i, row + j].type);
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

                if (column + i >= 0 && column + i < MapHolder.width &&
                    row + j >= 0 && row + j < MapHolder.height)
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
                            if (!CheckCanCurveWater(column + i, row + j))
                            {
                                MapHolder.tiles[column + i, row + j].type = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }

                    RedoTile(column + i, row + j, MapHolder.tiles[column + i, row + j].type);
                }
            }
        }
    }

    void CreateTile(int column, int row, TileType type)
    {
        //Debug.Log(MapHolder.tiles[column, row].elevation);
        RecalculateDiagonals(column, row, type);

        switch (type)
        {
            case TileType.Null:
                break;
            case TileType.Land:
                CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Water:
            case TileType.WaterDiagonal:
                WaterBuilder.MakeWaterTile(column, row, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Cliff:
                CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.CliffDiagonal:
                CliffBuilder.CreateCliffDiagonal(column, row);
                MapHolder.tiles[column, row].IgnoreElevation();//= ignoreRaycastLayer;
                break;
        }
    }
    
    bool CheckCanCurvePath(int column,int row, MapTile tile = null)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        //int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        if (row + 1 < MapHolder.width && elevation == MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] = MapHolder.tiles[column, row + 1].type;
        }

        if (column - 1 >= 0 && elevation == MapHolder.tiles[column - 1,row].elevation)
        {
            types[2] = MapHolder.tiles[column - 1, row].type;
        }

        if (row - 1 >= 0 && elevation == MapHolder.tiles[column,row - 1].elevation )
        {
            types[3] = MapHolder.tiles[column, row - 1].type;
        }

        if (column + 1 < MapHolder.height && elevation == MapHolder.tiles[column + 1,row].elevation)
        {
            types[4] = MapHolder.tiles[column + 1, row].type;
        }

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if ((types[i] == TileType.Path || types[i] == TileType.PathCurve) && (types[i - 1] == TileType.Path || types[i - 1] == TileType.PathCurve) 
                && types[i + 1] != TileType.Path && types[i + 1] != TileType.PathCurve && types[i + 2] != TileType.Path && types[i + 2] != TileType.PathCurve)
            {
                if (tile != null) 
                {
                    tile.diagonalPathRotation = i - 1;
                }
                return true;
            }
        }

        return false;
    }
    
    bool CheckCanCurveWater(int column,int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        if (row + 1 < MapHolder.width && elevation <= MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] = MapHolder.tiles[column, row + 1].type;
        }

        if (column - 1 >= 0 && elevation <= MapHolder.tiles[column - 1,row].elevation)
        {
            types[2] = MapHolder.tiles[column - 1, row].type;
        }

        if (row - 1 >= 0 && elevation <= MapHolder.tiles[column,row - 1].elevation )
        {
            types[3] = MapHolder.tiles[column, row - 1].type;
        }

        if (column + 1 < MapHolder.height && elevation <= MapHolder.tiles[column + 1,row].elevation)
        {
            types[4] = MapHolder.tiles[column + 1, row].type;
        }

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if ((types[i] == TileType.Water || types[i] == TileType.WaterDiagonal) && (types[i - 1] == TileType.Water || types[i - 1] == TileType.WaterDiagonal) 
                && types[i + 1] != TileType.Water && types[i + 1] != TileType.WaterDiagonal && types[i + 2] != TileType.Water && types[i + 2] != TileType.WaterDiagonal)
            {
                return true;
            }
        }

        return false;
    }
    
    bool CheckCanCurveCliff(int column,int row)
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
                return true;
            }
        }

        return false;
    }

    void RecalculateDiagonals(int column,int row, TileType type)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        TileType influencer = type;
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if ( column + i >= 0 && column + i < MapHolder.width &&
                    row + j >= 0 && row + j < MapHolder.height)
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
                            MapHolder.tiles[column + i, row + j].GetDirectionOfPath(),
                            MapHolder.tiles[column + i , row + j].GetDirectionOfWater()))
                    {
                        SwitchTileType(column + i, row + j);
                        changedCoordinates.Add(new Vector2Int(column+i, row+j));
                    }
                    RedoTile(column + i, row + j, type);
                }
            }
        }
    }

    void RedoTile(int column, int row, TileType type)
    {
        switch (MapHolder.tiles[column, row].type)
        {
            case TileType.Land:
                CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.WaterDiagonal:
            case TileType.Water:
                WaterBuilder.MakeWaterTile(column, row, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Cliff:
            case TileType.CliffDiagonal:
                //MapHolder.tiles[index] = TileType.Cliff;
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
        }
    }

    void CreateLandTile( int column ,int row, int elevation = 0)
    {
        //creates tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].RemoveQuarters();
                MapHolder.tiles[column,row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], MapHolder.tiles[column,row].colliderObject.transform);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
                
                MapHolder.tiles[column,row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land]));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            
            MapHolder.tiles[column,row].SetCreatedElevation(MapHolder.elevationLevels[elevation]);
            MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
            MapHolder.tiles[column,row].colliderObject.name = $"{column} {row}";
        }

        MapHolder.tiles[column, row].elevation = elevation;
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;
        MapHolder.tiles[column, row].type = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
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
            tile.IgnoreElevation(false);
        }
        
        ignoreRaycastTiles.Clear();
    }


}
