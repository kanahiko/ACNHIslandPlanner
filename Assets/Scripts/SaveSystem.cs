using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

public static class SaveSystem
{
    public static SaveSlot[] saveSlots;
    
    public static int currentSlot = -1;

    private static string savePath;
    
    static readonly string[] slotsNames = new string[]
    {
        "slot1.islandplan", "slot2.islandplan", "slot3.islandplan"
    };

    public static Action<int, bool> activateSlot;

    public static CameraInfo cameraInfo;
    public static Action LoadCameraInfo;
    public static void FindSlots()
    {
#if UNITY_WEBGL
        savePath = $"idbfs/acnhinslanddesigner";
#else
        savePath = $"{Application.persistentDataPath}/saves";
#endif
        saveSlots = new SaveSlot[slotsNames.Length];
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        for (int i = 0; i < slotsNames.Length;i++)
        {
            saveSlots[i] = new SaveSlot();

            saveSlots[i].path = $"{savePath}/{slotsNames[i]}";

            if (File.Exists(saveSlots[i].path))
            {
                saveSlots[i].isWritten = true;
                saveSlots[i].date = File.GetLastWriteTime(saveSlots[i].path).ToString("dd/MM/yyyy\nHH:mm:ss");
            }
        }
    }
    
    public static void Save(int index)
    {
        if (!Directory.Exists(savePath))
        {
            return;
        }

        SelectIndex(index);
        
        using (BinaryWriter writer = new BinaryWriter(File.Open(saveSlots[index].path, FileMode.Create)))
        {
            writer.Write(cameraInfo.position.x);
            writer.Write(cameraInfo.position.y);
            writer.Write(cameraInfo.position.z);
            writer.Write(cameraInfo.currentScroll);
            writer.Write(cameraInfo.isCameraParallelToGround);
            //writer.Write(width);
            //writer.Write(height);
            for (int i = 0; i < MapHolder.height; i++)
            {
                for (int j = 0; j < MapHolder.width; j++)
                {
                    writer.Write((byte)MapHolder.tiles[j,i].type);
                    writer.Write((byte)MapHolder.tiles[j, i].backgroundType);
                    for (int k = 0; k < 4; k++)
                     {
                         writer.Write((byte)MapHolder.tiles[j, i].prefabType[k]);
                     }
                     for (int k = 0; k < 4; k++)
                     {
                         writer.Write(MapHolder.tiles[j, i].cliffSidesType[k]);
                     }
                     writer.Write(MapHolder.tiles[j, i].diagonalRotation);
                     writer.Write(MapHolder.tiles[j, i].variation);
                     writer.Write(MapHolder.tiles[j, i].elevation);
                     writer.Write(MapHolder.tiles[j, i].curvedTileVariation);
                }
            }

            List<DecorationTiles> decorationTilesSave = new List<DecorationTiles>();
            List<Vector2Int> decorationTilesCoordinates = new List<Vector2Int>();

            for (int i = 0; i < MapHolder.height; i++)
            {
                for (int j = 0; j < MapHolder.width; j++)
                {
                    if (MapHolder.decorationsTiles[j,i] != null)
                    {
                        decorationTilesSave.Add(MapHolder.decorationsTiles[j, i]);
                        decorationTilesCoordinates.Add(new Vector2Int(j, i));
                    }
                }
            }
            writer.Write(decorationTilesSave.Count);

            for (int i = 0; i < decorationTilesSave.Count; i++)
            {
                writer.Write(decorationTilesCoordinates[i].x);
                writer.Write(decorationTilesCoordinates[i].y);
                writer.Write((byte)decorationTilesSave[i].type);
                //will check with type
                //writer.Write(tile.building != null);
                writer.Write(decorationTilesSave[i].startingColumn);
                writer.Write(decorationTilesSave[i].startingRow);
                if (decorationTilesSave[i].building != null)
                {
                    writer.Write((byte)decorationTilesSave[i].building.type);
                }

                for (int j = 0; j < 4; j++)
                {
                    writer.Write((byte)decorationTilesSave[i].isLinked[j]);
                }
                writer.Write(decorationTilesSave[i].rotation);
                writer.Write(decorationTilesSave[i].variation);
                writer.Write(decorationTilesSave[i].size);
                writer.Write((byte)decorationTilesSave[i].color);
            }
        }

        saveSlots[index].isWritten = true;
        saveSlots[index].date = DateTime.Now.ToString("dd/MM/yyyy\nHH:mm:ss");
        MapHolder.isDirty = false;

#if UNITY_WEBGL
        Application.ExternalEval("FS.syncfs(false, function (err) {})");
#endif
    }

