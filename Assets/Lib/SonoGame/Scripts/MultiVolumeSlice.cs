using mKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    public class MultiVolumeSlice : MonoBehaviour
    {
        public Material multiSliceMaterial;

        Volume trackedVolume;

        /// <summary>
        /// Enable blending of all volume slices.
        /// </summary>
        /// <param name="sliceSize"></param>
        /// <param name="visualization"></param>
        public virtual Material SetupMultiVolumeSlice(int trackedVolumeIndex, EVisualization visualization, Vector2 sliceSize)
        {
            trackedVolume = Volume.Volumes[trackedVolumeIndex];

            // configure mkit volumes
            for (int i = 0; i < Volume.Volumes.Count; i++)
            {
                Volume.Volumes[i].ActivateToolgroupSlice1(true);
                Volume.Volumes[i].SetToolSize(sliceSize);
                VolumeManager.Instance.SetVolumeOutlineState(Volume.Volumes[i], false);
                VolumeManager.Instance.UseMaterialConfigVisualization(Volume.Volumes[i], visualization);
            }

            // US GPU
            UltrasoundSimulation.Instance.Init(trackedVolume);
            UltrasoundSimulation.Instance.MultiVolumeSampling = true;

            // start slice syncing coroutine
            StartCoroutine(MultiVolumeSliceSync(trackedVolume.ToolTransform));

            return CreateMultiSliceMaterial();
        }

        protected virtual Material CreateMultiSliceMaterial()
        {
            // setup viewport
            Material multiSliceMaterialInstance = Instantiate(multiSliceMaterial);

            int currentIndex = 0;

            // find active slice textures
            for (int i = 0; i < Volume.Volumes.Count; i++)
            {
                if (Volume.Volumes[i] != null && Volume.Volumes[i].VolumeState == VolumeStateEnum.READY)
                {
                    multiSliceMaterialInstance.SetTexture("texArray_" + currentIndex++, Volume.Volumes[i].GetSliceRenderTexture());
                }
            }

            multiSliceMaterialInstance.SetInt("texCount", currentIndex);

            return multiSliceMaterialInstance;
        }

        /// <summary>
        /// Align every volume slice with one leading slice transform
        /// </summary>
        /// <param name="leadingSliceTransform"></param>
        /// <returns></returns>
        protected virtual IEnumerator MultiVolumeSliceSync(Transform leadingSliceTransform)
        {
            while (trackedVolume.VolumeState == VolumeStateEnum.READY)
            {
                for (int i = 0; i < Volume.Volumes.Count; i++)
                {
                    if (Volume.Volumes[i] != null && Volume.Volumes[i].VolumeState == VolumeStateEnum.READY)
                    {
                        Volume.Volumes[i].ToolTransform.SetPositionAndRotation(leadingSliceTransform.position, leadingSliceTransform.rotation);
                    }
                }

                yield return null;
            }
        }
    }

}
