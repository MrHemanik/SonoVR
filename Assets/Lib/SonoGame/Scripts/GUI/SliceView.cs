using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mKit;

namespace SonoGame
{
    public class SliceView : MonoBehaviour
    {
        [Header("Runtime settings")]
        public EVisualization visualization;

        [Header ("Settings")]
        [Tooltip("For color mode, automatically tint slice background")]
        public bool autoColorBg = true;

        RawImage rawImage;
        Renderer meshRenderer;

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
            if (rawImage == null)
                meshRenderer = GetComponent<Renderer>();
        }

        public void InitSliceView(EVisualization visType, UltrasoundScannerTypeEnum scannerType, Texture texture)
        {
            Material mat = null;

            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    mat = Instantiate(AppConfig.assets.usViewRawImageLinearMaterial);
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    mat = Instantiate(AppConfig.assets.usViewRawImageSectorMaterial);
                    break;

                default:

                    mat = Instantiate(AppConfig.assets.usViewRawImageCurvedMaterial);
                    break;
            }

            if (mat!=null)
            {
                InitVisualizationType(visType, mat);
                mat.SetTexture("_MainTex", texture);

                if (rawImage!=null)
                {
                    rawImage.texture = null;
                    rawImage.material = mat;
                    rawImage.texture = texture;
                }
                else if (meshRenderer != null)
                {
                    meshRenderer.material = mat;
                }
                else
                {
                    Debug.LogError("Neither RawImage nor Renderer on this gameobect!");
                }
            }

        }

        /// <summary>
        /// Init slice view for the visualization type
        /// </summary>
        public virtual void InitVisualizationType(EVisualization visType, Material mat)
        {
            //Debug.Log("InitVisualizationType" + visType);
            this.visualization = visType;
            bool bgColoring = false;

            switch (visType)
            {
                case EVisualization.Colored:
                case EVisualization.Gray:
                    bgColoring = true;
                    break;

                case EVisualization.Ultrasound:
                case EVisualization.Anatomic:
                    bgColoring = false;
                    break;
            }

            mat.SetInt("bgColoring", autoColorBg && bgColoring ? 1 : 0);
        }

    }

}