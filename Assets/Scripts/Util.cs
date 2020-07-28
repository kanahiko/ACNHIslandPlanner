using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TileType
{
    Null = 0, Land, Water, WaterDiagonal,
    Path, PathCurve, Cliff, CliffDiagonal, Sea, Sand, SandDiagonal, SeaDiagonal
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
    CliffDiagonal,
    Sand, SandDiagonal, SandDiagonalLand,
    Sea, SeaDiagonal
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
    Null = 10,
    SandPermit = 11
}

public enum ToolMode
{
    None,Add,Remove
}

public enum DecorationType
{
   Null = 13, Fence = 0, Plaza = 1, NookShop = 2, Tailors = 3, Museum = 4, PlayerHouse = 5, House =6, Incline =7, Bridge = 8, Camp =9, Flora = 10, Tree = 11, Building = 12,
    Flower = 14, Rock = 15
}

public enum FenceType
{
    Linked = 0, Unlinked = 1, Diagonal = 2
}

public enum Direction
{
    up = 0, right = 1, down = 2, left = 3
}

public enum BuildingInfluence
{
    noInfluence = 0, pathsOnly = 1, fullInfluence =2
}

public enum FenceLinked
{
    noFence = 0, noLink = 1, isLinked = 2
}

public enum FlowerColors
{
    Red =0, White = 1, Yellow = 2, Pink = 3, Orange = 4, Black = 5, Blue = 6, Purple = 7, Green = 8, Gold = 9
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

public static  class Util
{
    public static List<int> sortedDirectionalIndexes = new List<int>
    {
        (int) Direction.left,(int)Direction.right,(int)Direction.up, (int)Direction.down
    };

    public static Vector3[] offset = new Vector3[]
        {
            new Vector3(0.25f,0,0.75f),
            new Vector3(0.75f,0,0.75f),
            new Vector3(0.75f,0,0.25f),
            new Vector3(0.25f,0,0.25f)

        };
    
    public static Vector3 halfOffset = new Vector3(0.5f, 0, 0.5f);
    public static Vector3 cullingPosition = new Vector3(50, 0, 50);

    /// <summary>
    /// x - row, y - column
    /// </summary>
    public static List<Vector2Int> indexOffsetCross = new List<Vector2Int>
    {
        //up
        //x = row
        //y = column
        new Vector2Int(-1 , 0), new Vector2Int (0,1), new Vector2Int(1,0),new Vector2Int(0,-1)
    };
    
    /// <summary>
    /// x  - row, y - column
    /// </summary>
    public static List<Vector2Int> indexOffsetDiagonal = new List<Vector2Int>
    {
        new Vector2Int(-1 , -1), new Vector2Int (-1,1), new Vector2Int(1,1),new Vector2Int(1,-1)
    };
    
    public static Vector2Int[] oppositeCorner = new Vector2Int[]
    {
        new Vector2Int(0,0), new Vector2Int(0,2), new Vector2Int(2,2), new Vector2Int(2,0)  
    };
    
    public static Vector2Int[] oppositeCornerForSand = new Vector2Int[]
    {
        new Vector2Int(-1 , -1), new Vector2Int (1,-1), new Vector2Int(1,1),new Vector2Int(-1,1) 
    };
    
    public static Vector2Int[] inclineRotationsOffset = new Vector2Int[]
    {
        new Vector2Int(0,0), new Vector2Int(0,2), new Vector2Int(2,1), new Vector2Int(1,0)   
    };

    public static Vector2[] bridgeRotationsOffset = new Vector2[]
    {
        new Vector2(0,0), new Vector2(-2.449f,3.371f), new Vector2(0,4), new Vector2(0.62f,0.556f)  
    };

    public static Vector2[] bridgeAdditionalRotationsOffset = new Vector2[]
    {
        new Vector2(0,0), new Vector2(-0.51f,-0.51f), new Vector2(0,0), new Vector2(0.51f,-0.51f)
    };

    public static Quaternion[] bridgeRotations = new Quaternion[]
     {
        Quaternion.identity, Quaternion.Euler(0,45,0), Quaternion.Euler(0,90,0), Quaternion.Euler(0,-45,0)
     };
    
