using UnityEngine;

namespace mKit
{
    /// <summary>
    /// This script must not be packaged in the DLL. Do not place this script in the _Scripts folder.
    /// </summary>
    public class mKitInit : MonoBehaviour
    {

        // Use this for initialization
        private void Awake()
        {
            bool success = false;
            string errmsg = "";

            try
            {
                if (!HelperLib.CheckProjectLayerSetup())
                {
                    throw new System.Exception("mKit: Please update Unity project settings!");
                }

                // Core modules
                gameObject.AddComponent<ImagerCore>();
                gameObject.AddComponent<Volume>();

                success = true;
            }
            catch (System.Exception ex)
            {
                errmsg = ex.Message;
            }



            if (!success)
            {
                Debug.LogError(errmsg);
            }



        }



    }

}