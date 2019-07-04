using System.Collections.Generic;
using UnityEngine;


public class LoadObjectFromBundle : MonoBehaviour {
    public string bundleName;
    public List<string> assetNames;
    public string terrainName;
    public static List<GameObject> sceneStones = new List<GameObject>();
    public static List<TextAsset> stonesMetadata = new List<TextAsset>();
    
    // Use this for initialization
    void Start ()
    {
        InstantiateTheScene(EnvSceneGui.assetBundle);
    }


    void InstantiateTheScene(AssetBundle bundle)
    {
        var dir = new Dictionary<string, Texture2D>();
        var light = new Dictionary<string, Texture2D>();
        sceneStones.Clear();
        assetNames.ForEach(n =>
        {
            if (bundleName.Equals("Wall") && n.Equals("stones"))
            {
                GameObject obj = bundle.LoadAsset<GameObject>(n);
                Instantiate<GameObject>(obj);
                int count = obj.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    GameObject gameObject = obj.transform.GetChild(i).gameObject;
                    sceneStones.Add(gameObject);
                }
            }
            else
            {
                GameObject obj = bundle.LoadAsset<GameObject>(n);
                if (!bundleName.Equals("stones"))
                {
                    Instantiate<GameObject>(obj);
                }
                sceneStones.Add(obj);
            }
        });

        if (bundleName.Equals("stones"))
        {
            sceneStones.Clear();
            string[] names = bundle.GetAllAssetNames();
            for (int i = 0; i < names.Length; i++)
            {
                GameObject obj = bundle.LoadAsset<GameObject>(names[i]);
                sceneStones.Add(obj);
            }
        }

        GameObject terrain = (GameObject)bundle.LoadAsset(terrainName);
        if (bundleName.Equals("echmiadzinally"))
        {
            // terrain.transform.Translate(new Vector3(-13.5f, -11.3f, -15.0f));
        }
        if (terrain != null)
            Instantiate(terrain);

        // Metadata bundle
        stonesMetadata.Clear();
        string[] names_metadata = EnvSceneGui.metadataAssetBundle.GetAllAssetNames();
        for (int i = 0; i < names_metadata.Length; i++)
        {
            TextAsset obj = EnvSceneGui.metadataAssetBundle.LoadAsset<TextAsset> (names_metadata[i]);
            stonesMetadata.Add(obj);
        }
    }

    void Update()
    {

    }
}
