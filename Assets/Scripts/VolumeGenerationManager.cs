using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using UnityEngine;
using UnityEngine.UI;
using SonoGame;
using mKit;
using UnityEngine.XR.Interaction.Toolkit;
using Object = System.Object;

// uses Assets/DemoApp/Scripts/DemoApp1.cs as base
public class VolumeGenerationManager : MonoBehaviour
{
    #region Variables

    private EVisualization visualization = EVisualization.Colored;
    private UltrasoundScannerTypeEnum scannerType = UltrasoundScannerTypeEnum.CURVED;
    public XRDirectInteractor leftController;
    /// <summary>
    /// Probe-attached visual placeholder for the mKit slice
    /// </summary>
    [Header("Scene")] public Transform sliceCopyTransform;

    /// <summary>
    /// 2D slice views (on Unity quad mesh) that shows current activeSlice
    /// </summary>
    public SliceView[] sliceViews;

    /// <summary>
    /// List of objects that get deleted when a new level is loaded
    /// </summary>
    private List<GameObject> temporaryObjects = new List<GameObject>();

    #endregion

    #region Initiation

    //Sets the layer of every child to either the default or an invisible layer
    internal void SetVisibility(Transform obj, bool visible)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = visible ? 0 : 3;
        }

        obj.gameObject.SetActive(visible);
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

    internal IEnumerator GenerateLevel(Level currentLevel, int winningAnswerID, Transform[] answerAnchors,
        Transform compareAnchor)
    {
        enabled = false; // will be re-enabled after generating artificials
        yield return ResetComponents(answerAnchors, compareAnchor);
        yield return GenerateVolumesWithVolumeManager(currentLevel, winningAnswerID);
        SetupVolumes(answerAnchors);
        enabled = true;
        Transform mKitVolume = answerAnchors[winningAnswerID].GetChild(0).GetChild(0).GetChild(1);
        Transform compareVolumeGrabBox = compareAnchor.GetChild(0).GetChild(0);
        Transform compareVolumeAnchor = compareAnchor.GetChild(0);
        List<Transform> mKitVolumeVisibleObjects = new List<Transform>();
        for (var i = 0; i < mKitVolume.childCount; i++)
        {
            mKitVolumeVisibleObjects.Add(mKitVolume.GetChild(i));
        }
        //Set
        if (currentLevel.levelType.compareObject == ObjectType.HiddenVolume ||
            currentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterglow)
        {
            foreach (var mKitVolumeVisibleObject in mKitVolumeVisibleObjects)
            {
                mKitVolumeVisibleObject.SetParent(mKitVolume.parent);
            }
            mKitVolume.SetParent(compareVolumeGrabBox);
            mKitVolume.Translate(compareVolumeAnchor.position - mKitVolume.position);
            mKitVolume.Rotate(Vector3.up,-90);
            SetVisibility(compareVolumeAnchor, true);
        }
        //Sets visible volume to compareAnchor while mKitVolume stays on answer
        if (currentLevel.levelType.compareObject == ObjectType.Volume)
        {
            Transform compareAnchorVisibleVolume =
                Instantiate(new GameObject("mKitVolumeVisibleObjects"), compareVolumeGrabBox).transform;
            temporaryObjects.Add(compareAnchorVisibleVolume.gameObject);
            foreach (var mKitVolumeVisibleObject in mKitVolumeVisibleObjects)
            {
                mKitVolumeVisibleObject.SetParent(compareAnchorVisibleVolume);
                mKitVolumeVisibleObject.Translate(compareVolumeAnchor.position - mKitVolume.position);
            }

            compareAnchorVisibleVolume.localRotation = compareAnchor.rotation;
            SetVisibility(compareVolumeAnchor, true);
            //mKitVolume.SetParent(compareVolumeGrabBox);
        }

        if (currentLevel.levelType.compareObject == ObjectType.Slice)
        {
            yield return GetStillDefaultSlice(winningAnswerID, answerAnchors[winningAnswerID].GetChild(1),
                compareAnchor.GetChild(1));
                
        }
    }

    private IEnumerator ResetComponents(Transform[] answerAnchors, Transform compareAnchor)
    {
        yield return leftController.allowSelect = false; //If there is an object currently grabbed it will cancel it.
        //Resets all grabbable boxes to their respective anchor
        Transform[] anchors = {answerAnchors[0], answerAnchors[1], answerAnchors[2], answerAnchors[3], compareAnchor};
        foreach (var anchor in anchors)
        {
            //TODO: Will fail if object is currently grabbed, need to fix!
            Transform volumeAnchor = anchor.GetChild(0);
            Transform sliceAnchor = anchor.GetChild(1);
            Transform volumeBoxGrabbable = volumeAnchor.GetChild(0);
            Transform sliceBoxGrabbable = sliceAnchor.GetChild(0);
            SetVisibility(volumeAnchor, false);
            SetVisibility(sliceAnchor, false);
            volumeBoxGrabbable.SetPositionAndRotation(volumeAnchor.position, volumeAnchor.rotation);
            sliceBoxGrabbable.SetPositionAndRotation(sliceAnchor.position, sliceAnchor.rotation);
        }


        //Delete temporary Objects
        foreach (var temporaryObject in temporaryObjects)
        {
            Destroy(temporaryObject);
        }

        yield return leftController.allowSelect = true;
    }

    internal IEnumerator GenerateVolumesWithVolumeManager(Level currentLevel, int winningAnswerID)
    {
        ObjectType ao = currentLevel.levelType.answerOptions;
        ObjectType co = currentLevel.levelType.compareObject;
        for (int i = 0; i < currentLevel.volumeList.Count; i++)
        {
            yield return VolumeManager.Instance.GenerateArtificialVolume(currentLevel.volumeList[i],
                volumeSlot: i,
                //Make Model visible if answerOptions are volumes or if compareObject is Volume and this is the winningAnswer
                addObjectModels: ao == ObjectType.Volume || co == ObjectType.Volume && winningAnswerID == i);
        }

        Debug.Log("GenerateArtificialVolume finished");
    }

    internal void SetupVolumes(Transform[] answerAnchors)
    {
        //position of volume toolgroup needs to be set before configuration. only for toolgroup 1 as it is used as multiview
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopyTransform.position,
            sliceCopyTransform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0]
            .SetToolSize(new Vector2(sliceCopyTransform.transform.localScale.x, sliceCopyTransform.localScale.y));

        //Configuration of Volumes
        for (int i = 0; i < Volume.Volumes.Count; i++)
        {
            ConfigureVolume(Volume.Volumes[i], scannerType, visualization, i, answerAnchors[i]);
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

    void ConfigureVolume(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization, int index,
        Transform answerAnchor)
    {
        v.SliceMaskingTexture = AppConfig.assets.GetScannerMask(scannerType);
        v.UseSliceMasking = scannerType != UltrasoundScannerTypeEnum.LINEAR;
        v.UltrasoundScannerType = scannerType;
        v.Threshold = 0.001f;
        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);
        Transform volumeAnchor = answerAnchor.GetChild(0);
        v.VolumeProxy.position = volumeAnchor.position; // set volume position
        SetVisibility(volumeAnchor, true); //Sets all elements of volumeanchor to visible
        //Only works the first time as it will be disabled on further level
        GameObject.Find("mKitVolume #" + index + " (ArtificialVolume.vm2)").transform
            .SetParent(volumeAnchor.GetChild(0)); //set volumeAnchor's grabbable box as parent of volume
    }

    /// <summary>
    /// Generates a texture from volume with id of volumeId in default position (centered, straight from the top directed at the bottom) and assigns it to the RawImage targetRawImage
    /// </summary>
    internal IEnumerator GetStillDefaultSlice(int volumeId, Transform sourceSliceAnchor, Transform targetSliceAnchor)
    {

        Transform sliceCopyParent = sliceCopyTransform.parent;
        foreach (var sliceView in sliceViews
        ) //temporarily sets volumes in multiVolume texture to 0 so nothing will get rendered
        {
            sliceView.GetComponent<MeshRenderer>().material
                .SetFloat("texCount",
                    0);
        }
        sliceCopyTransform.gameObject.layer = 3; //Make slice temporarily invisible so 
        Vector3 defaultPosition = sliceCopyTransform.localPosition;
        Quaternion defaultRotation = sliceCopyTransform.localRotation;
        sliceCopyTransform.SetPositionAndRotation(sourceSliceAnchor.position, Quaternion.identity);
        sliceCopyTransform.parent = null;
        SetVisibility(targetSliceAnchor,
            true); //Needs to be done before GetSliceCamCapture, as it will not work without being active
        yield return null; //Needs yield return or else the SetPositionAndRotation will be executed after the sliceCamCapture
        yield return targetSliceAnchor.GetComponentInChildren<RawImage>().texture =
            VolumeManager.Instance
                .GetSliceCamCapture(Volume.Volumes[volumeId]); //Adds still shot of volume of volumeID to stillView
        sliceCopyTransform.SetParent(sliceCopyParent);
        sliceCopyTransform.SetLocalPositionAndRotation(defaultPosition, defaultRotation);
        yield return null; //needs yield return or else it will be executed after the foreach
        sliceCopyTransform.gameObject.layer = 0;
        foreach (var sliceView in sliceViews
        ) //sets volumes in multiVolume texture back so everything will get rendered again
        {
            sliceView.GetComponent<MeshRenderer>().material
                .SetFloat("texCount",
                    Volume.Volumes
                        .Count);
        }
        
    }

    #endregion
}