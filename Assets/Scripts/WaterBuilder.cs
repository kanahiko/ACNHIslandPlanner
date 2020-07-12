using UnityEngine;

public class WaterBuilder
{
    public static void MakeWaterTile(int column, int row,  int elevationLevel)
    {
        int currentIndex = Util.GetIndex(column, row);
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
        MapHolder.tiles[column, row].diagonalPathRotation = -1;

        TileType[,] corners = Util.CreateMatrix(MapHolder.grid,column, row);
        //corners = Util.RemoveNulls(corners);

        if (MapHolder.grid[currentIndex] == TileType.WaterDiagonal)
        {
            CreateDiagonalWater(corners, column, row);
        }
        else
        {
            MapHolder.tiles[column, row].diagonaWaterRotation = -1;
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
            CliffBuilder.CreateCliffSides(column, row);
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
        if (MapHolder.tiles[column, row].type[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);

            GameObject prefab = MapHolder.mapPrefab.prefabDictionary[type];
            MapHolder.tiles[column, row].type[rotation] = type;

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

        MapHolder.tiles[column, row].diagonaWaterRotation = rotation;
        //Debug.Log($"{MapHolder.tiles[column, row].GetDirectionOfWater()}");
        Quaternion rotate = Quaternion.Euler(0, rotation * 90, 0);
        

        if (corners[0, 0] == TileType.Land)
        {
            if (MapHolder.tiles[column, row].type[rotation] != TilePrefabType.WaterDiagonalQuarter)
            {
                MapHolder.tiles[column, row].RemoveQuarter(rotation);
                GameObject corner = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonalQuarter], MapHolder.tiles[column, row].backgroundTile.transform);

                corner.transform.localPosition = Util.offset[rotation];
                corner.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = corner;
                MapHolder.tiles[column, row].type[rotation] = TilePrefabType.WaterDiagonalQuarter;
            }
            else
            {
                MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = Util.offset[rotation];
                MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
            }
        }

        int oppositeRotation = Util.SubstractRotation(rotation, 2);
        MapHolder.tiles[column,row].RemoveQuarters(rotation, oppositeRotation);
        
        if (MapHolder.tiles[column, row].type[oppositeRotation] != TilePrefabType.WaterDiagonal)
        {
            MapHolder.tiles[column, row].RemoveQuarter(oppositeRotation);
            GameObject diagonal = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.WaterDiagonal], MapHolder.tiles[column, row].backgroundTile.transform);
            diagonal.transform.localPosition = Util.halfOffset;
            diagonal.transform.localRotation = rotate;
            MapHolder.tiles[column, row].quarters[oppositeRotation] = diagonal;
            MapHolder.tiles[column, row].type[oppositeRotation] = TilePrefabType.WaterDiagonal;
        }
        else
        {
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localPosition = Util.halfOffset;
            MapHolder.tiles[column, row].quarters[oppositeRotation].transform.localRotation = rotate;
        }
    }
}
