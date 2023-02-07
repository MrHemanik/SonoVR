using mKit;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Component for the Sonogame "EmptySliceCopy" prefab.
    /// 
    /// Usage: var sliceCopy = Instantiate(AppManager.Instance.sliceCopy).GetComponent<EmptySliceCopy>();
    ///        sliceCopy.SetVolume(Volume.Volumes[0]);
    /// </summary>
    public class EmptySliceCopy : MonoBehaviour
    {
        [Header("Runtime settings")]
        public int volumeId;
        public bool showDefaultSlice = false;
        public Color backgroundColor;

        [Header("Settings")]
        private Volume volume;
        private int sliceIndex;
        private Texture2D currentOverlay;
        private Renderer rend;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            rend.material.mainTexture = Texture2D.blackTexture;
        }

        private void Start()
        {
            SetSliceMask(UltrasoundScannerTypeEnum.CURVED);
            ResetSliceBackgroundColor();
        }

        public void SetSliceMask(UltrasoundScannerTypeEnum scannerType)
        {
            currentOverlay = AppConfig.assets.GetSliceLineOverlay(scannerType);
            SetOverlay(currentOverlay);

            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    SetMask(Texture2D.whiteTexture);
                    break;
                case UltrasoundScannerTypeEnum.CURVED:
                    SetMask(AppConfig.assets.ultrasoundMaskCurved);
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    SetMask(AppConfig.assets.ultrasoundMaskSector);
                    break;
            }
        }

        /// <summary>
        /// Init slice for volume instance.
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(Volume volume, int sliceIndex = 0)
        {
            this.volume = volume;
            volumeId = volume.ID;
            this.sliceIndex = sliceIndex;

            SetToolSize(volume.GetToolSize(sliceIndex));
        }

        /// <summary>
        /// Adjust slice size
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        public void SetToolSize(Vector2 size)
        {
            transform.localScale = new Vector3(size.x, size.y, 1);
            transform.localScale = new Vector3(size.x, size.y, 1);
        }

        public void SetSliceBackgroundColor(Color bgColor)
        {
            rend.material.SetColor("_BgColor", bgColor);
        }

        public void SetMask(Texture2D maskTex)
        {
            rend.material.SetTexture("uMaskTex", maskTex);
        }

        public void SetOverlay(Texture2D overlayTex)
        {
            rend.material.SetTexture("uOverlayTex", overlayTex);
        }

        /// <summary>
        /// ShowSlice: In disabled state, the slice shows only the configured background color.
        /// </summary>
        /// <param name="state">true: enable mainTexture, false: enable null mask</param>
        public void ShowSlice(bool state)
        {
            SetOverlay(state ? currentOverlay : Texture2D.blackTexture);
        }

        public void ResetSliceBackgroundColor()
        {
            backgroundColor = AppConfig.current.uiColorConfig.GetAppColorByName(InterfaceColor.SliceBackground);
            SetSliceBackgroundColor(backgroundColor);
        }

        private void Update()
        {
            if (volume != null)
            {
                SetToolSize(volume.GetToolSize(sliceIndex));
            }
        }
    }

}