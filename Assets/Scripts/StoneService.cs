/*
    StoneService.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections;
using System.IO;
using System.Linq;
using Networking;
using UI;
using UI.UIScripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class StoneService : MonoBehaviour
{
    public struct BundleName
    {
        public BundleName(string stoneBundleName, string metadataBundleName)
        {
            this.stoneBundleName = stoneBundleName;
            this.metadataBundleName = metadataBundleName;
            thumbsBundleName = null;
        }

        public BundleName(string thumbsBundleName)
        {
            stoneBundleName = null;
            metadataBundleName = null;
            this.thumbsBundleName = thumbsBundleName;
        }

        public string stoneBundleName { get; }
        public string metadataBundleName { get; }
        public string thumbsBundleName { get; }
    }

    public VisualElement LoadScreen;

    public string thumbsBundleName = "stones_thumbs";
    private const string Domain = "https://corsproxy.io/?https://saduewa.dcc.uchile.cl/museum/AssetBundles/";

    public IEnumerator DownloadThumbs(Action doLast = null)
    {
        Debug.Log("Download Thumbs");
        yield return StartCoroutine(this.DownloadBundle(new BundleName(this.thumbsBundleName)));
        doLast?.Invoke();
    }

    public IEnumerator SpawnStoneWithPositionAndRotation(int dictId, int stoneId, Vector3 sp, Quaternion rt, Action doLast = null, bool isDetailStone = false)
    {
        Debug.Log("Rotation");
        // Check if the stone is already downloaded
        GameObject stone = this.SearchStone(stoneId);

        if (stone == null)
        {
            // If not, download it
            BundleName bundleName = CalculateAssetBundleNameByStoneIndex(stoneId);
            yield return StartCoroutine(this.DownloadBundle(bundleName));
        }

        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        GameObject obj = Instantiate(this.SearchStone(stoneId), sp, rt);
        obj.transform.localScale *= scale;

        // add the spawned stone to the list
        if (!isDetailStone) ServerManager.Instance.AddSpawnedStone(dictId, stoneId, obj);
        else obj.name = "detailStone";
        doLast?.Invoke();
    }

    public IEnumerator SpawnStoneWithPositionRotationScale(int dictId, int stoneId, Vector3 sp, Quaternion rt, Vector3 sc, Action doLast = null)
    {
        // Check if the stone is already downloaded
        GameObject stone = this.SearchStone(stoneId);

        if (null == stone)
        {
            BundleName bundleName = CalculateAssetBundleNameByStoneIndex(stoneId);
            yield return StartCoroutine(this.DownloadBundle(bundleName));
        }

        GameObject obj = Instantiate(this.SearchStone(stoneId), sp, rt);
        obj.transform.localScale = sc;

        // add the spawned stone to the list
        ServerManager.Instance.AddSpawnedStone(dictId, stoneId, obj);
        doLast?.Invoke();
    }

    private GameObject SearchStone(int index)
    {
        string stoneName = this.GetStoneName(index);
        foreach (AssetBundle ab in StonesValues.assetBundles)
        {
            if (CheckStoneInBundle(stoneName, ab))
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

        #if !UNITY_WEBGL
           Caching.ClearOtherCachedVersions("anything", new Hash128());
        #endif

        LoadScreen.Display(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Stone download
        if (bundleName is { stoneBundleName: not null, metadataBundleName: not null })
        {
            AssetBundle metadataBundle = null;
            AssetBundle stonesBundle = null;

            if (StaticValues.Online)
            {
                // Download metadata
                using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(Domain + bundleName.metadataBundleName))
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
                        metadataBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                    }
                }

                // Download Stones
                using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(Domain + bundleName.stoneBundleName))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(uwr.error);
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        //Debug.Log("Downloaded stone bundle " + bundleName.stoneBundleName);
                        stonesBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                    }
                }
            }
            else
            {
                //Debug.Log("Loaded offline " + bundleName.metadataBundleName);
                metadataBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName.metadataBundleName));

                //Debug.Log("Loaded offline " + bundleName.stoneBundleName);
                stonesBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName.stoneBundleName));
            }

            // Add Bundles to Stones Values
            if (metadataBundle != null)
            {
                StonesValues.metadataAssetBundles.Add(metadataBundle);
            }

            if (stonesBundle != null)
            {
                StonesValues.assetBundles.Add(stonesBundle);
            }
        }
        else if (bundleName.thumbsBundleName != null) // Thumbs download
        {
            // Download thumbs
            AssetBundle thumbsBundle = null;
            if (StaticValues.Online)
            {
                using UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(Domain + bundleName.thumbsBundleName);
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    Debug.Log("Downloaded thumbs bundle " + bundleName.thumbsBundleName);
                    thumbsBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                }
            }
            else
            {
                //Debug.Log("Loaded offline " + bundleName.thumbsBundleName);
                thumbsBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName.thumbsBundleName));
            }

            if (thumbsBundle != null)
            {
                string[] assetNames = thumbsBundle.GetAllAssetNames();
                foreach (string assetName in assetNames)
                {
                    Texture2D texture = thumbsBundle.LoadAsset<Texture2D>(assetName);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f);
                    sprite.name = texture.name;
                    StonesValues.stonesThumbs.Add(sprite);
                }
            }

            StonesValues.stonesThumbs.Sort((p, q) => Int32.Parse(p.name) - Int32.Parse(q.name));
        }

        LoadScreen.Display(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private bool CheckStoneInBundle(string stoneName, AssetBundle ab)
    {
        return ab.GetAllAssetNames().Any(assetName => assetName.EndsWith(stoneName));
    }

    public static BundleName CalculateAssetBundleNameByStoneIndex(int index)
    {
        if (47 <= index)
        {
            return new BundleName("stones_" + index, "stones_metadata_" + index);
        }

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
