/*
    StoneSpawnHelper.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System.Collections.Generic;
using UnityEngine;

public static class StoneSpawnHelper
{
    public static AssetProps GetStoneAssetPropsById(int stoneId)
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
}
