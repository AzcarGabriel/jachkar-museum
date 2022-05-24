/*
    StoneSpawnHelper.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System.Collections.Generic;
using UnityEngine;

public static class StoneSpawnHelper
{
    public static float GetStoneScaleById(int stoneId)
    {
        List<int> fiftyGroup = new List<int> { 47, 48 };
        List<int> seventyFiveGroup = new List<int> { 27, 28, 29, 41, 46, 49 };
        List<int> hundredGroup = new List<int> { 6, 22, 23, 24, 26 };

        if (fiftyGroup.Contains(stoneId))
        {
            return 50.0f;
        }
        else if (seventyFiveGroup.Contains(stoneId))
        {
            return 75.0f;
        }
        else if (hundredGroup.Contains(stoneId))
        {
            return 100.0f;
        }
        else if (stoneId == 4 || stoneId == 40)
        {
            return 400.0f;
        }

        return 200.0f;
    }

    public static Quaternion GetStoneRotationById(int stoneId)
    {
        if (stoneId == 6)
        {
            return Quaternion.Euler(-90, 0, 90);
        }
        else if (stoneId == 10)
        {
            return Quaternion.Euler(-90, 0, 180);
        }
        else if (47 == stoneId)
        {
            return Quaternion.Euler(0, 180, -180);
        }
        else if (49 == stoneId)
        {
            return Quaternion.Euler(0, 180, 0);
        }

        return Quaternion.Euler(-90, 0, 0);
    }

    public static Vector3 GetStoneSpawnPointOffsetById(int stoneId)
    {
        if (stoneId == 22 || stoneId == 23 || stoneId == 24 || stoneId == 25 || stoneId == 26)
        {
            return new Vector3(0.0f, 1.0f, 0.0f);
        }
        else if (stoneId == 40)
        {
            return new Vector3(0.0f, 5.0f, 0.0f);
        }

        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}
