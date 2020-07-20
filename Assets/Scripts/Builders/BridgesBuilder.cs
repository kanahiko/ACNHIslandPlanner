using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgesBuilder : MonoBehaviour
{
    public static List<List<List<DecorationTiles>>> bridgesLimbo;

    public static List<List<DecorationTiles>> inclinesLimbo;
    private static int maxInclines = 8;

    private static int bridgesPlaced = 0;
    private static int inclinesPlaced = 0;


    public static void ChangeTile(ToolType tool, ToolMode mode,int column, int row, byte variation, int rotation)
    {
        Debug.Log(rotation);
        if (tool == ToolType.BridgeMarkUp)
        {
            if (mode == ToolMode.Add)
            {
                AddBridges(column,row,variation,rotation);
            }
            else
            {
                //RemoveInclines(column,row);
            }
        }
        else
        {
            if (mode == ToolMode.Add)
            {
                AddInclines(column,row,variation,rotation);
            }
            else
            {
                RemoveInclines(column,row);
            }
        }
    }
    
    public static void AddBridges(int column, int row, byte variation, int rotation)
    {
        int size = -1;
        if (CheckCanPlaceBridgeDiagonal(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge], rotation, out size))
        {
            Debug.Log("yay");
        }
        
        if (bridgesPlaced < MapHolder.mapPrefab.maxCount[DecorationType.Bridge] &&
            CheckCanPlaceBridge(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge], rotation, out size))
        {
            DecorationTiles tile = GetFromBridgeLimbo(variation,size);
            
            tile.decorationBackground.parent = MapHolder.decorationsParent;
            tile.decorationBackground.localPosition = new Vector3(column + Util.inclineRotationsOffset[rotation].x, 
                Util.GetHeight(column, row), 
                -row + Util.inclineRotationsOffset[rotation].y);
            tile.decorationBackground.localRotation = Quaternion.Euler(0,90*rotation,0);
            tile.type = DecorationType.Bridge;
            tile.rotation = rotation;
            tile.size = size;
            
            MarkTiles(column,row,MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge],rotation,tile);
            bridgesPlaced += 1;
        }
    }

    public static bool CheckCanPlaceBridge(int column, int row, Vector3Int size, int rotation, out int sizeBridge)
    {
        //rotation 0 is vertical
        //rotation 1 is diagonal bottom left to top right
        //rotation 2 is sideways
        //rotation 3 is diagonal top left to bottom right
        
        sizeBridge = -1;
        int elevation = MapHolder.tiles[column, row].elevation;
        if (rotation == 0 || rotation == 2)
        {
            int columnIndexEnd = rotation == 0 ? size.x : (size.y + 2);
            int rowIndexEnd = rotation == 0 ? (size.y + 2) :size.x ; //to account for land //those are water sizes

            int bridgeSize = -1;
            
            for (int i = 0; i < rowIndexEnd; i++)
            {
                for (int j = 0; j < columnIndexEnd; j++)
                {
                    //check for undesirable blocks
                    if (!Util.CoordinateExists(column + j, row - i) || 
                        MapHolder.tiles[column+j,row-i].elevation != elevation ||
                        MapHolder.decorationsTiles[column+j, row - i] != null ||
                        MapHolder.treeInfluence[column+j, row - i] != 0)
                    {
                        return false;
                    }

                    //check if first row or column (depends on rotation) is land
                    if (rotation == 0 && i == 0 || rotation == 2 && j == 0)
                    {
                        if (MapHolder.tiles[column + j, row - i].backgroundType != TilePrefabType.Land)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //if found land block and there's no bridge size yet
                        //then add it and ask if it's minimum enough size
                        //if there is size ask if it's same size
                        if (MapHolder.tiles[column + j, row - i].backgroundType == TilePrefabType.Land)
                        {
                            if (bridgeSize == -1)
                            {
                                bridgeSize = (rotation == 0 ? i : j) - 1;
                                if (bridgeSize < 3)
                                {
                                    return false;
                                }
                            }
                            else
                            {

                                if (rotation == 0 && j + 1 == columnIndexEnd ||
                                    rotation == 2 && i + 1 == rowIndexEnd)
                                {
                                    sizeBridge = bridgeSize;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            //if there's water when bridge size is written then it's obviously cant function
                            if (MapHolder.tiles[column + j, row - i].backgroundType == TilePrefabType.Water)
                            {
                                if (bridgeSize != -1)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if (bridgeSize == -1)
            {
                return false;
            }
        }
        
        return false;
    }
    
    public static bool CheckCanPlaceBridgeDiagonal(int column, int row, Vector3Int size, int rotation, out int sizeBridge)
    {
        
        sizeBridge = -1;
        int elevation = MapHolder.tiles[column, row].elevation;
        if (rotation == 1 || rotation == 3)
        {
            int columnIndexEnd = rotation == 0 ? size.x : (size.y + 2);
            int rowIndexEnd = rotation == 0 ? (size.y + 2) :size.x ; //to account for land //those are water sizes

            int bridgeSize = -1;

            if (Util.CoordinateExists(column - 3, row - 3))
            {
                if (MapHolder.tiles[column - 3, row - 3].type == TileType.Water)
                {
                    bridgeSize = 4;
                }
                else
                {
                    if (MapHolder.tiles[column - 3, row - 3].type == TileType.Land ||
                        MapHolder.tiles[column - 3, row - 3].type == TileType.WaterDiagonal)
                    {
                        bridgeSize = 3;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            for (int j = 0; j < size.x; j++)
            {
                for (int i = 0; i < bridgeSize + 1; i++)
                {
                    int newColumn = column + j + (-1 * i);
                    int newRow = row - j + (-1 * i);
                    int newRow2 = row - 1 - j + (-1 * i);

                    if (!Util.CoordinateExists(newColumn, newRow) ||
                        !Util.CoordinateExists(newColumn, newRow2))
                    {
                        return false;
                    }
                    
                    if (i == 0)
                    {
                        if (MapHolder.tiles[newColumn, newRow].type != TileType.Land)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (i + 1 != bridgeSize + 1)
                        {
                            if (MapHolder.tiles[newColumn, newRow].type != TileType.Water)
                            {
                                return false;
                            }  
                        }
                        else
                        {
                            if (j == 0 || j + 1 == size.x)
                            {
                                if (MapHolder.tiles[newColumn, newRow].type != TileType.Land &&
                                    MapHolder.tiles[newColumn, newRow].type != TileType.WaterDiagonal)
                                {
                                    return false;
                                } 
                            }   
                        }
                    }
                    
                    if (j + 1 < size.x)
                    {
                        if (i == 0)
                        {
                            if (MapHolder.tiles[newColumn, newRow2].type != TileType.WaterDiagonal)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (i + 1 != bridgeSize + 1)
                            {
                                if (MapHolder.tiles[newColumn, newRow2].type != TileType.Water)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (MapHolder.tiles[newColumn, newRow2].type != TileType.Land)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        return false;
    }
    
    static void GoToBridgeLimbo(DecorationTiles tile)
    {
        if (bridgesLimbo == null)
        {
            bridgesLimbo = new List<List<List<DecorationTiles>>>();
        }

        if (bridgesLimbo.Count < tile.variation + 1)
        {
            while (bridgesLimbo.Count < tile.variation + 1)
            {
                bridgesLimbo.Add(new List<List<DecorationTiles>>
                {
                    new List<DecorationTiles>(),new List<DecorationTiles>(),new List<DecorationTiles>()
                });
            }
        }

        if (bridgesLimbo[tile.variation][tile.size].Count < maxInclines)
        {
            bridgesLimbo[tile.variation][tile.size].Add(tile);
            tile.GoToLimbo();
        }
        else
        {
            GameObject.Destroy(tile.mainTile);
            GameObject.Destroy(tile.decorationBackground.gameObject);
            tile.Dispose();
        }
    }
    
    static DecorationTiles GetFromBridgeLimbo(byte variation, int size)
    {
        if (bridgesLimbo == null || bridgesLimbo.Count < variation + 1 || bridgesLimbo[variation][size].Count == 0)
        {
            DecorationTiles newTile = new DecorationTiles(DecorationType.Bridge);
            newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.bridgePrefabDictionary[variation].bridgePrefabs[size - 3], newTile.decorationBackground));
            newTile.variation = variation;
            return newTile;
        }
        DecorationTiles tile = bridgesLimbo[variation][size][bridgesLimbo[variation][size - 3].Count - 1];
            
        bridgesLimbo[variation][size].RemoveAt(bridgesLimbo[variation][size].Count - 1);
        tile.ReturnFromLimbo();
        return tile;
    }

    static void AddInclines(int column, int row, byte variation, int rotation)
    {
        if (inclinesPlaced < MapHolder.mapPrefab.maxCount[DecorationType.Incline] &&
            CheckCanPlaceIncline(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline], rotation))
        {
            DecorationTiles tile = GetFromInclineLimbo(variation);
            
            tile.decorationBackground.parent = MapHolder.decorationsParent;
            tile.decorationBackground.localPosition = new Vector3(column + Util.inclineRotationsOffset[rotation].x, 
                Util.GetHeight(column, row), 
                -row + Util.inclineRotationsOffset[rotation].y);
            tile.decorationBackground.localRotation = Quaternion.Euler(0,90*rotation,0);
            tile.type = DecorationType.Incline;
            tile.rotation = rotation;
            
            MarkTiles(column,row,MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline],rotation,tile);
            inclinesPlaced += 1;
        }
    }
    
    static void RemoveInclines(int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row]  == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Incline))
        {
            return;
        }

        int rotation = MapHolder.decorationsTiles[column, row].rotation;
        
        GoToInclineLimbo(MapHolder.decorationsTiles[column, row]);
        MapHolder.decorationsTiles[column, row] = null;
        MarkTiles(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline], rotation,null);
        inclinesPlaced -= 1;
    }

    static void MarkTiles(int column, int row, Vector3Int size, int rotation, DecorationTiles tile)
    {
        HashSet<Vector2Int> pathTiles = new HashSet<Vector2Int>();

        HashSet<Vector2Int> changedTiles = new HashSet<Vector2Int>();
        
        int columnIndexEnd = (rotation == 0 || rotation == 2) ? size.x : size.y;
        int columnIndexMult = (rotation == 3) ? -1 : 1;
        
        int rowIndexEnd = (rotation == 0 || rotation == 2) ? size.y : size.x;
        int rowIndexMult = (rotation == 2) ? -1 : 1;

        int elevation = MapHolder.tiles[column, row].elevation;
        for (int i = 0; i < rowIndexEnd; i++)
        {
            for (int j = 0; j < columnIndexEnd; j++)
            {
                MapHolder.decorationsTiles[column + j * columnIndexMult, row - i * rowIndexMult] = tile;
                
                if (MapHolder.tiles[column+j * columnIndexMult,row-i * rowIndexMult].type == TileType.Path || 
                    MapHolder.tiles[column + j* columnIndexMult, row - i * rowIndexMult].type == TileType.PathCurve)
                {
                    //for adding path tiles outside of incline size
                    if (j == 0 || j == columnIndexEnd - 1)
                    {
                        int newColumn = column + j * columnIndexMult + (j == 0 ? -1: 1);
                        pathTiles.Add(new Vector2Int(newColumn, row - i* rowIndexMult - 1));
                        pathTiles.Add(new Vector2Int(newColumn, row - i* rowIndexMult));
                        pathTiles.Add(new Vector2Int(newColumn, row - i* rowIndexMult + 1));
                    }

                    if (i == 0 || i == rowIndexEnd - 1)
                    {
                        int newRow = row - i*rowIndexMult + (i == 0 ? 1 : -1);
                        pathTiles.Add(new Vector2Int(column + j * columnIndexMult, newRow));
                    }

                    MapHolder.tiles[column + j, row - i].type = TileType.Land;
                    LandBuilder.CreateLandTile(column + j, row - i, MapHolder.tiles[column + j, row - i].elevation);
                    changedTiles.Add(new Vector2Int(column + j, row - i));
                }
                
            }
        }
        
        
        PathBuilder.RedoTiles(pathTiles);
        MiniMap.ChangeMiniMap(changedTiles);
    }

    static bool CheckCanPlaceIncline(int column, int row, Vector3Int size, int rotation)
    {
        int columnIndexEnd = (rotation == 0 || rotation == 2) ? size.x : size.y;
        int columnIndexMult = (rotation == 3) ? -1 : 1;
        
        int rowIndexEnd = (rotation == 0 || rotation == 2) ? size.y : size.x;
        int rowIndexMult = (rotation == 2) ? -1 : 1;

        int elevation = MapHolder.tiles[column, row].elevation;
        for (int i = 0; i < rowIndexEnd; i++)
        {
            for (int j = 0; j < columnIndexEnd; j++)
            {
                if (!Util.CoordinateExists(column + j * columnIndexMult, row - i * rowIndexMult) || 
                    MapHolder.tiles[column + j * columnIndexMult, row - i * rowIndexMult].backgroundType != TilePrefabType.Land ||
                    MapHolder.decorationsTiles[column+j * columnIndexMult, row - i * rowIndexMult] != null ||
                    MapHolder.treeInfluence[column+j * columnIndexMult, row - i * rowIndexMult] != 0)
                {
                    return false;
                }

                
                if (j == 0 || j + 1 == columnIndexEnd)
                {
                    int newColumn = column + j*columnIndexMult + (j == 0 ? -1 : 1);
                    if (i == 0 || i + 1 == rowIndexEnd)
                    {
                        int newRow = row - i*rowIndexMult+ (i == 0 ? 1 : -1);
                        if (MapHolder.decorationsTiles[newColumn, newRow] != null && 
                            MapHolder.decorationsTiles[newColumn, newRow].type == DecorationType.Incline)
                        {
                            return false;
                        }
                    }

                    if (MapHolder.decorationsTiles[newColumn, row + i * rowIndexMult] != null && 
                        MapHolder.decorationsTiles[newColumn, row + i * rowIndexMult].type == DecorationType.Incline)
                    {
                        return false;
                    }
                }
                
                if (i == 0 || i + 1 == rowIndexEnd)
                {
                    int newRow = row - i*rowIndexMult+ (i == 0 ? 1 : -1);
                    if (MapHolder.decorationsTiles[column + j * columnIndexMult, newRow] != null && 
                        MapHolder.decorationsTiles[column + j * columnIndexMult, newRow].type == DecorationType.Incline)
                    {
                        return false;
                    }
                }

                if ((rotation == 0 || rotation == 2) && i+ 1 == rowIndexEnd ||
                    (rotation == 1 || rotation == 3) && j + 1 == columnIndexEnd)
                {
                    if (MapHolder.tiles[column + j * columnIndexMult, row - i * rowIndexMult].elevation != elevation + 1)
                    {
                        return false;
                    }
                }
                else
                {
                    if (MapHolder.tiles[column + j * columnIndexMult, row - i * rowIndexMult].elevation != elevation)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    static void GoToInclineLimbo(DecorationTiles tile)
    {
        if (inclinesLimbo == null)
        {
            inclinesLimbo = new List<List<DecorationTiles>>();
        }

        if (inclinesLimbo.Count < tile.variation + 1)
        {
            while (inclinesLimbo.Count < tile.variation + 1)
            {
                inclinesLimbo.Add(new List<DecorationTiles>());
            }
        }


        if (inclinesLimbo[tile.variation].Count < maxInclines)
        {
            inclinesLimbo[tile.variation].Add(tile);
            tile.GoToLimbo();
        }
        else
        {
            GameObject.Destroy(tile.mainTile);
            GameObject.Destroy(tile.decorationBackground.gameObject);
            tile.Dispose();
        }
    }

    static DecorationTiles GetFromInclineLimbo(byte variation)
    {
        if (inclinesLimbo == null || inclinesLimbo.Count < variation + 1 || inclinesLimbo[variation].Count == 0)
        {
            DecorationTiles newTile = new DecorationTiles(DecorationType.Incline);
            newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.inclinePrefabDictionary[variation], newTile.decorationBackground));
            newTile.variation = variation;
            return newTile;
        }
        DecorationTiles tile = inclinesLimbo[variation][inclinesLimbo[variation].Count - 1];
            
        inclinesLimbo[variation].RemoveAt(inclinesLimbo[variation].Count - 1);
        tile.ReturnFromLimbo();
        return tile;
    }
}
