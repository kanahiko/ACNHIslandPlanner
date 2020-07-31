using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBuildingsBuilder
{
    static int maxDecorationLimboCount = 20;
    static Dictionary<DecorationType, List<List<DecorationTiles>>> decorationTilesLimbo;
    //for non unique structures like fence, flowers, bushes, trees

    public static void ChangeTile(int column, int row,DecorationType type, ToolMode mode, byte variation)
    {
        switch (type)
        {
            case DecorationType.Flora:
            case DecorationType.Tree:
                if (mode == ToolMode.Add)
                {
                    AddDecoration(type, column, row, variation);
                }
                else
                {
                    RemoveDecoration(type, column, row);
                }

                break;
        }
    }

    public static void RebuildTile(int column, int row, PreDecorationTile tile)
    {
        MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(tile.type, tile.variation);
        MapHolder.decorationsTiles[column, row].decorationBackground.parent = MapHolder.decorationsParent;
        MapHolder.decorationsTiles[column, row].decorationBackground.localPosition = new Vector3(column, Util.GetHeight(column, row), -row);

        MapHolder.decorationsTiles[column, row].type = tile.type;
        MapHolder.decorationsTiles[column, row].variation = tile.variation;

        if (tile.type == DecorationType.Tree)
        {
            AddTreeInfluence(column, row, true);
        }
    }

    static void AddDecoration(DecorationType type,int column, int row, byte variation)
    {
        
        if (MapHolder.tiles[column,row].type != TileType.Land && MapHolder.tiles[column,row].type != TileType.Sand ||
            MapHolder.tiles[column,row].type == TileType.Sand && (type != DecorationType.Tree || variation != 9 || Util.IsOnLandSandBorder(column, row)) ||
            MapHolder.decorationsTiles[column, row] != null  &&
            (MapHolder.decorationsTiles[column,row].type != DecorationType.Flora && MapHolder.decorationsTiles[column, row].type != DecorationType.Tree))
        {
            return;
        }
        
        if (type == DecorationType.Tree)
        {
            if (!Util.CheckSurroundedByLandElevation(column, row, variation != MapHolder.mapPrefab.treePrefabDictionary.Count - 1) ||
                MapHolder.treeInfluence[column,row] > 0)
            {
                return;
            }

            if (variation == MapHolder.mapPrefab.treePrefabDictionary.Count - 1 && !Util.CheckSurroundedBySand(column, row))
            {
                return;
            }
        }
        

        if (MapHolder.decorationsTiles[column, row] != null)
        {
               
            if (MapHolder.decorationsTiles[column, row].variation != variation)
            { 
                AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
                MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(type, variation);
            }
        }
        else
        {
            MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(type, variation);
        }


        MapHolder.decorationsTiles[column, row].decorationBackground.parent = MapHolder.decorationsParent;
        MapHolder.decorationsTiles[column, row].decorationBackground.localPosition = new Vector3(column, Util.GetHeight(column, row), -row);

        MapHolder.decorationsTiles[column, row].type = type;
        MapHolder.decorationsTiles[column, row].variation = variation;

        if (type == DecorationType.Tree)
        {
            AddTreeInfluence(column, row, true);
        }
    }

    static void AddTreeInfluence(int column, int row, bool add)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Util.CoordinateExists(column+j,row + i))
                {
                    MapHolder.treeInfluence[column+j, row+i] += add ? 1 : -1;
                }
            }
        }
    }

    
    static void RemoveDecoration(DecorationType type, int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row]  == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Flora && 
            MapHolder.decorationsTiles[column, row].type != DecorationType.Tree))
        {
            return;
        }

        AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
        MapHolder.decorationsTiles[column, row] = null;

        if (type == DecorationType.Tree)
        {
            AddTreeInfluence(column, row, false);
        }
    }

    public static void AddToDecorationLimbo(DecorationTiles tile)
    {
        if (decorationTilesLimbo == null)
        {
            decorationTilesLimbo = new Dictionary<DecorationType, List<List<DecorationTiles>>>();
        }
        if (!decorationTilesLimbo.ContainsKey(tile.type))
        {
            decorationTilesLimbo[tile.type] = new List<List<DecorationTiles>>();
        }
        if (decorationTilesLimbo[tile.type].Count < tile.variation + 1)
        {
            while (decorationTilesLimbo[tile.type].Count < tile.variation + 1)
            {
                decorationTilesLimbo[tile.type].Add(new List<DecorationTiles>());
            }
        }
        if (decorationTilesLimbo[tile.type][tile.variation].Count < maxDecorationLimboCount)
        {
            decorationTilesLimbo[tile.type][tile.variation].Add(tile);
            tile.GoToLimbo();
        }
        else
        {
            GameObject.Destroy(tile.mainTile);
            GameObject.Destroy(tile.decorationBackground.gameObject);
            tile.Dispose();
        }
    }
    static DecorationTiles GetTileFromDecorationLimbo(DecorationType type, byte variation)
    {
        if (decorationTilesLimbo == null || decorationTilesLimbo.Count == 0 ||
            !decorationTilesLimbo.ContainsKey(type) || decorationTilesLimbo[type].Count < variation + 1 ||
            decorationTilesLimbo[type][variation].Count == 0)
        {
            if (type == DecorationType.Flora) {
                DecorationTiles newTile = new DecorationTiles(DecorationType.Flora);
                newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.floraPrefabDictionary[variation], newTile.decorationBackground));
                return newTile;
            }
            else
            {
                DecorationTiles newTile = new DecorationTiles(DecorationType.Tree);
                newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.treePrefabDictionary[variation], newTile.decorationBackground));
                return newTile;
            }
        }
        DecorationTiles oldTile = decorationTilesLimbo[type][variation][decorationTilesLimbo[type][variation].Count - 1];
        decorationTilesLimbo[type][variation].RemoveAt(decorationTilesLimbo[type][variation].Count - 1);
        oldTile.ReturnFromLimbo();
        //tile.decorationBackground.SetActive(true);
        return oldTile;
    }    
    
}
