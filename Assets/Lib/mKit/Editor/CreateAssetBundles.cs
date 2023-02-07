using UnityEditor;
using UnityEngine;

#pragma warning disable 219

public class CreateAssetBundles
{
    [MenuItem ("Assets/Build AssetBundles/Android")]
    static void BuildAllAssetBundlesAndroid ()
    {
        AssetBundleManifest manifestAndroid = BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets/AssetBundles/Android", 
            BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode, 
            BuildTarget.Android);

        Debug.Log("BuildAllAssetBundles() Android");
    
    }

    [MenuItem("Assets/Build AssetBundles/Windows64")]
    static void BuildAllAssetBundlesWin64()
    {
        AssetBundleManifest manifestWindows = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles/Win64",
            BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode,
            BuildTarget.StandaloneWindows64);


        Debug.Log("BuildAllAssetBundles() Windows 64 DONE");
    }

}