    public static Dictionary<ToolType, DecorationType> toolToDecorationType = new Dictionary<ToolType, DecorationType>
    {
        {ToolType.Waterscaping, DecorationType.Null},
        {ToolType.CliffConstruction, DecorationType.Null},
        {ToolType.SandPermit, DecorationType.Null},
        {ToolType.PathPermit, DecorationType.Null},
        {ToolType.FenceBuilding, DecorationType.Fence},
        {ToolType.FlowerPlanting, DecorationType.Flower},
        {ToolType.BushPlanting, DecorationType.Flora},
        {ToolType.TreePlanting, DecorationType.Tree},
        {ToolType.BridgeMarkUp, DecorationType.Bridge},
        {ToolType.InclineMarkUp, DecorationType.Incline},
        {ToolType.BuildingsMarkUp, DecorationType.Building}
        
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
    

    public static byte SubstractRotation(int rotation, int subtrahend)
    {
        int result = rotation + subtrahend;
        return (byte) (result > 3 ? result - 4 : result);
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    public static TileType[,] CreateMatrix(int column, int row, byte variation = 0)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        TileType[,] corners = new TileType[3, 3];
        corners[1, 1] = MapHolder.tiles[column, row].type;

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

                corners[i + 1, j + 1] = MapHolder.tiles[column + j, row + i].type;
                
                //Debug.Log($"{variation} { MapHolder.tiles[column + j, row + i].variation}");
                if ((corners[1, 1] == TileType.Path || corners[1, 1] == TileType.PathCurve) &&
                    ((corners[i + 1, j + 1] != TileType.Path && corners[i + 1, j + 1] != TileType.PathCurve) || 
                     MapHolder.tiles[column + j, row + i].elevation != elevation ||
                     MapHolder.tiles[column + j, row + i].variation != variation))
                {
                    corners[i + 1, j + 1] = TileType.Land;
                }
            }
        }

        return corners;
    }

    public static bool[] CreateFenceMatrix(int column, int row, byte variation)
    {
        bool[] corners = new bool[4];
        int elevation = MapHolder.tiles[column, row].elevation;

        for (int i = 0; i < indexOffsetCross.Count; i++)
        {
            if ((column + indexOffsetCross[i].y >= 0 && column + indexOffsetCross[i].y < MapHolder.width && 
                row + indexOffsetCross[i].x >= 0 && row + indexOffsetCross[i].x < MapHolder.height))
            {
                corners[i] = MapHolder.decorationsTiles[column + indexOffsetCross[i].y, row + indexOffsetCross[i].x] != null &&
                             MapHolder.decorationsTiles[column + indexOffsetCross[i].y, row + indexOffsetCross[i].x].type == DecorationType.Fence &&
                             elevation == MapHolder.tiles[column + indexOffsetCross[i].y, row + indexOffsetCross[i].x].elevation &&
                             MapHolder.decorationsTiles[column + indexOffsetCross[i].y, row + indexOffsetCross[i].x].variation == variation;
            }
        }

        return corners;
    }
    
    public static bool[,] CreateSandMatrix(int column, int row)
    {
        bool[,] corners = new bool[3,3];

        for (int i = 0; i < 3; i++)
        {
            for (int j=0;j<3;j++)
            {
                if ((column + j >= 0 && column + j < MapHolder.width &&
                     row + i >= 0 && row + i < MapHolder.height))
                {
                    corners[j,i] = MapHolder.tiles[column + j, row + i] != null &&
                                 MapHolder.tiles[column + j, row + i].type == TileType.Sand &&
                                 MapHolder.tiles[column + j, row + i].elevation == 0;
                }
            }
        }

        return corners;
    }
    /// <summary>
    /// returns true if different orientation
    /// false if still the same orientation or inconclusive
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="isHorizontal"></param>
    /// <returns></returns>
    public static bool CheckIfChangeOrientation(int column, int row,bool isHorizontal)
    {
        if (MapHolder.decorationsTiles[column, row].quarters[0] == null &&
            MapHolder.decorationsTiles[column, row].quarters[2] == null)
        {
            return !isHorizontal;
        }
        
        if (MapHolder.decorationsTiles[column, row].quarters[1] == null &&
            MapHolder.decorationsTiles[column, row].quarters[3] == null)
        {
            return isHorizontal;
        }

        return false;
    }

