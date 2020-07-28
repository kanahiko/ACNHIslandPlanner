using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Prefabs", fileName = "MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public int width = 112; //92
    public int height = 96; //73

    public int maxBackLandIndex = 13;
    public int maxFrontLandIndex = 6;
    
    /// <summary>
    /// x - sides,
    /// y - bottom,
    /// z - top
    /// </summary>
    public Vector3Int sandStandardCreation;
    /// <summary>
    /// x - sides,
    /// y - bottom,
    /// z - top
    /// </summary>
    public Vector3Int seaStandardCreation;

    [Header("Materials")]
    public List<Material> gridMaterials;
    public List<Material> elevationMaterials;
    
    [Header("Prefabs")]

    public GameObject colliderPrefab;
    public List<GameObject> specialCurvedPath;
    public List<GameObject> cliffSidePrefabs;
    
    public TypeToPrefabDictionary prefabDictionary;
    public List<Material> pathVariationMaterial;

    [Header("Decoration prefabs")]
    public DecorationsPrefabDictionary decorationsPrefabDictionary;
    public List<VariationList> fencePrefabDictionary;
    public List<GameObject> floraPrefabDictionary;
    public List<GameObject> treePrefabDictionary;
    public List<FlowerPrefabs> flowerPrefabDictionary;
    [NonSerialized]
    public List<FlowerPrefabDictionary> flowerPrefabConvertedDictionary;

    public DecorationsSizeDictionary decorationsSizeDictionary;
    public DecorationsMaxCountDictionary maxCount;

    public List<GameObject> inclinePrefabDictionary;
    public List<BridgeSizeList> bridgePrefabDictionary;

    [Header("Minimap")]
    public TileTypeToColorDictionary tileTypeColorDictionary;

    public List<Color> elevationColors;
    
    public Vector2Int miniMapOffset = new Vector2Int(5,5);
    public Color plazaColor;

    public Color[] inclineColors =new Color[2];
    public Color bridgeColor =new Color();

    private int showGridId; 
    private int showElevationId;
    private bool isGridShown = false;
    private bool isElevationShown = false;

    public void StartPrefab()
    {
        ResetShaders();

        showGridId =Shader.PropertyToID("ShowGrid");
        showElevationId =Shader.PropertyToID("ShowElevation");

        flowerPrefabConvertedDictionary = new List<FlowerPrefabDictionary>();
        for (int i = 0; i < flowerPrefabDictionary.Count; i++)
        {
            FlowerPrefabDictionary dictionary = new FlowerPrefabDictionary();

            for (int j = 0; j < flowerPrefabDictionary[i].flowers.Count; j++)
            {
                if (flowerPrefabDictionary[i].flowers[j] != null)
                {
                    dictionary.Add((FlowerColors)j,flowerPrefabDictionary[i].flowers[j]);
                }
            }
            
            flowerPrefabConvertedDictionary.Add(dictionary);
        }
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
#if UNITY_EDITOR
    public void ReplacePrefab(GameObject newPrefab)
    {
        for (int i = 0; i < specialCurvedPath.Count; i++)
        {
            if (specialCurvedPath[i].name == newPrefab.name)
            {
                specialCurvedPath[i] = newPrefab;
                return;
            }
        }
        for (int i = 0; i < cliffSidePrefabs.Count; i++)
        {
            if (cliffSidePrefabs[i].name == newPrefab.name)
            {
                cliffSidePrefabs[i] = newPrefab;
                return;
            }
        }
        string key = "";
        TilePrefabType keyType = TilePrefabType.Null;
        foreach (var prefab in prefabDictionary)
        {
            if (prefab.Value && prefab.Value.name == newPrefab.name)
            {
                keyType = prefab.Key;
            }
        }
        if (keyType != TilePrefabType.Null)
        {
            prefabDictionary[keyType] = newPrefab;
            return;
        }
        DecorationType decKey = DecorationType.Null;
        foreach (var prefab in decorationsPrefabDictionary)
        {
            if (prefab.Value.name == newPrefab.name)
            {
                decKey = prefab.Key;
            }
        }
        if (decKey != DecorationType.Null)
        {
            decorationsPrefabDictionary[decKey] = newPrefab;
            return;
        }
        for (int i = 0; i < fencePrefabDictionary.Count; i++)
        {
            for (int j = 0; j < fencePrefabDictionary[i].variationPrefabs.Count; j++)
            {
                if (fencePrefabDictionary[i].variationPrefabs[j].name == newPrefab.name)
                {
                    fencePrefabDictionary[i].variationPrefabs[j] = newPrefab;
                    return;
                }
            }
        }
        for (int i = 0; i < floraPrefabDictionary.Count; i++)
        {
            if (floraPrefabDictionary[i].name == newPrefab.name)
            {
                floraPrefabDictionary[i] = newPrefab;
                return;
            }
        }
        for (int i = 0; i < treePrefabDictionary.Count; i++)
        {
            if (treePrefabDictionary[i].name == newPrefab.name)
            {
                treePrefabDictionary[i] = newPrefab;
                return;
            }
        }
        for (int i = 0; i < flowerPrefabDictionary.Count; i++)
        {
            for (int j = 0; j < flowerPrefabDictionary[i].flowers.Count; j++)
            {
                if (flowerPrefabDictionary[i].flowers[j] && flowerPrefabDictionary[i].flowers[j].name == newPrefab.name)
                {
                    flowerPrefabDictionary[i].flowers[j] = newPrefab;
                    return;
                }
            }
        }
        for (int i = 0; i < inclinePrefabDictionary.Count; i++)
        {
            if (inclinePrefabDictionary[i].name == newPrefab.name)
            {
                inclinePrefabDictionary[i] = newPrefab;
                return;
            }
        }
        for (int i = 0; i < bridgePrefabDictionary.Count; i++)
        {
            for (int j = 0; j < bridgePrefabDictionary[i].bridgePrefabs.Count; j++)
            {
                if (bridgePrefabDictionary[i].bridgePrefabs[j].name == newPrefab.name)
                {
                    bridgePrefabDictionary[i].bridgePrefabs[j] = newPrefab;
                    return;
                }
            }
        }
    }
#endif
}

[Serializable]
public class VariationList
{
    public List<GameObject> variationPrefabs;
}
[Serializable]
public class BridgeSizeList
{
    public List<GameObject> bridgePrefabs;

    /*public BridgeSizeList()
    {
        bridgePrefabs = new GameObject[6];
    }*/
}

[Serializable]
public class FlowerPrefabs
{
    public List<GameObject> flowers;
}

[System.Serializable]
public class FlowerPrefabDictionary : SerializableDictionaryBase<FlowerColors, GameObject> { }

[System.Serializable]
public class DecorationsPrefabDictionary : SerializableDictionaryBase<DecorationType, GameObject> { }

[System.Serializable]
public class DecorationsSizeDictionary : SerializableDictionaryBase<DecorationType, Vector3Int> { }

[System.Serializable]
public class DecorationsMaxCountDictionary : SerializableDictionaryBase<DecorationType, int> { }
/*
[System.Serializable]
public class FencePrefabDictionary : SerializableDictionaryBase<int, FenceList> { }*/

[System.Serializable]
public class TypeToPrefabDictionary : SerializableDictionaryBase<TilePrefabType, GameObject> { }

[System.Serializable]
public class TileTypeToColorDictionary : SerializableDictionaryBase<TileType, Color> { }