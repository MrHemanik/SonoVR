using UnityEngine;

namespace mKit
{

    public class MirrorVolume : MonoBehaviour
    {
        public Volume volume;
        public GameObject ocCube;

        /// <summary>
        /// Relative rotation removed when mirroring.
        /// </summary>
        public bool hideRotation = true;



        private void Start()
        {
          
            if (volume == null)
            {
                volume = Volume.Volumes[0];
            }

            if (ocCube == null)
            {
                ocCube = this.gameObject;
            }
        }

        public bool HideRotation
        {
            get { return hideRotation; }    
            set { hideRotation = value; }
        }

        private void Update()
        {
            if (volume != null && ocCube != null)
            {
                ocCube.transform.localRotation = hideRotation ? Quaternion.identity : volume.VolumeWorldSpaceRotation;
            }
        }

    }

}