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
    #region Variables

    private EVisualization visualization = EVisualization.Colored;
    private UltrasoundScannerTypeEnum scannerType = UltrasoundScannerTypeEnum.CURVED;

    /// <summary>
    /// Position anchor for volume placement
    /// </summary>
    [Header("Scene")] public Transform[] volumeAnchors;

    public AnswerBoxManager abm;

    /// <summary>
    /// Probe-attached visual placeholder for the mKit slice
    /// </summary>
    public Transform sliceCopyTransform;

    /// <summary>
    /// MaterialConfig for different visualizations
    /// </summary>
    public MaterialConfig materialConfig;
    
    /// <summary>
    /// 2D slice view (on Unity quad mesh)
    /// </summary>
    public SliceView[] sliceViews;

    /// <summary>
    /// still 2D slice view (on Canvas with RawImage)
    /// </summary>
    public GameObject answerSliceView;


    private List<Level> levelList;
    private int currentLevel = 0;

    private int winningAnswerId;

    public bool activeRound = true;
    #endregion

    #region Initiation

    private void Awake()
    {
        levelList = new LevelList(materialConfig).levelList;
    }

    IEnumerator Start()
    {
        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        yield return InitLevel();
    }

    IEnumerator InitLevel()
    {
        enabled = false; // will be re-enabled after generating artificials
        ResetComponents();
        yield return GenerateVolumesWithVolumeManager();
        SetupVolumes();
        enabled = true;
        SetWinningAnswerVolume();
        yield return GetStillDefaultSlice(winningAnswerId, answerSliceView.GetComponent<RawImage>());
        activeRound = true;
    }

    private void ResetComponents()
    {
        //Resets volumeAnchor positions so new Volumes can be generated
        foreach (var volumeAnchor in volumeAnchors)
        {
            SetVisibility(volumeAnchor, false);
            Transform volumeBox = volumeAnchor.GetChild(0);
            volumeBox.position = volumeAnchor.position;
            volumeBox.rotation = volumeAnchor.rotation;
        }

        //Resets answerSliceBox to AnswerAnchor
        Transform answerSliceBox = answerSliceView.transform.parent.parent;
        answerSliceBox.position = answerSliceBox.parent.position;
        answerSliceBox.rotation = answerSliceBox.parent.rotation;

        //Resets answerBox
        Debug.Log("Loading level " + currentLevel + " with " + levelList[currentLevel].volumeList.Count + "Volumes");
        abm.InitAnswerBox(levelList[currentLevel].volumeList.Count);
    }
    //Sets the layer of every child to either the default or an invisible layer
    private void SetVisibility(Transform obj, bool visible)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
                trans.gameObject.layer = visible ? 0 : 3;
        }
    }

    #endregion

    void Update()
    {
        // make mKit slice follow sliceTransform
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopyTransform.position,
            sliceCopyTransform.rotation * Quaternion.Euler(-90, 0, 0));
        //Volume.Volumes[0].SetToolSize(new Vector2(sliceCopyTransform.transform.localScale.x, sliceCopyTransform.transform.localScale.y));
    }

    #region Generation & Configuration of Volume and Slice

    IEnumerator GenerateVolumesWithVolumeManager()
    {
        for (int i = 0; i < levelList[currentLevel].volumeList.Count; i++)
        {
            yield return VolumeManager.Instance.GenerateArtificialVolume(levelList[currentLevel].volumeList[i],
                volumeSlot: i,
                addObjectModels: true);
        }
        Debug.Log("GenerateArtificialVolume finished");
    }

    void SetupVolumes()
    {
        //position of volume toolgroup needs to be set before configuration. only for toolgroup 1 as it is used as multiview
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopyTransform.position,
            sliceCopyTransform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0].SetToolSize(new Vector2(sliceCopyTransform.transform.localScale.x, sliceCopyTransform.localScale.y));
        for (int i = 0; i < Volume.Volumes.Count; i++)
        {
            ConfigureVolume(Volume.Volumes[i], scannerType, visualization, i);
            ConfigureSliceViews(Volume.Volumes[i], scannerType, visualization);

            //Bugfix: problem where render is flickering, Gets temporarily fixed when clicking on OsCamera, even when it is inactive at the time. Changing the CameraType also works
            Volume.Volumes[i].GetToolgroupCamera(0).cameraType = CameraType.Preview;
        }
    }
    void ConfigureSliceViews(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization)
    {
        MultiVolumeSlice mvs = GetComponent<MultiVolumeSlice>();
        if (mvs == null)
        {
            foreach (var sliceView in sliceViews)
            {
                sliceView.InitSliceView(visualization, scannerType,
                    v.GetSliceRenderTexture()); // assign mkit texture to slice display
            }
        }
        else
        {
            // multi-volume-slice Material
            Material mvsMat = mvs.SetupMultiVolumeSlice(0, visualization, Volume.Volumes[0].GetToolSize(0));
            foreach (var sliceView in sliceViews)
            {
                sliceView.SetMaterial(mvsMat);
            }
        }

        //sliceCopyTransform.SetSliceMask(scannerType);
    }

    void ConfigureVolume(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization, int index)
    {
        v.SliceMaskingTexture = AppConfig.assets.GetScannerMask(scannerType);
        v.UseSliceMasking = scannerType != UltrasoundScannerTypeEnum.LINEAR;
        v.UltrasoundScannerType = scannerType;
        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);

        v.VolumeProxy.position = volumeAnchors[index].position; // set volume position
        GameObject.Find("mKitVolume #" + index + " (ArtificialVolume.vm2)").transform
            .SetParent(volumeAnchors[index].GetChild(0)); //set volumeAnchor's grabbable box as parent of volume
        SetVisibility(volumeAnchors[index], true); //Sets all elements of volumeanchor to visible
        v.Threshold = 0.001f;
    }

    /// <summary>
    /// Generates a texture from volume with id of volumeId in default position (centered, straight from the top directed at the bottom) and assigns it to the RawImage image
    /// </summary>
    IEnumerator GetStillDefaultSlice(int volumeId, RawImage image)
    {
        
        Transform sliceAnchorTransform = sliceCopyTransform.parent.GetChild(3).transform;
        foreach (var sliceView in sliceViews)
        {
            sliceView.GetComponent<MeshRenderer>().material.SetFloat("texCount",0); //temporarily sets volumes in multiVolume texture to 0 so nothing will get rendered
        }
        sliceCopyTransform.gameObject.layer = 3; //Make slice temporarily invisible so 
        Vector3 defaultPosition = sliceAnchorTransform.localPosition;
        Quaternion defaultRotation = sliceAnchorTransform.localRotation;
        yield return sliceAnchorTransform.position =
            GameObject.Find("VolumeAnchor (Volume" + (volumeId + 1) + ")").transform.position;
        yield return sliceAnchorTransform.rotation = Quaternion.identity;
        yield return image.texture =
            VolumeManager.Instance
                .GetSliceCamCapture(Volume.Volumes[volumeId]); //Adds still shot of volume of volumeID to stillView
        yield return sliceAnchorTransform.localPosition = defaultPosition;
        yield return sliceAnchorTransform.localRotation = defaultRotation;
        sliceCopyTransform.gameObject.layer = 0;
        foreach (var sliceView in sliceViews)
        {
            sliceView.GetComponent<MeshRenderer>().material.SetFloat("texCount",Volume.Volumes.Count); //temporarily sets volumes in multiVolume texture to 0 so nothing will get rendered
        }
        //return null;
    }

    #endregion

    #region levelhandling

    private void SetWinningAnswerVolume()
    {
        winningAnswerId = UnityEngine.Random.Range(0, Volume.Volumes.Count);
        Debug.Log(winningAnswerId);
    }

    public void CheckAnswer(int answerID)
    {
        activeRound = false;
        StartCoroutine(EndLevel(winningAnswerId == answerID));
    }

    private IEnumerator EndLevel(bool winning)
    {
        Debug.Log(winning ? "Richtige Antwort abgegeben!" : "Falsche Antwort abgegeben!");
        currentLevel++;
        yield return new WaitForSeconds(1f);
        yield return InitLevel();
    }

    #endregion
}