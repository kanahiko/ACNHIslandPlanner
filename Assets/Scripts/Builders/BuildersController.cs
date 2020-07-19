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
    }

    public void ChangeTile(int column, int row, ToolType tool, ToolMode mode , int variation, DecorationType decorationType, int rotation)
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
                RebuildMap();
                return;
                BridgesBuilder.ChangeTile(tool,mode,column,row,variation,rotation);
                break;
            case ToolType.TreePlanting:
            case ToolType.BushPlanting:
            case ToolType.FlowerPlanting:
            case ToolType.FenceBuilding:
            case ToolType.BuildingsMarkUp:
                DecorationsBuilder.ChangeTile(column,row,decorationType,mode,variation, (rotation == 0 || rotation == 2));
                break;
            /*case ToolType.BuildingsMarkUp:
                BuildingsBuilder.ChangeTile(column, row, mode, decorationType);
                break;*/
            case ToolType.Null:
                break;
        }
    }

    public void RebuildMap()
    {
        /*File.WriteAllText(@"c:\movie.json", JsonConvert.SerializeObject(MapHolder.tiles));

        // serialize JSON directly to a file
        using (StreamWriter file = File.CreateText(@"c:\movie.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, movie);
        }*/
        string json = JsonUtility.ToJson(MapHolder.tiles[30,30]);
        
        File.WriteAllText(@"D:\test.txt",json);
        
        XmlSerializer serializer = new XmlSerializer(typeof(MapTile));
        StreamWriter writer = new StreamWriter(@"D:\hero.xml");
        serializer.Serialize(writer.BaseStream, MapHolder.tiles[30,30]);
        writer.Close();
        
        serializer = new XmlSerializer(typeof(MapTile));
        StreamReader reader = new StreamReader(@"D:\hero.xml");
        MapTile deserialized = (MapTile)serializer.Deserialize(reader.BaseStream);
        reader.Close();
    }

}
