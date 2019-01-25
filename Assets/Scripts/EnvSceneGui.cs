using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnvSceneGui : MonoBehaviour {

    public GameObject loadScreen;
    public Slider slider;

    private const string domain = "http://saduewa.dcc.uchile.cl/museum/AssetBundles/";

    // Use this for initialization
    void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void openScene(String name) {
        StartCoroutine(loadBundle(name));
       
    }
    public static AssetBundle assetBundle;

    IEnumerator loadBundle(string name)
    {
        if (assetBundle != null)
        {

            assetBundle.Unload(false);
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

        var www = WWW.LoadFromCacheOrDownload(domain + n, 1);

        loadScreen.SetActive(true);
        
        while (!www.isDone)
        {
            slider.value = www.progress;
            yield return null;
        }
        Debug.Log("Downloaded");
        assetBundle = www.assetBundle;
        SceneManager.LoadScene(n, LoadSceneMode.Single);
        Debug.Log(assetBundle);
    }


    public void exit()
    {
        Application.Quit();
    }
}
