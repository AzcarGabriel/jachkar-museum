using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class StoneSpawnHelper
{
    /*0 al 7 EchmiadzinAlly
    8 al 13 Museum
    14 al 26 Noradus
    27 al 45 Noravank
    46 al 58 WallStones*/

    public static float GetStoneScaleById(int stoneId)
    {
        float[] scales = new float[] { 1.0f, 1.0f, 1.0f, 0.55f, 1.0f };
        int index = 0;
        if (stoneId <= 7)
        {
            index = 0;
        }
        else if (stoneId <= 13)
        {
            index = 1;
        }
        else if (stoneId <= 26)
        {
            index = 2;
        }
        else if (stoneId <= 45)
        {
            index = 3;
        }
        else if (stoneId <= 58)
        {
            index = 4;
        }

        return scales[index];
    }

    public static int GetStoneId(int number, string name)
    {
        int id = 0;
        if (name.Equals("EchmiadzinAlly"))
        {
            id = number;
        }
        else if (name.Equals("museum"))
        {
            id = number + 7;
        }
        else if (name.Equals("Noradus"))
        {
            id = number + 13;
        }
        else if (name.Equals("Noravank"))
        {
            id = number + 26;
        }
        else if (name.Equals("wallStones"))
        {
            id = number + 45;
        }

        return id + 1;
    }
}

