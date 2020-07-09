using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainBuilder : MonoBehaviour
{
    public int width = 92;
    public int height = 73;

    public List<Transform> elevationLevels;

    public Vector3 offsetTerrain;
    public Texture2D terrain;
    
    public List<TileObject> lookUpTile;
    public List<ColorTile> lookUpColor;

    static Dictionary<Color32, TileType> lookUpTileType;
    static Dictionary<TilePrefabType, GameObject> lookUpTilePrefab;

    public List<GameObject> specialCurvedPath;
    static List<GameObject> specialCurvedPathPrefabs;

    static Vector3 halfOffset = new Vector3(0.5f, 0, 0.5f);

    static Vector3[] offset = new Vector3[]
    {
        new Vector3(0.25f,0,0.75f),
        new Vector3(0.75f,0,0.75f),
        new Vector3(0.75f,0,0.25f),
        new Vector3(0.25f,0,0.25f)

    };
    // Start is called before the first frame update
    void Start()
    {
        MapHolder.elevationLevels = elevationLevels;
        //MapHolder.parent = transform;
        ConvertToDictionary();
        //MapHolder.width = terrain.width;
        //MapHolder.height = terrain.height;
        MapHolder.width = width;
        MapHolder.height = height;

        MapHolder.grid = new TileType[MapHolder.width * MapHolder.height];
        MapHolder.tiles = new MapTile[MapHolder.width, MapHolder.height];
        //MapHolder.offset = this.offsetTerrain;

        /*for (int j = 0; j < terrain.height; j++)
        {
            for (int i = 0; i < terrain.width; i++)
            {            
                Color32 pixel = terrain.GetPixel(i, j);
                MapHolder.grid[j * terrain.width + i] = lookUpTileType[pixel];
            
            }
        }*/
        CreateEmptyLand(MapHolder.width, MapHolder.height);
        //CreateLand(MapHolder.grid, MapHolder.width, MapHolder.height);
        MapHolder.offset = offsetTerrain;
        transform.position = offsetTerrain;
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

        specialCurvedPathPrefabs = new List<GameObject>();
        for (int i = 0; i < specialCurvedPath.Count; i++)
        {
            specialCurvedPathPrefabs.Add(specialCurvedPath[i]);
        }
    }
    void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                CreateLandTile(column ,row);
            }
        }
    }

    public static void ChangeTile(TileType type, int column, int row)
    {
        int index = row * MapHolder.width + column;
        TileType previousTileType = MapHolder.grid[index];

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
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                }
                else
                {
                    if (previousTileType== TileType.WaterDiagonal)
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Water;
                    }
                }
                CreateTile(column, row, MapHolder.grid[index], index);
                break;
            case TileType.Path:
                if (previousTileType == TileType.Path)
                {
                    if (CheckCanCurve(type, TileType.PathCurve, index, MapHolder.tiles[column,row]))
                    {
                        MapHolder.grid[index] = TileType.PathCurve;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                }
                else
                {
                    if (previousTileType == TileType.PathCurve)
                    {
                        MapHolder.grid[index] = TileType.Land;
                    }
                    else
                    {
                        MapHolder.grid[index] = TileType.Path;
                    }
                }
                CreateTile(column, row, MapHolder.grid[index], index);
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

    static void CreateTile(int column, int row, TileType type, int index)
    {
        RecalculateDiagonals(column, row, type);

        switch (type)
        {
            case TileType.Null:
                break;
            case TileType.Land:
                CreateLandTile(column, row);
                break;
            case TileType.Water:
            case TileType.WaterDiagonal:
                MakeWaterTile(column, row, index, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                CreatePath(column, row, index, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Cliff:
            case TileType.CliffDiagonal:
                break;
        }
    }

    static bool CheckCanCurve(TileType type, TileType secondaryType,int index, MapTile tile = null)
    {
        TileType[] types = new TileType[7];
        if (index - MapHolder.width >= 0)
        {
            types[1] = MapHolder.grid[index - MapHolder.width];
        }
        if (index + 1 < MapHolder.grid.Length)
        {
            types[2] = MapHolder.grid[index + 1];
        }
        if (index + MapHolder.width < MapHolder.grid.Length)
        {
            types[3] = MapHolder.grid[index + MapHolder.width];
        }
        if (index - 1 >= 0)
        {
            types[4] = MapHolder.grid[index - 1];
        }
        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if ((types[i] == type || types[i] == secondaryType) && (types[i - 1] == type || types[i - 1] == secondaryType) 
                && types[i + 1] != type && types[i + 1] != secondaryType && types[i + 2] != type && types[i + 2] != secondaryType)
            {
                if (tile != null) 
                {
                    Debug.Log($"{i} {Util.SubstractRotation(i, 1)} {Util.SubstractRotation(i, 2)} {Util.SubstractRotation(i, 3)}");
                    tile.diagonalPathRotation =  Util.SubstractRotation(i,3);
                }
                return true;
            }
        }

        return false;
    }

    static void RecalculateDiagonals(int column,int row, TileType type)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if ( column + i >= 0 && column + i < MapHolder.width &&
                    row + j >= 0 && row + j < MapHolder.height)
                {
                    if (!(i!=0 && j!=0) && 
                        type.CanInfluence(MapHolder.grid[(row+j) * MapHolder.width + column + i], 4, MapHolder.tiles[column + i, row + j].GetDirectionOfPath()))
                    {
                        SwitchTileType(column +i, row +j, (row + j) * MapHolder.width + column + i);
                    }
                    RedoTile(column + i, row + j, type);
                }
            }
        }
    }

    private static void RedoTile(int column, int row, TileType type)
    {
        int index = row * MapHolder.width + column;
        switch (MapHolder.grid[index])
        {
            case TileType.WaterDiagonal:
            case TileType.Water:
                MakeWaterTile(column, row, index, MapHolder.tiles[column,row].elevation);
                break;
            case TileType.Path:
            case TileType.PathCurve:
                CreatePath(column, row, index, MapHolder.tiles[column, row].elevation);
                break;
            case TileType.Cliff:
            case TileType.CliffDiagonal:
                //MapHolder.grid[index] = TileType.Cliff;
                break;
        }
    }

    static void SwitchTileType(int column, int row, int index)
    {
        switch (MapHolder.grid[index])
        {
            case TileType.WaterDiagonal:
                MapHolder.grid[index] = TileType.Water;
                break;
            case TileType.PathCurve:
                MapHolder.grid[index] = TileType.Path;
                break;
            case TileType.CliffDiagonal:
                MapHolder.grid[index] = TileType.Cliff;
                break;
        }

    }

    /*void CreateLand(TileType[] grid, int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                int currentIndex = row * width + column;
                switch (grid[currentIndex])
                {
                    case TileType.Null:
                        break;
                    case TileType.Land:
                            CreateLandTile(column, row);
                        break;
                    case TileType.Water:
                    case TileType.WaterDiagonal:
                        MakeWaterTile(column, row,  currentIndex);
                        break;
                    case TileType.Path:
                    case TileType.PathCurve:
                        CreatePath(column, row, currentIndex);
                        break;
                }
            }
        }
    }*/

    static void CreateLandTile( int column ,int row)
    {
        //creates tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].EraseQuarters();
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Land], MapHolder.elevationLevels[0]);
                MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Land],  MapHolder.elevationLevels[0]));
            MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
        }

        MapHolder.tiles[column, row].elevation = 0;
        MapHolder.grid[row * MapHolder.width + column] = TileType.Land;
    }

    static void CreatePath(int column, int row, int currentIndex, int elevationLevel)
    {
        Debug.Log($"-------");
        Debug.Log($"{column} {row} {MapHolder.tiles[column, row] != null}");
        //creates background tile and adds its reference to MapHolder
        if (MapHolder.tiles[column,row] != null)
        {
            Debug.Log($"{column} {row} {MapHolder.tiles[column, row].backgroundType}");
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].SoftErase();
            }
            else
            {
                MapHolder.tiles[column, row].HardErase(); 
                MapHolder.tiles[column, row].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Land],MapHolder.elevationLevels[elevationLevel]);
                MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Land], MapHolder.elevationLevels[elevationLevel]));
            MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
        }

        TileType[,] corners = Util.CreateMatrix(MapHolder.grid, MapHolder.width, MapHolder.height, currentIndex, row, column);
        corners = Util.RemoveNulls(corners);

        if (MapHolder.grid[currentIndex] == TileType.PathCurve)
        {
            CreateCurvedPath(corners, column, row);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindCornerPath(corners, k, column, row);
                corners = Util.RotateMatrix(corners);
            }
        }
    }
    static void FindCornerPath(TileType[,] corners, int rotation, int column, int row)
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

        Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);

        GameObject prefab = lookUpTilePrefab[type];
        if (type == TilePrefabType.PathSideRotated)
        {
            rotate *= Quaternion.Euler(0, -90, 0);
        }

        if (MapHolder.tiles[column, row].type[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);
               MapHolder.tiles[column, row].type[rotation] = type;
            //creates corner and adds its reference to MapHolder
            if (prefab != null)
            {
                GameObject tile = Instantiate(prefab, MapHolder.tiles[column, row].backgroundTile.transform);
                tile.transform.localPosition = offset[rotation];
                tile.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = tile;
            }
        }
        else
        {
            MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = offset[rotation];
            MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
        }
    }

    static void CreateCurvedPath(TileType[,] corners, int column, int row)
    {
        int rotation = MapHolder.tiles[column, row].diagonalPathRotation;

        int oppositeRotation = Util.SubstractRotation(rotation, 2);

        int curvedTile = 0; //1  = only down| 2 = only right | 3 = both sides

        if (corners[1, 2] != TileType.Path && corners[1, 2] != TileType.PathCurve)
        {
            curvedTile += 1;
        }
        if (corners[2, 1] != TileType.Path && corners[2, 1] != TileType.PathCurve)
        {
            curvedTile += 2;
        }

        //opposite tile cant be created if both sides are not path or curved path
        if (curvedTile > 0)
        {
            GameObject oppositePrefab = null;
            bool createTile = false;
            var oppositePrefabType = (corners[2, 2] != TileType.Path && corners[2, 2] != TileType.PathCurve) ? TilePrefabType.PathSmallCorner : TilePrefabType.PathFull;

            if (MapHolder.tiles[column, row].type[oppositeRotation] != oppositePrefabType)
            {
                oppositePrefab = lookUpTilePrefab[oppositePrefabType];
                MapHolder.tiles[column, row].type[oppositeRotation] = oppositePrefabType;
                //creates opposite corner and adds its reference to MapHolder
                GameObject oppositeTile = Instantiate(oppositePrefab, MapHolder.tiles[column, row].backgroundTile.transform);

                oppositeTile.transform.localPosition = offset[oppositeRotation];
                oppositeTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);
                MapHolder.tiles[column, row].quarters[oppositeRotation] = oppositeTile;
            }
            else
            {
                MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localPosition = offset[oppositeRotation];
                MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);

            }
        }
        else
        {
            MapHolder.tiles[column, row].RemoveQuarter(oppositeRotation);
        }

        if (MapHolder.tiles[column, row].type[rotation] != TilePrefabType.PathCurved)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);
            //creates curved corner and adds its reference to MapHolder
            GameObject tile = Instantiate(specialCurvedPathPrefabs[curvedTile], MapHolder.tiles[column, row].backgroundTile.transform);
            tile.transform.localPosition = halfOffset;
            tile.transform.localRotation = Quaternion.Euler(0, rotation * 90, 0); 

            MapHolder.tiles[column, row].quarters[rotation] = tile;
            MapHolder.tiles[column, row].type[rotation] = TilePrefabType.PathCurved;
            //MapHolder.tiles[column, row].diagonalPathRotation = rotation;
        }
        else
        {
            MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = halfOffset;
            MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = Quaternion.Euler(0, rotation * 90, 0);
        }

        MapHolder.tiles[column, row].EraseQuarters(rotation, oppositeRotation);
    }
       
    static void MakeWaterTile(int column, int row,  int currentIndex, int elevationLevel)
    {
        //creates background tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Water)
            {
                MapHolder.tiles[column, row].SoftErase();
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(lookUpTilePrefab[TilePrefabType.Water], MapHolder.elevationLevels[elevationLevel]);
                MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Water;

            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(lookUpTilePrefab[TilePrefabType.Water], MapHolder.elevationLevels[elevationLevel]));
            MapHolder.tiles[column, row].backgroundTile.transform.localPosition = new Vector3(column, 0, -row);
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Water;
        }

        TileType[,] corners = Util.CreateMatrix(MapHolder.grid, MapHolder.width, MapHolder.height, currentIndex, row, column);
        corners = Util.RemoveNulls(corners);

        if (MapHolder.grid[currentIndex] == TileType.WaterDiagonal)
        {
            CreateDiagonalWater(corners, column, row);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindWaterCorner(corners, k, column, row);
                corners = Util.RotateMatrix(corners);
            }
        }
    }

    static void FindWaterCorner(TileType[,] corners,int rotation, int column, int row)
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

        Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);
        if (type == TilePrefabType.WaterSideRotated)
        {
            rotate *= Quaternion.Euler(0, -90, 0);
        }
        if (MapHolder.tiles[column, row].type[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);

            GameObject prefab = lookUpTilePrefab[type];
            MapHolder.tiles[column, row].type[rotation] = type;

            if (prefab != null)
            {
                GameObject tile = Instantiate(prefab,MapHolder.tiles[column, row].backgroundTile.transform);
                tile.transform.localPosition = offset[rotation];
                tile.transform.localRotation = rotate;
                MapHolder.tiles[column, row].quarters[rotation] = tile;
            }
        }
        else
        {
            MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = offset[rotation];
            MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
        }
    }

    static void CreateDiagonalWater(TileType[,] corners, int column, int row)
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

        if (corners[0, 0] == TileType.Land)
        {
            if (MapHolder.tiles[column, row].type[rotation] != TilePrefabType.WaterDiagonalQuarter)
            {
                MapHolder.tiles[column, row].RemoveQuarter(rotation);
                GameObject corner = Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonalQuarter], MapHolder.tiles[column, row].backgroundTile.transform);

                corner.transform.localPosition = offset[rotation];
                corner.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = corner;
                MapHolder.tiles[column, row].type[rotation] = TilePrefabType.WaterDiagonalQuarter;
            }
            else
            {
                MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = offset[rotation];
                MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
            }
        }

        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        if (MapHolder.tiles[column, row].type[oppositeRotation] != TilePrefabType.WaterDiagonal)
        {
            MapHolder.tiles[column, row].RemoveQuarter(oppositeRotation);
            GameObject diagonal = Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonal], MapHolder.tiles[column, row].backgroundTile.transform);
            diagonal.transform.localPosition = halfOffset;
            diagonal.transform.localRotation = rotate;
            MapHolder.tiles[column, row].quarters[oppositeRotation] = diagonal;
            MapHolder.tiles[column, row].type[oppositeRotation] = TilePrefabType.WaterDiagonal;
        }
        else
        {
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localPosition = offset[rotation];
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localRotation = rotate;
        }
    }

}
