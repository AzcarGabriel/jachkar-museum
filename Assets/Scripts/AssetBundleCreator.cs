#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleCreator : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundle")]
    static void BuildBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }
}
#endif