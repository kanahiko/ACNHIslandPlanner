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
    
    public static void ChangeTile(int column, int row, DecorationType type, ToolMode mode,int variation = -1, bool isHorizontal = true)
    {
        //check if tile has something in it
        //add to tile

        //what to do with house and building?
        //maybe make them fixed

        UniqueBuilding building = null;

        switch (type)
        {
            case DecorationType.Tree:
                break;
            case DecorationType.Flora:
                NonBuildingsBuilder.ChangeTile(column, row, type, mode, variation, isHorizontal);
                break;
            case DecorationType.Fence:
                FenceBuilder.ChangeTile(column,row,mode,variation,isHorizontal);
                //FenceBuilder
                break;
            case DecorationType.Plaza:
            case DecorationType.NookShop:
            case DecorationType.Tailors:
            case DecorationType.Museum:
            case DecorationType.PlayerHouse:
            case DecorationType.House:
            case DecorationType.Incline:
            case DecorationType.Bridge:
            case DecorationType.Camp:
                building = MapHolder.FindAvailiableBuilding(type);
                break;
        }
    }
}
