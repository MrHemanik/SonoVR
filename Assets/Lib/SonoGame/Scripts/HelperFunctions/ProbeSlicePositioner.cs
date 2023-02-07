using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mKit;

namespace SonoGame
{
    //[ExecuteAlways]
    public class ProbeSlicePositioner : MonoBehaviour
    {
        public Transform sliceAnchor;
        public Transform sliceTransform;

        Volume volume;

        void Start()
        {
            UpdateSize(sliceTransform.localScale);
            enabled = false;
        }

        public void Init(Volume volume)
        {
            this.volume = volume;
            enabled = true;
        } 

        public void UpdateSize(Vector3 sliceSize)
        {
            var zPos = sliceAnchor.localPosition.z + sliceSize.y * -0.5f;
            sliceTransform.localPosition = new Vector3(0, 0, zPos);
            //Debug.Log("ProbeSlicePositioner: localPosition.z=" + zPos);
        }

        private void OnApplicationQuit()
        {
            enabled = false;
        }


    }

}