/*
    StaticValues.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;

public class StaticValues : MonoBehaviour
{
    public static string stone_name = "";
    public static string previos_scene = "MainMenu";
    public static bool back_from_details = false;
    public static bool writing = false;
    public static bool online = true;

    void Start()
    {

    }

    void Update()
    {

    }
    public static bool should_lock = false;
    public static bool should_enable = false;
    public static GameObject self_fps;
    public static bool isServer = false;
}
