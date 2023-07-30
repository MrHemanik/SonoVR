using System.Collections;
using System.Collections.Generic;
using Classes;
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
    /// Probe-attached visual placeholder for the mKit slice
    /// </summary>
    [Header("Scene")] public Transform sliceCopyTransform;

    /// <summary>
    /// 2D slice views (on Unity quad mesh) that shows current activeSlice
    /// </summary>
    public SliceView[] sliceViews;

    #endregion

    #region Initiation

    //Sets the layer of every child to either the default or an invisible layer
    internal void SetVisibility(Transform obj, bool visible)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = visible ? 0 : 3;

            //TODO: Later change to variation of:
            //trans.gameObject.SetActive(visible);
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

    internal IEnumerator GenerateLevel(Level currentLevel, int winningAnswerID, Transform[] answerAnchors,
        Transform compareAnchor)
    {
        yield return GenerateVolumesWithVolumeManager(currentLevel, winningAnswerID);
        SetupVolumes(answerAnchors);
        //Sets visible volume to compareAnchor while mKitVolume stays on answer
        if (currentLevel.levelType.compareObject == ObjectType.Volume)
        {
            Transform mKitVolume = answerAnchors[winningAnswerID].GetChild(0).GetChild(0).GetChild(1);
            Transform compareVolumeGrabBox = compareAnchor.GetChild(0).GetChild(0);
            List<Transform> mKitVolumeVisibleObjects = new List<Transform>();
            for (var i = 0; i < mKitVolume.childCount; i++)
            {
                mKitVolumeVisibleObjects.Add(mKitVolume.GetChild(i));
            }
            //TODO: Needs to be deleted on Level change!
            Transform compareAnchorVisibleVolume = Instantiate(new GameObject("mKitVolumeVisibleObjects"), compareVolumeGrabBox).transform;
            foreach (var mKitVolumeVisibleObject in mKitVolumeVisibleObjects)
            {
                mKitVolumeVisibleObject.SetParent(compareAnchorVisibleVolume);
                mKitVolumeVisibleObject.Translate(compareAnchor.GetChild(0).position- mKitVolume.position);
            }
            compareAnchorVisibleVolume.localRotation= compareAnchor.rotation;
            //mKitVolume.SetParent(compareVolumeGrabBox);
        }
        
        if (currentLevel.levelType.compareObject == ObjectType.Slice)
        {
            yield return GetStillDefaultSlice(winningAnswerID, answerAnchors[winningAnswerID],
                compareAnchor.GetComponentInChildren<RawImage>());
            SetVisibility(compareAnchor, true);
        }
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
        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);
        Transform volumeAnchor = answerAnchor.GetChild(0);
        v.VolumeProxy.position = volumeAnchor.position; // set volume position
        GameObject.Find("mKitVolume #" + index + " (ArtificialVolume.vm2)").transform
            .SetParent(volumeAnchor.GetChild(0)); //set volumeAnchor's grabbable box as parent of volume
        SetVisibility(volumeAnchor, true); //Sets all elements of volumeanchor to visible
        v.Threshold = 0.001f;
    }

    /// <summary>
    /// Generates a texture from volume with id of volumeId in default position (centered, straight from the top directed at the bottom) and assigns it to the RawImage targetRawImage
    /// </summary>
    internal IEnumerator GetStillDefaultSlice(int volumeId, Transform answerAnchor, RawImage targetRawImage)
    {
        Transform sliceAnchorTransform = sliceCopyTransform.parent.GetChild(3).transform;
        foreach (var sliceView in sliceViews)
        {
            sliceView.GetComponent<MeshRenderer>().material
                .SetFloat("texCount",
                    0); //temporarily sets volumes in multiVolume texture to 0 so nothing will get rendered
        }

        sliceCopyTransform.gameObject.layer = 3; //Make slice temporarily invisible so 
        Vector3 defaultPosition = sliceAnchorTransform.localPosition;
        Quaternion defaultRotation = sliceAnchorTransform.localRotation;
        yield return sliceAnchorTransform.position = answerAnchor.GetChild(0).position;
        yield return sliceAnchorTransform.rotation = Quaternion.identity;
        yield return targetRawImage.texture =
            VolumeManager.Instance
                .GetSliceCamCapture(Volume.Volumes[volumeId]); //Adds still shot of volume of volumeID to stillView
        yield return sliceAnchorTransform.localPosition = defaultPosition;
        yield return sliceAnchorTransform.localRotation = defaultRotation;
        sliceCopyTransform.gameObject.layer = 0;
        foreach (var sliceView in sliceViews)
        {
            sliceView.GetComponent<MeshRenderer>().material
                .SetFloat("texCount",
                    Volume.Volumes
                        .Count); //temporarily sets volumes in multiVolume texture to 0 so nothing will get rendered
        }

        //return null;
    }

    #endregion
}