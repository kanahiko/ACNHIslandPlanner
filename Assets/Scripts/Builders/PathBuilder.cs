using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PathBuilder
{
    public static bool CheckPath(int column, int row, TileType previousTileType,ref ToolMode toolMode, byte variation)
    {
        if (previousTileType == TileType.CliffDiagonal || previousTileType == TileType.Sand || previousTileType == TileType.Sea)
        {
            return false;
        }
        if (previousTileType == TileType.Path)
        {
            if (MapHolder.tiles[column,row].variation == variation &&
                CheckCanCurvePath(column, row, variation, MapHolder.tiles[column,row]))
            {
                if (toolMode == ToolMode.None)
                {
                    toolMode = ToolMode.Add;
                    MapHolder.tiles[column, row].type = TileType.PathCurve;
                    return true;
                }
                else
                {
                    return Util.ToolModeChange(true, column, row, TileType.Path, ref toolMode);
                }
            }
            else
            {
                return  Util.ToolModeChange(false, column, row, TileType.Land,ref toolMode);
            }
        }
        else
        {
            if (previousTileType == TileType.PathCurve)
            {
                return Util.ToolModeChange(false, column, row, TileType.Land,ref toolMode);
            }
            else
            {
                return Util.ToolModeChange(true, column, row, TileType.Path,ref toolMode);
            }
        }

        return false;
    }
    
    static bool CheckCanCurvePath(int column,int row, int variation, MapTile tile)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        //int index = row * MapHolder.width + column;
        bool[] types = new bool[7];

        if (row + 1 < MapHolder.width && elevation == MapHolder.tiles[column,row + 1].elevation)
        {
            types[1] =( MapHolder.tiles[column, row + 1].type == TileType.Path || MapHolder.tiles[column, row + 1].type == TileType.PathCurve) && 
                MapHolder.tiles[column, row + 1].variation == variation;
        }

        if (column - 1 >= 0 && elevation == MapHolder.tiles[column - 1,row].elevation)
        {
            types[2] = (MapHolder.tiles[column - 1, row].type == TileType.Path || MapHolder.tiles[column - 1, row].type == TileType.PathCurve) && 
                MapHolder.tiles[column - 1, row].variation == variation;
        }

        if (row - 1 >= 0 && elevation == MapHolder.tiles[column,row - 1].elevation )
        {
            types[3] = (MapHolder.tiles[column, row - 1].type == TileType.Path || MapHolder.tiles[column, row - 1].type == TileType.PathCurve) && 
                MapHolder.tiles[column, row - 1].variation == variation;
        }

        if (column + 1 < MapHolder.height && elevation == MapHolder.tiles[column + 1,row].elevation)
        {
            types[4] = (MapHolder.tiles[column + 1, row].type == TileType.Path || MapHolder.tiles[column + 1, row].type == TileType.PathCurve) && 
                MapHolder.tiles[column + 1, row].variation == variation;
        }

        types[5] = types[1];
        types[6] = types[2];
        types[0] = types[4];
        for (int i = 1; i < 5; i++)
        {
            if (types[i] && types[i - 1] && !types[i + 1] && !types[i + 2])
            {
                if (tile != null) 
                {
                    tile.diagonalRotation = (byte)(i - 1);
                }
                return true;
            }
        }
        
        return false;
    }
    
    public static void CreatePath(int column, int row, int elevationLevel, byte variation)
    {
        if (MapHolder.tiles[column,row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType != TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = GameObject.Instantiate(
                    MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land]);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
                MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], MapHolder.tiles[column,row].colliderObject.transform));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;
            MapHolder.tiles[column,row].SetPosition(new Vector3(column, 0, -row));
        }

        TileType[,] corners = Util.CreateMatrix(column, row, variation);

        if (MapHolder.tiles[column,row].type == TileType.PathCurve)
        {
            CreateCurvedPath(corners, column, row,variation);
        }
        else
        {
            for (int k = 0; k < 4; k++)
            {
                FindCornerPath(corners, k, column, row,variation);
                corners = Util.RotateMatrix(corners);
            }
            MapHolder.tiles[column, row].diagonalRotation = 255;
        }
        MapHolder.tiles[column, row].variation = variation;
        if (elevationLevel > 0)
        {
            CliffBuilder.CreateCliffSides(column, row, MapHolder.tiles[column, row]);
        }
    }

    public static void FindCornerPath(TileType[,] corners, int rotation, int column, int row, byte variation)
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

        GameObject prefab = MapHolder.mapPrefab.prefabDictionary[type];
        if (type == TilePrefabType.PathSideRotated)
        {
            rotate *= Quaternion.Euler(0, -90, 0);
        }

        if (MapHolder.tiles[column, row].prefabType[rotation] != type)
        {
            MapHolder.tiles[column, row].RemoveQuarter(rotation);
               MapHolder.tiles[column, row].prefabType[rotation] = type;
            //creates corner and adds its reference to MapHolder
            if (prefab != null)
            {
                GameObject tile = GameObject.Instantiate(prefab, MapHolder.tiles[column, row].backgroundTile.transform);
                tile.transform.localPosition = Util.offset[rotation];
                tile.transform.localRotation = rotate;

                MapHolder.tiles[column, row].quarters[rotation] = tile;
            }
        }
        else
        {
            MapHolder.tiles[column, row].quarters[rotation].transform.localPosition = Util.offset[rotation];
            MapHolder.tiles[column, row].quarters[rotation].transform.localRotation = rotate;
        }

        if (variation >= 0)
        {
            MapHolder.tiles[column, row].quarters[rotation].GetComponentInChildren<MeshRenderer>().material =
                MapHolder.mapPrefab.pathVariationMaterial[variation];
        }
    }

    public static void RebuildPathCorner(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], tile.colliderObject.transform);
        for (int i = 0; i < 4; i++)
        {
            GameObject prefab = MapHolder.mapPrefab.prefabDictionary[tile.prefabType[i]];

            Quaternion rotate = Quaternion.Euler(0, 90 * i, 0);
            if (tile.prefabType[i] == TilePrefabType.PathSideRotated)
            {
                rotate *= Quaternion.Euler(0, -90, 0);
            }
            if (prefab != null)
            {
                GameObject quarter = GameObject.Instantiate(prefab, tile.backgroundTile.transform);
                quarter.transform.localPosition = Util.offset[i];
                quarter.transform.localRotation = rotate;

                tile.quarters[i] = quarter; 
                if (tile.variation >= 0)
                {
                    quarter.GetComponentInChildren<MeshRenderer>().material = MapHolder.mapPrefab.pathVariationMaterial[tile.variation];
                }
            }
        }
    }

    public static void RebuildPathDiagonal(MapTile tile)
    {
        tile.SoftErase();
        tile.backgroundTile = GameObject.Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], tile.colliderObject.transform);

        int rotation = tile.diagonalRotation;
        int oppositeRotation = Util.SubstractRotation(rotation, 2);


        if (tile.prefabType[oppositeRotation] != TilePrefabType.Null)
        {
            var oppositePrefab = MapHolder.mapPrefab.prefabDictionary[tile.prefabType[oppositeRotation]];

            GameObject oppositeTile = GameObject.Instantiate(oppositePrefab, tile.backgroundTile.transform);
            oppositeTile.transform.localPosition = Util.offset[oppositeRotation];
            oppositeTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);
            tile.quarters[oppositeRotation] = oppositeTile;

            if (tile.variation >= 0)
            {
                oppositeTile.GetComponentInChildren<MeshRenderer>().material = MapHolder.mapPrefab.pathVariationMaterial[tile.variation];
            }
        }

        //creates curved corner and adds its reference to MapHolder
        GameObject quarter = GameObject.Instantiate(MapHolder.mapPrefab.specialCurvedPath[tile.curvedTileVariation], tile.backgroundTile.transform);
        quarter.transform.localPosition = Util.halfOffset;
        quarter.transform.localRotation = Quaternion.Euler(0, rotation * 90, 0);
        tile.quarters[rotation] = quarter;

        if (tile.variation >= 0)
        {
            quarter.GetComponentInChildren<MeshRenderer>().material = MapHolder.mapPrefab.pathVariationMaterial[tile.variation];
        }
    }

    public static void CreateCurvedPath(TileType[,] corners, int column, int row, byte variation)
    {        
        int rotation = MapHolder.tiles[column, row].diagonalRotation;

        int oppositeRotation = Util.SubstractRotation(rotation, 2);

        //rotates corners matrix for easier calculations
        for (int i = 0; i < rotation; i++)
        {
            corners = Util.RotateMatrix(corners);
        }

        //check which curved path needs to be in place
        byte curvedTile = 0; //1  = only down| 2 = only right | 3 = both sides dont extend anwhere

        if (corners[1, 2] != TileType.Path && corners[1, 2] != TileType.PathCurve)
        {
            curvedTile += 1;
        }
        if (corners[2, 1] != TileType.Path && corners[2, 1] != TileType.PathCurve)
        {
            curvedTile += 2;
        }
        MapHolder.tiles[column, row].curvedTileVariation = curvedTile;

        //useless to do it another way
        //TODO:add better prefab type check
        MapHolder.tiles[column, row].RemoveQuarters();

        //opposite tile cant be created if both sides are not path or curved path
        if (curvedTile == 0)
        {
            GameObject oppositePrefab = null;
            var oppositePrefabType = (corners[2, 2] != TileType.Path && corners[2, 2] != TileType.PathCurve) ? TilePrefabType.PathSmallCorner : TilePrefabType.PathFull;

            oppositePrefab = MapHolder.mapPrefab.prefabDictionary[oppositePrefabType];
            MapHolder.tiles[column, row].prefabType[oppositeRotation] = oppositePrefabType;
            //creates opposite corner and adds its reference to MapHolder
            GameObject oppositeTile = GameObject.Instantiate(oppositePrefab,MapHolder.tiles[column, row].backgroundTile.transform);

            oppositeTile.transform.localPosition = Util.offset[oppositeRotation];
            oppositeTile.transform.localRotation = Quaternion.Euler(0, oppositeRotation * 90, 0);
            MapHolder.tiles[column, row].quarters[oppositeRotation] = oppositeTile;
            
            if (variation >= 0)
            {
                oppositeTile.GetComponentInChildren<MeshRenderer>().material = MapHolder.mapPrefab.pathVariationMaterial[variation];
            }
        }

        //creates curved corner and adds its reference to MapHolder
        GameObject tile = GameObject.Instantiate(MapHolder.mapPrefab.specialCurvedPath[curvedTile], MapHolder.tiles[column, row].backgroundTile.transform);
        tile.transform.localPosition = Util.halfOffset;
        tile.transform.localRotation = Quaternion.Euler(0, rotation * 90, 0); 
        
        MapHolder.tiles[column, row].quarters[rotation] = tile;
        MapHolder.tiles[column, row].prefabType[rotation] = TilePrefabType.PathCurved;
        //MapHolder.tiles[column, row].diagonalRotation = rotation;
        
        
        if (variation >= 0)
        {
            tile.GetComponentInChildren<MeshRenderer>().material = MapHolder.mapPrefab.pathVariationMaterial[variation];
        }
    }

    public static void RedoTiles(HashSet<Vector2Int> pathTiles)
    {
        if (pathTiles.Count == 0)
        {
            return;
        }

        foreach(var path in pathTiles)
        {
            if (path.x >= 0 && path.x < MapHolder.width &&
                path.y >=0 && path.y < MapHolder.height && 
                (MapHolder.tiles[path.x,path.y].type == TileType.Path || MapHolder.tiles[path.x, path.y].type == TileType.PathCurve))
            {
                CreatePath(path.x, path.y, MapHolder.tiles[path.x, path.y].elevation, MapHolder.tiles[path.x,path.y].variation);
            }
        }

    }

}