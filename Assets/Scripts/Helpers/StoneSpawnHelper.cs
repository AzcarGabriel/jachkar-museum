/*
    StoneSpawnHelper.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;

public static class StoneSpawnHelper
{
    public static float GetStoneScaleById(int stoneId)
    {
        if (stoneId == 4)
        {
            return 400.0f;
        }
        else if (stoneId == 6 || stoneId == 22 || stoneId == 23 || stoneId == 24 || stoneId == 25 || stoneId == 26)
        {
            return 100.0f;
        }
        else if (stoneId == 49)
        {
            return 50.0f;
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

        return Quaternion.Euler(-90, 0, 0);
    }

    public static Vector3 GetStoneSpawnPointOffsetById(int stoneId)
    {
        if (stoneId == 22 || stoneId == 23 || stoneId == 24 || stoneId == 25 || stoneId == 26)
        {
            return new Vector3(0.0f, 1.0f, 0.0f);
        }
        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}
