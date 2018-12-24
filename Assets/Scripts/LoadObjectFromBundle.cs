﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadObjectFromBundle : MonoBehaviour {
    public string bundleName;
    public List<string> assetNames;
    public string terrainName;
    public static List<GameObject> sceneStones = new List<GameObject>(); 
    
    // Use this for initialization
    void Start () {
        InstantiateTheScene(EnvSceneGui.assetBundle);
    }


    void InstantiateTheScene(AssetBundle bundle)
    {
        var dir = new Dictionary<string, Texture2D>();
        var light = new Dictionary<string, Texture2D>();
        sceneStones.Clear();
        assetNames.ForEach(n =>
        {
            if (bundleName.Equals("Wall-stream") && n.Equals("stones"))
            {
                GameObject obj = (GameObject)bundle.LoadAsset(n);
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
                GameObject obj = (GameObject)bundle.LoadAsset(n);
                if (!bundleName.Equals("museum-stream"))
                {
                    Instantiate<GameObject>(obj);
                }
                sceneStones.Add(obj);
            }

        });

        GameObject terrain = (GameObject)bundle.LoadAsset(terrainName);
        if (bundleName.Equals("echmiadzinally-stream"))
        {
            // terrain.transform.Translate(new Vector3(-13.5f, -11.3f, -15.0f));
        }
        if (terrain != null)
            Terrain.Instantiate<GameObject>(terrain);
    }

  void Update()
{
    if (Input.GetKey("m"))
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}


}
