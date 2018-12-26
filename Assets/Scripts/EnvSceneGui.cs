using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnvSceneGui : MonoBehaviour {

    public GameObject loadScreen;
    public Slider slider;

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
        var www = WWW.LoadFromCacheOrDownload("http://saduewa.dcc.uchile.cl/" + name + "?c=3", 1); //31.7.162.98, inside aua 10.1.0.118
        loadScreen.SetActive(true);
        //
        while (!www.isDone)
        {
            Debug.Log(www.progress);
            slider.value = www.progress;
            yield return null;
        }
        Debug.Log("Downloaded");
        assetBundle = www.assetBundle;
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        Debug.Log(assetBundle);
    }


    public void exit()
    {
        Application.Quit();
    }
}
