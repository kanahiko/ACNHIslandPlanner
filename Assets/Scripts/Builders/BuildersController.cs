using System.Collections;
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
    }

    public void ChangeTile(int column, int row, ToolType tool, ToolMode mode , byte variation, DecorationType decorationType, byte rotation)
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
                DecorationsBuilder.ChangeTile(column,row,decorationType,tool,mode,rotation, variation);
                break;
            /*case ToolType.BuildingsMarkUp:
                BuildingsBuilder.ChangeTile(column, row, mode, decorationType);
                break;*/
            case ToolType.Null:
                break;
        }

    }

    public void RebuildMap(Dictionary<Vector2Int, List<Vector2Int>> buildings, List<PreDecorationTile> preDecorationTiles)
    {
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

}
[XmlRoot("main")]
public class SaveMap
{
    [XmlIgnore]
    public MapTile[,] tiles;

    [XmlArray("Tiles"), XmlArrayItem("tile")]
    public MapTile[] tilesForSave
    {
        get => Flatten(tiles);
        set
        {
            Expand(value, 96);
        }
    }

    public static T[] Flatten<T>(T[,] arr)
    {
        int rows0 = arr.GetLength(0);
        int rows1 = arr.GetLength(1);
        T[] arrFlattened = new T[rows0 * rows1];
        for (int j = 0; j < rows1; j++)
        {
            for (int i = 0; i < rows0; i++)
            {
                var test = arr[i, j];
                arrFlattened[i + j * rows0] = arr[i, j];
            }
        }
        return arrFlattened;
    }
    public static T[,] Expand<T>(T[] arr, int rows0)
    {
        int length = arr.GetLength(0);
        int rows1 = length / rows0;
        T[,] arrExpanded = new T[rows0, rows1];
        for (int j = 0; j < rows1; j++)
        {
            for (int i = 0; i < rows0; i++)
            {
                arrExpanded[i, j] = arr[i + j * rows0];
            }
        }
        return arrExpanded;
    }
}
