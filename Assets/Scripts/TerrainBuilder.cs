using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainBuilder : MonoBehaviour
{
    public int width = 92;
    public int height = 73;

    public List<Transform> elevationLevels;

    public Vector3 offsetTerrain;

    public MapPrefabs mapPrefabObject;

    static int normalLayer = 8;
    static int ignoreRaycastLayer = 2;
    static int cliffConstructionElevationLevel = -1;

    private static HashSet<MapTile> ignoreRaycastTiles;

    private static ToolMode toolMode = ToolMode.None;

    private static Action<HashSet<Vector2Int>> ChangeMiniMap;
    
    static HashSet<Vector2Int> changedCoordinates;
    
    // Start is called before the first frame update
    void Awake()
    {
        MapHolder.elevationLevels = elevationLevels;
        MapHolder.mapPrefab = mapPrefabObject;
        MapHolder.mapPrefab.StartPrefab();
        
        MapHolder.width = width;
        MapHolder.height = height;

        MapHolder.grid = new TileType[MapHolder.width * MapHolder.height];
        MapHolder.tiles = new MapTile[MapHolder.width, MapHolder.height];
        
        CreateEmptyLand(MapHolder.width, MapHolder.height);
        MapHolder.offset = offsetTerrain;
        transform.position = offsetTerrain;
        
        ignoreRaycastTiles = new HashSet<MapTile>();
        changedCoordinates = new HashSet<Vector2Int>();
        ChangeMiniMap = MiniMap.ChangeMiniMap;
    }
    
    void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                CreateLandTile(column ,row);
            }
        }
    }
    
    static bool ToolModeChange(bool isAdd, int index, TileType type)
    {
        //Debug.Log($"{toolMode} {isAdd} {index} {type}  willdo?");
        if (toolMode == ToolMode.None)
        {
            toolMode = isAdd? ToolMode.Add : ToolMode.Remove;
            MapHolder.grid[index] = type;
            return true;
        }
        else
        {
            if ((toolMode == ToolMode.Add && isAdd) || (toolMode == ToolMode.Remove && !isAdd))
            {
                MapHolder.grid[index] = type;
                return true;
            }
        }
        return false;
    }

    public static void ChangeTile(TileType type, int column, int row)
    {
        changedCoordinates.Clear();
        int index = row * MapHolder.width + column;
        TileType previousTileType = MapHolder.grid[index];

        switch (type)
        {
            case TileType.Land:
                MapHolder.grid[index] = TileType.Land;
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
                            MapHolder.grid[index] = TileType.WaterDiagonal;
                        }
                        else
                        {
                            ToolModeChange(true, index, TileType.Water);
                            //return;
                        }
                    }
                    else
                    {
                        if (!ToolModeChange(false, index, TileType.Land))
                        {
                            return;
                        }

                    }
                }
                else
                {
                    if (previousTileType== TileType.WaterDiagonal)
                    {
                        if (!ToolModeChange(false, index, TileType.Land))
                        {
                            ToolModeChange(true, index, TileType.Water);
                        }
                    }
                    else
                    {
                        if (!ToolModeChange(true, index, TileType.Water))
                        {
                            return;
                        }
                    }
                }
                changedCoordinates.Add(new Vector2Int(column, row));
                CreateTile(column, row, MapHolder.grid[index]);
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
                            MapHolder.grid[index] = TileType.PathCurve;
                        }
                        else
                        {
                            ToolModeChange(true, index, TileType.Path);
                            //return;
                        }
                    }
                    else
                    {
                        ToolModeChange(false, index, TileType.Land);
                    }
                }
                else
                {
                    if (previousTileType == TileType.PathCurve)
                    {
                        ToolModeChange(false, index, TileType.Land);
                    }
                    else
                    {
                        ToolModeChange(true, index, TileType.Path);
                    }
                }
                changedCoordinates.Add(new Vector2Int(column, row));
                CreateTile(column, row, MapHolder.grid[index]);
                break;
            case TileType.Cliff:
                //Debug.Log($"{cliffConstructionElevationLevel} {MapHolder.tiles[column, row].elevation}");
                if (cliffConstructionElevationLevel != MapHolder.tiles[column, row].elevation)
                {
                    return;
                }
                if ( MapHolder.tiles[column, row].elevation > 0)
                {
                    if (MapHolder.grid[index] == TileType.CliffDiagonal)
                    {
                        ToolModeChange(true, index, TileType.Land);
                        cliffConstructionElevationLevel -= 1;
                    }
                    else
                    {
                        if (CheckCanCurveCliff(column, row))
                        {
                            if (toolMode == ToolMode.None)
                            {
                                toolMode = ToolMode.Add;
                                MapHolder.grid[index] = TileType.CliffDiagonal;
                                
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
                                ToolModeChange(true, index, TileType.Land);
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
                    ToolModeChange(true, index, TileType.Land);
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

    public static void RemoveTile(TileType type, int column, int row)
    {
        changedCoordinates.Clear();
        toolMode = ToolMode.Remove;
        int index = row * MapHolder.width + column;
        TileType previousTileType = MapHolder.grid[index];

        switch (type)
        {
            case TileType.Land:
                //MapHolder.grid[index] = TileType.Land;
                break;
            case TileType.Water:
                if (previousTileType == TileType.Water || previousTileType == TileType.WaterDiagonal)
                {
                    MapHolder.grid[index] = TileType.Land;
                    CreateTile(column, row, MapHolder.grid[index]);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case TileType.Path:
                if (previousTileType == TileType.Path || previousTileType == TileType.Path)
                {
                    MapHolder.grid[index] = TileType.Land;
                    CreateTile(column, row, MapHolder.grid[index]);
                    changedCoordinates.Add(new Vector2Int(column, row));
                }
                break;
            case TileType.Cliff:
                if (cliffConstructionElevationLevel == MapHolder.tiles[column, row].elevation && MapHolder.tiles[column, row].elevation > 0)
                {
                    MapHolder.grid[index] = TileType.Land;
                    MapHolder.tiles[column, row].elevation -= 1;
                    if (CanRemoveCliff(column,row))
                    {
                        RemoveCliff(column, row, TileType.Cliff);
                        CreateTile(column, row, MapHolder.grid[index]);
                        MapHolder.tiles[column, row].IgnoreElevation();
                        //MapHolder.tiles[column, row].colliderObject.layer = ignoreRaycastLayer;
                        ignoreRaycastTiles.Add(MapHolder.tiles[column, row]);
                        changedCoordinates.Add(new Vector2Int(column, row));
                    }
                    else
                    {
                        MapHolder.grid[index] = previousTileType;
                        MapHolder.tiles[column, row].elevation += 1;
                    }
                }
                break;
        }
        ChangeMiniMap?.Invoke(changedCoordinates);
    }

    static bool CanRemoveCliff(int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation + 1;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (column + i >= 0 && column + i < MapHolder.width &&
                    row + j >= 0 && row + j < MapHolder.height)
                {
                    if (MapHolder.tiles[column + i, row+j].elevation > elevation && !Util.CheckSurroundedBySameElevation(column + i, row + j))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    static void RemoveCliff(int column, int row, TileType type)
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
                    int influenceeIndex = Util.GetIndex(column + i, row + j);
                    TileType influencee = MapHolder.grid[influenceeIndex];
                    if (influencer == TileType.CliffDiagonal && CheckCanCurveCliff(column, row))
                    {
                        MapHolder.grid[influenceeIndex] = TileType.Land;
                    }

                    if (influencee == TileType.Water || influencee == TileType.WaterDiagonal)
                    {
                        if ((MapHolder.tiles[column + i, row + j].elevation > 0 &&
                             !Util.CheckHalfSurroundedBySameElevation(column + i, row + j)))
                        {
                            MapHolder.grid[influenceeIndex] = TileType.Land;
                            changedCoordinates.Add(new Vector2Int(column + i, row + j));
                        }else
                        {
                            if (!CheckCanCurvePath(column + i, row + j, MapHolder.tiles[column + i, row +j]))
                            {
                                MapHolder.grid[influenceeIndex] = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }
                    RedoTile(column + i, row + j, MapHolder.grid[influenceeIndex]);
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
                    int influenceeIndex = Util.GetIndex(column + i, row + j);
                    TileType influencee = MapHolder.grid[influenceeIndex];

                    if (influencee == TileType.Water || influencee == TileType.WaterDiagonal)
                    {
                        if ((MapHolder.tiles[column + i, row + j].elevation > 0 &&
                             !Util.CheckHalfSurroundedBySameElevation(column + i, row + j)))
                        {
                            MapHolder.grid[influenceeIndex] = TileType.Land;
                            changedCoordinates.Add(new Vector2Int(column + i, row+j));
                        }
                        else
                        {
                            if (!CheckCanCurvePath(column + i, row + j, MapHolder.tiles[column + i, row + j]))
                            {
                                MapHolder.grid[influenceeIndex] = TileType.Water;
                                changedCoordinates.Add(new Vector2Int(column + i, row + j));
                            }
                        }
                    }

                    RedoTile(column + i, row + j, MapHolder.grid[influenceeIndex]);
                }
            }
        }
    }


    static void CreateTile(int column, int row, TileType type)
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
                MapHolder.tiles[column, row].backgroundTile.layer = ignoreRaycastLayer;
                break;
        }
    }
    
    static bool CheckCanCurvePath(int column,int row, MapTile tile = null)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        if (row + 1 < MapHolder.width && elevation == MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] = MapHolder.grid[index + MapHolder.width];
        }

        if (column - 1 >= 0 && elevation == MapHolder.tiles[column - 1,row].elevation)
        {
            types[2] = MapHolder.grid[index - 1];
        }

        if (row - 1 >= 0 && elevation == MapHolder.tiles[column,row - 1].elevation )
        {
            types[3] = MapHolder.grid[index - MapHolder.width];
        }

        if (column + 1 < MapHolder.height && elevation == MapHolder.tiles[column + 1,row].elevation)
        {
            types[4] = MapHolder.grid[index + 1];
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
    
    static bool CheckCanCurveWater(int column,int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        if (row + 1 < MapHolder.width && elevation <= MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] = MapHolder.grid[index + MapHolder.width];
        }

        if (column - 1 >= 0 && elevation <= MapHolder.tiles[column - 1,row].elevation)
        {
            types[2] = MapHolder.grid[index - 1];
        }

        if (row - 1 >= 0 && elevation <= MapHolder.tiles[column,row - 1].elevation )
        {
            types[3] = MapHolder.grid[index - MapHolder.width];
        }

        if (column + 1 < MapHolder.height && elevation <= MapHolder.tiles[column + 1,row].elevation)
        {
            types[4] = MapHolder.grid[index + 1];
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
    
    static bool CheckCanCurveCliff(int column,int row)
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

    static void RecalculateDiagonals(int column,int row, TileType type)
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
                        influencer.CanInfluence(MapHolder.grid[(row+j) * MapHolder.width + column + i], Util.GetRotation(i,j), 
                            MapHolder.tiles[column + i, row + j].GetDirectionOfPath(),
                            MapHolder.tiles[column + i , row + j].GetDirectionOfWater()))
                    {
                        SwitchTileType(column +i, row +j, (row + j) * MapHolder.width + column + i);
                        changedCoordinates.Add(new Vector2Int(column+i, row+j));
                    }
                    RedoTile(column + i, row + j, type);
                }
            }
        }
    }

    private static void RedoTile(int column, int row, TileType type)
    {
        int index = Util.GetIndex(column,row);
        switch (MapHolder.grid[index])
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
                //MapHolder.grid[index] = TileType.Cliff;
                break;
        }
    }

    static void SwitchTileType(int column, int row, int index)
    {
        switch (MapHolder.grid[index])
        {
            case TileType.WaterDiagonal:
                MapHolder.grid[index] = TileType.Water;
                break;
            case TileType.PathCurve:
                MapHolder.grid[index] = TileType.Path;
                break;
            case TileType.CliffDiagonal:
                MapHolder.grid[index] = TileType.Land;
                break;
        }
    }

    static void CreateLandTile( int column ,int row, int elevation = 0)
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
        MapHolder.grid[row * MapHolder.width + column] = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
        }
    }
    public static void StartConstruction(int elevation)
    {
        toolMode = ToolMode.None;
        cliffConstructionElevationLevel = elevation;
    }

    public static void EndConstruction()
    {
        toolMode = ToolMode.None;
        cliffConstructionElevationLevel = -1;
        //Debug.Log($"construction ended {ignoreRaycastTiles.Count}");
        foreach (var tile in ignoreRaycastTiles)
        {
            tile.IgnoreElevation(false);
            //tile.backgroundTile.layer = normalLayer;
        }
        
        ignoreRaycastTiles.Clear();
    }
    
    
}
