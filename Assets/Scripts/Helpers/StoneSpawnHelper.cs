/*
    StoneSpawnHelper.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System.Collections.Generic;
using UnityEngine;

public static class StoneSpawnHelper
{
    private static AssetProps GetStoneAssetPropsById(int stoneId)
    {
        TextAsset metadata = null;
        StoneService.BundleName bundleName = StoneService.CalculateAssetBundleNameByStoneIndex(stoneId);
        string stoneName = "Stone" + stoneId;

        AssetProps assetProps = null;
        foreach (AssetBundle mab in StonesValues.metadataAssetBundles)
        {
            if (mab.name == bundleName.metadataBundleName)
            {
                metadata = mab.LoadAsset<TextAsset>(stoneName);
                Khachkar khachkar = JsonUtility.FromJson<Khachkar>(metadata.text);
                assetProps = khachkar.assetProps;
            }
        }
        return assetProps;
    }

    public static float GetStoneScaleById(int stoneId)
    {
        AssetProps props = GetStoneAssetPropsById(stoneId);
        if (props != null)
        {   
            return props.scale;
        }
        return 200.0f;
    }

    public static Quaternion GetStoneRotationById(int stoneId)
    {
        AssetProps props = GetStoneAssetPropsById(stoneId);
        if (props != null)
        {   
            return Quaternion.Euler(props.rotationX, props.rotationY, props.rotationZ);
        }
        return Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    }

    public static Vector3 GetStoneSpawnPointOffsetById(int stoneId)
    {
        AssetProps props = GetStoneAssetPropsById(stoneId);
        if (props != null)
        {   
            return new Vector3(props.offsetX, props.offsetY, props.offsetZ);
        }
        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}
