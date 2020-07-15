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
        //3 -> 1 (0, 1 ,2)
        //5 -> 2 (0, 1, 2, 3, 4)
        //4-> 2  (0, 1, 2, 3)

        if (size%2 == 0)
        {
            halfSize -= 1;
        }

        return column - halfSize;
    }

    static void MarkTile(int column, int row, Vector2Int size, DecorationTiles tile)
    {
        for(int i = 0; i < size.y; i++)
        {
            for(int j = 0; j < size.x; j++)
            {
                MapHolder.decorationsTiles[column+j, row-i] = tile;
            }
        }
    }

}
