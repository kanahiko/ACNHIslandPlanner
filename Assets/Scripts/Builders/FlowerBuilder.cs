using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBuilder : MonoBehaviour
{
    private static List<Dictionary<FlowerColors, List<DecorationTiles>>> flowersLimbo;
    private static int maxFlowersLimbo = 30;
    
    public static void ChangeTile(int column, int row,DecorationType type, ToolMode mode, byte variation, FlowerColors color)
    {
        if (mode == ToolMode.Add)
        {
            AddDecoration(type, column, row, variation, color);
        }
        else
        {
            RemoveDecoration(type, column, row);
        }
    }
    
    public static void RebuildTile(int column, int row, PreDecorationTile tile)
    {
        MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(tile.type, tile.variation, tile.color);
        MapHolder.decorationsTiles[column, row].decorationBackground.parent = MapHolder.decorationsParent;
        MapHolder.decorationsTiles[column, row].decorationBackground.localPosition = new Vector3(column, Util.GetHeight(column, row), -row);

        MapHolder.decorationsTiles[column, row].type = tile.type;
        MapHolder.decorationsTiles[column, row].variation = tile.variation;
        MapHolder.decorationsTiles[column, row].color = tile.color;
    }

    static void AddDecoration(DecorationType type,int column, int row, byte variation, FlowerColors color)
    {
        
        if (MapHolder.tiles[column,row].type != TileType.Land && MapHolder.tiles[column,row].type != TileType.Sand ||
            MapHolder.tiles[column,row].type == TileType.Sand && (type != DecorationType.Flora || Util.IsOnLandSandBorder(column, row)) ||
            MapHolder.decorationsTiles[column, row] != null  &&
            (MapHolder.decorationsTiles[column,row].type != DecorationType.Flower) ||
            !MapHolder.mapPrefab.flowerPrefabConvertedDictionary[variation].ContainsKey(color))
        {
            return;
        }

        if (MapHolder.decorationsTiles[column, row] != null)
        {
               
            if (MapHolder.decorationsTiles[column, row].variation != variation || MapHolder.decorationsTiles[column, row].color != color)
            { 
                AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
                MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(type, variation,color);
            }
        }
        else
        {
            MapHolder.decorationsTiles[column, row] = GetTileFromDecorationLimbo(type, variation,color);
        }


        MapHolder.decorationsTiles[column, row].decorationBackground.parent = MapHolder.decorationsParent;
        MapHolder.decorationsTiles[column, row].decorationBackground.localPosition = new Vector3(column, Util.GetHeight(column, row), -row);

        MapHolder.decorationsTiles[column, row].type = type;
        MapHolder.decorationsTiles[column, row].variation = variation;
        MapHolder.decorationsTiles[column, row].color = color;
    }
    
    static void RemoveDecoration(DecorationType type, int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row]  == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Flower))
        {
            return;
        }

        AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
        MapHolder.decorationsTiles[column, row] = null;
    }

    public static void AddToDecorationLimbo(DecorationTiles tile)
    {
        if (flowersLimbo == null)
        {
            flowersLimbo = new List<Dictionary<FlowerColors, List<DecorationTiles>>>();
        }
        if (flowersLimbo.Count < tile.variation + 1)
        {
            while (flowersLimbo.Count < tile.variation + 1)
            {
                flowersLimbo.Add(new Dictionary<FlowerColors, List<DecorationTiles>>());
            }
        }
        if (!flowersLimbo[tile.variation].ContainsKey(tile.color))
        {
            flowersLimbo[tile.variation].Add(tile.color,new List<DecorationTiles>());
        }
        
        if (flowersLimbo[tile.variation][tile.color].Count < maxFlowersLimbo)
        {
            flowersLimbo[tile.variation][tile.color].Add(tile);
            tile.GoToLimbo();
        }
        else
        {
            GameObject.Destroy(tile.mainTile);
            GameObject.Destroy(tile.decorationBackground.gameObject);
            tile.Dispose();
        }
    }
    
    static DecorationTiles GetTileFromDecorationLimbo(DecorationType type, byte variation, FlowerColors color)
    {
        if (flowersLimbo == null || flowersLimbo.Count < variation + 1 ||
            !flowersLimbo[variation].ContainsKey(color) || 
            flowersLimbo[variation][color].Count > 0)
        {
            if (type == DecorationType.Flora) {
                DecorationTiles newTile = new DecorationTiles(DecorationType.Flower);
                newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.flowerPrefabConvertedDictionary[variation][color], newTile.decorationBackground));
                return newTile;
            }
            else
            {
                DecorationTiles newTile = new DecorationTiles(DecorationType.Flower);
                newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.flowerPrefabConvertedDictionary[variation][color], newTile.decorationBackground));
                return newTile;
            }
        }
        DecorationTiles oldTile = flowersLimbo[variation][color][flowersLimbo[variation][color].Count - 1];
        flowersLimbo[variation][color].RemoveAt(flowersLimbo[variation][color].Count - 1);
        oldTile.ReturnFromLimbo();
        //tile.decorationBackground.SetActive(true);
        return oldTile;
    }    
}
