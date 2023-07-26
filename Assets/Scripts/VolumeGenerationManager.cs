using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SonoGame;
using mKit;

// uses Assets/DemoApp/Scripts/DemoApp1.cs as base
public class VolumeGenerationManager : MonoBehaviour
{
    private EVisualization visualization = EVisualization.Colored;
    private UltrasoundScannerTypeEnum scannerType = UltrasoundScannerTypeEnum.CURVED;

    /// <summary>
    /// Position anchor for volume placement
    /// </summary>
    [Header("Scene")] public Transform[] volumeAnchors;

    /// <summary>
    /// Probe-attached visual placeholder for the mKit slice
    /// </summary>
    public EmptySliceCopy sliceCopy;

    /// <summary>
    /// MaterialConfig for different visualizations
    /// </summary>
    public MaterialConfig materialConfig;

    /// <summary>
    /// 2D slice view (on Canvas with RawImage)
    /// </summary>
    public SliceView sliceViewRawImage;

    /// <summary>
    /// 2D slice view (on Unity quad mesh)
    /// </summary>
    public SliceView sliceViewQuad;

    /// <summary>
    /// 2D slice view (on Unity quad mesh)
    /// </summary>
    public GameObject stillSliceViewQuad;

    public GameObject stillSliceViewRawImage;

    private List<Level> levelList;

    private void Awake()
    {
        levelList = new LevelList(materialConfig).levelList;
    }

    IEnumerator Start()
    {
        Debug.Log(Application.dataPath);
        enabled = false; // will be re-enabled after generating artificials

        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        yield return GenerateVolumeWithVolumeManager();
        stillSliceViewRawImage.GetComponent<RawImage>().texture =
            VolumeManager.Instance.GetSliceCamCapture(Volume.Volumes[0]); //Adds still shot of volume 0 to stillView
    }

    void Update()
    {
        // make mKit slice follow sliceTransform
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopy.transform.position,
            sliceCopy.transform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0].SetToolSize(new Vector2(sliceCopy.transform.localScale.x, sliceCopy.transform.localScale.y));
    }

    #region GenerateVolumeWithVolumeManager

    IEnumerator GenerateVolumeWithVolumeManager()
    {
        yield return VolumeManager.Instance.GenerateArtificialVolume(levelList[0].volumeList[0], volumeSlot: 0,
            addObjectModels: true);
        yield return VolumeManager.Instance.GenerateArtificialVolume(levelList[0].volumeList[1], volumeSlot: 1,
            addObjectModels: true);
        Debug.Log("GenerateArtificialVolume finished");

        /*
         * position of volume needs to be set before configuration
         * Without:         https://i.imgur.com/4Su6Wyp.png
         * With:            https://i.imgur.com/iIdQK1p.png
         * Problem as GIF:  https://i.imgur.com/RKlqztO.gif
         */
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopy.transform.position,
            sliceCopy.transform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0].SetToolSize(new Vector2(sliceCopy.transform.localScale.x, sliceCopy.transform.localScale.y));

        for (int i = 0; i < Volume.Volumes.Count; i++)
        {
            ConfigureVolume(Volume.Volumes[i], scannerType, visualization, i);
            ConfigureSliceViews(Volume.Volumes[i], scannerType, visualization);
        }
        /*foreach (var volume in Volume.Volumes) //Initializes Visualization
        {
            ConfigureVolume(volume, scannerType, visualization, 0);
            ConfigureSliceViews(volume, scannerType, visualization);
        }*/

        enabled = true; // enable Update()
    }

    #endregion

    #region SliceViewConfiguration

    void ConfigureSliceViews(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization)
    {
        MultiVolumeSlice mvs = GetComponent<MultiVolumeSlice>();
        if (mvs == null)
        {
            sliceViewQuad.InitSliceView(visualization, scannerType,
                v.GetSliceRenderTexture()); // assign mkit texture to slice display
            sliceViewRawImage.InitSliceView(visualization, scannerType,
                v.GetSliceRenderTexture()); // assign mkit texture to slice display
        }
        else
        {
            // multi-volume-slice Material
            Material mvsMat = mvs.SetupMultiVolumeSlice(0, visualization, Volume.Volumes[0].GetToolSize(0));
            sliceViewRawImage.SetMaterial(mvsMat);
            sliceViewQuad.SetMaterial(mvsMat);
        }

        sliceCopy.SetSliceMask(scannerType);
    }

    #endregion

    #region VolumeConfiguration

    void ConfigureVolume(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization, int index)
    {
        v.SliceMaskingTexture = AppConfig.assets.GetScannerMask(scannerType);
        v.UseSliceMasking = scannerType != UltrasoundScannerTypeEnum.LINEAR;
        v.UltrasoundScannerType = scannerType;
        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);
        
        v.VolumeProxy.position = volumeAnchors[index].position; // set volume position
        GameObject.Find("mKitVolume #"+index+" (ArtificialVolume.vm2)").transform.SetParent(volumeAnchors[index]); //set volumeAnchor as parent of volume
        v.Threshold = 0.001f;
        Debug.Log(GameObject.Find("mKitVolume #0 (ArtificialVolume.vm2)").name);
    }

    #endregion
}