﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    static int maxDecorationLimboCount = 20;
    static int maxFencePartsLimboCount = 20;
    static List<DecorationTiles> fenceTilesLimbo;
    static List<List<List<Transform>>> fencePartsLimbo;
    public static void ChangeTile(int column, int row, ToolMode mode, int variation, bool isHorizontal)
    {
        if (mode == ToolMode.Add)
        {
            AddFence(column, row, variation, isHorizontal);
        }
        else
        {
            RemoveFence(column, row);
        }
    }

    //-----------------Adding fence
    /// <summary>
    /// Different methods because they are interconnected
    /// </summary>
    static void AddFence(int column, int row, int variation, bool isHorizontal)
    {
        if (MapHolder.tiles[column, row].type != TileType.Land || MapHolder.decorationsTiles[column, row] != null && MapHolder.decorationsTiles[column, row].type != DecorationType.Fence)
        {
            return;
        }
        if (MapHolder.decorationsTiles[column, row] != null)
        {
            if (MapHolder.decorationsTiles[column, row].variation != variation)
            {
                AddToFenceLimbo(MapHolder.decorationsTiles[column, row]);
            }
            /*if (MapHolder.decorationsTiles[column,row].type == DecorationType.Fence)
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
            }*/
        }
        else
        {
            MapHolder.decorationsTiles[column, row] = GetTileFromFenceLimbo();
        }


        MapHolder.decorationsTiles[column, row].decorationBackground.transform.parent = MapHolder.decorationsParent;
        MapHolder.decorationsTiles[column, row].decorationBackground.transform.localPosition = new Vector3(column, Util.GetHeight(column, row), -row);

        CheckQuarters(column, row, variation, isHorizontal);

        MapHolder.decorationsTiles[column, row].type = DecorationType.Fence;
        MapHolder.decorationsTiles[column, row].variation = variation;
        MapHolder.decorationsTiles[column, row].isHorizontal = Util.CheckIfChangeOrientation(column, row, isHorizontal) ? !isHorizontal : isHorizontal;

        RedoSurroundingFences(column, row);
    }

    static void RemoveFence(int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row] == null ||
            MapHolder.decorationsTiles[column, row].type != DecorationType.Fence)
        {
            return;
        }

        AddToFenceLimbo(MapHolder.decorationsTiles[column, row]);
        MapHolder.decorationsTiles[column, row] = null;

        RedoSurroundingFences(column, row);
    }


    static void RedoSurroundingFences(int column, int row)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!(column + Util.indexOffsetCross[i].y >= 0 && column + Util.indexOffsetCross[i].y < MapHolder.width &&
                 row + Util.indexOffsetCross[i].x >= 0 && row + Util.indexOffsetCross[i].x < MapHolder.height))
            {
                continue;
            }
            if (MapHolder.decorationsTiles[column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x] != null &&
                MapHolder.decorationsTiles[column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x].type == DecorationType.Fence)
            {
                DecorationTiles tile = MapHolder.decorationsTiles[column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x];

                CheckQuarters(column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x,
                    tile.variation,
                    tile.isHorizontal);

                tile.isHorizontal = Util.CheckIfChangeOrientation(column + Util.indexOffsetCross[i].y, row + Util.indexOffsetCross[i].x, tile.isHorizontal) ? !tile.isHorizontal : tile.isHorizontal;
            }
        }
    }

    static void CheckQuarters(int column, int row, int variation, bool isHorizontal)
    {

        bool[] corners = Util.CreateFenceMatrix(column, row);

        for (int i = 0; i < 4; i++)
        {
            //Debug.Log($"{isHorizontal}");
            //means (isHorizontal && i <= 1) || (!isHorizontal && i> 1)
            if ((isHorizontal && i <= 1) || (!isHorizontal && i > 1))
            {
                if (corners[Util.sortedDirectionalIndexes[i]])
                {
                    //linked
                    CreateQuarter(column, row, variation, true, Util.sortedDirectionalIndexes[i]);
                }
                else
                {
                    if (Util.NoneOrOne(corners))
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
                    //linked
                    CreateQuarter(column, row, variation, true, Util.sortedDirectionalIndexes[i]);
                }
                else
                {
                    RemoveQuarter(column, row, Util.sortedDirectionalIndexes[i]);
                }
            }
        }
    }

    static void CreateQuarter(int column, int row, int variation, bool isLinked, int rotation)
    {
        if (MapHolder.decorationsTiles[column, row].quarters[rotation] != null)
        {
            //Debug.Log($"{MapHolder.decorationsTiles[column, row].quarters[rotation]}");
            if (MapHolder.decorationsTiles[column, row].isLinked[rotation] == isLinked)
            {
                return;
            }
            AddToFencePartsLimbo(MapHolder.decorationsTiles[column, row].quarters[rotation],
                MapHolder.decorationsTiles[column, row].variation,
                MapHolder.decorationsTiles[column, row].isLinked[rotation]);
        }
        Debug.Log($"!!---- {column} {row}");
        MapHolder.decorationsTiles[column, row].quarters[rotation] = GetTileFromFencePartLimbo(variation, isLinked);

        MapHolder.decorationsTiles[column, row].quarters[rotation].transform.parent = MapHolder.decorationsTiles[column, row].decorationBackground.transform;
        MapHolder.decorationsTiles[column, row].quarters[rotation].transform.localPosition = Util.halfOffset;
        MapHolder.decorationsTiles[column, row].quarters[rotation].transform.localRotation = Quaternion.Euler(0, 90 * rotation, 0);

        MapHolder.decorationsTiles[column, row].isLinked[rotation] = isLinked;
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



    //-----------------Fence limbo
    static void AddToFenceLimbo(DecorationTiles tile)
    {
        if (fenceTilesLimbo == null)
        {
            fenceTilesLimbo = new List<DecorationTiles>();
        }

        if (fenceTilesLimbo.Count < maxDecorationLimboCount)
        {
            fenceTilesLimbo.Add(tile);
            tile.GoToLimbo();
            //tile.decorationBackground.SetActive(false);
            //tile.decorationBackground.parent = MapHolder.limboDecorationsParent;
            AddToFencePartsLimbo(tile);
        }
        else
        {
            AddToFencePartsLimbo(tile);
            GameObject.Destroy(tile.decorationBackground.gameObject);
            tile.Dispose();
        }
    }

    static void AddToFencePartsLimbo(DecorationTiles tile)
    {
        CheckForFenceParts(tile.variation);

        Debug.Log($"before limbo contains unlinked={fencePartsLimbo[0][0].Count} linked={fencePartsLimbo[0][1].Count}");
        for (int i = 0; i < 4; i++)
        {
            AddToFencePartLimbo(tile.quarters[i], tile.variation, tile.isLinked[i]);
            tile.quarters[i] = null;
        }

        Debug.Log($"after limbo contains unlinked={fencePartsLimbo[0][0].Count} linked={fencePartsLimbo[0][1].Count}");
    }

    static void AddToFencePartsLimbo(Transform part, int variation, bool isLinked)
    {
        CheckForFenceParts(variation);

        AddToFencePartLimbo(part, variation, isLinked);
    }
    static void CheckForFenceParts(int variation)
    {
        if (fencePartsLimbo == null)
        {
            fencePartsLimbo = new List<List<List<Transform>>>();
        }

        if (fencePartsLimbo.Count < variation + 1)
        {
            while (fencePartsLimbo.Count < variation + 1)
            {
                fencePartsLimbo.Add(new List<List<Transform>>() { new List<Transform>(), new List<Transform>() });
            }
        }
    }
        static void AddToFencePartLimbo(Transform part, int variation, bool isLinked)
    {
        if (part == null)
        {
            return;
        }
        if (fencePartsLimbo[variation][(isLinked ? 1 : 0)].Count < maxFencePartsLimboCount)
        {
            part.parent = MapHolder.limboDecorationsParent;
            part.position = Util.cullingPosition;
            //part.SetActive(false);
            fencePartsLimbo[variation][(isLinked ? 1 : 0)].Add(part);

            //Debug.Log($"!!---- {column} {fencePartsLimbo[variation][(isLinked ? 1 : 0)].Count}");
        }
        else
        {
            GameObject.Destroy(part.gameObject);
        }
    }

    static DecorationTiles GetTileFromFenceLimbo()
    {
        if (fenceTilesLimbo == null || fenceTilesLimbo.Count == 0)
        {
            return new DecorationTiles(DecorationType.Fence);
        }
        DecorationTiles tile = fenceTilesLimbo[fenceTilesLimbo.Count - 1];
        //Debug.Log($"{fenceTilesLimbo.Count}");
        fenceTilesLimbo.RemoveAt(fenceTilesLimbo.Count - 1);
        //Debug.Log($"{fenceTilesLimbo.Count}");

        //tile.decorationBackground.SetActive(true);
        return tile;
    }

    static Transform GetTileFromFencePartLimbo(int variation, bool isLinked)
    {
        if (fencePartsLimbo == null || fencePartsLimbo.Count <= variation || fencePartsLimbo[variation][(isLinked ? 1 : 0)].Count == 0)
        {
            return GameObject.Instantiate(MapHolder.mapPrefab.fencePrefabDictionary[variation].variationPrefabs[isLinked ? 1 : 0]).transform;
        }
        Transform tile = fencePartsLimbo[variation][isLinked ? 1 : 0][fencePartsLimbo[variation][isLinked ? 1 : 0].Count - 1];
        Debug.Log($"{isLinked} {fencePartsLimbo[variation][isLinked ? 1 : 0].Count}");
        fencePartsLimbo[variation][isLinked ? 1 : 0].RemoveAt(fencePartsLimbo[variation][isLinked ? 1 : 0].Count - 1);
        Debug.Log($"{isLinked} {fencePartsLimbo[variation][isLinked ? 1 : 0].Count}");
        //tile.SetActive(true);
        return tile;
    }
}