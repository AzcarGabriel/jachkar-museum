using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnvSceneGui : MonoBehaviour {

    public GameObject loadScreen;
    public Slider slider;

    public static AssetBundle assetBundle;
    public static AssetBundle metadataAssetBundle;

    private const string domain = "http://saduewa.dcc.uchile.cl/museum/AssetBundles/";

    // Use this for initialization
    void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void openScene(String name) {
        StartCoroutine(loadBundle(name));
    }
    
    IEnumerator loadBundle(string name)
    {
        if (assetBundle != null)
        {
            assetBundle.Unload(false);
            metadataAssetBundle.Unload(false);
        }
        Caching.ClearCache();

        string[] tokens = name.Split('/');
        string n = "";
        if (1 < tokens.Length)
        {
            n = tokens[1];
        }
        else
        {
            n = name;
        }

        loadScreen.SetActive(true);

        var www_metadata = WWW.LoadFromCacheOrDownload(domain + "stones_metadata", 1);
        while (!www_metadata.isDone)
        {
            Debug.Log(www_metadata.progress);
            yield return null;
        }
        Debug.Log("Downloaded metadata");
        metadataAssetBundle = www_metadata.assetBundle;

        var www = WWW.LoadFromCacheOrDownload(domain + "stones", 1);
        
        while (!www.isDone)
        {
            slider.value = www.progress;
            yield return null;
        }
        Debug.Log("Downloaded bundle");
        assetBundle = www.assetBundle;
        SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void exit()
    {
        Application.Quit();
    }
}
