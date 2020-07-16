using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandBuilder : MonoBehaviour
{
    public static void CreateEmptyLand(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                CreateLandTile(column, row);
            }
        }
    }
    public static void CreateLandTile(int column, int row, int elevation = 0)
    {
        //creates tile and adds its reference to MapHolder
        if (MapHolder.tiles[column, row] != null)
        {
            if (MapHolder.tiles[column, row].backgroundType == TilePrefabType.Land)
            {
                MapHolder.tiles[column, row].RemoveQuarters();
                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
            else
            {
                MapHolder.tiles[column, row].HardErase();
                MapHolder.tiles[column, row].backgroundTile = Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land], MapHolder.tiles[column, row].colliderObject.transform);
                MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;

                MapHolder.tiles[column, row].SetElevation(MapHolder.elevationLevels[elevation]);
            }
        }
        else
        {
            MapHolder.tiles[column, row] = new MapTile(Instantiate(MapHolder.mapPrefab.prefabDictionary[TilePrefabType.Land]));
            MapHolder.tiles[column, row].backgroundType = TilePrefabType.Land;

            MapHolder.tiles[column, row].SetCreatedElevation(MapHolder.elevationLevels[elevation]);
            MapHolder.tiles[column, row].SetPosition(new Vector3(column, 0, -row));
            MapHolder.tiles[column, row].colliderObject.name = $"{column} {row}";
        }

        MapHolder.tiles[column, row].elevation = elevation;
        MapHolder.tiles[column, row].diagonalPathRotation = -1;
        MapHolder.tiles[column, row].diagonaWaterRotation = -1;
        MapHolder.tiles[column, row].type = TileType.Land;

        if (elevation > 0)
        {
            CliffBuilder.CreateCliffSides(column, row);
        }
    }
}
