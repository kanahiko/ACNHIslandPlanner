using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TileType
{
    Null = 0, Land, Water, WaterDiagonal,
    Path, PathCurve, Cliff, CliffDiagonal
}

public enum TilePrefabType
{
    Null = 0,
    Land = 1,
    WaterSide = 2,
    WaterSideRotated = 3,
    WaterBigCorner = 4,
    WaterSmallCorner = 5,
    WaterDiagonal = 6,
    WaterDiagonalQuarter = 7,
    Water,
    PathSide,
    PathSideRotated,
    PathCorner,
    PathSmallCorner,
    PathCurved,
    PathFull,
    CliffDiagonal
}

public enum ToolType
{
    Waterscaping=0, 
    CliffConstruction = 1,
    PathPermit =2,
    BridgeMarkUp = 3,
    InclineMarkUp = 4,
    BushPlanting =5,
    TreePlanting =6,
    FlowerPlanting =7,
    FenceBuilding = 8,
    BuildingsMarkUp = 9,
    Null
}

[Serializable]
public class ColorTile
{
    public Color32 color;
    public TileType type;
}

[Serializable]
public class TileObject
{
    public TilePrefabType type;
    public GameObject prefab;
}

public enum Direction 
{ 
    Up, Left, Down,Right
}

public static  class Util
{
    public static T[,] RotateMatrix<T>(T[,] corners)
    {
        T[,] newMatrix = corners;

        T temp = corners[2, 0];
        newMatrix[2, 0] = newMatrix[0, 0];
        newMatrix[0, 0] = newMatrix[0, 2];
        newMatrix[0, 2] = newMatrix[2, 2];
        newMatrix[2, 2] = temp;

        temp = corners[1, 0];
        newMatrix[1, 0] = newMatrix[0, 1];
        newMatrix[0, 1] = newMatrix[1, 2];
        newMatrix[1, 2] = newMatrix[2, 1];
        newMatrix[2, 1] = temp;

        return newMatrix;
    }

