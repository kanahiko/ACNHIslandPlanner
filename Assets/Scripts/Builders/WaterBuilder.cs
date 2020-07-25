using UnityEngine;

public class WaterBuilder
{
    
    public static bool CheckWater(int column, int row, TileType previousTileType,ref ToolMode toolMode)
    {
        if (previousTileType == TileType.CliffDiagonal|| previousTileType == TileType.SandDiagonal || previousTileType == TileType.Sand || previousTileType == TileType.Sea)
        {
            return false;
        }

        if (MapHolder.tiles[column, row].elevation > 0 && !Util.CheckHalfSurroundedBySameElevation(column,row))
        {
            return false;
        }
                
        if (previousTileType == TileType.Water)
        {
            if (CheckCanCurveWater(column, row, MapHolder.tiles[column,row]))
            {
                if (toolMode == ToolMode.None)
                {
                    toolMode = ToolMode.Add;
                    MapHolder.tiles[column, row].type = TileType.WaterDiagonal;
                }
                else
                {
                    return Util.ToolModeChange(true, column, row, TileType.Water,ref toolMode);
                }
            }
            else
            {
                
                return Util.ToolModeChange(false, column, row, TileType.Land,ref toolMode);

            }
        }
        else
        {
            if (previousTileType== TileType.WaterDiagonal)
            {
                if (!Util.ToolModeChange(false, column, row, TileType.Land,ref toolMode))
                {
                    return Util.ToolModeChange(true, column, row, TileType.Water,ref toolMode);
                }
            }
            else
            {
                return Util.ToolModeChange(true, column, row, TileType.Water,ref toolMode);
            }
        }

        return true;
    }
    
     public static bool CheckCanCurveWater(int column,int row, MapTile tile = null)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        int index = row * MapHolder.width + column;
        bool[] types = new bool[7];

        types[1] = CheckTile(column,row + 1, elevation);

        types[2] = CheckTile(column - 1,row, elevation);


        types[3] = CheckTile(column,row - 1, elevation);

