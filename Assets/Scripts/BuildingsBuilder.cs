using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsBuilder
{
    //for building unique buildings

    public static void AddBuilding(int column, int row, UniqueBuilding building)
    {
        //check if can fit
    }

    static int FindStartingPoint(int column, int size)
    {
        int halfSize = size / 2;
        //3 -> 1 (0, 1 ,2)
        //5 -> 2 (0, 1, 2, 3, 4)
        //4-> 2  (0, 1, 2, 3)

        if (size%2 == 0)
        {
            halfSize -= 1;
        }

        return column - halfSize;
    }
}
