using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TileType
{
    Null, Land, Water, WaterDiagonal,
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
    PathFull
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


    public static TileType[,] CreateMatrix(TileType[] grid, int width, int height, int currentIndex, int i, int j)
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
}

