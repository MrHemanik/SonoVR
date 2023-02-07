using UnityEngine;
using UnityEditor;

namespace mKit
{
    /// <summary>
    /// This Class displays a Button in the Inspector, for the CameraImages.
    /// A Click on this Button will save the texture to png.
    /// </summary>
    [CustomEditor(typeof(VolumeCube))]
    public class SaveToVM2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save To VM2"))
            {
                Volume.Volumes[0].SaveVm2(HelperLib.StreamingAssetsPath + "/ArtificalVolumes/artVolume" + FolderCtrl.CountFilesWithExtension(HelperLib.StreamingAssetsPath + "/ArtificalVolumes/", "vm2") + ".vm2", null);
            }
        }
    }
}
