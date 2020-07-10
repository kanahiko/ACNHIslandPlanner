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
    public static Vector3[] offset = new Vector3[]
        {
            new Vector3(0.25f,0,0.75f),
            new Vector3(0.75f,0,0.75f),
            new Vector3(0.75f,0,0.25f),
            new Vector3(0.25f,0,0.25f)

        };
    
    public static Vector3 halfOffset = new Vector3(0.5f, 0, 0.5f);

    public static List<Vector2Int> indexOffsetCross = new List<Vector2Int>
    {
        new Vector2Int(-1 , 0), new Vector2Int (0,1), new Vector2Int(1,0),new Vector2Int(0,-1)
    };
    public static List<Vector2Int> indexOffsetDiagonal = new List<Vector2Int>
    {
        new Vector2Int(-1 , -1), new Vector2Int (1,-1), new Vector2Int(-1,1),new Vector2Int(1,1)
    };
    
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

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) 
                {
                    continue;
                }
                if(!(column + j >= 0 && column + j < MapHolder.width && row + i >= 0 && row + i < MapHolder.height))
                {
                    corners[i + 1, j + 1] = TileType.Land;
                    continue;
                }

                corners[i + 1, j + 1] = MapHolder.grid[(row + i) * MapHolder.width + column + j];
                
                if ((corners[1, 1] == TileType.Path || corners[1, 1] == TileType.PathCurve) &&
                    ((corners[i + 1, j + 1] != TileType.Path && corners[i + 1, j + 1] != TileType.PathCurve) || MapHolder.tiles[column + j, row + i].elevation != elevation))
                {
                    corners[i + 1, j + 1] = TileType.Land;
                }
            }
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
    
    public static bool CheckHalfSurroundedBySameElevation(int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        Vector2Int emptyIndex = new Vector2Int(-2,-2);
        for (int i = 0; i < 4; i++)
        {
            if ((column + indexOffsetCross[i].x >= 0 && column + indexOffsetCross[i].y < MapHolder.width &&
                  row + indexOffsetCross[i].y >= 0 && row + indexOffsetCross[i].y < MapHolder.height))
            {
                if (MapHolder.tiles[column + indexOffsetCross[i].x, row + indexOffsetCross[i].y].elevation != elevation)
                {
                    if (emptyIndex.x != -2)
                    {
                        return false;
                    }
                    else
                    {
                        emptyIndex.x = indexOffsetCross[i].x;
                        emptyIndex.y = indexOffsetCross[i].y;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if ((column + indexOffsetDiagonal[i].x >= 0 && column + indexOffsetDiagonal[i].y < MapHolder.width &&
                  row + indexOffsetDiagonal[i].y >= 0 && row + indexOffsetDiagonal[i].y < MapHolder.height))
            {
                if (MapHolder.tiles[column + indexOffsetDiagonal[i].x, row + indexOffsetDiagonal[i].y].elevation != elevation)
                {
                    if (emptyIndex.x == -2 || indexOffsetDiagonal[i].x != emptyIndex.x && indexOffsetDiagonal[i].y != emptyIndex.y)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        
        return true;
    }

    public static int[,] GetElevationCorners(int column, int row)
    {
        int[,] corners = new int[3, 3];
        corners[1, 1] =MapHolder.tiles[column,row].elevation;
        int elevationMinusOnes = corners[1, 1] == 0 ? 0 : corners[1, 1] - 1;
        //Debug.Log($"{column} {row} {elevationMinusOnes}");
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (!(column + j >= 0 && column + j < MapHolder.width && row + i >= 0 && row + i < MapHolder.height))
                {
                    corners[i + 1, j + 1] = elevationMinusOnes;
                    continue;
                }

                corners[i + 1, j + 1] = MapHolder.tiles[column + j, row + i].elevation;
            }
        }
        
        return corners;
    }

}