    public static void Load(int index)
    {
        
        if (!Directory.Exists(savePath) || !File.Exists(saveSlots[index].path))
        {
            return;
        }
        
        SelectIndex(index);
        
        Dictionary<Vector2Int, List<Vector2Int>> buildings = new Dictionary<Vector2Int, List<Vector2Int>>();
        List<PreDecorationTile> preDecorationTiles = new List<PreDecorationTile>();

        using (BinaryReader reader = new BinaryReader(File.Open(saveSlots[index].path, FileMode.Open)))
        {
            //width = reader.ReadInt32();
            //height = reader.ReadInt32();
            cameraInfo.position.x = reader.ReadSingle();
            cameraInfo.position.y = reader.ReadSingle();
            cameraInfo.position.z = reader.ReadSingle();
            cameraInfo.currentScroll = reader.ReadSingle();
            cameraInfo.isCameraParallelToGround = reader.ReadBoolean();

            for (int i = 0; i< MapHolder.height; i++)
            {
                for (int j = 0; j < MapHolder.width; j++)
                {
                    MapHolder.tiles[j, i].type = (TileType)reader.ReadByte();
                    MapHolder.tiles[j, i].backgroundType = (TilePrefabType)reader.ReadByte();

                    for (int k = 0; k < 4; k++)
                    {
                        MapHolder.tiles[j, i].prefabType[k] = (TilePrefabType)reader.ReadByte();
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        MapHolder.tiles[j, i].cliffSidesType[k] = reader.ReadByte();
                    }
                    MapHolder.tiles[j, i].diagonalRotation = reader.ReadByte();
                    MapHolder.tiles[j, i].variation = reader.ReadByte();
                    MapHolder.tiles[j, i].elevation = reader.ReadByte();
                    MapHolder.tiles[j, i].curvedTileVariation = reader.ReadByte();
                }
            }
            int decorationsCount = reader.ReadInt32();

            for (int i = 0; i < decorationsCount; i++)
            {
                PreDecorationTile tile = new PreDecorationTile();
                tile.coordinates = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());

                tile.type = (DecorationType)reader.ReadByte();

                tile.startingCoords = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                if (tile.type == DecorationType.Building)
                {
                    tile.buildingType = (DecorationType)reader.ReadByte();

                    if (!buildings.ContainsKey(tile.startingCoords))
                    {
                        buildings.Add(tile.startingCoords, new List<Vector2Int>());
                    }

                    buildings[tile.startingCoords].Add(tile.coordinates);
                }

                for (int j = 0; j < 4; j++)
                {
                    tile.isLinked[j] = (FenceLinked)reader.ReadByte();
                }
                tile.rotation = reader.ReadInt32();
                tile.variation = reader.ReadByte();
                tile.size = reader.ReadInt32();
                tile.color = (FlowerColors)reader.ReadByte();

                preDecorationTiles.Add(tile);
            }
        }

        Controller.RebuildMap?.Invoke(buildings, preDecorationTiles);
        //Rebuild entire map
        //goes to building controller

        LoadCameraInfo?.Invoke();
        MapHolder.isDirty = false;
    }

    public static void Export(int index)
    {
        byte[] data = File.ReadAllBytes(saveSlots[index].path);
        WebDownloadHelper.InitiateDownload(slotsNames[index], data);
    }

    public static void Delete(int index)
    {
        if (Directory.Exists(savePath) && File.Exists(saveSlots[index].path))
        {
            File.Delete(saveSlots[index].path);
            saveSlots[index].isWritten = false;

            if (currentSlot == index)
            {
                MapHolder.isDirty = true;
            }
        }
    }
    
    public static void New(int index)
    {
        if (Directory.Exists(savePath) && File.Exists(saveSlots[index].path))
        {
            File.Delete(saveSlots[index].path);
            saveSlots[index].isWritten = false;
        }
        
        SelectIndex(index);
        
        //create empty map
        Controller.RebuildEmptyMap?.Invoke();
    }

    static void SelectIndex(int index)
    {
        if (currentSlot != index)
        {
            if (currentSlot != -1)
            {
                activateSlot?.Invoke(currentSlot,false);
            }
            currentSlot = index;
            activateSlot?.Invoke(currentSlot,true);
        }
    }
}
public static class WebGLExtensions
{
    /// <summary>
    /// Calls FS.syncfs in native js.
    /// </summary>
    [DllImport("__Internal")]
    public static extern void SyncFs();
}
public class WebDownloadHelper
{
    // Source: http://stackoverflow.com/a/27284736/1607924
    static string scriptTemplate = @"
             var link = document.createElement(""a"");
             link.download = '{0}';
             link.href = 'data:application/octet-stream;charset=utf-8;base64,{1}';
             document.body.appendChild(link);
             link.click();
             document.body.removeChild(link);
             delete link;
         ";

    static string test = " test test test";

    public static void InitiateDownload(string aName, byte[] aData)
    {
        string base64 = System.Convert.ToBase64String(aData);
        string script = string.Format(scriptTemplate, aName, base64);
        Application.ExternalEval(script);
    }
    public static void InitiateDownload(string aName, string aData)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(aData);
        InitiateDownload(aName, data);
    }
}
public class SaveSlot
{
    public bool isWritten;
    public string date = "";

    public string path;
}

[Serializable]
public class CameraInfo
{
    public Vector3 position;
    public bool isCameraParallelToGround;
    public float currentScroll;
}

public class PreDecorationTile
{
    public Vector2Int coordinates;
    public DecorationType type;

    public DecorationType buildingType;
    public Vector2Int startingCoords;

    public FenceLinked[] isLinked = new FenceLinked[4];
    public int rotation;
    public byte variation;
    public int size;
    public FlowerColors color;
}
