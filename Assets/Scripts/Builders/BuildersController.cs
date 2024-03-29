﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class BuildersController : MonoBehaviour
{
    public TerrainBuilder terrainBuilder;
    public DecorationsBuilder decorationsBuilder;
    public Vector3 offsetTerrain = new Vector3(-6, 0, 2);

    public MapPrefabs mapPrefab;
    public List<Transform> elevationLevels;
    public Transform decorationsLimbo;

    private void Awake()
    {
        MapHolder.elevationLevels = elevationLevels;
        MapHolder.mapPrefab = mapPrefab;
        MapHolder.limboDecorationsParent = decorationsLimbo;
        MapHolder.StartMapHolder();

        MapHolder.offset = offsetTerrain;
        transform.position = offsetTerrain;


        LandBuilder.CreateEmptyLand(MapHolder.width, MapHolder.height);
        
        BuildingsBuilder.CreateUniqueBuildings();

        Controller.ChangeTile = ChangeTile;
        Controller.StartConstructionAction = terrainBuilder.StartConstruction;
        Controller.EndConstructionAction = terrainBuilder.EndConstruction;
        Controller.RebuildMap = RebuildMap;
        Controller.RebuildEmptyMap = RebuildEmptyMap;
    }

    public void ChangeTile(int column, int row, ToolType tool, ToolMode mode , byte variation, DecorationType decorationType, byte rotation,FlowerColors color)
    {
        switch (tool)
        {
            case ToolType.Waterscaping:
            case ToolType.CliffConstruction:
            case ToolType.PathPermit:
            case ToolType.SandPermit:
                terrainBuilder.ChangeTile(tool, mode, column, row, variation);
                break;
            case ToolType.BridgeMarkUp:
            case ToolType.InclineMarkUp:
            case ToolType.TreePlanting:
            case ToolType.BushPlanting:
            case ToolType.FlowerPlanting:
            case ToolType.FenceBuilding:
            case ToolType.BuildingsMarkUp:
                DecorationsBuilder.ChangeTile(column,row,decorationType,tool,mode,rotation, variation, color);
                break;
            /*case ToolType.BuildingsMarkUp:
                BuildingsBuilder.ChangeTile(column, row, mode, decorationType);
                break;*/
            case ToolType.Null:
                break;
        }

        MapHolder.isDirty = true;
    }

    void RebuildEmptyMap()
    {
        ResetInfluence();
        DecorationsBuilder.ResetDecorations();
        LandBuilder.CreateEmptyLand(MapHolder.width, MapHolder.height);
        MiniMap.RebuildMap();
    }

    public static void ResetInfluence()
    {
        for (int i = 0; i < MapHolder.height; i++)
        {
            for (int j = 0; j < MapHolder.width; j++)
            {
                MapHolder.treeInfluence[j, i] = 0;
                MapHolder.buildingsInfluence[j, i] = BuildingInfluence.noInfluence;
            }
        }
    }
    public void RebuildMap(Dictionary<Vector2Int, List<Vector2Int>> buildings, List<PreDecorationTile> preDecorationTiles)
    {
        ResetInfluence();
        DecorationsBuilder.ResetDecorations();

        for (int i = 0; i < MapHolder.height; i++)
        {
            for (int j = 0; j < MapHolder.width; j++)
            {
                terrainBuilder.RebuildTile(MapHolder.tiles[j,i]);
            }
        }

        DecorationsBuilder.RebuildTile(buildings, preDecorationTiles);

        MiniMap.RebuildMap();
    }


    public static int CheckBridgeSize(int column, int row, int rotation)
    {
        if (rotation == 1 || rotation == 3)
        {
            int bridgeSize = 0;
            for (int i =0; i< 5; i++)
            {
                if (i==0)
                {
                    if (MapHolder.tiles[column, row].type != TileType.WaterDiagonal &&
                    MapHolder.tiles[column, row].backgroundType != TilePrefabType.Land)
                    {
                        return 3; 
                    }
                    continue;            
                }

                if (MapHolder.tiles[column + i * (rotation == 3 ? -1 : 1), row - i].backgroundType == TilePrefabType.Land ||
                   MapHolder.tiles[column + i * (rotation == 3 ? -1 : 1), row - i].type == TileType.WaterDiagonal)
                {
                    if (bridgeSize <= 2)
                    {
                        return 3;
                    }

                    if (MapHolder.tiles[column + i * (rotation == 3 ? -1 : 1), row - i].backgroundType == TilePrefabType.Land &&
                        MapHolder.tiles[column, row].type != TileType.WaterDiagonal)
                    {

                        return 6;
                    }

                    return 7;
                }

                if (MapHolder.tiles[column + i *(rotation == 3 ? -1 : 1) ,row - i].type == TileType.Water)
                {
                    bridgeSize++;
                }
            }
        }
        //Debug.Log($"{column}, {row}");
        if (MapHolder.tiles[column,row].backgroundType == TilePrefabType.Land)
        {
            int columnIncrement = rotation == 0 ? 0 : 1;
            int rowIncrement = rotation == 0 ? 1 : 0;
            int size = 0;
            for (int i = 1; i < 7; i++)
            {
                if (Util.CoordinateExists(column + i * columnIncrement, row - i * rowIncrement))
                {
                    if (MapHolder.tiles[column + i * columnIncrement, row - i * rowIncrement].type == TileType.Water)
                    {
                        size++;
                    }
                    else
                    {
                        if (MapHolder.tiles[column + i * columnIncrement, row - i * rowIncrement].backgroundType == TilePrefabType.Land)
                        {
                            if (size < 3)
                            {
                                return 3;
                            }

                            return size;
                        }

                        return 3;
                    }
                }
                else
                {
                    if (size == 0)
                    {
                        return 3;
                    }
                    return size;
                }
            }
        }

        return 3;
    }
}
