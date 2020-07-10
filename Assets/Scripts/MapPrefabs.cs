using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Prefabs", fileName = "MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<GameObject> specialCurvedPath;
    public List<GameObject> cliffSidePrefabs;
    
    public List<TileObject> lookUpTile;
    
    public Dictionary<TilePrefabType, GameObject> lookUpTilePrefab;

    public void ConvertToDictionary()
    {
        lookUpTilePrefab = new Dictionary<TilePrefabType, GameObject>();
        for (int i = 0; i < lookUpTile.Count; i++)
        {
            lookUpTilePrefab.Add(lookUpTile[i].type, lookUpTile[i].prefab);
        }
    }
}