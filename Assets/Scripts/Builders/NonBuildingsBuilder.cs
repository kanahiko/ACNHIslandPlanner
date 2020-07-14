using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBuildingsBuilder
{
    static int maxDecorationLimboCount = 20;
    static Dictionary<DecorationType, List<List<DecorationTiles>>> decorationTilesLimbo;
    //for non unique structures like fence, flowers, bushes, trees

    public static void ChangeTile(int column, int row,DecorationType type, ToolMode mode, int variation, bool isHorizontal = false)
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

    static void AddDecoration(DecorationType type,int column, int row, int variation)
    {
        if (MapHolder.tiles[column,row].type != TileType.Land || MapHolder.decorationsTiles[column, row] != null && MapHolder.decorationsTiles[column,row].type != DecorationType.Flora)
        {
            return;
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
    }
    static void RemoveDecoration(DecorationType type, int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row]  == null ||
            MapHolder.decorationsTiles[column, row].type != DecorationType.Flora)
        {
            return;
        }

        AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
        MapHolder.decorationsTiles[column, row] = null;
    }

    static void AddToDecorationLimbo(DecorationTiles tile)
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
        decorationTilesLimbo[tile.type][tile.variation].Add(tile);
        tile.GoToLimbo();
    }
    static DecorationTiles GetTileFromDecorationLimbo(DecorationType type, int variation)
    {
        if (decorationTilesLimbo == null || decorationTilesLimbo.Count == 0 || 
            !decorationTilesLimbo.ContainsKey(type) || decorationTilesLimbo[type].Count < variation + 1 ||
            decorationTilesLimbo[type][variation].Count == 0)
        {
            DecorationTiles newTile =  new DecorationTiles(DecorationType.Flora);
            newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.floraPrefabDictionary[variation],newTile.decorationBackground));
            return newTile;
        }
        DecorationTiles oldTile = decorationTilesLimbo[type][variation][decorationTilesLimbo[type][variation].Count - 1];
        decorationTilesLimbo[type][variation].RemoveAt(decorationTilesLimbo[type][variation].Count - 1);
        oldTile.ReturnFromLimbo();
        //tile.decorationBackground.SetActive(true);
        return oldTile;
    }    
    
}
