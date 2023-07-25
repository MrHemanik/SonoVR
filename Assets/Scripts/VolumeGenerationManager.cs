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
    [Header ("Scene")]
    public Transform volumeAnchor;
    
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
    
    IEnumerator Start()
    {
        enabled = false; // will be re-enabled after generating artificials

        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        
        yield return GenerateVolumeWithVolumeManager();
    }
    
    void Update()
    {
        // make mKit slice follow sliceTransform
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopy.transform.position, sliceCopy.transform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0].SetToolSize(new Vector2(sliceCopy.transform.localScale.x, sliceCopy.transform.localScale.y));
    }
    #region ShapeDefinition
    List<ShapeConfig> DefineShapeList()
    {
        // define shaped with color from materialConfig.map[n].color

        List<ShapeConfig> shapeConfigs = new List<ShapeConfig>();
        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.ELIPSOID,
            color: materialConfig.map[1].color,
            edgeWidth: 20,
            size: new Vector3(80, 80, 80),
            center: new Vector3(40, 100, 100),
            rotation: Quaternion.identity));

        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.CUBOID,
            color: materialConfig.map[2].color,
            edgeWidth: 20,
            size: new Vector3(40, 40, 40),
            center: new Vector3(100, 100, 100),
            rotation: Quaternion.identity));

        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.TUBE_Y,
            color: materialConfig.map[3].color,
            edgeWidth: 20,
            size: new Vector3(80, 80, 80),
            center: new Vector3(160, 100, 100),
            rotation: Quaternion.Euler(0, 0, 90)));

        return shapeConfigs;
    }
    List<ShapeConfig> DefineShapeList2()
    {
        // define shaped with color from materialConfig.map[n].color

        List<ShapeConfig> shapeConfigs = new List<ShapeConfig>();
        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.ELIPSOID,
            color: materialConfig.map[1].color,
            edgeWidth: 20,
            size: new Vector3(80, 80, 60),
            center: new Vector3(40, 160, 100),
            rotation: Quaternion.identity));

        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.CUBOID,
            color: materialConfig.map[3].color,
            edgeWidth: 20,
            size: new Vector3(40, 20, 40),
            center: new Vector3(80, 170, 100),
            rotation: Quaternion.identity));

        shapeConfigs.Add(new ShapeConfigVoxel(ShapeType.TUBE_Y,
            color: materialConfig.map[2].color,
            edgeWidth: 20,
            size: new Vector3(80, 80, 80),
            center: new Vector3(160, 180, 100),
            rotation: Quaternion.Euler(0, 0, 90)));

        return shapeConfigs;
    }
    #endregion
    #region GenerateVolumeWithVolumeManager
    IEnumerator GenerateVolumeWithVolumeManager()
    {
        var shapeConfig1 = DefineShapeList();
        var shapeConfig2 = DefineShapeList2();

        yield return VolumeManager.Instance.GenerateArtificialVolume(shapeConfig1, volumeSlot: 0, addObjectModels: true);
        yield return VolumeManager.Instance.GenerateArtificialVolume(shapeConfig2, volumeSlot: 1, addObjectModels: true);
        Debug.Log("GenerateArtificialVolume finished");
        
        /*
         * position of volume needs to be set before configuration
         * Without:         https://i.imgur.com/4Su6Wyp.png
         * With:            https://i.imgur.com/iIdQK1p.png
         * Problem as GIF:  https://i.imgur.com/RKlqztO.gif
         */
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopy.transform.position, sliceCopy.transform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0].SetToolSize(new Vector2(sliceCopy.transform.localScale.x, sliceCopy.transform.localScale.y));
        
        ConfigureVolume(Volume.Volumes[0], scannerType, visualization);
        ConfigureVolume(Volume.Volumes[1], scannerType, visualization);
        ConfigureSliceViews(Volume.Volumes[0], scannerType, visualization);
        ConfigureSliceViews(Volume.Volumes[1], scannerType, visualization);
        
        enabled = true; // enable Update()
    }
    #endregion
    #region SliceViewConfiguration
    void ConfigureSliceViews(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization)
    {
        MultiVolumeSlice mvs = GetComponent<MultiVolumeSlice>();
        if (mvs == null)
        {
            sliceViewQuad.InitSliceView(visualization, scannerType, v.GetSliceRenderTexture()); // assign mkit texture to slice display
            sliceViewRawImage.InitSliceView(visualization, scannerType, v.GetSliceRenderTexture()); // assign mkit texture to slice display
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
    void ConfigureVolume(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization)
    {
        v.SliceMaskingTexture = AppConfig.assets.GetScannerMask(scannerType);
        v.UseSliceMasking = scannerType != UltrasoundScannerTypeEnum.LINEAR;
        v.UltrasoundScannerType = scannerType;

        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);

        v.VolumeProxy.position = volumeAnchor.position; // set volume position
        
        //replaces Volume rendering with models
        v.VolumeProxy.GetComponent<Renderer>().enabled = true; // enable volume rendering
        v.Threshold = 0.001f;
    }
    #endregion
}
