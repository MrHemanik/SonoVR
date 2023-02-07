using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using mKit;

/// <summary>
/// Inspector for .VM2 assets
/// </summary>
[CustomEditor(typeof(DefaultAsset))]
public class Vm2FileTypeEditor : Editor
{
    GUIStyle headingStyle;
    GUIStyle errorStyle;

    readonly string vm2Suffix = ".vm2";
    readonly string essSuffix = ".ess";

    void Awake()
    {
        headingStyle = new GUIStyle();
        headingStyle.fontStyle = FontStyle.Bold;

        errorStyle = new GUIStyle();
        errorStyle.normal.textColor = Color.red;
    }

    public override void OnInspectorGUI()
    {
        // .svg files are imported as a DefaultAsset.
        // Need to determine that this default asset is an .svg file
        var path = AssetDatabase.GetAssetPath(target);

        if (path.EndsWith(vm2Suffix))
        {
            InspectorVm2GUI(path.Substring(0, path.Length - vm2Suffix.Length));
        }
        else if (path.EndsWith(essSuffix))
        {
            InspectorVm2GUI(path.Substring(0, path.Length - essSuffix.Length));
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    private void InspectorVm2GUI(string filePath)
    {
        string vm2File = filePath + vm2Suffix;
        string essFile = filePath + essSuffix;

        bool vm2Exists = File.Exists(vm2File);
        bool essExists = File.Exists(essFile);

        if (vm2Exists)
        {
            GUILayout.Label(new GUIContent("VM2 filename: " + System.IO.Path.GetFileName(vm2File), tooltip: "Volume data"), headingStyle);

            try
            {
                var header = Vm2Header.FromFile(vm2File);

                GUILayout.Label(new GUIContent("Format: " + header.FormatNumber.ToString()));
                GUILayout.Label(new GUIContent("Voxel data dimensions: " + header.VoxelDataDimensions.ToString("F0")));
                GUILayout.Label(new GUIContent("Millimeter dimensions: " + header.MillimeterSize));
                GUILayout.Label(new GUIContent("Bytes per voxel: " + header.BytesPerVoxel));
                GUILayout.Label(new GUIContent("Data size: " + (header.VolumeSize / (1024*1024)) + " MB"));

                if (header.TransformRotation != Quaternion.identity)
                    GUILayout.Label(new GUIContent("Dicom rotation: " + header.TransformRotation.eulerAngles));

                if (header.MatrixTranslation != Vector3.zero)
                    GUILayout.Label(new GUIContent("Dicom position: " + header.MatrixTranslation));

            }
            catch (Exception ex)
            {
                GUILayout.Label(new GUIContent("Could not read file (" + ex.Message + ")"));
            }
        }

        if (essExists)
        {
            if (vm2Exists)
                GUILayout.Label(new GUIContent(""));
            else
                GUILayout.Label(new GUIContent("VM2 filename: MISSING"), errorStyle);

            GUILayout.Label(new GUIContent("ESS filename: " + System.IO.Path.GetFileName(essFile), tooltip:"Empty-space-skipping data"), headingStyle);

            try
            {
                Stream stream = File.OpenRead(essFile);
                var streamLength = (double) stream.Length / (1024 * 1024);

                BinaryReader reader = new BinaryReader(stream);

                using (stream)
                {
                    var blockWidth = reader.ReadInt32();

                    GUILayout.Label(new GUIContent("ESS block width: " + blockWidth));

                    GUILayout.Label(new GUIContent("ESS data size: " + streamLength.ToString("F1") + " MB"));
                }

                
            }
            catch (Exception ex)
            {
                GUILayout.Label(new GUIContent("Could not read file (" + ex.Message + ")"));
            }
        }
    }
}

