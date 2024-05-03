using UnityEngine;

[System.Serializable]
public class AssetProps
{
    public float scale = 1.0f;
    public float rotationX = 0.0f;
    public float rotationY = 0.0f;
    public float rotationZ = 0.0f;
    public float offsetX = 0.0f;
    public float offsetY = 0.0f;
    public float offsetZ = 0.0f;
}


[System.Serializable]
public class Khachkar
{
    public int id;
    public string location;
    public string scenario;
    public string setting;
    public string landscape;
    public string accessibility;
    public string category;
    public string productionPeriod;
    public string coonditionOfPreservation;
    public string inscription;
    public string importantFeatures;
    public string references;
    public AssetProps assetProps;
    public static Khachkar CreateFromJSON(string jsonString)
    {
        Khachkar khachkar = JsonUtility.FromJson<Khachkar>(jsonString);
        return khachkar;

    }

}