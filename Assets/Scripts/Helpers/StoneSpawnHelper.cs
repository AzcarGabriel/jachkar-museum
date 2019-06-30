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
        float[] scales = new float[] { 1.0f, 1.0f, 1.0f, 0.55f, 20.0f };
        int index = 0;
        if (stoneId <= 7)
        {
            index = 0;
        }
        else if (stoneId <= 14)
        {
            index = 1;
        }
        else if (stoneId <= 27)
        {
            index = 2;
        }
        else if (stoneId <= 46)
        {
            index = 3;
        }
        else if (stoneId <= 59)
        {
            index = 4;
        }

        return scales[index];
    }
}

