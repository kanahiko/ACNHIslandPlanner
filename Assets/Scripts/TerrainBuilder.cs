using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder : MonoBehaviour
{
    public Texture2D terrain;

    public GameObject landPrefab;
    public GameObject waterPrefab;
    public GameObject waterSinglePrefab;
    public GameObject waterSmallCornerPrefab;
    public GameObject waterBigCornerPrefab;
    public GameObject waterSidePrefab;

    public GameObject waterSideQuarterPrefab;
    public GameObject waterSmallCornerQuarterPrefab;
    public GameObject waterBigCornerQuarterPrefab;
    public GameObject waterLandQuarterPrefab;

    Vector3[] offset = new Vector3[]
    {
        new Vector3(-0.25f,0,+0.25f),
        new Vector3(0.25f,0,0.25f),
        new Vector3(0.25f,0,-0.25f),
        new Vector3(-0.25f,0,-0.25f)

    };
    // Start is called before the first frame update
    void Start()
    {
        Color[] pixels = new Color[terrain.width * terrain.height];
        for (int j = 0; j < terrain.height; j++)
        {
            for (int i = 0; i < terrain.width; i++)
        {
            
                pixels[j* terrain.width+i]=terrain.GetPixel(i, j);
                //Debug.Log($"{j} {i} {j * terrain.width + i} { terrain.GetPixel(i, j)}");
            
        }
        }

        CreateLand(pixels, terrain.width, terrain.height);

        //Instantiate((pixels[i * terrain.height + j].g > 0.5f ? landPrefab : waterPrefab), new Vector3(i, 0, j), Quaternion.identity);
    }

    void CreateLand(Color[] pixels, int width, int height)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int currentIndex = i * width + j;
                if (pixels[currentIndex].g > 0.5f)
                {
                    Instantiate(landPrefab, new Vector3(j, 0, -i), Quaternion.identity);
                }
                else
                {
                    Instantiate(waterPrefab, new Vector3(j, 0, -i), Quaternion.identity);

                    int[,] corners = CreateMatrix(pixels, width, height,currentIndex, i, j);
                    corners = RemoveNulls(corners);

                    //for quarter 1
                    for (int k = 0; k < 4; k++) 
                    {
                        FindCorner(corners, k,i,j);

                        string debugstring = "";
                        for (int l = 0; l < 3; l++)
                        {
                            debugstring += ($"\n{corners[l, 0]} {corners[l, 1]} {corners[l, 2]} ");
                        }
                        Debug.Log(debugstring);
                        corners = RotateMatrix(corners);
                    }
                    Debug.Log($"-----{currentIndex}-----");

                }
            }
        }
    }


    void FindCorner(int[,] corners,int rotation, int x, int y)
    {
        CornerType type = CornerType.Nothing;
        if (corners[0, 1] == 0)
        {
            if (corners[1, 0] == 0)
            {
                type = CornerType.BigCorner;
            }
            else
            {
                type = CornerType.Side;
            }
        }
        else
        {
            if (corners[1, 0] == 0)
            {
                type = CornerType.SideRotated;
            }
            else
            {
                if (corners[0, 0] == 0)
                {
                    type = CornerType.SmallCorner;
                }
            }
        }
        /*for (int i = 0; i < 3; i++)
        {
            Debug.Log($"{corners[i, 0]} {corners[i, 1]} {corners[i, 2]} ");
        }
        Debug.Log($"{corners[0, 0]} {corners[0, 1]} {corners[1, 0]} ");*/

        Quaternion rotate = Quaternion.Euler(0, 90 * rotation, 0);

        GameObject prefab = null;
        switch (type)
        {
            case CornerType.Nothing:
                break;
            case CornerType.Side:
                prefab = waterSideQuarterPrefab;
                break;
            case CornerType.SideRotated:
                prefab = waterSideQuarterPrefab;
                rotate *= Quaternion.Euler(0, -90, 0);
                break;
            case CornerType.BigCorner:
                prefab = waterBigCornerQuarterPrefab;
                break;
            case CornerType.SmallCorner:
                prefab = waterSmallCornerQuarterPrefab;
                break;
            case CornerType.Land:
                break;
        }

        if (prefab != null)
        {
            Debug.Log($"{rotation} {type}  {offset[rotation]}");
            GameObject block = Instantiate(prefab, new Vector3(y, 0, -x) + offset[rotation] + new Vector3(0.5f, 0, 0.5f), rotate);
            block.name = type.ToString();
        }
    }

    int[,] CreateMatrix(Color[] pixels, int width, int height, int currentIndex, int i, int j)
    {
        int[,] corners = new int[3, 3];
        corners[1, 1] = 1;

        if (i == 0)
        {
            corners[0, 0] = -1;
            corners[0, 1] = -1;
            corners[0, 2] = -1;
        }
        else
        {
            if (j != 0)
            {
                corners[0, 0] = pixels[currentIndex - width - 1].g < 0.5f ? 1 : 0;
            }

            corners[0, 1] = pixels[currentIndex - width].g < 0.5f ? 1 : 0;

            if (j != width - 1)
            {
                corners[0, 2] = pixels[currentIndex - width + 1].g < 0.5f ? 1 : 0;
            }
        }

        if (j == 0)
        {
            corners[0, 0] = -1;
            corners[1, 0] = -1;
            corners[2, 0] = -1;
        }
        else
        {
            corners[1, 0] = pixels[currentIndex - 1].g < 0.5f ? 1 : 0;
        }

        if (i == height - 1)
        {
            corners[2, 0] = -1;
            corners[2, 1] = -1;
            corners[2, 2] = -1;
        }
        else
        {
            if (j != 0)
            {
                corners[2, 0] = pixels[currentIndex + width - 1].g < 0.5f ? 1 : 0;
            }

            corners[2, 1] = pixels[currentIndex + width].g < 0.5f ? 1 : 0;

            if (j != width - 1)
            {
                corners[2, 2] = pixels[currentIndex + width + 1].g < 0.5f ? 1 : 0;
            }
        }

        if (j == width - 1)
        {
            corners[0, 2] = -1;
            corners[1, 2] = -1;
            corners[2, 2] = -1;
        }
        else
        {
            corners[1, 2] = pixels[currentIndex + 1].g < 0.5f ? 1 : 0;
        }

        return corners;
    }

    int[,] RotateMatrix(int[,] corners)
    {
        int[,] newMatrix = corners;

        int temp = corners[2, 0];
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

    /*int[] GetCorners(int rotation)
    {
        int[] result = new int[3];
    }*/
    int[,] RemoveNulls(int[,] corners)
    {
        for (int i = 0; i < 3; i++)
        {
            if (corners[0,i] == -1)
            {
                corners[0, i] = corners[1, i];
            }

            if (corners[2, i] == -1)
            {
                corners[2, i] = corners[1, i];
            }
            if (corners[i, 0] == -1)
            {
                corners[i, 0] = corners[i, 1];
            }
            if (corners[i, 2] == -1)
            {
                corners[i, 2] = corners[i, 1];
            }
        }

        if (corners[0, 0] == -1)
        {
            corners[0, 0] = corners[0, 1];
        }
        if (corners[0, 2] == -1)
        {
            corners[0, 2] = corners[0, 1];
        }
        if (corners[2, 0] == -1)
        {
            corners[2, 0] = corners[2, 1];
        }
        if (corners[2, 2] == -1)
        {
            corners[2, 2] = corners[2, 1];
        }

        return corners;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum CornerType
{
    Nothing = 0,
    Side = 1,
    SideRotated = 2,
    BigCorner = 3,
    SmallCorner = 4,
    Land =5
}
