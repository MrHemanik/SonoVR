
using mKit;
using SonoGame;
using UnityEngine;

public class AddMkitManagerAndVolumeManagerScript : MonoBehaviour
{
    void Awake()
    {
        //If this scene wasn't loaded from tutorial
        if (VolumeManager.Instance == null)
        {
            gameObject.AddComponent<VolumeManager>();
            gameObject.AddComponent<MKitManager>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