        types[4] = CheckTile(column + 1,row, elevation);

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if (types[i] && types[i - 1] && !types[i + 1] && !types[i + 2])
            {
                if (tile != null)
                {
                    tile.diagonalRotation = Util.SubstractRotation(i - 1,2);
                }
                return true;
            }
        }

        return false;
    }
    
    static bool CheckTile(int column, int row, int elevation)
    {
        if (Util.CoordinateExists(column,row))
        {
            if (MapHolder.tiles[column, row].type == TileType.Water || MapHolder.tiles[column, row].type == TileType.WaterDiagonal)
                return elevation <= MapHolder.tiles[column, row].elevation;
        }

        return false;
    }
    
    public static void MakeWaterTile(int column, int row,  int elevationLevel)
    {
        //Debug.Log($"{column} {row} {MapHolder.grid[currentIndex]}");
        //creates background tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType != TilePrefabType.Water)
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(
                    MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Water], MapHolder.tiles[column,row].colliderObject.transform);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Water;
                MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Water]));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Water;
            MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
        }

        TileType[,] corners = Util.CreateMatrix(column, row);
        //corners = Util.RemoveNulls(corners);

        if (MapHolder.tiles[column,row].type == TileType.WaterDiagonal)
        {
            CreateDiagonalWater(corners, column, row);
        }
        else
        {
            MapHolder.tiles[column, row].diagonalRotation = 255;
            int[,] elevationCorners = Util.GetElevationCorners(column, row);
            for (int k = 0; k < 4; k++)
            {
                FindWaterCorner(corners, elevationCorners, k, column, row);
                corners = Util.RotateMatrix(corners);
                elevationCorners = Util.RotateMatrix(elevationCorners);
            }
        }

        if (elevationLevel >0)
        {
            CliffBuilder.CreateCliffSides(column, row, MapHolder.tiles[column,row]);
        }

    }

    static void FindWaterCorner(TileType[,] corners,int[,] elevationCorners, int rotation, int column, int row)
    {
        TilePrefabType type = TilePrefabType.Null;


        if (corners[0, 1] != TileType.Water && corners[0,1] != TileType.WaterDiagonal)
        {
            if (corners[1, 0] != TileType.Water && corners[1, 0] != TileType.WaterDiagonal)
            {
                if (elevationCorners[1,1] <= elevationCorners[0,1] && elevationCorners[1, 1] <= elevationCorners[1, 0]) 
                {
                    type = TilePrefabType.WaterBigCorner;
                }
                else
                {
                    if (elevationCorners[1, 1] == elevationCorners[1, 0])
                    {
                        type = TilePrefabType.WaterSideRotated;
                    }
                    else
                    {
                        type = TilePrefabType.WaterSide;
                    }
                }
            }
            else
            {
                if (!(elevationCorners[1, 1] > elevationCorners[0, 1]))
                {
                    type = TilePrefabType.WaterSide;
                }
            }
        }
        else
        {
            if (corners[1, 0] != TileType.Water && corners[1, 0] != TileType.WaterDiagonal )
            {
                //if (elevationCorners[1, 1] <= elevationCorners[0, 1] && elevationCorners[1, 1] <= elevationCorners[1, 0])
                if (!(elevationCorners[1, 1] > elevationCorners[1, 0]))
                {
                    type = TilePrefabType.WaterSideRotated;
                }
            }
            else
            {
                if (corners[0, 0] != TileType.Water && corners[0, 0] != TileType.WaterDiagonal && elevationCorners[0,0] >= elevationCorners[1,1])
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
        if (MapHolder.tiles[column, row].prefabType[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);

            GameObject prefab = MapHolder.mapPrefab.prefabDictionary[type];
            MapHolder.tiles[column, row].prefabType[rotation] = type;

            if (prefab != null)
            {
                GameObject tile = GameObject.Instantiate(prefab,MapHolder.tiles[column, row].backgroundTile.transform);
                tile.transform.localPosition = Util.offset[rotation];
                tile.transform.localRotation = rotate;
                MapHolder.tiles[column, row].quarters[rotation] = tile;
            }
        }
        else
        {
            if (MapHolder.tiles[column, row].quarters[rotation] != null)
            {
                MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = Util.offset[rotation];
                MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
            }
        }
    }

    static void CreateDiagonalWater(TileType[,] corners, int column, int row)
    {
        int rotation = MapHolder.tiles[column, row].diagonalRotation;
        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        Quaternion rotate = Quaternion.Euler(0, rotation * 90, 0);
        

        if (corners[Util.oppositeCorner[rotation].x, Util.oppositeCorner[rotation].y] == TileType.Land)
        {
            if (MapHolder.tiles[column, row].prefabType[rotation] != TilePrefabType.WaterDiagonalQuarter)
            {
                MapHolder.tiles[column, row].RemoveQuarter(rotation);
                GameObject corner = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonalQuarter], MapHolder.tiles[column, row].backgroundTile.transform);

                corner.transform.localPosition = Util.offset[rotation];
                corner.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = corner;
                MapHolder.tiles[column, row].prefabType[rotation] = TilePrefabType.WaterDiagonalQuarter;
            }
            else
            {
                MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = Util.offset[rotation];
                MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
            }
        }
        else
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);
        }

        MapHolder.tiles[column,row].RemoveQuarters(rotation, oppositeRotation);
        
        if (MapHolder.tiles[column, row].prefabType[oppositeRotation] != TilePrefabType.WaterDiagonal)
        {
            MapHolder.tiles[column, row].RemoveQuarter(oppositeRotation);
            GameObject diagonal = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonal], MapHolder.tiles[column, row].backgroundTile.transform);
            diagonal.transform.localPosition = Util.halfOffset;
            diagonal.transform.localRotation = rotate;
            MapHolder.tiles[column, row].quarters[oppositeRotation] = diagonal;
            MapHolder.tiles[column, row].prefabType[oppositeRotation] = TilePrefabType.WaterDiagonal;
        }
        else
        {
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localPosition = Util.halfOffset;
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localRotation = rotate;
        }
    }

    public static void RebuildWaterCorner(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Water], tile.colliderObject.transform);

        for (int i = 0; i < 4; i++) 
        {
            Quaternion rotate = Quaternion.Euler(0, 90 * i, 0);
            if (tile.prefabType[i] == TilePrefabType.WaterSideRotated)
            {
                rotate *= Quaternion.Euler(0, -90, 0);
            }

            GameObject prefab = MapHolder.mapPrefab.prefabDictionary[tile.prefabType[i]];

            if (prefab != null)
            {
                GameObject quarter = GameObject.Instantiate(prefab, tile.backgroundTile.transform);
                quarter.transform.localPosition = Util.offset[i];
                quarter.transform.localRotation = rotate;
                tile.quarters[i] = quarter;
            }
        }
    }

    public static void RebuildDiagonalWater(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Water], tile.colliderObject.transform);

        int rotation = tile.diagonalRotation;
        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        Quaternion rotate = Quaternion.Euler(0, rotation * 90, 0);


        if (tile.prefabType[rotation] == TilePrefabType.WaterDiagonalQuarter)
        {
            GameObject corner = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonalQuarter], tile.backgroundTile.transform);

            corner.transform.localPosition = Util.offset[rotation];
            corner.transform.localRotation = rotate;

            tile.quarters[rotation] = corner;
        }


        GameObject diagonal = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonal], tile.backgroundTile.transform);
        diagonal.transform.localPosition = Util.halfOffset;
        diagonal.transform.localRotation = rotate;
        tile.quarters[oppositeRotation] = diagonal;
        tile.prefabType[oppositeRotation] = TilePrefabType.WaterDiagonal;
    }
}
