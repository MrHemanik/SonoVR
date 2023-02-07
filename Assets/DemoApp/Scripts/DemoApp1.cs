using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonoGame;
using mKit;

public class DemoApp1 : MonoBehaviour
{
    [Header("Settings")]
    public EVisualization visualization;
    public UltrasoundScannerTypeEnum scannerType;
    public bool replaceVolumeRenderingWithModels = true;

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

    private void Awake()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    IEnumerator Start()
    {
        enabled = false; // will be re-enabled after generating artificials

        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        
        yield return GenerateVolumeWithVolumeManager();
        yield return CycleVisualizations(); 
    }

    public void Update()
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
    #endregion

    #region GenerateVolumeWithVolumeManager
    IEnumerator GenerateVolumeWithVolumeManager()
    {
        var shapeConfigs = DefineShapeList();

        yield return VolumeManager.Instance.GenerateArtificialVolume(shapeConfigs, volumeSlot:0, addObjectModels: replaceVolumeRenderingWithModels);
        Debug.Log("GenerateArtificialVolume finished");

        ConfigureVolume(Volume.Volumes[0], scannerType, visualization);
        ConfigureSliceViews(Volume.Volumes[0], scannerType, visualization);

        enabled = true; // enable Update()
    }
    #endregion


    #region SliceViewConfiguration
    void ConfigureSliceViews(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization)
    {
        sliceViewQuad.InitSliceView(visualization, scannerType, v.GetSliceRenderTexture()); // assign mkit texture to slice display
        sliceViewRawImage.InitSliceView(visualization, scannerType, v.GetSliceRenderTexture()); // assign mkit texture to slice display
        
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

        Volume.Volumes[0].VolumeProxy.position = volumeAnchor.position; // set volume position

        if (!replaceVolumeRenderingWithModels)
        {
            Volume.Volumes[0].VolumeProxy.GetComponent<Renderer>().enabled = true; // enable volume rendering
            Volume.Volumes[0].Threshold = 0.001f;
        }
    }
    #endregion

    #region CycleVisualizations
    IEnumerator CycleVisualizations()
    {
        // cycle through visualizations (demo)
        while (true)
        {
            ConfigureVolume(Volume.Volumes[0], scannerType, EVisualization.Colored);
            ConfigureSliceViews(Volume.Volumes[0], scannerType, EVisualization.Colored);
            yield return new WaitForSeconds(2.0f);

            ConfigureVolume(Volume.Volumes[0], scannerType, EVisualization.Gray);
            ConfigureSliceViews(Volume.Volumes[0], scannerType, EVisualization.Gray);
            yield return new WaitForSeconds(2.0f);

            ConfigureVolume(Volume.Volumes[0], scannerType, EVisualization.Ultrasound);
            ConfigureSliceViews(Volume.Volumes[0], scannerType, EVisualization.Ultrasound);
            yield return new WaitForSeconds(2.0f);
        }
    }
    #endregion


    #region GenerateArtifical_mKit
    IEnumerator GenerateVolumeWithMKit()
    {
        var shapeConfigs = // DefineShapeList();
        materialConfig.GetRenderShapeConfigList(DefineShapeList());

        Vm2Header vm2header = new Vm2Header(Vm2.FormatEnum.CT8, dataDim: new Vector3(200, 200, 200), millimeterDim: new Vector3(150, 150, 150), 1);
        Volume volume = Volume.AddGeneratedShapesGPU("mKitVolume.vm2", vm2header, shapeConfigs);

        while (volume.VolumeState != VolumeStateEnum.READY)
            yield return null;

        volume.VolumeTextureFilterMode = FilterMode.Point;
        VolumeManager.Instance.UseMaterialConfigVisualization(volume, EVisualization.Colored);
        replaceVolumeRenderingWithModels = false;

        ConfigureVolume(volume, scannerType, visualization);
        ConfigureSliceViews(volume, scannerType, visualization);

        enabled = true; // enable Update()
    }

    #endregion
}