    public static int SubstractRotation(int rotation, int subtrahend)
    {
        int result = rotation + subtrahend;
        return result > 3 ? result - 4 : result;
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    public static TileType[,] CreateMatrix(TileType[] grid, int currentIndex, int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        TileType[,] corners = new TileType[3, 3];
        corners[1, 1] = grid[currentIndex];

        if (row != 0)
        {
            if (column != 0)
            {
                corners[0, 0] = grid[currentIndex - MapHolder.width - 1];
                if (corners[0, 0] == TileType.Path && corners[0, 0] == TileType.PathCurve && MapHolder.tiles[column - 1, row - 1].elevation != elevation)
                {
                    corners[0, 0] = TileType.Land;
                }
            }
            else
            {
                corners[0, 0] = TileType.Land;
            }

            corners[0, 1] = grid[currentIndex - MapHolder.width]; 
            if (corners[0, 1] == TileType.Path && corners[0, 1] == TileType.PathCurve && MapHolder.tiles[column, row - 1].elevation != elevation)
            {
                corners[0, 1] = TileType.Land;
            }

            if (column != MapHolder.width - 1)
            {
                corners[0, 2] = grid[currentIndex - MapHolder.width + 1]; 
                if (corners[0, 2] == TileType.Path && corners[0, 2] == TileType.PathCurve && MapHolder.tiles[column + 1, row - 1].elevation != elevation)
                {
                    corners[0, 2] = TileType.Land;
                }
            }
            else
            {
                corners[0,2] = TileType.Land;
            }
        }
        else
        {
            corners[0,0] = TileType.Land;
            corners[0,1] = TileType.Land;
            corners[0,2] = TileType.Land;
        }

        if (column != 0)
        {
            corners[1, 0] = grid[currentIndex - 1]; 
            if (corners[1, 0] == TileType.Path && corners[1, 0] == TileType.PathCurve && MapHolder.tiles[column - 1, row].elevation != elevation)
            {
                corners[1, 0] = TileType.Land;
            }
        }
        else
        {
            corners[1, 0] = TileType.Land;
        }

        if (row != MapHolder.height - 1)
        {
            if (column != 0)
            {
                corners[2, 0] = grid[currentIndex + MapHolder.width - 1]; 
                if (corners[2, 0] == TileType.Path && corners[2, 0] == TileType.PathCurve && MapHolder.tiles[column - 1, row + 1].elevation != elevation)
                {
                    corners[2, 0] = TileType.Land;
                }
            }
            else
            {
                corners[2,0] = TileType.Land;
            }

            corners[2, 1] = grid[currentIndex + MapHolder.width];
            if (corners[2, 1] == TileType.Path && corners[2, 1] == TileType.PathCurve && MapHolder.tiles[column, row + 1].elevation != elevation)
            {
                corners[2, 1] = TileType.Land;
            }

            if (column != MapHolder.width - 1)
            {
                corners[2, 2] = grid[currentIndex + MapHolder.width + 1];
                if (corners[2, 2] == TileType.Path && corners[2, 2] == TileType.PathCurve && MapHolder.tiles[column + 1, row + 1].elevation != elevation)
                {
                    corners[2, 2] = TileType.Land;
                }
            }
            else
            {
                corners[2,2] = TileType.Land;
            }
        }
        else
        {
            corners[2, 0] = TileType.Land;
            corners[2,1] = TileType.Land;
            corners[2,2] = TileType.Land;
        }

        if (column != MapHolder.width - 1)
        {
            corners[1, 2] = grid[currentIndex + 1];
            if (corners[1, 2] == TileType.Path && corners[1, 2] == TileType.PathCurve && MapHolder.tiles[column + 1, row].elevation != elevation)
            {
                corners[1, 2] = TileType.Land;
            }
        }
        else
        {
            corners[1,2] = TileType.Land;
        }

        return corners;
    }

    public static TileType[,] RemoveNulls(TileType[,] corners)
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (corners[i, j] == TileType.Null)
                {
                    corners[i, j] = TileType.Land;
                }
            }
        }

        return corners;
    }

    public static bool CanInfluence(this TileType influencer, TileType influencee, int direction, Vector2Int directionOfPath)
    {
        if (influencee == TileType.CliffDiagonal || influencee == TileType.PathCurve)
        {
            return influencer == TileType.Cliff || influencer == TileType.CliffDiagonal; 
        }

        if (influencee == TileType.PathCurve && (direction == directionOfPath.x || direction == directionOfPath.y))
        {
            return influencer == TileType.Path || influencer == TileType.PathCurve;
        }

        return (influencee == TileType.WaterDiagonal);
    }

    public static bool CheckSurroundedBySameElevation(int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((i == 0 && j == 0)||!(column + j >=0 && column +j < MapHolder.width && row + i >=0 && row + i < MapHolder.height))
                {
                    continue;
                }
                if (elevation > MapHolder.tiles[column + j, row + i].elevation)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static int[,] GetElevationCorners(int column, int row)
    {
        int[,] corners = new int[3, 3];
        corners[1, 1] =MapHolder.tiles[column,row].elevation;
        int elevationMinusOnes = corners[1, 1] == 0 ? 0 : corners[1, 1] - 1;

        if (row != 0)
        {
            if (column != 0)
            {
                corners[0, 0] = MapHolder.tiles[column - 1, row - 1].elevation;// grid[currentIndex - MapHolder.width - 1];
            }
            else
            {
                corners[0, 0] = elevationMinusOnes;
            }


            corners[0, 1] = MapHolder.tiles[column, row - 1].elevation;//grid[currentIndex - MapHolder.width];

            if (column != MapHolder.width - 1)
            {
                corners[0, 2] = MapHolder.tiles[column + 1, row -1].elevation;//grid[currentIndex - MapHolder.width + 1];
            }
            else
            {
                corners[0, 2] = elevationMinusOnes;
            }

        }
        else
        {
            corners[0, 0] = elevationMinusOnes;
            corners[0, 1] = elevationMinusOnes;
            corners[0, 2] = elevationMinusOnes;
        }

        if (column != 0)
        {
            corners[1, 0] = MapHolder.tiles[column - 1, row].elevation;//grid[currentIndex - 1];
        }
        else
        {
            corners[1, 0] = elevationMinusOnes;
        }

        if (row != MapHolder.height - 1)
        {
            if (column != 0)
            {
                corners[2, 0] = MapHolder.tiles[column - 1, row + 1].elevation;//grid[currentIndex + MapHolder.width - 1];
            }
            else
            {
                corners[2, 0] = elevationMinusOnes;
            }

            corners[2, 1] = MapHolder.tiles[column, row + 1].elevation;//grid[currentIndex + MapHolder.width];

            if (column != MapHolder.width - 1)
            {
                corners[2, 2] = MapHolder.tiles[column + 1, row + 1].elevation;//grid[currentIndex + MapHolder.width + 1];
            }
            else
            {
                corners[2, 2] = elevationMinusOnes;
            }
        }
        else
        {
            corners[2, 0] = elevationMinusOnes;
            corners[2, 1] = elevationMinusOnes;
            corners[2, 2] = elevationMinusOnes;
        }

        if (column != MapHolder.width - 1)
        {
            corners[1, 2] = MapHolder.tiles[column + 1, row].elevation;//grid[currentIndex + 1];
        }
        else
        {
            corners[1, 2] = elevationMinusOnes;
        }

        return corners;

    }

}

