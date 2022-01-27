/*
    SaveGame.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;
using System.Collections.Generic;

public class SaveGame
{
    private static SaveGame _instance;
    public static SaveGame Instance
    {
        get
        {
            if (_instance == null)
            {
                Load("save");
            }
            return _instance;
        }

    }

    // Serialized fields
    public List<int> StonesNames = new List<int>();
    public List<Vector3> StonesPositions = new List<Vector3>();
    public List<Quaternion> StonesRotations = new List<Quaternion>();

    public static void Save(string file_name)
    {
        FileManager.Save(file_name+".json", _instance);
    }

    public static void Load(string file_name)
    {
        _instance = FileManager.Load<SaveGame>(file_name + ".json");
    }

    public void Clear() {
        this.StonesNames.Clear();
        this.StonesPositions.Clear();
        this.StonesRotations.Clear();
    }
}
