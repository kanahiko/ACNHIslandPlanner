using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainBuilder : MonoBehaviour
{
    public Vector3 offsetTerrain;
    public Texture2D terrain;
    
    public List<TileObject> lookUpTile;
    public List<ColorTile> lookUpColor;

    static Dictionary<Color32, TileType> lookUpTileType;
    static Dictionary<TilePrefabType, GameObject> lookUpTilePrefab;

    static Vector3 halfOffset = new Vector3(0.5f, 0.5f, 0.5f);

    static Vector3[] offset = new Vector3[]
    {
        new Vector3(-0.25f,0,+0.25f),
        new Vector3(0.25f,0,0.25f),
        new Vector3(0.25f,0,-0.25f),
        new Vector3(-0.25f,0,-0.25f)

    };
    // Start is called before the first frame update
    void Start()
    {
        MapHolder.parent = transform;
        ConvertToDictionary();
        MapHolder.grid = new TileType[MapHolder.width * MapHolder.height];
        MapHolder.tiles = new MapTile[MapHolder.width, MapHolder.height];

        //MapHolder.offset = this.offsetTerrain;

       /* for (int j = 0; j < terrain.height; j++)
        {
            for (int i = 0; i < terrain.width; i++)
            {            
                Color32 pixel = terrain.GetPixel(i, j);
                MapHolder.grid[j * terrain.width + i] = lookUpTileType[pixel];
            
            }
        }*/
        CreateEmptyLand(MapHolder.width, MapHolder.height);
        //CreateLand(MapHolder.grid, MapHolder.width, MapHolder.height);

        transform.position = offsetTerrain;
    }
    void CreateEmptyLand(int width, int height)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CreateLandTile(i, j);
            }
        }
    }

    public static void ChangeTile(TileType type, int x, int y)
    {
        int index = y * MapHolder.width + x;
        TileType previousTileType = MapHolder.grid[index];
        RecalculateDiagonals(x, y, type);

        switch (type)
        {
            case TileType.Land:
                MapHolder.grid[index] = TileType.Land;
                break;
            case TileType.Water:
                if (previousTileType == TileType.Water)
                {
                    if (CheckCanCurve(type, TileType.WaterDiagonal, index))
                    {
                        MapHolder.grid[index] = TileType.WaterDiagonal;
                        MakeWaterTile(y, x, index);
                    }
                }
                else
                {
                    if (previousTileType== TileType.WaterDiagonal)
                    {
                        MapHolder.grid[index] = TileType.Land;
                        CreateLandTile(y, x);
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Water;
                        MakeWaterTile(y, x, index);
                    }
                }
                break;
            case TileType.Path:
                break;
            case TileType.Cliff:
                if (previousTileType == TileType.Cliff)
                {
                    if (CheckCanCurve(type, TileType.CliffDiagonal, index))
                    {
                        MapHolder.grid[index] = TileType.CliffDiagonal;
                    }
                }
                else
                {
                    MapHolder.grid[index] = TileType.Cliff;
                }
                break;
        }
    }

    static bool CheckCanCurve(TileType type, TileType secondaryType,int index)
    {
        TileType[] types = new TileType[7];
        types[1] = MapHolder.grid[index - 1];
        types[2] = MapHolder.grid[index + 1];
        types[3] = MapHolder.grid[index - MapHolder.width];
        types[4] = MapHolder.grid[index + MapHolder.width];
        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if ((types[i] == type || types[i] == secondaryType) && (types[i - 1] == type || types[i - 1] == secondaryType) 
                && types[i + 1] != type && types[i + 1] != secondaryType && types[i + 2] != type && types[i + 2] != secondaryType)
            {
                return true;
            }
        }

        return false;
    }

    static void RecalculateDiagonals(int x,int y, TileType type)
    {
        if (type.CanInfluence(MapHolder.grid[y * MapHolder.width + x - 1], 4,MapHolder.tiles[y,x-1]. GetDirectionOfPath()))
        {
            SwitchTileType(x,y,y * MapHolder.width + x - 1);
        }
        if (type.CanInfluence(MapHolder.grid[y * MapHolder.width + x + 1], 2,MapHolder.tiles[y, x+1].GetDirectionOfPath()))
        {
            SwitchTileType(x, y, y * MapHolder.width + x + 1);

        }
        if (type.CanInfluence(MapHolder.grid[(y-1) * MapHolder.width + x],1, MapHolder.tiles[y-1, x].GetDirectionOfPath()))
        {
            SwitchTileType(x, y, (y - 1) * MapHolder.width + x );

        }
        if (type.CanInfluence(MapHolder.grid[(y+1) * MapHolder.width + x - 1], 3,MapHolder.tiles[y+1, x].GetDirectionOfPath()))
        {
            SwitchTileType(x, y, (y + 1) * MapHolder.width + x);

        }
    }

    static void SwitchTileType(int x, int y ,int index)
    {
        switch (MapHolder.grid[index])
        {
            case TileType.WaterDiagonal:
                MapHolder.grid[index] = TileType.Water;
                MakeWaterTile(x, y, index);
                break;
            case TileType.PathCurve:
                MapHolder.grid[index] = TileType.Path;
                CreatePath(x, y, index);
                break;
            case TileType.CliffDiagonal:
                MapHolder.grid[index] = TileType.Cliff;
                break;
        }

    }

    void ConvertToDictionary()
    {
        lookUpTileType = new Dictionary<Color32, TileType>();
        for (int i = 0; i < lookUpColor.Count; i++)
        {
            lookUpTileType.Add(lookUpColor[i].color, lookUpColor[i].type);
        }

        lookUpTilePrefab = new Dictionary<TilePrefabType, GameObject>();
        for (int i = 0; i < lookUpTile.Count; i++)
        {
            lookUpTilePrefab.Add(lookUpTile[i].type, lookUpTile[i].prefab);
        }
    }

    void CreateLand(TileType[] grid, int width, int height)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int currentIndex = i * width + j;
                switch (grid[currentIndex])
                {
                    case TileType.Null:
                        break;
                    case TileType.Land:
                            CreateLandTile(i, j);
                        break;
                    case TileType.Water:
                    case TileType.WaterDiagonal:
                        MakeWaterTile(i, j, currentIndex);
                        break;
                    case TileType.Path:
                    case TileType.PathCurve:
                        CreatePath(i, j,currentIndex);
                        break;
                }
            }
        }
    }

    static void CreateLandTile(int x, int y)
    {
        //creates tile and adds its reference to MapHolder
        if (MapHolder.tiles[y, x] != null)
        {
            if (MapHolder.tiles[y,x].backgroundType != TilePrefabType.Land)
            {
                MapHolder.tiles[y, x].HardErase();
                MapHolder.tiles[y, x].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Land], MapHolder.parent);
                MapHolder.tiles[y, x].backgroundTile.transform.localPosition = new Vector3(y, 0, -x);
            }
            else
            {
                MapHolder.tiles[y, x].EraseQuarters();
            }
        }
        else
        {
            MapHolder.tiles[y, x] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Land],  MapHolder.parent));
            MapHolder.tiles[y, x].backgroundTile.transform.localPosition = new Vector3(y, 0, -x);
        }
    }

    static void CreatePath(int i, int j, int currentIndex)
    {
        //creates background tile and adds its reference to MapHolder
        if (MapHolder.tiles[j,i] != null)
        {
            if (MapHolder.tiles[j, i].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[j, i].SoftErase();
            }
            else
            {
                MapHolder.tiles[j, i].HardErase(); 
                MapHolder.tiles[j, i].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Land],MapHolder.parent);
                MapHolder.tiles[j, i].backgroundTile.transform.localPosition = new Vector3(j, 0, -i);
            }
        }
        else
        {
            MapHolder.tiles[j, i] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Land],MapHolder.parent));
            MapHolder.tiles[j, i].backgroundTile.transform.localPosition = new Vector3(j, 0, -i);
        }

        TileType[,] corners = Util.CreateMatrix(MapHolder.grid, MapHolder.width, MapHolder.height, currentIndex, i, j);
        corners = Util.RemoveNulls(corners);

        if (MapHolder.grid[currentIndex] == TileType.PathCurve)
        {
            CreateCurvedPath(corners, i, j);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindCornerPath(corners, k, i, j);
                corners = Util.RotateMatrix(corners);
            }
        }
    }
    static void FindCornerPath(TileType[,] corners, int rotation, int x, int y)
    {
        TilePrefabType type = TilePrefabType.PathFull;
        if (corners[0, 1] == TileType.Land)
        {
            if (corners[1, 0] == TileType.Land)
            {
                type = TilePrefabType.PathCorner;
            }
            else
            {
                type = TilePrefabType.PathSide;
            }
        }
        else
        {
            if (corners[1, 0] == TileType.Land)
            {
                type = TilePrefabType.PathSideRotated;
            }
            else
            {
                if (corners[0, 0] == TileType.Land)
                {
                    type = TilePrefabType.PathSmallCorner;
                }
            }
        }

        if (MapHolder.tiles[y, x].type[rotation] != type)
        {
            MapHolder.tiles[y, x].type[rotation] = type;
            Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);
            Vector3 pos = new Vector3(y, 0, -x) + halfOffset;

            GameObject prefab = lookUpTilePrefab[type];
            if (type == TilePrefabType.PathSideRotated)
            {
                rotate *= Quaternion.Euler(0, -90, 0);
            }

            //creates corner and adds its reference to MapHolder
            if (prefab != null)
            {
                GameObject tile = Instantiate(prefab, MapHolder.tiles[y, x].backgroundTile.transform);
                MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos + offset[rotation];
                MapHolder.tiles[y, x].backgroundTile.transform.localRotation = rotate;
                MapHolder.tiles[y, x].quarters[rotation] = tile;
            }
        }
    }

    static void CreateCurvedPath(TileType[,] corners, int x, int y)
    {
        int rotation = 0;
        for (int i = 0; i < 4; i++)
        {
            rotation = i;
            if (corners[0, 1] != TileType.Path && corners[0, 1] != TileType.PathCurve && corners[1, 0] != TileType.Path && corners[1, 0] != TileType.PathCurve)
            {
                break;
            }
            corners = Util.RotateMatrix(corners);
        }

        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        GameObject oppositePrefab = null;
        bool createTile = false;
        Vector3 pos = new Vector3(y, 0, -x) + halfOffset;

        var oppositePrefabType = (corners[2, 2] != TileType.Path && corners[2, 2] != TileType.PathCurve) ? TilePrefabType.PathSmallCorner : TilePrefabType.PathFull;
        if (MapHolder.tiles[y, x].type[oppositeRotation] != oppositePrefabType)
        {
            oppositePrefab = lookUpTilePrefab[oppositePrefabType];
            MapHolder.tiles[y, x].type[oppositeRotation] = oppositePrefabType;
            createTile = true;
        }

        if (createTile)
        {
            //creates opposite corner and adds its reference to MapHolder
            GameObject oppositeTile = Instantiate(oppositePrefab,MapHolder.tiles[y, x].backgroundTile.transform);

            MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos + offset[oppositeRotation];
            MapHolder.tiles[y, x].backgroundTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);
            MapHolder.tiles[y, x].quarters[oppositeRotation] = oppositeTile;
        }

        if (MapHolder.tiles[y, x].type[rotation] != TilePrefabType.PathCurved)
        {
            //creates curved corner and adds its reference to MapHolder
            GameObject tile = Instantiate(lookUpTilePrefab[TilePrefabType.PathCurved], MapHolder.tiles[y, x].backgroundTile.transform);
            MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos;
            MapHolder.tiles[y, x].backgroundTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0); 
            MapHolder.tiles[y, x].quarters[rotation] = tile;
            MapHolder.tiles[y, x].type[rotation] = TilePrefabType.PathCurved;
        }

        MapHolder.tiles[y, x].EraseQuarters(rotation, oppositeRotation);
    }
       
    static void MakeWaterTile(int i, int j, int currentIndex)
    {
        //creates background tile and adds its reference to MapHolder
        GameObject tile = Instantiate(lookUpTilePrefab[TilePrefabType.Water], new Vector3(j, 0, -i), Quaternion.identity, MapHolder.parent);
        if (MapHolder.tiles[j, i] != null)
        {
            if (MapHolder.tiles[j, i].backgroundType == TilePrefabType.Water)
            {
                MapHolder.tiles[j, i].SoftErase();
            }
            else
            {
                MapHolder.tiles[j, i].HardErase();
                MapHolder.tiles[j, i].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Water],MapHolder.parent);

                MapHolder.tiles[j, i].backgroundTile.transform.localPosition = new Vector3(j, 0, -i);

            }
        }
        else
        {
            MapHolder.tiles[j, i] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Water], MapHolder.parent));
            MapHolder.tiles[j, i].backgroundTile.transform.localPosition = new Vector3(j, 0, -i);
        }

        TileType[,] corners = Util.CreateMatrix(MapHolder.grid, MapHolder.width, MapHolder.height, currentIndex, i, j);
        corners = Util.RemoveNulls(corners);

        if (MapHolder.grid[currentIndex] == TileType.WaterDiagonal)
        {
            CreateDiagonalWater(corners, i, j);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindWaterCorner(corners, k, i, j);
                corners = Util.RotateMatrix(corners);
            }
        }
    }

    static void FindWaterCorner(TileType[,] corners,int rotation, int x, int y)
    {
        TilePrefabType type = TilePrefabType.Null;
        if (corners[0, 1] != TileType.Water && corners[0,1] != TileType.WaterDiagonal)
        {
            if (corners[1, 0] != TileType.Water && corners[1, 0] != TileType.WaterDiagonal)
            {
                type = TilePrefabType.WaterBigCorner;
            }
            else
            {
                type = TilePrefabType.WaterSide;
            }
        }
        else
        {
            if (corners[1, 0] != TileType.Water && corners[1, 0] != TileType.WaterDiagonal)
            {
                type = TilePrefabType.WaterSideRotated;
            }
            else
            {
                if (corners[0, 0] != TileType.Water && corners[0, 0] != TileType.WaterDiagonal)
                {
                    type = TilePrefabType.WaterDiagonalQuarter;
                }
            }
        }
        if (MapHolder.tiles[y, x].type[rotation] != type)
        {
            Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);

            GameObject prefab = lookUpTilePrefab[type];
            if (type == TilePrefabType.WaterSideRotated)
            {
                rotate *= Quaternion.Euler(0, -90, 0);
            }
            MapHolder.tiles[y, x].type[rotation] = type;

            if (prefab != null)
            {
                Vector3 pos = new Vector3(y, 0, -x) + halfOffset;
                GameObject tile = Instantiate(prefab,MapHolder.tiles[y, x].backgroundTile.transform);
                MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos + offset[rotation];
                MapHolder.tiles[y, x].backgroundTile.transform.localRotation = rotate;
                MapHolder.tiles[y, x].quarters[rotation] = tile;
            }
        }
    }

    static void CreateDiagonalWater(TileType[,] corners, int x, int y)
    {
        int rotation = 0;
        for (int i = 0; i < 4; i++)
        {
            rotation = i;
            if (corners[0, 1] == TileType.Water && corners[1, 0] == TileType.Water)
            {
                break;
            }
            corners = Util.RotateMatrix(corners);

        }

        Quaternion rotate = Quaternion.Euler(0, rotation * 90, 0);

        Vector3 pos = new Vector3(y, 0, -x) + halfOffset;

        if (corners[0, 0] == TileType.Land)
        {
            if (MapHolder.tiles[y, x].type[rotation] != TilePrefabType.WaterDiagonalQuarter)
            {
                GameObject corner = Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonalQuarter], MapHolder.tiles[y, x].backgroundTile.transform);

                MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos + offset[rotation];
                MapHolder.tiles[y, x].backgroundTile.transform.localRotation = rotate;

                MapHolder.tiles[y, x].quarters[rotation] = corner;
                MapHolder.tiles[y, x].type[rotation] = TilePrefabType.WaterDiagonalQuarter;
            }
        }

        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        if (MapHolder.tiles[y, x].type[oppositeRotation] != TilePrefabType.WaterDiagonal)
        {
            GameObject diagonal = Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonal], pos , rotate, MapHolder.tiles[y, x].backgroundTile.transform);
            MapHolder.tiles[y, x].backgroundTile.transform.localPosition = pos;
            MapHolder.tiles[y, x].backgroundTile.transform.localRotation = rotate;
            MapHolder.tiles[y, x].quarters[oppositeRotation] = diagonal;

            MapHolder.tiles[y, x].type[oppositeRotation] = TilePrefabType.WaterDiagonal;
        }

        MapHolder.tiles[y, x].EraseQuarters(rotation, oppositeRotation);
    }

}
