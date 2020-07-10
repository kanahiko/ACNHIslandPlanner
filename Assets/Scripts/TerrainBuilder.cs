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
    public static MapPrefabs mapPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        MapHolder.elevationLevels = elevationLevels;
        mapPrefab = mapPrefabObject;
        mapPrefab.ConvertToDictionary();
        CliffBuilder.mapPrefab = mapPrefab;
        PathBuilder.mapPrefab = mapPrefab;
        WaterBuilder.mapPrefab = mapPrefab;
        
        MapHolder.width = width;
        MapHolder.height = height;

        MapHolder.grid = new TileType[MapHolder.width * MapHolder.height];
        MapHolder.tiles = new MapTile[MapHolder.width, MapHolder.height];
        
        CreateEmptyLand(MapHolder.width, MapHolder.height);
        MapHolder.offset = offsetTerrain;
        transform.position = offsetTerrain;
    }
    
    void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                CreateLandTile(column ,row, 0);
            }
        }
    }

    public static void ChangeTile(TileType type, int column, int row)
    {
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
                    if (CheckCanCurve(type, TileType.WaterDiagonal, column, row))
                    {
                        MapHolder.grid[index] = TileType.WaterDiagonal;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                }
                else
                {
                    if (previousTileType== TileType.WaterDiagonal)
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Water;
                    }
                }
                CreateTile(column, row, MapHolder.grid[index], index);
                break;
            case TileType.Path:
                if (previousTileType == TileType.CliffDiagonal)
                {
                    return;
                }
                if (previousTileType == TileType.Path)
                {
                    if (CheckCanCurve(type, TileType.PathCurve, column, row, MapHolder.tiles[column,row]))
                    {
                        MapHolder.grid[index] = TileType.PathCurve;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                }
                else
                {
                    if (previousTileType == TileType.PathCurve)
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Path;
                    }
                }
                CreateTile(column, row, MapHolder.grid[index], index);
                break;
            case TileType.Cliff:
                if (MapHolder.tiles[column, row].elevation > 0)
                {
                    if (MapHolder.grid[index] == TileType.CliffDiagonal)
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                    else
                    {
                        if (CheckCanCurve(TileType.Land, TileType.CliffDiagonal, column, row))
                        {
                            MapHolder.grid[index] = TileType.CliffDiagonal;
                            CreateTile(column, row, TileType.CliffDiagonal, index);
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
                                MapHolder.tiles[column, row].elevation += 1;
                                MapHolder.grid[index] = TileType.Land;
                            }
                        }
                    }

                }
                else
                {
                    MapHolder.tiles[column, row].elevation += 1;
                    MapHolder.grid[index] = TileType.Land;
                }
                CreateTile(column, row, TileType.Cliff, index);
                break;
        }
    }


    static void CreateTile(int column, int row, TileType type, int index)
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
                WaterBuilder.MakeWaterTile(column, row, index, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, index, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Cliff:
                CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.CliffDiagonal:
                CliffBuilder.CreateCliffDiagonal(column, row);
                break;
        }
    }
    static bool CheckCanCurve(TileType type, TileType secondaryType,int column,int row, MapTile tile = null)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        if (row + 1 < MapHolder.width  && elevation == MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] = MapHolder.grid[index + MapHolder.width];
        }

        if (column - 1 >= 0 && elevation == MapHolder.tiles[column - 1, row].elevation)
        {
            types[2] = MapHolder.grid[index - 1];
        }

        if (row - 1 >= 0 && elevation == MapHolder.tiles[column, row - 1].elevation)
        {
            types[3] = MapHolder.grid[index - MapHolder.width];
        }

        if (column + 1 < MapHolder.height && elevation == MapHolder.tiles[column + 1, row].elevation)
        {
            types[4] = MapHolder.grid[index + 1];
        }

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if ((types[i] == type || types[i] == secondaryType) && (types[i - 1] == type || types[i - 1] == secondaryType) 
                && types[i + 1] != type && types[i + 1] != secondaryType && types[i + 2] != type && types[i + 2] != secondaryType)
            {
                if (tile != null) 
                {
                    //Debug.Log($"!!!!!!!! {i} {Util.SubstractRotation(i, 1)} {Util.SubstractRotation(i, 2)} {Util.SubstractRotation(i, 3)}");
                    tile.diagonalPathRotation = i - 1;
                }
                return true;
            }
        }

        return false;
    }

    static void RecalculateDiagonals(int column,int row, TileType type)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        TileType influencer = type;
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
                    if (!(i!=0 && j!=0) && influencer != TileType.Null && 
                        influencer.CanInfluence(MapHolder.grid[(row+j) * MapHolder.width + column + i], 4, MapHolder.tiles[column + i, row + j].GetDirectionOfPath()))
                    {
                        SwitchTileType(column +i, row +j, (row + j) * MapHolder.width + column + i);
                    }
                    RedoTile(column + i, row + j, type);
                }
            }
        }
    }

    private static void RedoTile(int column, int row, TileType type)
    {
        int index = row * MapHolder.width + column;
        switch (MapHolder.grid[index])
        {
            case TileType.Land:
                CreateLandTile(column, row, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.WaterDiagonal:
            case TileType.Water:
                WaterBuilder.MakeWaterTile(column, row, index, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                PathBuilder.CreatePath(column, row, index, MapHolder.tiles[column, row].elevation);
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
            //Debug.Log("1 "+elevation);
            //Debug.Log("2 " + MapHolder.tiles[column, row].elevation);
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].RemoveQuarters();
                MapHolder.tiles[column, row].backgroundTile.transform.parent = MapHolder.elevationLevels[elevation];
                Vector3 position = MapHolder.tiles[column, row].backgroundTile.transform.localPosition;
                position.y = 0;
                MapHolder.tiles[column, row].backgroundTile.transform.localPosition = position;
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(mapPrefab.lookUpTilePrefab[TilePrefabType.Land], MapHolder.elevationLevels[elevation]);
                MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(mapPrefab.lookUpTilePrefab[TilePrefabType.Land],  MapHolder.elevationLevels[elevation]));
            MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
        }

        MapHolder.tiles[column, row].elevation = elevation;
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.grid[row * MapHolder.width + column] = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
        }
    }

}
