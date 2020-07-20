using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationsBuilder : MonoBehaviour
{
    ToolMode toolMode = ToolMode.None;

    HashSet<MapTile> ignoreRaycastTiles;

    Action<HashSet<Vector2Int>> ChangeMiniMap;
    HashSet<Vector2Int> changedCoordinates;

    private void Awake()
    {
        MapHolder.decorationsParent = transform;
    }
    
    public static void ChangeTile(int column, int row, DecorationType type,ToolType tool, ToolMode mode, byte rotation, byte variation = 0)
    {
        //check if tile has something in it
        //add to tile

        //what to do with house and building?
        //maybe make them fixed


        switch (type)
        {
            case DecorationType.Tree:
            case DecorationType.Flora:
                NonBuildingsBuilder.ChangeTile(column, row, type, mode, variation);
                break;
            case DecorationType.Fence:
                FenceBuilder.ChangeTile(column,row,mode,variation, (rotation == 0 || rotation == 2));
                //FenceBuilder
                break;
            case DecorationType.Plaza:
            case DecorationType.NookShop:
            case DecorationType.Tailors:
            case DecorationType.Museum:
            case DecorationType.PlayerHouse:
            case DecorationType.House:
            case DecorationType.Camp:
                BuildingsBuilder.ChangeTile(column, row, mode, (DecorationType)variation);
                break;
            case DecorationType.Incline:
            case DecorationType.Bridge:
                BridgesBuilder.ChangeTile(tool, mode, column, row, variation, rotation);
                break;
        }
    }

    public static void RebuildTile(Dictionary<Vector2Int, List<Vector2Int>> buildings, List<PreDecorationTile> preDecorationTiles)
    {
        Dictionary<Vector2Int, DecorationTiles> buildingDictionary = new Dictionary<Vector2Int, DecorationTiles>();

        for (int i = 0; i < preDecorationTiles.Count; i++)
        {
            int column = preDecorationTiles[i].coordinates.x;
            int row = preDecorationTiles[i].coordinates.y;
            if (MapHolder.decorationsTiles[column, row] != null)
            {
                switch (MapHolder.decorationsTiles[column, row].type)
                {
                    case DecorationType.Fence:
                        FenceBuilder.AddToFenceLimbo(MapHolder.decorationsTiles[column, row]);
                        break;
                    case DecorationType.Incline:
                        break;
                    case DecorationType.Bridge:
                        break;
                    case DecorationType.Flora:
                    case DecorationType.Tree:
                        NonBuildingsBuilder.AddToDecorationLimbo(MapHolder.decorationsTiles[column, row]);
                        break;
                    case DecorationType.Building:
                        MiniMap.PutPin(MapHolder.decorationsTiles[column, row].building.startingColumn, MapHolder.decorationsTiles[column, row].building.startingRow,
                            MapHolder.decorationsTiles[column, row].building.size.x, 0, MapHolder.decorationsTiles[column, row].building.type, false);
                        MapHolder.decorationsTiles[column, row].GoToLimbo();
                        break;
                }
            }

            switch (preDecorationTiles[i].type)
            {
                case DecorationType.Fence:
                    FenceBuilder.RebuildTile(column, row, preDecorationTiles[i]);
                    break;
                case DecorationType.Flora:
                case DecorationType.Tree:
                    NonBuildingsBuilder.RebuildTile(column, row, preDecorationTiles[i]);
                    break;
                case DecorationType.Building:
                    if (buildingDictionary.ContainsKey(preDecorationTiles[i].startingCoords))
                    {
                        MapHolder.decorationsTiles[column, row] = buildingDictionary[preDecorationTiles[i].startingCoords];
                    }
                    else
                    {
                        DecorationTiles buildingTile = BuildingsBuilder.RebuildTile(preDecorationTiles[i]);
                        buildingDictionary.Add(preDecorationTiles[i].startingCoords, buildingTile);
                    }
                    buildings[preDecorationTiles[i].startingCoords].Remove(preDecorationTiles[i].coordinates);

                    if (buildings[preDecorationTiles[i].startingCoords].Count == 0)
                    {
                        BuildingsBuilder.RebuildBuildingInfluence(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[preDecorationTiles[i].buildingType], 
                            buildingDictionary[preDecorationTiles[i].startingCoords]);

                       MiniMap.PutPin(preDecorationTiles[i].startingCoords.x, 
                           preDecorationTiles[i].startingCoords.y, 
                           MapHolder.mapPrefab.decorationsSizeDictionary[preDecorationTiles[i].buildingType].x,
                           MapHolder.mapPrefab.decorationsSizeDictionary[preDecorationTiles[i].buildingType].y, 
                           preDecorationTiles[i].buildingType, true);
                    }

                    break;
                case DecorationType.Incline:
                case DecorationType.Bridge:
                    break;

            }
        }

    }
}
