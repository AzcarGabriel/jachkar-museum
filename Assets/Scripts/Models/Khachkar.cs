using UnityEngine;

[System.Serializable]
public class AssetProps
{
    public float scale;
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
        Debug.Log(jsonString);
        Khachkar khachkar = JsonUtility.FromJson<Khachkar>(jsonString);
        Debug.Log(khachkar.id);
        Debug.Log(khachkar.location);
        Debug.Log(khachkar.scenario);
        
        return khachkar;

    }

}
