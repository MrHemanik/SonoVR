using UnityEngine;
using UnityEditor;

namespace mKit
{
    /// <summary>
    /// This Class displays a Button in the Inspector, for the CameraImages.
    /// A Click on this Button will save the texture to png.
    /// </summary>
    [CustomEditor(typeof(CameraImage))]
    public class SaveToPNGEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();

            CameraImage myScript = (CameraImage)target;
            if (GUILayout.Button("Save To PNG"))
            {
                string filename = Application.dataPath + "/../SavedScreen.png";
                Debug.Log("EDITOR Save To PNG: saving camera target as " + filename);

                myScript.SaveToPng(filename);
            }
        }
    }
}