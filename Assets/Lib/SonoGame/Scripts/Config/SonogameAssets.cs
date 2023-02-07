using System;
using UnityEngine;
using mKit;

namespace SonoGame
{
    [CreateAssetMenu(fileName = "SonogameAssets", menuName = "ScriptableObjects/SonogameAssets", order = 1)]
    [Serializable]
    public class SonogameAssets : ScriptableObject
    {
        [Header("Materials")]
        public Material usViewRawImageLinearMaterial;
        public Material usViewRawImageCurvedMaterial;
        public Material usViewRawImageSectorMaterial;

        [Header("Masks")]
        public Texture2D ultrasoundMaskCurved;
        public Texture2D ultrasoundMaskSector;

        [Header("Mask lines")]
        public Texture2D curvedLineOverlay;
        public Texture2D linearLineOverLay;
        public Texture2D sectorLineOverlay;

        [Header("Prefabs")]
        public GameObject hideSpherePrefab;
        public GameObject hideCubePrefab;
        public GameObject hideCylinder;
        public GameObject tubePrefab;

        [Header("Artificials")]
        public Material[] artificialMat;

        public Texture2D GetScannerMask(UltrasoundScannerTypeEnum scannerType)
        {
            Texture2D result = null;

            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    result = null;
                    break;
                case UltrasoundScannerTypeEnum.CURVED:
                    result = ultrasoundMaskCurved;
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    result = ultrasoundMaskSector;
                    break;
            }

            return result;
        }

        public Texture2D GetSliceLineOverlay(UltrasoundScannerTypeEnum scannerType)
        {
            Texture2D result = null;

            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    result = linearLineOverLay;
                    break;
                case UltrasoundScannerTypeEnum.CURVED:
                    result = curvedLineOverlay;
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    result = sectorLineOverlay;
                    break;
            }

            return result;
        }
    }

}