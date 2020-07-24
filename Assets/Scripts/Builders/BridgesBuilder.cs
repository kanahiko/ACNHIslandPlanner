using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        if (tool == ToolType.BridgeMarkUp)
        {
            if (mode == ToolMode.Add)
            {
                AddBridges(column,row,variation,rotation);
            }
            else
            {
                RemoveBridges(column,row);
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
        
        if (bridgesPlaced < MapHolder.mapPrefab.maxCount[DecorationType.Bridge])
        {
            bool canBePlaced = false;
            HashSet<Vector2Int> changedTiles;
            HashSet<Vector2Int> pathTiles;
            if (rotation == 0 || rotation == 2)
            {
                canBePlaced = CheckCanPlaceBridge(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge], rotation,out changedTiles, out size);
            }
            else
            {
                canBePlaced = CheckCanPlaceBridgeDiagonal(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge], rotation,out changedTiles, out size);
                
            }

            if (!canBePlaced)
            {
                return;
            }

            DecorationTiles tile = GetFromBridgeLimbo(variation,size);
            
            tile.decorationBackground.parent = MapHolder.decorationsParent;
            Vector3 position = new Vector3(column + Util.bridgeRotationsOffset[rotation].x,
                Util.GetHeight(column, row),
                -row + Util.bridgeRotationsOffset[rotation].y);
            if ((rotation == 1 || rotation == 3) && MapHolder.tiles[column,row].type == TileType.WaterDiagonal)
            {
                position.x += Util.bridgeAdditionalRotationsOffset[rotation].x;
                position.z += Util.bridgeAdditionalRotationsOffset[rotation].y;
            }

            tile.decorationBackground.localRotation = Util.bridgeRotations[rotation];
            tile.decorationBackground.localPosition = position;
            tile.type = DecorationType.Bridge;
            tile.rotation = rotation;
            tile.size = size;
            tile.startingColumn = column;
            tile.startingRow = row;


            MiniMap.PutBridgePin(tile.startingColumn, tile.startingRow, tile.rotation, tile.size, true);

            MarkTiles(tile, changedTiles, null);
            bridgesPlaced += 1;
        }
    }

    public static DecorationTiles RebuildBridge(PreDecorationTile preTile)
    {
        DecorationTiles tile = GetFromBridgeLimbo(preTile.variation, preTile.size);

        tile.decorationBackground.parent = MapHolder.decorationsParent;

        Vector3 position = new Vector3(preTile.startingCoords.x + Util.bridgeRotationsOffset[preTile.rotation].x,
                Util.GetHeight(preTile.startingCoords.x, preTile.startingCoords.y),
                -preTile.startingCoords.y + Util.bridgeRotationsOffset[preTile.rotation].y);
        if ((preTile.rotation == 1 || preTile.rotation == 3) && MapHolder.tiles[preTile.startingCoords.x, preTile.startingCoords.y].type == TileType.WaterDiagonal)
        {
            position.x += Util.bridgeAdditionalRotationsOffset[preTile.rotation].x;
            position.z += Util.bridgeAdditionalRotationsOffset[preTile.rotation].y;
        }
        tile.decorationBackground.localRotation = Util.bridgeRotations[preTile.rotation];
        tile.decorationBackground.localPosition = position;

        tile.type = DecorationType.Bridge;
        tile.rotation = preTile.rotation;
        tile.size = preTile.size;
        tile.startingColumn = preTile.startingCoords.x;
        tile.startingRow = preTile.startingCoords.y;

        bridgesPlaced += 1;

        MiniMap.PutBridgePin(tile.startingColumn, tile.startingRow, tile.rotation, tile.size, true);

        return tile;
    }
    
    public static void RemoveBridges(int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row]  == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Bridge))
        {
            return;
        }
        int newColumn = MapHolder.decorationsTiles[column, row].startingColumn;
        int newRow = MapHolder.decorationsTiles[column, row].startingRow;
        int rotation = MapHolder.decorationsTiles[column, row].rotation;
        
        Vector3Int size = MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge];
        int sizeX = size.x;
        int sizeY = MapHolder.decorationsTiles[column, row].size; 
        if (sizeY >= 6)
        {
            sizeY = 4;
        }
        
        GoToBridgeLimbo(MapHolder.decorationsTiles[column, row]);
        HashSet<Vector2Int> changedTiles;
        CreateChangedTiles(newColumn, newRow, rotation, sizeX, sizeY, out changedTiles);
        MarkTiles(null, changedTiles,null);
        bridgesPlaced -= 1;


        MiniMap.PutBridgePin(newColumn, newRow, rotation, sizeY, false);
    }

    public static void RemoveBridgesBeforeLoad(int column, int row)
    {
        if (MapHolder.decorationsTiles[column, row] == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Bridge))
        {
            return;
        }
        int newColumn = MapHolder.decorationsTiles[column, row].startingColumn;
        int newRow = MapHolder.decorationsTiles[column, row].startingRow;
        int rotation = MapHolder.decorationsTiles[column, row].rotation;

        Vector3Int size = MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Bridge];
        int sizeX = size.x;
        int sizeY = MapHolder.decorationsTiles[column, row].size; 
        if (sizeY >= 6)
        {
            sizeY = 4;
        }

        GoToBridgeLimbo(MapHolder.decorationsTiles[column, row]);
        HashSet<Vector2Int> changedTiles;
        CreateChangedTiles(newColumn, newRow, rotation, sizeX, sizeY, out changedTiles); 
        
        foreach (var changedTile in changedTiles)
        {
            MapHolder.decorationsTiles[changedTile.x, changedTile.y] = null;
        }
        bridgesPlaced -= 1;

        MiniMap.PutBridgePin(newColumn, newRow, rotation, sizeY, false);
    }

    static void CreateChangedTiles(int column, int row, int rotation, int sizeX, int sizeY, out HashSet<Vector2Int> changedTiles)
    {
        changedTiles = new HashSet<Vector2Int>();

        if (rotation == 0 || rotation == 2)
        {
            int columnIndexEnd = rotation == 0 ? sizeX : (sizeY + 2);
            int rowIndexEnd = rotation == 0 ? (sizeY + 2) : sizeX;

            for (int i = 0; i < rowIndexEnd; i++)
            {
                for (int j = 0; j < columnIndexEnd; j++)
                {
                    changedTiles.Add(new Vector2Int(column + j, row - i));
                }
            }
        }
        else
        {
            int columnIndexEnd = rotation == 3 ? sizeX : (sizeY + 2);
            int rowIndexEnd = rotation == 3 ? (sizeY + 2) : sizeX;
            bool diagonalStart = MapHolder.tiles[column, row].type == TileType.WaterDiagonal;

            int rotationMult = rotation == 3 ? 1 : -1;

            for (int j = 0; j < sizeX; j++)
            {
                for (int i = 0; i < sizeY + 2; i++)
                {
                    int newColumn = column + j * rotationMult + (-1 * i * rotationMult);
                    int newRow = row - j + (-1 * i);
                    int newColumn2 = column + (diagonalStart ? 1 * rotationMult : 0) + j * rotationMult + (-1 * i * rotationMult);
                    int newRow2 = row - (diagonalStart ? 0 : 1) - j + (-1 * i);

                    changedTiles.Add(new Vector2Int(newColumn, newRow));
                    if (j + 1 < sizeX)
                    {
                        changedTiles.Add(new Vector2Int(newColumn2, newRow2));
                    }
                }
            }
        }
    }

    public static bool CheckCanPlaceBridge(int column, int row, Vector3Int size, int rotation, out HashSet<Vector2Int> changedTiles, out int sizeBridge)
    {
        //rotation 0 is vertical
        //rotation 1 is diagonal bottom left to top right
        //rotation 2 is sideways
        //rotation 3 is diagonal top left to bottom right
        changedTiles = new HashSet<Vector2Int>();
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
                    if (bridgeSize != -1 && 
                        (rotation == 0 && i - 1 > bridgeSize ||
                        rotation == 2 && j - 1 > bridgeSize))
                    {
                        continue;
                    }

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

                                changedTiles.Add(new Vector2Int(column + j, row - i));
                                if (rotation == 0 && j + 1 == columnIndexEnd ||
                                    rotation == 2 && i + 1 == rowIndexEnd)
                                {
                                    sizeBridge = bridgeSize;
                                    //changedTiles.Add(new Vector2Int(column + j, row - i));
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            //if there's water when bridge size is written then it's obviously cant function
                            if (MapHolder.tiles[column + j, row - i].backgroundType == TilePrefabType.Water)
                            {
                                if (rotation == 2 && bridgeSize != -1 && j - 1 > bridgeSize ||
                                    rotation == 0 && bridgeSize != -1)
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
                    changedTiles.Add(new Vector2Int(column + j, row - i));
                }
            }

            if (bridgeSize == -1)
            {
                return false;
            }
        }
        
        return false;
    }

    public static bool CheckCanPlaceBridgeDiagonal(int column, int row, Vector3Int size, int rotation, out HashSet<Vector2Int> changedTiles,out int sizeBridge)
    {
        changedTiles = new HashSet<Vector2Int>();
        sizeBridge = -1;
        int elevation = MapHolder.tiles[column, row].elevation;
        bool diagonalStart = diagonalStart = MapHolder.tiles[column, row].type == TileType.WaterDiagonal;
        int rotationMult = rotation == 3 ? 1 : -1;
        bool isThreeAndHalfSize = false;
        if (rotation == 1 || rotation == 3)
        {
            int columnIndexEnd = rotation == 0 ? size.x : (size.y + 2);
            int rowIndexEnd = rotation == 0 ? (size.y + 2) :size.x ; //to account for land //those are water sizes

            int bridgeSize = -1;

            if (Util.CoordinateExists(column - 3* rotationMult, row - 3))
            {
                if (MapHolder.tiles[column - 3 * rotationMult, row - 3].type == TileType.Water)
                {
                    if (!Util.CoordinateExists(column - 4 * rotationMult, row - 4))
                    {
                        return false;
                    }
                    if (MapHolder.tiles[column - 4 * rotationMult, row - 4].type != TileType.WaterDiagonal && !diagonalStart)
                    {
                        isThreeAndHalfSize = true;
                    }
                    bridgeSize = 4;
                }
                else
                {
                    if (MapHolder.tiles[column - 3 * rotationMult, row - 3].backgroundType == TilePrefabType.Land ||
                        MapHolder.tiles[column - 3 * rotationMult, row - 3].type == TileType.WaterDiagonal)
                    {
                        bridgeSize = 3;
                    }
                    else
                    {
                        return false;
                    }
                }
                sizeBridge = bridgeSize;
            }
            else
            {
                return false;
            }
            //Debug.Log(bridgeSize);

            for (int j = 0; j < size.x; j++)
            {
                for (int i = 0; i < bridgeSize + 1; i++)
                {
                    int newColumn = column + j* rotationMult + (-1 * i * rotationMult);
                    int newRow = row - j + (-1 * i);
                    int newColumn2 = column + (diagonalStart ? 1 * rotationMult : 0) + j* rotationMult + (-1 * i * rotationMult);
                    int newRow2 = row - (diagonalStart ? 0: 1) - j + (-1 * i);

                    if (!Util.CoordinateExists(newColumn, newRow) ||
                        !Util.CoordinateExists(newColumn, newRow2))
                    {
                        return false;
                    }
                    
                    //this is for main row
                    //checks first diagonal row
                    //it should be either land or diagonal
                    if (i == 0)
                    {
                        if (diagonalStart && (MapHolder.tiles[newColumn, newRow].backgroundType != TilePrefabType.Land &&
                                    MapHolder.tiles[newColumn, newRow].type != TileType.WaterDiagonal) ||
                            !diagonalStart && MapHolder.tiles[newColumn, newRow].backgroundType != TilePrefabType.Land)
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
                                if (!diagonalStart && (MapHolder.tiles[newColumn, newRow].backgroundType != TilePrefabType.Land && 
                                                       MapHolder.tiles[newColumn, newRow].type != TileType.WaterDiagonal) ||
                                    diagonalStart && MapHolder.tiles[newColumn, newRow].type != TileType.Land)
                                {
                                    return false;
                                } 
                            }   
                        }
                    }
                    changedTiles.Add(new Vector2Int(newColumn, newRow));

                    //this is for sub row
                    if (j + 1 < size.x)
                    {
                        if (i == 0)
                        {
                            if (!diagonalStart && MapHolder.tiles[newColumn2, newRow2].type != TileType.WaterDiagonal ||
                                diagonalStart && MapHolder.tiles[newColumn2, newRow2].backgroundType != TilePrefabType.Land)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if ( !isThreeAndHalfSize && i != bridgeSize ||
                               isThreeAndHalfSize && i < bridgeSize - 1)
                            {
                                if (MapHolder.tiles[newColumn2, newRow2].type != TileType.Water)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((diagonalStart || (isThreeAndHalfSize && i < bridgeSize)) && MapHolder.tiles[newColumn2, newRow2].type != TileType.WaterDiagonal ||
                                   !diagonalStart && (!isThreeAndHalfSize || i == bridgeSize) && MapHolder.tiles[newColumn2, newRow2].backgroundType != TilePrefabType.Land)
                                {
                                    return false;
                                }
                            }
                        }
                        changedTiles.Add(new Vector2Int(newColumn2, newRow2));
                    }
                }
            }
            if (bridgeSize != 3)
            {
                sizeBridge = isThreeAndHalfSize ? 6 : 7;
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
                    new List<DecorationTiles>(),new List<DecorationTiles>(),new List<DecorationTiles>(),new List<DecorationTiles>(),new List<DecorationTiles>()
                });
            }
        }

        if (bridgesLimbo[tile.variation][tile.size - 3].Count < maxInclines)
        {
            bridgesLimbo[tile.variation][tile.size - 3].Add(tile);
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
        if (bridgesLimbo == null || bridgesLimbo.Count < variation + 1 || bridgesLimbo[variation][size - 3].Count == 0)
        {
            DecorationTiles newTile = new DecorationTiles(DecorationType.Bridge);
            newTile.AddMainTile(GameObject.Instantiate(MapHolder.mapPrefab.bridgePrefabDictionary[variation].bridgePrefabs[size - 3], newTile.decorationBackground));
            newTile.variation = variation;
            return newTile;
        }
        DecorationTiles tile = bridgesLimbo[variation][size - 3][bridgesLimbo[variation][size - 3].Count - 1];
            
        bridgesLimbo[variation][size - 3].RemoveAt(bridgesLimbo[variation][size - 3].Count - 1);
        tile.ReturnFromLimbo();
        return tile;
    }

    static void AddInclines(int column, int row, byte variation, int rotation)
    {
        HashSet<Vector2Int> changedTiles;
        HashSet<Vector2Int> pathTiles;
        if (inclinesPlaced < MapHolder.mapPrefab.maxCount[DecorationType.Incline] &&
            CheckCanPlaceIncline(column, row, MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline], rotation, out changedTiles, out pathTiles))
        {
            DecorationTiles tile = GetFromInclineLimbo(variation);
            
            tile.decorationBackground.parent = MapHolder.decorationsParent;
            tile.decorationBackground.localPosition = new Vector3(column + Util.inclineRotationsOffset[rotation].x, 
                Util.GetHeight(column, row), 
                -row + Util.inclineRotationsOffset[rotation].y);
            tile.decorationBackground.localRotation = Quaternion.Euler(0,90*rotation,0);
            tile.type = DecorationType.Incline;
            tile.rotation = rotation;

            tile.startingColumn = column;
            tile.startingRow = row;  
            
            MarkTiles(tile, changedTiles, pathTiles);
            inclinesPlaced += 1;
        }
    }
    public static DecorationTiles RebuildIncline(PreDecorationTile preTile)
    {
        DecorationTiles tile = GetFromInclineLimbo(preTile.variation);

        tile.decorationBackground.parent = MapHolder.decorationsParent;
        tile.decorationBackground.localPosition = new Vector3(preTile.startingCoords.x + Util.inclineRotationsOffset[preTile.rotation].x,
            Util.GetHeight(preTile.startingCoords.x, preTile.startingCoords.y),
            -preTile.startingCoords.y + Util.inclineRotationsOffset[preTile.rotation].y);
        tile.decorationBackground.localRotation = Quaternion.Euler(0, 90 * preTile.rotation, 0);
        tile.type = DecorationType.Incline;
        tile.rotation = preTile.rotation;

        tile.startingColumn = preTile.startingCoords.x;
        tile.startingRow = preTile.startingCoords.y;


        inclinesPlaced += 1;

        return tile;
    }

    public static void RemoveInclines(int column, int row)
    {
        Debug.Log(column+" "+ row);
        if (MapHolder.decorationsTiles[column, row]  == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Incline))
        {
            return;
        }
        int newColumn = MapHolder.decorationsTiles[column, row].startingColumn;
        int newRow = MapHolder.decorationsTiles[column, row].startingRow;
        int rotation = MapHolder.decorationsTiles[column, row].rotation;
        
        Vector3Int size = MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline];
        int sizeX = (rotation == 0 || rotation == 2) ? size.x : size.y;
        int sizeY = (rotation == 0 || rotation == 2) ? size.y : size.x;
        
        GoToInclineLimbo(MapHolder.decorationsTiles[column, row]);
        HashSet<Vector2Int> changedTiles;
        CreateChangedInclineTiles(newColumn, newRow, rotation, sizeX, sizeY, out changedTiles);
        MarkTiles(null, changedTiles, null);
        inclinesPlaced -= 1;
    }
    public static void RemoveInclinesBeforeLoad(int column, int row)
    {
        Debug.Log(column + " " + row);
        if (MapHolder.decorationsTiles[column, row] == null ||
            (MapHolder.decorationsTiles[column, row].type != DecorationType.Incline))
        {
            return;
        }
        int newColumn = MapHolder.decorationsTiles[column, row].startingColumn;
        int newRow = MapHolder.decorationsTiles[column, row].startingRow;
        int rotation = MapHolder.decorationsTiles[column, row].rotation;

        Vector3Int size = MapHolder.mapPrefab.decorationsSizeDictionary[DecorationType.Incline];
        int sizeX = (rotation == 0 || rotation == 2) ? size.x : size.y;
        int sizeY = (rotation == 0 || rotation == 2) ? size.y : size.x;

        GoToInclineLimbo(MapHolder.decorationsTiles[column, row]);
        HashSet<Vector2Int> changedTiles;
        CreateChangedInclineTiles(newColumn, newRow, rotation, sizeX, sizeY, out changedTiles); 
        foreach (var changedTile in changedTiles)
        {
            MapHolder.decorationsTiles[changedTile.x, changedTile.y] = null;
        }
        inclinesPlaced -= 1;
    }

    static void CreateChangedInclineTiles(int column, int row, int rotation, int sizeX, int sizeY, out HashSet<Vector2Int> changedTiles)
    {
        changedTiles = new HashSet<Vector2Int>();
        
        int columnIndexMult = (rotation == 3) ? -1 : 1;
        int rowIndexMult = (rotation == 2) ? -1 : 1;

        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                changedTiles.Add(new Vector2Int(column + j * columnIndexMult, row - i * rowIndexMult));
            }
        }
    }

    static void MarkTiles(DecorationTiles tile,HashSet<Vector2Int> changedTiles, HashSet<Vector2Int> pathTiles)
    {           

        foreach(var changedTile in changedTiles)
        {
            MapHolder.decorationsTiles[changedTile.x, changedTile.y] = tile;

            if (tile!= null && tile.type == DecorationType.Incline && 
                (MapHolder.tiles[changedTile.x, changedTile.y].type == TileType.Path ||
                MapHolder.tiles[changedTile.x, changedTile.y].type == TileType.PathCurve))
            {
                MapHolder.tiles[changedTile.x, changedTile.y].type = TileType.Land;
                LandBuilder.CreateLandTile(changedTile.x, changedTile.y, MapHolder.tiles[changedTile.x, changedTile.y].elevation);
            }
        }

        if (pathTiles != null && pathTiles.Count > 0)
        {
            PathBuilder.RedoTiles(pathTiles); 
        }
        MiniMap.ChangeMiniMap(changedTiles);
    }

    static bool CheckCanPlaceIncline(int column, int row, Vector3Int size, int rotation, out HashSet<Vector2Int> changedTiles, out HashSet<Vector2Int> pathTiles)
    {
        changedTiles = new HashSet<Vector2Int>();
        pathTiles = new HashSet<Vector2Int>();

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
                    int newColumn = column + j * columnIndexMult + (j == 0 ? -1 : 1) * columnIndexMult;

                    if (i == 0 || i + 1 == rowIndexEnd)
                    {
                        int newRow = row - i * rowIndexMult + (i == 0 ? 1 : -1) * rowIndexMult;
                        if (MapHolder.decorationsTiles[newColumn, newRow] != null && 
                            MapHolder.decorationsTiles[newColumn, newRow].type == DecorationType.Incline)
                        {
                            return false;
                        }
                        if (MapHolder.tiles[newColumn, newRow].type == TileType.Path ||
                            MapHolder.tiles[newColumn, newRow].type == TileType.PathCurve)
                        {
                            pathTiles.Add(new Vector2Int(newColumn, newRow));
                        }
                    }

                    if (MapHolder.decorationsTiles[newColumn, row - i * rowIndexMult] != null && 
                        MapHolder.decorationsTiles[newColumn, row - i * rowIndexMult].type == DecorationType.Incline)
                    {
                        return false;
                    }

                    if (MapHolder.tiles[newColumn,row - i * rowIndexMult].type == TileType.Path ||
                        MapHolder.tiles[newColumn, row - i * rowIndexMult].type == TileType.PathCurve)
                    {
                        pathTiles.Add(new Vector2Int(newColumn, row - i * rowIndexMult));
                    }
                }
                
                if (i == 0 || i + 1 == rowIndexEnd)
                {
                    int newRow = row - i * rowIndexMult + (i == 0 ? 1 : -1) * rowIndexMult;
                    if (MapHolder.decorationsTiles[column + j * columnIndexMult, newRow] != null && 
                        MapHolder.decorationsTiles[column + j * columnIndexMult, newRow].type == DecorationType.Incline)
                    {
                        return false;
                    }
                    if (MapHolder.tiles[column + j * columnIndexMult, newRow].type == TileType.Path ||
                        MapHolder.tiles[column + j * columnIndexMult, newRow].type == TileType.PathCurve)
                    {
                        pathTiles.Add(new Vector2Int(column + j * columnIndexMult, newRow));
                    }
                    //changedTiles.Add(new Vector2Int(column + j * columnIndexMult, newRow));
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
                changedTiles.Add(new Vector2Int(column + j * columnIndexMult, row - i * rowIndexMult));
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
