using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsBuilder
{

    public static Dictionary<DecorationType, List<UniqueBuilding>> uniqueBuildings;
    //for building unique buildings

    public static void CreateUniqueBuildings()
    {
        uniqueBuildings = new Dictionary<DecorationType, List<UniqueBuilding>>();

        foreach (var building in MapHolder.mapPrefab.maxCount)
        {
            if (MapHolder.mapPrefab.decorationsPrefabDictionary.ContainsKey(building.Key))
            {
                List<UniqueBuilding> list = new List<UniqueBuilding>();
                for (int i = 0; i < building.Value; i++)
                {
                    list.Add(new UniqueBuilding(GameObject.Instantiate(MapHolder.mapPrefab.decorationsPrefabDictionary[building.Key]),
                        building.Key, MapHolder.mapPrefab.decorationsSizeDictionary[building.Key]));                    
                }
                uniqueBuildings.Add(building.Key, list);
            }
        }
    }
    
    public static void ChangeTile(int column, int row, ToolMode mode, DecorationType type)
    {
        if (mode == ToolMode.Add)
        {
            AddTile(column, row, type);
        }
        else
        {
            RemoveTile(column, row);
        }
    }

    static void AddTile(int column, int row, DecorationType type)
    {
        //all buildings DecorationTiles are going to have type building

        UniqueBuilding building = FindAvailiableBuilding(type);

        if (building == null)
        {
            return;
        }

        int newColumn = FindStartingPoint(column, building.size.x);

        if (Util.CheckEmptyLandElevation(newColumn, row, building.size.x, building.size.y))
        {
            //can place
            building.tile.decorationBackground.parent = MapHolder.decorationsParent;
            building.tile.decorationBackground.localPosition = new Vector3(newColumn, Util.GetHeight(newColumn, row), -row);
            building.startingColumn = newColumn;
            building.startingRow = row;
            building.tile.ReturnFromLimbo();

            //RedoTilesOfPath          
            
            MarkTile(newColumn, row, building.size, building.tile);
            MiniMap.CreateBuilding(newColumn, row - MapHolder.mapPrefab.decorationsSizeDictionary[type].z, 
                building.size.x, building.size.y - MapHolder.mapPrefab.decorationsSizeDictionary[type].z, building.type);
        }
    }

    static void RemoveTile(int column, int row)
    {
        if (MapHolder.decorationsTiles[column,row] == null || MapHolder.decorationsTiles[column,row].type != DecorationType.Building)
        {
            return;
        }

        UniqueBuilding building = MapHolder.decorationsTiles[column, row].building;

        int startingColumn = building.startingColumn;
        int startingRow = building.startingRow;
        
        MarkTile(building.startingColumn, building.startingRow, building.size, null);
        building.tile.GoToLimbo();

        MiniMap.CreateBuilding(startingColumn, startingRow - MapHolder.mapPrefab.decorationsSizeDictionary[building.type].z, 
            building.size.x, building.size.y - MapHolder.mapPrefab.decorationsSizeDictionary[building.type].z, building.type);
        
    }

    static int FindStartingPoint(int column, int size)
    {
        int halfSize = size / 2;
        if (size%2 == 0)
        {
            halfSize -= 1;
        }

        return column - halfSize;
    }

    static void MarkTile(int column, int row, Vector3Int size, DecorationTiles tile)
    {
        HashSet<Vector2Int> pathTiles = new HashSet<Vector2Int>();

        HashSet<Vector2Int> changedTiles = new HashSet<Vector2Int>();

        for(int i = 0; i < size.y; i++)
        {
            for(int j = 0; j < size.x; j++)
            {
                MapHolder.decorationsTiles[column+j, row-i] = tile;
                MapHolder.buildingsInfluence[column + j, row - i] = tile != null ? (i < size.z ? BuildingInfluence.pathsOnly : BuildingInfluence.fullInfluence) : BuildingInfluence.noInfluence;

                if (MapHolder.tiles[column+j,row-i].type == TileType.Path || MapHolder.tiles[column + j, row - i].type == TileType.PathCurve)
                {
                    //for adding path tiles outside of building size
                    if (j == 0 || j + 1 == size.x)
                    {
                        int newColumn = column + j + (j == 0 ? -1: 1);
                        if (i == 0 || i + 1 == size.y)
                        {
                            int newRow = row - i + (i == 0 ? 1 : -1);
                            pathTiles.Add(new Vector2Int(newColumn, newRow));
                        }
                        pathTiles.Add(new Vector2Int(newColumn, row - i));
                    }

                    if (i == 0 || i + 1 == size.y)
                    {
                        int newRow = row - i + (i == 0 ? 1 : -1);
                        pathTiles.Add(new Vector2Int(column + j, newRow));
                    }

                    if (MapHolder.buildingsInfluence[column + j, row - i] == BuildingInfluence.fullInfluence)
                    {
                        MapHolder.tiles[column + j, row - i].type = TileType.Land;
                        LandBuilder.CreateLandTile(column + j, row - i, MapHolder.tiles[column + j, row - i].elevation);
                    }
                    pathTiles.Add(new Vector2Int(column + j, row - i));
                    changedTiles.Add(new Vector2Int(column + j, row - i));
                }
            }
        }

        PathBuilder.RedoTiles(pathTiles);
        MiniMap.ChangeMiniMap(changedTiles);
    }


    public static UniqueBuilding FindAvailiableBuilding(DecorationType type)
    {
        if (uniqueBuildings.ContainsKey(type))
        {
            foreach(var building in uniqueBuildings[type])
            {
                if (building.startingColumn == -1)
                {
                    return building;
                }
            }
        }

        return null;
    }
}