    public static bool NoneOrOne(bool[] corners)
    {
        int sum = 0;
        for (int i = 0; i< 4; i++)
        {
            if (corners[i])
            { 
                sum++; 
            }
        }
        //Debug.Log(sum);
        return sum<= 1;
    }
    
    public static float GetHeight(int column, int row)
    {
        return MapHolder.elevationLevels[MapHolder.tiles[column, row].elevation].transform.localPosition.y;
    }

    public static int GetRotation(int x, int y)
    {
        if (x == 0)
        {
            return y == -1 ? 2 : 0;
        }
        else
        {
            return x == -1 ? 1 : 3;
        }
    }

    public static bool CanInfluence(this TileType influencer, TileType influencee, int direction, 
        Vector2Int directionInfluencee, bool isSameVariation)
    {
        if (influencee == TileType.SeaDiagonal)
        {
            if (influencer == TileType.SeaDiagonal)
            {
                int oppositeDirection = Util.SubstractRotation(direction, 2);
                if (oppositeDirection == directionInfluencee.x || oppositeDirection == directionInfluencee.y)
                {
                    return true;
                }
            }
            else
            {
                if (influencer == TileType.Sand || influencer == TileType.Sea)
                {
                    return true;
                }
                if (influencer == TileType.Land  || influencer == TileType.Sand)
                {
                    int oppositeDirection = Util.SubstractRotation(direction, 2);
                    if (direction == directionInfluencee.x || direction == directionInfluencee.y)
                    {
                        return true;
                    }
                }
            }
        }
        if (influencee == TileType.SandDiagonal)
        {
            if (influencer == TileType.SandDiagonal)
            {
                if (direction == directionInfluencee.x || direction == directionInfluencee.y)
                {
                    return true;
                }
            }
            else
            {
                if (influencer == TileType.Sand)
                {
                    return true;
                }
                if (influencer == TileType.Land  || influencer == TileType.Sand)
                {
                    int oppositeDirection = Util.SubstractRotation(direction, 2);
                    if (direction == directionInfluencee.x || direction == directionInfluencee.y)
                    {
                        return true;
                    }
                }
            }
        }
        
        //Debug.Log($"{influencee} {influencer} {direction} dp={directionOfPath} dw={directionOfWater}");
        if ((influencee == TileType.CliffDiagonal) &&
            (influencer == TileType.Cliff || influencer == TileType.CliffDiagonal) )
        {
            return true; 
        }

        if (isSameVariation && influencee == TileType.PathCurve && (direction == directionInfluencee.x || direction == directionInfluencee.y))
        {
            return influencer == TileType.Path || influencer == TileType.PathCurve;
        }

        return (influencee == TileType.WaterDiagonal && (influencer == TileType.Water ||  (direction == directionInfluencee.x || direction == directionInfluencee.y)));
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

    public static bool CheckSurroundedByLandElevation(int column, int row, bool hasToBeLand)
    {
        int elevation = MapHolder.tiles[column, row].elevation;

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
                    return false;
                }
                if (elevation > MapHolder.tiles[column + j, row + i].elevation || 
                    (MapHolder.decorationsTiles[column + j, row - i] != null  && MapHolder.decorationsTiles[column + j, row - i].type == DecorationType.Building) ||
                    (hasToBeLand && !CheckSurroundedIsLand(column + j, row + i))) 
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public static bool CheckSurroundedBySand(int column, int row)
    {
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
                    return false;
                }
                if (MapHolder.tiles[column + j, row - i].backgroundType != TilePrefabType.Sand) 
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckEmptyLandElevation(int column, int row, int sizeX, int sizeY)
    {
        int elevation = MapHolder.tiles[column, row].elevation;


        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                if (!(column + j >= 0 && column + j < MapHolder.width && row - i >= 0 && row - i < MapHolder.height))
                {
                    return false;
                }
                if (elevation > MapHolder.tiles[column + j, row - i].elevation ||
                    !CheckSurroundedIsLand(column + j, row - i) || MapHolder.decorationsTiles[column+j,row-i] != null ||
                    MapHolder.treeInfluence[column+j,row-i] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckSurroundedIsLand(int column, int row)
    {
        return MapHolder.tiles[column, row].type != TileType.CliffDiagonal &&
                    MapHolder.tiles[column, row].type != TileType.Water &&
                    MapHolder.tiles[column, row].type != TileType.WaterDiagonal &&
                    MapHolder.tiles[column, row].type != TileType.Sand &&
                    MapHolder.tiles[column, row].type != TileType.SandDiagonal &&
                    MapHolder.tiles[column, row].type != TileType.Sea &&
                    MapHolder.tiles[column, row].type != TileType.SeaDiagonal;
    }

    public static bool CheckHalfSurroundedBySameElevation(int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation;
        Vector2Int emptyIndex = new Vector2Int(-2,-2);
        for (int i = 0; i < 4; i++)
        {
            if ((column + indexOffsetCross[i].y >= 0 && column + indexOffsetCross[i].y < MapHolder.width &&
                  row + indexOffsetCross[i].x >= 0 && row + indexOffsetCross[i].x < MapHolder.height))
            {
                if (MapHolder.tiles[column + indexOffsetCross[i].y, row + indexOffsetCross[i].x].elevation < elevation)
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

        for (int i = 0; i < 4; i++)
        {
            if ((column + indexOffsetDiagonal[i].y >= 0 && column + indexOffsetDiagonal[i].y < MapHolder.width &&
                  row + indexOffsetDiagonal[i].x >= 0 && row + indexOffsetDiagonal[i].x < MapHolder.height))
            {
                if (MapHolder.tiles[column + indexOffsetDiagonal[i].y, row + indexOffsetDiagonal[i].x].elevation < elevation)
                {
                    if (emptyIndex.x == -2 || indexOffsetDiagonal[i].x != emptyIndex.x && indexOffsetDiagonal[i].y != emptyIndex.y)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (emptyIndex.x == -2 || indexOffsetDiagonal[i].x != emptyIndex.x && indexOffsetDiagonal[i].y != emptyIndex.y)
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
    public static bool CanRemoveCliff(int column, int row)
    {
        int elevation = MapHolder.tiles[column, row].elevation + 1;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (column + i >= 0 && column + i < MapHolder.width &&
                    row + j >= 0 && row + j < MapHolder.height)
                {
                    if (MapHolder.tiles[column + i, row + j].elevation > elevation && !Util.CheckSurroundedBySameElevation(column + i, row + j))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    
    public static bool ToolModeChange(bool isAdd, int column, int row, TileType type,ref ToolMode toolMode)
    {
        //Debug.Log($"{toolMode} {isAdd} {index} {type}  willdo?");
        if (toolMode == ToolMode.None)
        {
            toolMode = isAdd? ToolMode.Add : ToolMode.Remove;
            MapHolder.tiles[column, row].type = type;
            return true;
        }
        else
        {
            if ((toolMode == ToolMode.Add && isAdd) || (toolMode == ToolMode.Remove && !isAdd))
            {
                MapHolder.tiles[column, row].type = type;
                return true;
            }
        }
        return false;
    }

    public static bool CoordinateExists(int column, int row)
    {
        return column >= 0 && column < MapHolder.width &&
               row >= 0 && row < MapHolder.height;
    }

    public static bool CoordinateOnBorderChunks(int column, int row)
    {
        return (column < 16 || column + 1 > MapHolder.width - 16) || (row < 16 || row + 1 > MapHolder.height - 16);
    }

    public static bool IsOnLandSandBorder(int column, int row)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (CoordinateExists(column + j, row + i))
                {
                    if (MapHolder.tiles[column + j, row + i].type != TileType.Sand &&
                        MapHolder.tiles[column + j, row + i].type != TileType.SandDiagonal &&
                        MapHolder.tiles[column + j, row + i].type != TileType.Sea)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}

