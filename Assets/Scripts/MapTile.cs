using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class MapTile
{
    public TileType type;
    
    public GameObject colliderObject;
    public GameObject backgroundTile;
    
    public GameObject[] quarters;
    public GameObject[] cliffSides;
    /// <summary>
    /// false means regular side
    /// true = water side
    /// </summary>
    
    public byte[] cliffSidesType;

    public TilePrefabType backgroundType;

    public TilePrefabType[] prefabType;

    public byte diagonalRotation = 255;

    private BoxCollider collider;

    public byte variation;

    public byte elevation = 0;

    public byte curvedTileVariation = 255;

    public MapTile()
    {
        colliderObject = GameObject.Instantiate(MapHolder.mapPrefab.colliderPrefab);
        collider = colliderObject.GetComponent<BoxCollider>();
        quarters = new GameObject[4];
        prefabType = new TilePrefabType[4];
        cliffSides = new GameObject[4];
        cliffSidesType = new byte[4] { 255, 255, 255, 255 };
    }
    
    public MapTile(GameObject tile)
    {
        colliderObject = GameObject.Instantiate(MapHolder.mapPrefab.colliderPrefab);
        collider = colliderObject.GetComponent<BoxCollider>();
        this.backgroundTile = tile;
        backgroundTile.transform.SetParent(colliderObject.transform,false);
        quarters = new GameObject[4];
        prefabType = new TilePrefabType[4];
        cliffSides = new GameObject[4];
        cliffSidesType = new byte[4] { 255, 255, 255, 255 };
    }

    public void SetCreatedElevation(Transform elevation)
    {
        colliderObject.transform.SetParent(elevation,false);
    }
    
    public void SetElevation(Transform elevation)
    {
        if (backgroundTile == null)
        {
            colliderObject.transform.SetParent(elevation, true);
            return;
        }
        if (Mathf.Abs(backgroundTile.transform.position.y - elevation.transform.position.y) > 0.5f)
        {
            Vector3 pos = backgroundTile.transform.position;
            pos.y = elevation.transform.position.y;
            backgroundTile.transform.position = pos;
            colliderObject.transform.SetParent(elevation, true);
        }
    }

    public Vector2Int GetDirection()
    {
        switch (type)
        {
            case TileType.WaterDiagonal:
                return new Vector2Int(diagonalRotation, Util.SubstractRotation(diagonalRotation, 3));
                break;
            case TileType.PathCurve:
                return new Vector2Int(diagonalRotation, Util.SubstractRotation(diagonalRotation, 3));
                break;
            case TileType.CliffDiagonal:
                break;
            case TileType.SeaDiagonal:
            case TileType.SandDiagonal:
                return new Vector2Int(Util.SubstractRotation(diagonalRotation, 1), Util.SubstractRotation(diagonalRotation, 2));
                break;
        }
        return new Vector2Int(-1, -1);
    }
    public void HardErase()
    {
        diagonalRotation = 255;
        curvedTileVariation = 255;
        GameObject.Destroy(backgroundTile);
        backgroundTile = null;
        for(int i=0;i<4;i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
                prefabType[i] = TilePrefabType.Null;
            }
        }
    }
    public void SoftErase()
    {
        GameObject.Destroy(backgroundTile);
        backgroundTile = null;
        for (int i = 0; i < 4; i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
            }
        }
    }
    public void RemoveQuarters()
    {
        for (int i = 0; i < 4; i++)
        {
            if (quarters[i] != null)
            {
                GameObject.Destroy(quarters[i]);
                quarters[i] = null;
                prefabType[i] = TilePrefabType.Null;
            }
        }
        //diagonalRotation = -1;
    }
    public void RemoveQuarters(int exception1, int exception2 = -1, int exception3 = -1)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != exception1 && i != exception2 && i != exception3)
            {
                if (quarters[i] != null)
                {
                    GameObject.Destroy(quarters[i]);
                    quarters[i] = null;
                    prefabType[i] = TilePrefabType.Null;
                }
            }
        }
    }
    public void RemoveQuarter(int index)
    {
        if (quarters[index] != null)
        {
            GameObject.Destroy(quarters[index]);
            quarters[index] = null;
            prefabType[index] = TilePrefabType.Null;
        }
    }

    public void RemoveCliffs()
    {
        for (int i = 0; i < 4; i++)
        {
            if (cliffSides[i] != null)
            {
                GameObject.Destroy(cliffSides[i]);
                cliffSides[i] = null;
                cliffSidesType[i] = 255;
            }
        }

    }
    public void RemoveCliff()
    {
        for (int i = 0; i < 4; i++)
        {
            if (cliffSides[i] != null)
            {
                GameObject.Destroy(cliffSides[i]);
                cliffSides[i] = null;
            }
        }
    }

    public void RemoveCliff(int cliff)
    {
        if (cliffSides[cliff] != null)
        {
            GameObject.Destroy(cliffSides[cliff]);
            cliffSides[cliff] = null;
            cliffSidesType[cliff] = 255;
        }
    }

    public void SetPosition(Vector3 position)
    {
        colliderObject.transform.localPosition = position;
        if (backgroundTile)
        {
            backgroundTile.transform.localPosition = Vector3.zero;
            backgroundTile.transform.SetParent(colliderObject.transform, false);
        }
    }
    public void SetPosition()
    {
        Vector3 pos = colliderObject.transform.localPosition;
        pos.y = 0;
        colliderObject.transform.localPosition = pos;
        if (backgroundTile)
        {
            backgroundTile.transform.localPosition = Vector3.zero;
            backgroundTile.transform.SetParent(colliderObject.transform, false);
        }
    }

    public void IgnoreElevation(bool isIgnoring = true)
    {
        backgroundTile.transform.localPosition = Vector3.zero;
        
        Vector3 pos = colliderObject.transform.localPosition;
        pos.y = 0;
        colliderObject.transform.localPosition = pos;
    }
}