using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandBuilder : MonoBehaviour
{
    public static void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                TileType type = TileType.Land;
                //in the first row of chunks
                if (row < 16 )
                {
                    if (row + 1 <= MapHolder.mapPrefab.seaStandardCreation.z)
                    {
                        type = TileType.Sea;
                    }
                    else
                    {
                        if (row + 1 <= MapHolder.mapPrefab.seaStandardCreation.z + MapHolder.mapPrefab.sandStandardCreation.z)
                        {
                            
                            type = TileType.Sand;
                        }
                    }
                }
                
                if (column < 16 || column >= width - 16)
                {
                    if (column + 1 <= MapHolder.mapPrefab.seaStandardCreation.x || 
                        column >= width - MapHolder.mapPrefab.seaStandardCreation.x)
                    {
                        type = TileType.Sea;
                    }
                    else
                    {
                        if (type != TileType.Sea && 
                            (column + 1 <= MapHolder.mapPrefab.sandStandardCreation.x + MapHolder.mapPrefab.seaStandardCreation.x || 
                            column >= width - MapHolder.mapPrefab.sandStandardCreation.x  - MapHolder.mapPrefab.seaStandardCreation.x))
                        {
                            type = TileType.Sand;
                        }
                    }
                }

                if (row >= height - MapHolder.mapPrefab.seaStandardCreation.y)
                {
                    type = TileType.Sea;
                }
                else
                {
                    if (row >= height - MapHolder.mapPrefab.sandStandardCreation.y -  MapHolder.mapPrefab.seaStandardCreation.y && 
                        type != TileType.Sea)
                    {
                        type = TileType.Sand;
                    }
                }
                
                
                if (type == TileType.Land)
                {
                    CreateLandTile(column, row);
                }
                else
                {
                    if (type == TileType.Sand)
                    {
                        CreateSandTile(column, row);
                    }
                    else
                    {
                        CreateSeaTile(column, row);
                    }
                }
            }
        }
    }
    public static void CreateLandTile(int column, int row, byte elevation = 0)
    {
        //creates tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].RemoveQuarters();
                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], MapHolder.tiles[column, row].colliderObject.transform);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;

                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land]));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;

            MapHolder.tiles[column, row].SetCreatedElevation(MapHolder.elevationLevels[elevation]);
            MapHolder.tiles[column, row].SetPosition(new Vector3(column, 0, -row));
            MapHolder.tiles[column, row].colliderObject.name = $"{column} {row}";
        }

        MapHolder.tiles[column, row].elevation = elevation;
        MapHolder.tiles[column, row].diagonalRotation = 255;
        MapHolder.tiles[column, row].type = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row, MapHolder.tiles[column,row]);
        }
        
        MapHolder.tiles[column, row].variation = 0;
    }

    public static void RebuildLandTile(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], tile.colliderObject.transform);
    }

    public static void CreateSandTile(int column, int row)
    {
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Sand)
            {
                MapHolder.tiles[column, row].RemoveQuarters();
                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[0]);
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Sand], 
                    MapHolder.tiles[column, row].colliderObject.transform);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Sand;

                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[0]);
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Sand]));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Sand;

            MapHolder.tiles[column, row].SetCreatedElevation(MapHolder.elevationLevels[0]);
            MapHolder.tiles[column, row].SetPosition(new Vector3(column, 0, -row));
            MapHolder.tiles[column, row].colliderObject.name = $"{column} {row}";
        }

        MapHolder.tiles[column, row].elevation = 0;
        MapHolder.tiles[column, row].diagonalRotation = 255;
        MapHolder.tiles[column, row].type = TileType.Sand;
        
        MapHolder.tiles[column, row].variation = 0;
    }
    public static void RebuildSandTile(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Sand], tile.colliderObject.transform);
    }
    public static void RebuildSandDiagonal(MapTile tile)
    {
        tile.SoftErase(); 
        byte rotation = tile.diagonalRotation;
        int oppositeRotation = Util.SubstractRotation(rotation, 2);


        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[tile.backgroundType], tile.colliderObject.transform);
        tile.backgroundTile.transform.localPosition = Util.halfOffset;
        tile.backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90 * rotation, 0);
        tile.diagonalRotation = rotation;
    }
    public static void RebuildSeaTile(MapTile tile)
    {
        tile.SoftErase();
    }
    public static void RebuildSeaDiagonal(MapTile tile)
    {
        tile.SoftErase();

        byte rotation = tile.diagonalRotation;

        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[tile.backgroundType],tile.colliderObject.transform);
        tile.backgroundTile.transform.localPosition = Util.halfOffset;
        tile.backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90 * rotation, 0);
        tile.diagonalRotation = rotation;
    }
    public static void CreateSeaTile(int column, int row)
    {
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Sea)
            {
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Sea;

                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[0]);
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile();
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Sea;

            MapHolder.tiles[column, row].SetCreatedElevation(MapHolder.elevationLevels[0]);
            MapHolder.tiles[column, row].SetPosition(new Vector3(column, 0, -row));
            MapHolder.tiles[column, row].colliderObject.name = $"{column} {row}";
        }

        MapHolder.tiles[column, row].elevation = 0;
        MapHolder.tiles[column, row].diagonalRotation = 255;
        MapHolder.tiles[column, row].type = TileType.Sea;
        
        MapHolder.tiles[column, row].variation = 0;
    }

    public static bool CheckSandTile(int column, int row, TileType previousTileType, ref ToolMode toolMode)
    {
        if (!Util.CoordinateOnBorderChunks(column,row) ||
            MapHolder.tiles[column,row].elevation > 0)
        {
            return false;
        }

        if (previousTileType == TileType.Sand)
        {
            if (CheckCanCurveSand(column,row, MapHolder.tiles[column,row]))
            {
                if (toolMode == ToolMode.None)
                {
                    toolMode = ToolMode.Add;
                    MapHolder.tiles[column, row].type = TileType.SandDiagonal;
                    return true;
                }
            }
            return Util.ToolModeChange(false, column, row, TileType.Land, ref toolMode);
        }
        else
        {
            if (previousTileType == TileType.SandDiagonal)
            {
                return Util.ToolModeChange(false, column, row, TileType.Land, ref toolMode);
            }
            else
            {
                if (previousTileType == TileType.SeaDiagonal)
                {
                    return Util.ToolModeChange(false, column, row, TileType.Sea, ref toolMode);
                }
                if (previousTileType == TileType.Land)
                {
                    if (CheckCanCurveSea(column, row, MapHolder.tiles[column, row]))
                    {
                        if (toolMode == ToolMode.None)
                        {
                            toolMode = ToolMode.Add;
                            MapHolder.tiles[column, row].type = TileType.SeaDiagonal;
                            return true;
                        }
                    }
                    return Util.ToolModeChange(true, column, row, TileType.Sand, ref toolMode);
                }
                if (previousTileType == TileType.Sea ||
                    previousTileType == TileType.Path || previousTileType == TileType.PathCurve ||
                    previousTileType == TileType.Water || previousTileType == TileType.WaterDiagonal)
                {
                    if (MapHolder.tiles[column, row].elevation == 0)
                    {
                        return Util.ToolModeChange(true, column, row, TileType.Sand, ref toolMode);
                    }
                }
            }
        }
        

        return false;
    }
    
    static bool CheckCanCurveSea(int column,int row, MapTile tile)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        //int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        types[1] = CheckTile(column, row + 1);
        types[2] = CheckTile(column - 1, row);
        types[3] = CheckTile(column, row - 1);
        types[4] = CheckTile(column + 1, row);

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if (types[i] == TileType.Sea && types[i - 1] == TileType.Sea && 
                types[i + 1] != TileType.Sea && types[i + 1] != TileType.Sand && types[i + 1] != TileType.SandDiagonal &&
                types[i + 2] != TileType.Sea && types[i + 2] != TileType.Sand && types[i + 2] != TileType.SandDiagonal &&types[i+1] == types[i+2])
            {
                if (tile != null)
                {
                    tile.diagonalRotation = (byte)(i - 1);
                }
                return true;
            }
        }

        return false;
    }
    
    static bool CheckCanCurveSand(int column,int row, MapTile tile)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        //int index = row * MapHolder.width + column;
        TileType[] types = new TileType[7];

        types[1] = CheckTile(column, row + 1);
        types[2] = CheckTile(column - 1, row);
        types[3] = CheckTile(column, row - 1);
        types[4] = CheckTile(column + 1, row);

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if (types[i] == TileType.Sand && types[i - 1] == TileType.Sand && 
                types[i + 1] != TileType.Sand && types[i + 2] != TileType.Sand && types[i+1] == types[i+2])
            {
                if (tile != null)
                {
                    tile.diagonalRotation = (byte)(i - 1);
                }
                return true;
            }
        }

        return false;
    }

    static TileType CheckTile(int column, int row)
    {
        if (column >= 0 && column < MapHolder.width &&
            row >= 0 && row < MapHolder.height)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Sea)
            {
                return TileType.Sea;
            }
            else
            {
                if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Sand || 
                    MapHolder.tiles[column, row].backgroundType == TilePrefabType.SandDiagonal)
                {
                    return TileType.Sand;
                }

                return TileType.Land;
            }
        }

        return TileType.Sea;
    }
    
    public static void CreateDiagonalSand(int column, int row)
    {
        byte rotation = MapHolder.tiles[column,row].diagonalRotation;
        int oppositeRotation = Util.SubstractRotation(rotation, 2);

        TilePrefabType type = TilePrefabType.SandDiagonal;
        if (Util.CoordinateExists(column + Util.oppositeCornerForSand[rotation].x, row + Util.oppositeCornerForSand[rotation].y) &&
            MapHolder.tiles[column + Util.oppositeCornerForSand[rotation].x, row + Util.oppositeCornerForSand[rotation].y].type != TileType.Sea)
        {
            type = TilePrefabType.SandDiagonalLand;
        }

        MapHolder.tiles[column, row].HardErase();
        MapHolder.tiles[column, row].RemoveCliffs();
        MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[type],
            MapHolder.tiles[column,row].colliderObject.transform);
        MapHolder.tiles[column, row].backgroundTile.transform.localPosition = Util.halfOffset;
        MapHolder.tiles[column, row].backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0,90*rotation,0);
        MapHolder.tiles[column, row].backgroundType = TilePrefabType.SandDiagonal;
        MapHolder.tiles[column, row].diagonalRotation = rotation;
    }
    
    
    
    public static void CreateDiagonalSea(int column, int row)
    {
        byte rotation = MapHolder.tiles[column,row].diagonalRotation;

        MapHolder.tiles[column, row].HardErase();
        MapHolder.tiles[column, row].RemoveCliffs();
        MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.SeaDiagonal],
            MapHolder.tiles[column,row].colliderObject.transform);
        MapHolder.tiles[column, row].backgroundTile.transform.localPosition = Util.halfOffset;
        MapHolder.tiles[column, row].backgroundTile.transform.GetChild(0).localRotation = Quaternion.Euler(0,90*rotation,0);
        MapHolder.tiles[column, row].backgroundType = TilePrefabType.SeaDiagonal;
        MapHolder.tiles[column, row].diagonalRotation = rotation;
    }
}
