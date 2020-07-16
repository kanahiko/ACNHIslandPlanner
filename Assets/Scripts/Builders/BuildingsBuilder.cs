using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsBuilder
{
    //for building unique buildings

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

        UniqueBuilding building = MapHolder.FindAvailiableBuilding(type);

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
        }
    }

    static void RemoveTile(int column, int row)
    {
        if (MapHolder.decorationsTiles[column,row].type != DecorationType.Building)
        {
            return;
        }

        UniqueBuilding building = MapHolder.decorationsTiles[column, row].building;

        MarkTile(building.startingColumn, building.startingRow, building.size, null);
        building.tile.GoToLimbo();
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
                MapHolder.buildingsInfluence[column + j, row - i] = tile != null ? (i < size.z ? 1 : 2) : 0;

                if (MapHolder.tiles[column+j,row-i].type == TileType.Path || MapHolder.tiles[column + j, row - i].type == TileType.PathCurve)
                {
                    if (j == 0 || j == size.x - 1)
                    {
                        int newColumn = column + j + (j == 0 ? -1: 1);
                        pathTiles.Add(new Vector2Int(newColumn, row - i - 1));
                        pathTiles.Add(new Vector2Int(newColumn, row - i));
                        pathTiles.Add(new Vector2Int(newColumn, row - i + 1));
                    }

                    if (i==0 || i == size.y - 1)
                    {
                        int newRow = row - 1 + (i == 0 ? 1 : -1);
                        pathTiles.Add(new Vector2Int(column + j, newRow));
                    }

                    if (MapHolder.buildingsInfluence[column + j, row - i] == 2)
                    {
                        MapHolder.tiles[column + j, row - i].type = TileType.Land;
                        LandBuilder.CreateLandTile(column + j, row - i, MapHolder.tiles[column + j, row - i].elevation);
                        changedTiles.Add(new Vector2Int(column + j, row - i));
                    }
                }
            }
        }

        PathBuilder.RedoTiles(pathTiles);
        MiniMap.ChangeMiniMap(changedTiles);
    }

}
