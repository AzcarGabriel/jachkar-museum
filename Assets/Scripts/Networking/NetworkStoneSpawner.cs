using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using System;
using System.IO;
using UnityEngine.Networking;


public class NetworkStoneSpawner : NetworkBehaviour
{
    public struct BundleName
    {
        public BundleName(string stoneBundleName, string metadataBundleName)
        {
            this.stoneBundleName = stoneBundleName;
            this.metadataBundleName = metadataBundleName;
            this.thumbsBundleName = null;
        }

        public BundleName(string thumbsBundleName)
        {
            this.stoneBundleName = null;
            this.metadataBundleName = null;
            this.thumbsBundleName = thumbsBundleName;
        }

        public string stoneBundleName { get; }
        public string metadataBundleName { get; }
        public string thumbsBundleName { get; }
    }

    public string thumbsBundleName = "stones_thumbs";
    private const string domain = "https://saduewa.dcc.uchile.cl/museum/AssetBundles/";

    public GameObject SearchStone(int index)
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

    private IEnumerator DownloadBundle(BundleName bundleName)
    {
        Caching.ClearCache();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Stone download
        if (bundleName.stoneBundleName != null && bundleName.metadataBundleName != null)
        {
            AssetBundle metadataBundle = null;
            AssetBundle stonesBundle = null;

            if (StaticValues.online)
            {
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
                        metadataBundle = DownloadHandlerAssetBundle.GetContent(uwr);
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
                        stonesBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                    }
                }
            }
            else
            {
                Debug.Log("Loaded offline " + bundleName.metadataBundleName);
                metadataBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName.metadataBundleName));

                Debug.Log("Loaded offline " + bundleName.stoneBundleName);
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
            if (StaticValues.online)
            {
                using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(domain + bundleName.thumbsBundleName))
                {
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
            }
            else
            {
                Debug.Log("Loaded offline " + bundleName.thumbsBundleName);
                thumbsBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName.thumbsBundleName));
            }

            string[] assetNames = thumbsBundle.GetAllAssetNames();
            foreach (string name in assetNames)
            {
                Texture2D texture = thumbsBundle.LoadAsset<Texture2D>(name);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100.0f);
                sprite.name = texture.name;
                StonesValues.stonesThumbs.Add(sprite);
            }
            StonesValues.stonesThumbs.Sort((Sprite p, Sprite q) => Int32.Parse(p.name) - Int32.Parse(q.name));
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnStoneServerRpc(int stoneId, Vector3 sp, Quaternion rt)
    {
        StartCoroutine(SpawnStone(stoneId, sp, rt));
    }

    private IEnumerator SpawnStone(int stoneId, Vector3 sp, Quaternion rt)
    {
        // Need to allow online download (or it will impossible to store the bundles online)
        GameObject stone = this.SearchStone(stoneId);

        if (null == stone)
        {
            Debug.Log("Fue nulo");
            // If not, download it
            BundleName bundleName = CalculateAssetBundleNameByStoneIndex(stoneId);
            yield return StartCoroutine(this.DownloadBundle(bundleName));
        }

        stone = this.SearchStone(stoneId);

        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        GameObject obj = Instantiate(this.SearchStone(stoneId), sp, rt);
        obj.transform.localScale *= scale;
        obj.GetComponent<NetworkObject>().Spawn();
        Debug.Log(obj.GetComponent<NetworkObject>().GetHashCode());
    }

}
