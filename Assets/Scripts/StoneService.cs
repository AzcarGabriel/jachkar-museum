/*
    StoneService.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StoneService : MonoBehaviour
{
    public struct BundleName
    {
        public BundleName(string x, string y)
        {
            stoneBundleName = x;
            metadataBundleName = y;
        }

        public string stoneBundleName { get; }
        public string metadataBundleName { get; }
    }

    public GameObject loadScreen;
    public Slider slider;

    private const string domain = "https://saduewa.dcc.uchile.cl/museum/AssetBundles/";

    // Use this for initialization
    void Start()
    {

    }

    public IEnumerator SpawnStoneWithPositionAndRotation(int stoneId, Vector3 sp, Quaternion rt, Action doLast = null)
    {
        // Check if the stone is already downloaded
        GameObject stone = this.SearchStone(stoneId);

        if (null == stone)
        {
            // If not, download it
            BundleName bundleName = CalculateAssetBundleNameByStoneIndex(stoneId);
            yield return StartCoroutine(this.DownloadBundle(bundleName));
        }

        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        GameObject obj = Instantiate(this.SearchStone(stoneId), sp, rt);
        obj.transform.localScale *= scale;

        doLast?.Invoke();
    }

    public GameObject SearchStone(int index)
    {
        string stoneName = this.GetStoneName(index);
        foreach (AssetBundle ab in StonesValues.assetBundles)
        {
            if (this.CheckStoneInBundle(stoneName, ab))
            {
                return ab.LoadAsset<GameObject>(stoneName);
            }
        }

        return null;
    }

    private string GetStoneName(int index)
    {
        string prefix = "stone";
        if (index < 10)
        {
            prefix += '0';
        }
        return prefix + index + ".prefab";
    }

    private IEnumerator DownloadBundle(BundleName bundleName)
    {
        Caching.ClearCache();

        // TODO mostrar barra avanzando
        loadScreen.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Download metadata
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(domain + bundleName.metadataBundleName))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Debug.Log("Downloaded metadata bundle" + bundleName.metadataBundleName);
                StonesValues.metadataAssetBundles.Add(DownloadHandlerAssetBundle.GetContent(uwr));
            }
        }

        // Download Stones
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(domain + bundleName.stoneBundleName))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Debug.Log("Downloaded stone bundle " + bundleName.stoneBundleName);
                StonesValues.assetBundles.Add(DownloadHandlerAssetBundle.GetContent(uwr));
            }
        }

        loadScreen.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private bool CheckStoneInBundle(string stoneName, AssetBundle ab)
    {
        foreach (string assetName in ab.GetAllAssetNames())
        {
            if (assetName.EndsWith(stoneName))
            {
                return true;
            }
        }
        return false;
    }

    public static BundleName CalculateAssetBundleNameByStoneIndex(int index)
    {
        // Stones start at Stone01
        int init = 1;
        int end = StonesValues.bundleSize;
        while (!(init <= index && index <= end))
        {
            init += StonesValues.bundleSize;
            end += StonesValues.bundleSize;
        }

        string stoneBundleName = "stones_" + init + "_" + end;
        string metadataBundleName = "stones_metadata_" + init + "_" + end;

        return new BundleName(stoneBundleName, metadataBundleName);
    }
}
