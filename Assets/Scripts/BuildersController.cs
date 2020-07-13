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

    private void Awake()
    {
        MapHolder.elevationLevels = elevationLevels;
        MapHolder.mapPrefab = mapPrefab;

        MapHolder.StartMapHolder();

        MapHolder.tiles = new MapTile[MapHolder.width, MapHolder.height];


        MapHolder.offset = offsetTerrain;
        transform.position = offsetTerrain;


        terrainBuilder.CreateEmptyLand(MapHolder.width, MapHolder.height);

        Controller.ChangeTile = ChangeTile;
        Controller.StartConstructionAction = terrainBuilder.StartConstruction;
        Controller.EndConstructionAction = terrainBuilder.EndConstruction;
    }

    public void ChangeTile(int column, int row, ToolType tool, ToolMode mode , int variation, DecorationType decorationType)
    {
        switch (tool)
        {
            case ToolType.Waterscaping:
            case ToolType.CliffConstruction:
            case ToolType.PathPermit:
                terrainBuilder.ChangeTile(tool, mode, column, row);
                break;
            case ToolType.BridgeMarkUp:
                break;
            case ToolType.InclineMarkUp:
                break;
            case ToolType.BushPlanting:
                break;
            case ToolType.TreePlanting:
                break;
            case ToolType.FlowerPlanting:
                break;
            case ToolType.FenceBuilding:
                break;
            case ToolType.BuildingsMarkUp:
                break;
            case ToolType.Null:
                break;
        }
    }
}
