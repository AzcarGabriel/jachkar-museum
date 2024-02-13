#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AssetBundleCreator : MonoBehaviour
{
    [MenuItem("Assets/Build Windows Asset Bundle")]
    static void BuildWindowsBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
    
    [MenuItem("Assets/Build Linux Asset Bundle")]
    static void BuildLinuxBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
    }
    
    [MenuItem("Assets/Build WebGL Asset Bundle")]
    static void BuildWebGLBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }
}
#endif