/*
    StaticValues.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;

public class StaticValues : MonoBehaviour
{
    public static string StoneName = "";
    public static bool BackFromDetails = false;
    public static bool Writing = false;
    public static readonly bool Online = false; 
    public static bool ShouldLock = false;
    public static bool ShouldEnable = false;
    public static GameObject SelfFPS;
    public static bool OfflineMode = false;

    public static Camera TopCamera; // THIS SHOULD CHANGE WHEN CHANGING TO NEW UI TOOLKIY
}
