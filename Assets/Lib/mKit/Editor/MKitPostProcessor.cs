using UnityEngine;
using UnityEditor;

public class MKitPostprocessor : AssetPostprocessor
{

    void OnPostprocessAssetbundleNameChanged ( string path, string previous, string next) {
        Debug.Log("AB MKitPostprocessor: " + path + " old: " + previous + " new: " + next);
    }
}