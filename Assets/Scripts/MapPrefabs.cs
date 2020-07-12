using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Prefabs", fileName = "MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    [Header("Materials")]
    public List<Material> gridMaterials;
    public List<Material> elevationMaterials;
    
    [Header("Prefabs")]

    public GameObject colliderPrefab;
    public List<GameObject> specialCurvedPath;
    public List<GameObject> cliffSidePrefabs;
    
    public TypeToPrefabDictionary prefabDictionary;

    [Header("Decoration prefabs")]
    public DecorationsPrefabDictionary decorationsPrefabDictionaryDictionary;
    public List<FenceList> fencePrefabDictionary;

    [Header("Minimap")]
    public TileTypeToColorDictionary tileTypeColorDictionary;

    public List<Color> elevationColors;
    
    public Vector2Int miniMapOffset = new Vector2Int(5,5);
    
    private int showGridId;
    private int showElevationId;
    private bool isGridShown = false;
    private bool isElevationShown = false;

    public void StartPrefab()
    {
        ResetShaders();

        showGridId =Shader.PropertyToID("ShowGrid");
        showElevationId =Shader.PropertyToID("ShowElevation");
    }

    public void ResetShaders()
    {
        if (gridMaterials != null)
        {
            isGridShown = false;
            for (int i = 0; i < gridMaterials.Count; i++)
            {
                gridMaterials[i].SetFloat(showGridId,isGridShown?1:0);
            }
        }
        
        if (elevationMaterials != null)
        {
            isElevationShown = false;
            for (int i = 0; i < elevationMaterials.Count; i++)
            {
                elevationMaterials[i].SetFloat(showElevationId,isElevationShown?1:0);
            }
        }
    }
    
    public void ShowGrid()
    {
        if (gridMaterials != null)
        {
            isGridShown = !isGridShown;
            for (int i = 0; i < gridMaterials.Count; i++)
            {
                gridMaterials[i].SetFloat(showGridId,isGridShown?1:0);
            }
        }
    }
    public void ShowElevation()
    {
        if (elevationMaterials != null)
        {
            isElevationShown = !isElevationShown;
            for (int i = 0; i < elevationMaterials.Count; i++)
            {
                elevationMaterials[i].SetFloat(showElevationId,isElevationShown?1:0);
            }
        }
    }
}

[Serializable]
public class FenceList
{
    public List<GameObject> fencePrefabs;
}

[System.Serializable]
public class DecorationsPrefabDictionary : SerializableDictionaryBase<DecorationType, GameObject> { }
/*

[System.Serializable]
public class FencePrefabDictionary : SerializableDictionaryBase<int, FenceList> { }*/

[System.Serializable]
public class TypeToPrefabDictionary : SerializableDictionaryBase<TilePrefabType, GameObject> { }

[System.Serializable]
public class TileTypeToColorDictionary : SerializableDictionaryBase<TileType, Color> { }