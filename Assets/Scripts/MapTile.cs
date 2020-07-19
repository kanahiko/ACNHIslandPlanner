using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class MapTile
{
    [XmlElement("Type")]
    public TileType type;
    
    [XmlIgnore]
    public GameObject colliderObject;
    [XmlIgnore]
    public GameObject backgroundTile;
    
    [XmlIgnore]
    public GameObject[] quarters;
    [XmlIgnore]
    public GameObject[] cliffSides;
    /// <summary>
    /// false means regular side
    /// true = water side
    /// </summary>
    
    [XmlArray("CliffSidesType"), XmlArrayItem("side")]
    public int[] cliffSidesType;

    [XmlElement("BackgroundType")]
    public TilePrefabType backgroundType;

    [XmlArray("QuarterPrefabType"), XmlArrayItem("quarter")]
    public TilePrefabType[] prefabType;

    [XmlElement("DiagonalRotation")]
    public int diagonalRotation = -1;

    /*public int diagonalRotation
    {
        get { return _diagonalRotation; }
        set { Debug.Log($"{colliderObject.gameObject.name}   {value}");_diagonalRotation = value; }
    }*/
    
    
    [XmlIgnore]
    private BoxCollider collider;

    [XmlElement("Variation")]
    public int variation;

    [XmlElement("Elevation")]
    public int elevation;
    

    public MapTile()
    {
        colliderObject = GameObject.Instantiate(MapHolder.mapPrefab.colliderPrefab);
        collider = colliderObject.GetComponent<BoxCollider>();
        quarters = new GameObject[4];
        prefabType = new TilePrefabType[4];
        cliffSides = new GameObject[4];
        cliffSidesType = new int[4] { -1, -1, -1, -1 };
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
        cliffSidesType = new int[4] { -1, -1, -1, -1 };
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
        diagonalRotation = -1;
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
                cliffSidesType[i] = -1;
            }
        }

    }
    
    public void RemoveCliff(int cliff)
    {
        if (cliffSides[cliff] != null)
        {
            GameObject.Destroy(cliffSides[cliff]);
            cliffSides[cliff] = null;
            cliffSidesType[cliff] = -1;
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

    public void IgnoreElevation(bool isIgnoring = true)
    {
        backgroundTile.transform.localPosition = Vector3.zero;
        
        Vector3 pos = colliderObject.transform.localPosition;
        pos.y = 0;
        colliderObject.transform.localPosition = pos;
    }
}