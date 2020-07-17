using UnityEngine;

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
    public int[] cliffSidesType;

    public TilePrefabType backgroundType;

    public TilePrefabType[] prefabType;

    public int diagonalPathRotation = -1;
    public int diagonaWaterRotation = -1;
    public int diagonaCliffRotation = -1;

    private BoxCollider collider;
    //public bool isDirty;

    public int variation;

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
        if (Mathf.Abs(backgroundTile.transform.position.y - elevation.transform.position.y) > 0.5f)
        {
            Vector3 pos = backgroundTile.transform.position;
            pos.y = elevation.transform.position.y;
            backgroundTile.transform.position = pos;
            colliderObject.transform.SetParent(elevation, true);
        }
    }

    public Vector2Int GetDirectionOfPath()
    {
        if (diagonalPathRotation != -1)
        {
            return new Vector2Int(diagonalPathRotation, Util.SubstractRotation(diagonalPathRotation, 3));
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int GetDirectionOfWater()
    {
        if (diagonaWaterRotation != -1)
        {
            return new Vector2Int(diagonaWaterRotation, Util.SubstractRotation(diagonaWaterRotation, 3));
        }
        return new Vector2Int(-1, -1);
    }
    public void HardErase()
    {
        diagonalPathRotation = -1;
        diagonaWaterRotation = -1;
        diagonaCliffRotation = -1;
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
        diagonalPathRotation = -1;
        diagonaWaterRotation = -1;
        diagonaCliffRotation = -1;
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