using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBuildingsBuilder
{
    static int maxDecorationLimboCount = 20;
    static int maxFencePartsLimboCount = 20;
    static Dictionary<DecorationType, List<List<DecorationTiles>>> decorationTilesLimbo;
    static List<DecorationTiles> fenceTilesLimbo;
    static List<List<List<GameObject>>> fencePartsLimbo;
    //for non unique structures like fence, flowers, bushes, trees

    public static void AddTile(int column, int row, int variation, bool isHorizontal = false)
    {

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
    }

    static void AddToFenceLimbo(DecorationTiles tile)
    {
        if (fenceTilesLimbo == null)
        {
            fenceTilesLimbo = new List<DecorationTiles>();
        }

        if (fenceTilesLimbo.Count < maxDecorationLimboCount)
        {
            fenceTilesLimbo.Add(tile);
            AddToFencePartsLimbo(tile);
        }
        else
        {
            AddToFencePartsLimbo(tile);
            tile.Dispose();
        }
    }

    static void AddToFencePartsLimbo(DecorationTiles tile)
    {
        if (fencePartsLimbo == null)
        {
            fencePartsLimbo = new List<List<List<GameObject>>>();
        }

        if (fencePartsLimbo.Count < tile.variation + 1)
        {
            while (fencePartsLimbo.Count < tile.variation + 1)
            {
                fencePartsLimbo.Add(new List<List<GameObject>>());
                fencePartsLimbo[fenceTilesLimbo.Count - 1].Add(new List<GameObject>());
                fencePartsLimbo[fenceTilesLimbo.Count - 1].Add(new List<GameObject>());
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (tile.quarters[i])
            {
                if (fencePartsLimbo[tile.variation][(tile.isLinked[i] ? 1 : 0)].Count < maxFencePartsLimboCount)
                {
                    fencePartsLimbo[tile.variation][(tile.isLinked[i] ? 1 : 0)].Add(tile.quarters[i]);
                    tile.quarters[i] = null;
                }
                else
                {
                    GameObject.Destroy(tile.quarters[i]);
                }
            }
        }
    }

    static void AddToFencePartsLimbo(GameObject part, int variation, bool isLinked)
    {
        if (fencePartsLimbo == null)
        {
            fencePartsLimbo = new List<List<List<GameObject>>>();
        }

        if (fencePartsLimbo.Count < variation + 1)
        {
            while (fencePartsLimbo.Count < variation + 1)
            {
                fencePartsLimbo.Add(new List<List<GameObject>>());
                fencePartsLimbo[fenceTilesLimbo.Count - 1].Add(new List<GameObject>());
                fencePartsLimbo[fenceTilesLimbo.Count - 1].Add(new List<GameObject>());
            }
        }
        if (fencePartsLimbo[variation][(isLinked ? 1 : 0)].Count < maxFencePartsLimboCount)
        {
            fencePartsLimbo[variation][(isLinked ? 1 : 0)].Add(part);
        }
        else
        {
            GameObject.Destroy(part);
        }
    }

    static DecorationTiles GetTileFromFenceLimbo()
    {
        if  (fenceTilesLimbo == null || fenceTilesLimbo.Count == 0)
        {
            return new DecorationTiles(DecorationType.Fence);
        }
        DecorationTiles tile = fenceTilesLimbo[fenceTilesLimbo.Count - 1];
        fenceTilesLimbo.RemoveAt(fenceTilesLimbo.Count - 1);

        return tile;
    }
    static GameObject GetTileFromFencePartLimbo(int variation, bool isLinked)
    {
        if (fencePartsLimbo == null || fencePartsLimbo.Count <= variation || fencePartsLimbo[variation][(isLinked?1:0)].Count == 0)
        {
            return GameObject.Instantiate(MapHolder.mapPrefab.fencePrefabDictionary[variation].fencePrefabs[isLinked ? 1 : 0]);
        }
        GameObject tile = fencePartsLimbo[variation][isLinked?1:0][fencePartsLimbo[variation][isLinked ? 1 : 0].Count- 1];
        fenceTilesLimbo.RemoveAt(fencePartsLimbo[variation][isLinked ? 1 : 0].Count - 1);

        return tile;
    }

    /// <summary>
    /// Diffenrent methods because they are interconnected
    /// </summary>
    static void AddFence(int column, int row, int variation, bool isHorizontal)
    {
        if (MapHolder.tiles[column, row].type != TileType.Land)
        {
            return;
        }
        if (MapHolder.decorationsTiles[column,row] != null)
        {
            if (MapHolder.decorationsTiles[column,row].type == DecorationType.Fence)
            {
                if (MapHolder.decorationsTiles[column,row].variation != variation)
                {
                    AddToFenceLimbo(MapHolder.decorationsTiles[column, row]);
                }
            }
            else
            {
                AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
                MapHolder.decorationsTiles[column, row] = GetTileFromFenceLimbo();
            }
        }
        else
        {
            MapHolder.decorationsTiles[column, row] = GetTileFromFenceLimbo();
        }

        CheckQuarters(column, row, variation , isHorizontal);

        MapHolder.decorationsTiles[column, row].type = DecorationType.Fence;
        MapHolder.decorationsTiles[column, row].variation = variation;
        MapHolder.decorationsTiles[column, row].isHorizontal = isHorizontal;
    } 

    static void CheckQuarters(int column, int row, int variation, bool isHorizontal)
    {
        
        bool[] corners = Util.CreateFenceMatrix(column,row);

        for (int i = 0; i < 4; i++)
        {
            //means (isHorizontal && i <= 1) || (!isHorizontal && i> 1)
            if (isHorizontal ^ i <= 1)
            {
                if (corners[Util.sortedDirectionalIndexes[i]])
                {
                    //linked
                    CreateQuarter(column, row, variation, true, Util.sortedDirectionalIndexes[i]);
                }
                else
                {
                    if (NoneOrOne(corners))
                    {
                        //unlinked
                        CreateQuarter(column, row, variation, false, Util.sortedDirectionalIndexes[i]);
                    }
                    else
                    {
                        RemoveQuarter(column, row, Util.sortedDirectionalIndexes[i]);
                    }


                }
            }
            else
            {
                if (corners[Util.sortedDirectionalIndexes[i]])
                {
                    CreateQuarter(column, row, variation, true, Util.sortedDirectionalIndexes[i]);
                }
                else
                {
                    RemoveQuarter(column, row, Util.sortedDirectionalIndexes[i]);
                }
            }
        }
    }

    static void RemoveQuarter(int column, int row, int rotation)
    {
        if (MapHolder.decorationsTiles[column, row].quarters[rotation] != null)
        {
            AddToFencePartsLimbo(MapHolder.decorationsTiles[column, row].quarters[rotation],
              MapHolder.decorationsTiles[column, row].variation,
              MapHolder.decorationsTiles[column, row].isLinked[rotation]);

            MapHolder.decorationsTiles[column, row].quarters[rotation] = null;
        }
    }

    static void CreateQuarter(int column, int row, int variation, bool isLinked, int rotation)
    {
        if (MapHolder.decorationsTiles[column, row].quarters[rotation] != null)
        {
            if (MapHolder.decorationsTiles[column, row].isLinked[rotation])
            {
                return;
            }
            else
            {
                AddToFencePartsLimbo(MapHolder.decorationsTiles[column, row].quarters[rotation],
                  MapHolder.decorationsTiles[column, row].variation,
                  MapHolder.decorationsTiles[column, row].isLinked[rotation]);
                MapHolder.decorationsTiles[column, row].quarters[rotation] = GetTileFromFencePartLimbo(variation, isLinked);
            }
        }
        else
        {
            MapHolder.decorationsTiles[column, row].quarters[rotation] = GetTileFromFencePartLimbo(variation, isLinked);
        }
        MapHolder.decorationsTiles[column, row].quarters[rotation].transform.localPosition = Vector3.zero;

        MapHolder.decorationsTiles[column, row].quarters[rotation].transform.localRotation = Quaternion.Euler(0, 90 * rotation, 0);
        MapHolder.decorationsTiles[column, row].isLinked[rotation] = isLinked;
    }

    static bool NoneOrOne(bool[] corners)
    {
        int sum = 0;
        for (int i = 0; i< 4; i++)
        {
            if (corners[i])
            { 
                sum++; 
            }
        }

        return sum<= 1;
    }

    
}
