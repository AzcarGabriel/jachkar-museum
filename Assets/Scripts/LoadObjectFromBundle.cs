using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadObjectFromBundle : MonoBehaviour {
    public string bundleName;
    public List<string> assetNames;
    public string terrainName;
    public static List<GameObject> sceneStones = new List<GameObject>(); 
    
    // Use this for initialization
    void Start () {
        Debug.Log(EnvSceneGui.assetBundle);
        InstantiateTheScene(EnvSceneGui.assetBundle);
    }


    void InstantiateTheScene(AssetBundle bundle)
    {
        var dir = new Dictionary<string, Texture2D>();
        var light = new Dictionary<string, Texture2D>();
        sceneStones.Clear();
        assetNames.ForEach(n =>
        {
            Debug.Log(n);
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
                if (!bundleName.Equals("museum"))
                {
                    Instantiate<GameObject>(obj);
                }
                sceneStones.Add(obj);
            }

        });

        GameObject terrain = (GameObject)bundle.LoadAsset(terrainName);
        if (bundleName.Equals("echmiadzinally"))
        {
            // terrain.transform.Translate(new Vector3(-13.5f, -11.3f, -15.0f));
        }
        if (terrain != null)
            Instantiate(terrain);
    }

  void Update()
{
    if (Input.GetKey("m"))
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}


}
