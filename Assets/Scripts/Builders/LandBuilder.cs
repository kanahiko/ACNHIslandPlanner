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
                    if (column <= MapHolder.mapPrefab.seaStandardCreation.x || 
                        column >= width - MapHolder.mapPrefab.seaStandardCreation.x)
                    {
                        type = TileType.Sea;
                    }
                    else
                    {
                        if (type != TileType.Sea && 
                            (column <= MapHolder.mapPrefab.sandStandardCreation.x + MapHolder.mapPrefab.seaStandardCreation.x || 
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
    public static void CreateLandTile(int column, int row, int elevation = 0)
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
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;
        MapHolder.tiles[column, row].type = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
        }
        
        MapHolder.tiles[column, row].variation = -1;
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
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;
        MapHolder.tiles[column, row].type = TileType.Sand;
        
        MapHolder.tiles[column, row].variation = -1;
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
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;
        MapHolder.tiles[column, row].type = TileType.Sea;
        
        MapHolder.tiles[column, row].variation = -1;
    }
}
