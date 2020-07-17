using System.Collections;
using System.Collections.Generic;
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

        Controller.ChangeTile = ChangeTile;
        Controller.StartConstructionAction = terrainBuilder.StartConstruction;
        Controller.EndConstructionAction = terrainBuilder.EndConstruction;
    }

    public void ChangeTile(int column, int row, ToolType tool, ToolMode mode , int variation, DecorationType decorationType, bool isHorizontal)
    {
        switch (tool)
        {
            case ToolType.Waterscaping:
            case ToolType.CliffConstruction:
            case ToolType.PathPermit:
                terrainBuilder.ChangeTile(tool, mode, column, row, variation);
                break;
            case ToolType.BridgeMarkUp:
                break;
            case ToolType.InclineMarkUp:
                break;
            case ToolType.TreePlanting:
                break;
            case ToolType.BushPlanting:
            case ToolType.FlowerPlanting:
            case ToolType.FenceBuilding:
            case ToolType.BuildingsMarkUp:
                DecorationsBuilder.ChangeTile(column,row,decorationType,mode,variation,isHorizontal);
                break;
            /*case ToolType.BuildingsMarkUp:
                BuildingsBuilder.ChangeTile(column, row, mode, decorationType);
                break;*/
            case ToolType.Null:
                break;
        }
    }
}
