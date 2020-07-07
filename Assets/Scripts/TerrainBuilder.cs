using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainBuilder : MonoBehaviour
{
    public Texture2D terrain;
    
    public List<TileObject> lookUpTile;
    public List<ColorTile> lookUpColor;

    Dictionary<Color32, TileType> lookUpTileType;
    Dictionary<TilePrefabType, GameObject> lookUpTilePrefab;

    Vector3[] offset = new Vector3[]
    {
        new Vector3(-0.25f,0,+0.25f),
        new Vector3(0.25f,0,0.25f),
        new Vector3(0.25f,0,-0.25f),
        new Vector3(-0.25f,0,-0.25f)

    };
    // Start is called before the first frame update
    void Start()
    {
        ConvertToDictionary();
        MapHolder.grid = new TileType[terrain.width * terrain.height];
        MapHolder.tiles = new MapTile[terrain.width,terrain.height];

        for (int j = 0; j < terrain.height; j++)
        {
            for (int i = 0; i < terrain.width; i++)
            {            
                Color32 pixel = terrain.GetPixel(i, j);
                MapHolder.grid[j * terrain.width + i] = lookUpTileType[pixel];
            
            }
        }

        CreateLand(MapHolder.grid, terrain.width, terrain.height);
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

                        GameObject tile = Instantiate(lookUpTilePrefab[TilePrefabType.Land], new Vector3(j, 0, -i), Quaternion.identity);
                        MapHolder.tiles[j, i] = new MapTile(tile);
                        break;
                    case TileType.Water:
                    case TileType.WaterDiagonal:
                        MakeWaterTile(i, j, grid, width, height, currentIndex);
                        break;
                    case TileType.Path:
                    case TileType.PathCurve:
                        CreatePath(i, j, grid, width, height, currentIndex);
                        break;
                }
            }
        }
    }

    void CreatePath(int i, int j, TileType[] grid, int width, int height, int currentIndex)
    {
        GameObject tile =  Instantiate(lookUpTilePrefab[TilePrefabType.Land], new Vector3(j, 0, -i), Quaternion.identity);
        MapHolder.tiles[j, i] = new MapTile(tile);

        TileType[,] corners = CreateMatrix(grid, width, height, currentIndex, i, j);
        corners = RemoveNulls(corners);

        if (grid[currentIndex] == TileType.PathCurve)
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

    void MakeWaterTile(int i, int j, TileType[]  grid, int width, int height, int currentIndex)
    {
        GameObject tile =Instantiate(lookUpTilePrefab[TilePrefabType.Water], new Vector3(j, 0, -i), Quaternion.identity);
        MapHolder.tiles[j, i] = new MapTile(tile);

        TileType[,] corners = CreateMatrix(grid, width, height, currentIndex, i, j);
        corners = RemoveNulls(corners);

        if (grid[currentIndex] == TileType.WaterDiagonal)
        {
            CreateDiagonal(corners, i, j);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindCorner(corners, k, i, j);
                corners = Util.RotateMatrix(corners);
            }
        }
    }
    void FindCornerPath(TileType[,] corners, int rotation, int x, int y)
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

        if (prefab != null)
        {
            GameObject tile = Instantiate(prefab, new Vector3(y, 0, -x) + offset[rotation] + new Vector3(0.5f, 0, 0.5f), rotate , MapHolder.tiles[y, x].backgroundTile.transform);
            MapHolder.tiles[y, x].quarters[rotation] = tile;
        }
    }

    void CreateCurvedPath(TileType[,] corners, int x, int y)
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
        GameObject oppositePrefab = lookUpTilePrefab[TilePrefabType.PathFull];

        if (corners[2, 2] != TileType.Path && corners[2, 2] != TileType.PathCurve)
        {
            oppositePrefab = lookUpTilePrefab[TilePrefabType.PathSmallCorner];
        }
        GameObject oppositeTile =  Instantiate(oppositePrefab, new Vector3(y, 0, -x) + offset[oppositeRotation] + new Vector3(0.5f, 0, 0.5f), 
            Quaternion.Euler(0, oppositeRotation * 90, 0),
                MapHolder.tiles[y, x].backgroundTile.transform);
        MapHolder.tiles[y, x].quarters[rotation] = oppositeTile;

        GameObject tile = Instantiate(lookUpTilePrefab[TilePrefabType.PathCurved],
            new Vector3(y, 0, -x) + new Vector3(0.5f, 0, 0.5f), Quaternion.Euler(0, rotation * 90, 0),
                MapHolder.tiles[y, x].backgroundTile.transform);
        MapHolder.tiles[y, x].quarters[rotation] = tile;
    }


    void FindCorner(TileType[,] corners,int rotation, int x, int y)
    {
        TilePrefabType type = TilePrefabType.Nothing;
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

        GameObject prefab = lookUpTilePrefab[type];
        if (type == TilePrefabType.WaterSideRotated)
        {
            rotate *= Quaternion.Euler(0, -90, 0);
        }

        if (prefab != null)
        {
            GameObject  tile = Instantiate(prefab, new Vector3(y, 0, -x) + offset[rotation] + new Vector3(0.5f, 0, 0.5f), rotate, 
                MapHolder.tiles[y, x].backgroundTile.transform);
            MapHolder.tiles[y, x].quarters[rotation] = tile;
        }
    }

    void CreateDiagonal(TileType[,] corners, int x, int y)
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
           GameObject corner =  Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonalQuarter],
                new Vector3(y, 0, -x) + offset[rotation] + new Vector3(0.5f, 0, 0.5f), rotate, MapHolder.tiles[y, x].backgroundTile.transform);
            MapHolder.tiles[y, x].quarters[rotation] = corner;
        }

        GameObject diagonal = Instantiate(lookUpTilePrefab[TilePrefabType.WaterDiagonal],
            new Vector3(y, 0, -x) + new Vector3(0.5f, 0, 0.5f), rotate, MapHolder.tiles[y, x].backgroundTile.transform);
        MapHolder.tiles[y, x].quarters[Util.SubstractRotation(rotation,2)] = diagonal;
    }
    
    void ChangeTile(int x, int y)
    {

    }

    TileType[,] CreateMatrix(TileType[] grid, int width, int height, int currentIndex, int i, int j)
    {
        TileType[,] corners = new TileType[3, 3];
        corners[1, 1] = TileType.Water;

        if (i != 0)
        {
            if (j != 0)
            {
                corners[0, 0] = grid[currentIndex - width - 1];
            }

            corners[0, 1] = grid[currentIndex - width];

            if (j != width - 1)
            {
                corners[0, 2] = grid[currentIndex - width + 1];
            }
        }

        if (j != 0)
        {
            corners[1, 0] = grid[currentIndex - 1];
        }

        if (i != height - 1)
        {
            if (j != 0)
            {
                corners[2, 0] = grid[currentIndex + width - 1];
            }

            corners[2, 1] = grid[currentIndex + width];

            if (j != width - 1)
            {
                corners[2, 2] = grid[currentIndex + width + 1];
            }
        }

        if (j != width - 1)
        {
            corners[1, 2] = grid[currentIndex + 1];
        }

        return corners;
    }
    
    TileType[,] RemoveNulls(TileType[,] corners)
    {
        for (int i = 0; i < 3; i++)
        {
            if (corners[0,i] == TileType.Null)
            {
                corners[0, i] = corners[1, i];
            }
            if (corners[2, i] == TileType.Null)
            {
                corners[2, i] = corners[1, i];
            }

            if (corners[i, 0] == TileType.Null)
            {
                corners[i, 0] = corners[i, 1];
            }
            if (corners[i, 2] == TileType.Null)
            {
                corners[i, 2] = corners[i, 1];
            }
        }

        if (corners[0, 0] == TileType.Null)
        {
            corners[0, 0] = corners[0, 1];
        }
        if (corners[0, 2] == TileType.Null)
        {
            corners[0, 2] = corners[0, 1];
        }

        if (corners[2, 0] == TileType.Null)
        {
            corners[2, 0] = corners[2, 1];
        }
        if (corners[2, 2] == TileType.Null)
        {
            corners[2, 2] = corners[2, 1];
        }

        return corners;
    }
}
