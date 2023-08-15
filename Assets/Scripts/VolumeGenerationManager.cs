using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Classes;
using UnityEngine;
using UnityEngine.UI;
using SonoGame;
using mKit;

/// <summary>
/// Generates and configures Volumes and Slices
/// </summary>
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

    /// <summary>
    /// List of objects that get deleted when a new level is loaded
    /// </summary>
    internal List<GameObject> temporaryObjects = new List<GameObject>();

    #endregion

    #region Initiation

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
        Transform compareAnchor, Transform[] mKitVolumes, Transform[] answerVolumeBoxGrabbables)
    {
        enabled = false; // will be re-enabled after generating artificials
        ResetComponents(answerAnchors, compareAnchor, mKitVolumes);
        yield return GenerateVolumesWithVolumeManager(currentLevel, winningAnswerID, answerAnchors);
        SetupVolumes(answerAnchors, mKitVolumes, answerVolumeBoxGrabbables, currentLevel.volumeList.Count);
        enabled = true;

        //Move LevelElements to their respective Anchor/Box
        Transform winningMKitVolume = mKitVolumes[winningAnswerID];
        Debug.Log("Winning MKitVolume Name: " + winningMKitVolume.name);
        Transform compareVolumeAnchor = compareAnchor.GetChild(0);
        Transform compareVolumeGrabBox = compareVolumeAnchor.GetChild(1);
        List<Transform> mKitVolumeVisibleObjects = new List<Transform>();
        for (var i = 0; i < winningMKitVolume.childCount; i++)
        {
            mKitVolumeVisibleObjects.Add(winningMKitVolume.GetChild(i));
        }

        //Setting Up rest of AnswerObjects
        if (currentLevel.levelType.answerOptions == ObjectType.Slice)
        {
            //Makes stillSlices for every Answeroption -- needs to be done before any mkitVolumes move
            for (int i = 0; i < currentLevel.volumeList.Count; i++)
            {
                yield return GetStillDefaultSlice(answerAnchors[i].GetChild(1),
                    answerAnchors[i].GetChild(1));
            }
        }

        //Setting Up CompareObject
        if (currentLevel.levelType.compareObject == ObjectType.HiddenVolume ||
            currentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterimage)
        {
            MoveWinningMKitVolumeToCompareObject(false);
        }

        else if (currentLevel.levelType.compareObject == ObjectType.Volume)
        {
            if (currentLevel.levelType.answerOptions != ObjectType.Slice)
            {
                //Sets visible volume to compareAnchor while mKitVolume stays on answer
                Transform compareAnchorVisibleVolume =
                    Instantiate(new GameObject("mKitVolumeVisibleObjects"), compareVolumeGrabBox).transform;
                temporaryObjects.Add(compareAnchorVisibleVolume.gameObject);
                foreach (var mKitVolumeVisibleObject in mKitVolumeVisibleObjects)
                {
                    mKitVolumeVisibleObject.SetParent(compareAnchorVisibleVolume);
                }

                compareAnchorVisibleVolume.Translate(compareVolumeAnchor.position - winningMKitVolume.position);
                compareAnchorVisibleVolume.localRotation = compareAnchor.rotation;
                GameHelper.SetVisibility(compareVolumeAnchor, true);
            }
            else
            {
                MoveWinningMKitVolumeToCompareObject(true);
            }
        }

        else if (currentLevel.levelType.compareObject == ObjectType.Slice)
        {
            //Generate stillSlice for winningAnswer
            yield return GetStillDefaultSlice(answerAnchors[winningAnswerID].GetChild(1),
                compareAnchor.GetChild(1));
            for (int i = 0; i < currentLevel.volumeList.Count; i++)
            {
                mKitVolumes[i].localScale = Vector3.one;
            }
        }

        //Parents winning mKitVolumes possible generated objects to corresponding answerOptions GrabBox
        void MoveWinningMKitVolumeToCompareObject(bool withVisibleVolume)
        {
            //Detach childen of mKitVolumes from them
            for (int i = 0; i < currentLevel.volumeList.Count; i++)
            {
                var mKitVolume = mKitVolumes[i];
                mKitVolume.localScale = Vector3.one; //sets scale to 1, filling out the grabbable box
                if (withVisibleVolume && mKitVolume == winningMKitVolume) break; //doesn't detach from winMKitVolume
                int childCount = mKitVolume.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    temporaryObjects.Add(mKitVolume.GetChild(0).gameObject); //Deletes on new Level
                    mKitVolume.GetChild(0).SetParent(mKitVolume.parent); //Keeps visibleObject in answerOption
                }
            }

            //Set winning mKitVolume into compareObject
            winningMKitVolume.SetParent(compareVolumeGrabBox);
            winningMKitVolume.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            winningMKitVolume.localScale =
                Vector3.one; //Should be unnesessary as it should be done in the for loop above, but to be safe, as there are some bugs regarding scale
            //"Remove" every other mKitVolume
            for (int i = 0; i < currentLevel.volumeList.Count; i++)
            {
                if (i != winningAnswerID)
                    mKitVolumes[i].position -= new Vector3(0, 100, 0);
            }

            GameHelper.SetVisibility(compareVolumeAnchor, true);
        }
    }


    internal IEnumerator GenerateVolumesWithVolumeManager(Level currentLevel, int winningAnswerID,
        Transform[] answerAnchors)
    {
        ObjectType ao = currentLevel.levelType.answerOptions;
        ObjectType co = currentLevel.levelType.compareObject;
        for (int i = 0; i < currentLevel.volumeList.Count; i++)
        {
            yield return VolumeManager.Instance.GenerateArtificialVolume(currentLevel.volumeList[i],
                volumeSlot: i,
                //Make Model visible if answerOptions are volumes or if compareObject is Volume and this is the winningAnswer
                addObjectModels: ao == ObjectType.Volume || co == ObjectType.Volume && winningAnswerID == i);
            if (ao != ObjectType.Slice)
                GameHelper.SetVisibility(answerAnchors[i].GetChild(0),
                    true); //Sets all elements of volumeanchor to visible
        }

        Debug.Log("GenerateArtificialVolume finished");
    }

    internal void SetupVolumes(Transform[] answerAnchors, Transform[] mKitVolumes,
        Transform[] answerVolumeBoxGrabbables, int volumeCount)
    {
        //position of volume toolgroup needs to be set before configuration. only for toolgroup 1 as it is used as multiview
        Volume.Volumes[0].ToolTransform.SetPositionAndRotation(sliceCopyTransform.position,
            sliceCopyTransform.rotation * Quaternion.Euler(-90, 0, 0));
        Volume.Volumes[0]
            .SetToolSize(new Vector2(sliceCopyTransform.transform.localScale.x, sliceCopyTransform.localScale.y));

        //Configuration of Volumes
        for (int i = 0; i < volumeCount; i++)
        {
            ConfigureVolume(Volume.Volumes[i], scannerType, visualization, answerAnchors[i], mKitVolumes[i],
                answerVolumeBoxGrabbables[i]);
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

    void ConfigureVolume(Volume v, UltrasoundScannerTypeEnum scannerType, EVisualization visualization,
        Transform answerAnchor, Transform mKitVolume, Transform answerVolumeBoxGrabbable)
    {
        v.SliceMaskingTexture = AppConfig.assets.GetScannerMask(scannerType);
        v.UseSliceMasking = scannerType != UltrasoundScannerTypeEnum.LINEAR;
        v.UltrasoundScannerType = scannerType;
        v.Threshold = 0.001f;
        VolumeManager.Instance.UseMaterialConfigVisualization(v, visualization);
        UltrasoundSimulation.Instance.Init(v);
        Transform volumeAnchor = answerAnchor.GetChild(0);
        v.VolumeProxy.position = volumeAnchor.position; // set volume position
        mKitVolume.transform
            .SetParent(answerVolumeBoxGrabbable); //set volumeAnchor's grabbable box as parent of volume
    }

    /// <summary>
    /// Generates a texture from volume with id of volumeId in default position (centered, straight from the top directed at the bottom) and assigns it to the RawImage targetRawImage
    /// </summary>
    internal IEnumerator GetStillDefaultSlice(Transform sourceSliceAnchor, Transform targetSliceAnchor)
    {
        Transform sliceCopyParent = sliceCopyTransform.parent;
        var targetSliceGrabBox = targetSliceAnchor.GetChild(1);
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
        GameHelper.SetVisibility(targetSliceAnchor,
            true); //Needs to be done before GetSliceCamCapture, as it will not work without being active
        yield return
            null; //Needs yield return or else the SetPositionAndRotation will be executed after the sliceCamCapture
        yield return targetSliceGrabBox.GetChild(0).GetChild(0).GetComponent<RawImage>().texture =
            VolumeManager.Instance.GetSliceCamCapture(Volume.Volumes[
                sourceSliceAnchor.GetComponentInChildren<InteractableInformation>().answerId -
                1]); //Adds still shot of volume of volumeID to stillView
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

    private void ResetComponents(Transform[] answerAnchors, Transform compareAnchor, Transform[] mKitVolumes)
    {
        //Delete temporary Objects
        foreach (var temporaryObject in temporaryObjects)
        {
            Destroy(temporaryObject);
        }

        //Volume.UnloadAllVolumes would remove them all from the mKitVolumes array, for now they'll just be moved out of the way
        foreach (var vol in mKitVolumes)
        {
            vol.parent = null;
            vol.position = new Vector3(0, -100, 0);
        }

        //Resets all grabbable boxes to their respective anchor
        Transform[] anchors = {answerAnchors[0], answerAnchors[1], answerAnchors[2], answerAnchors[3], compareAnchor};
        foreach (var anchor in anchors)
        {
            Transform volumeAnchor = anchor.GetChild(0);
            Transform sliceAnchor = anchor.GetChild(1);
            Transform volumeBoxGrabbable = volumeAnchor.GetChild(1);
            Transform sliceBoxGrabbable = sliceAnchor.GetChild(1);
            if (volumeBoxGrabbable.childCount >= 2) //Detach mKitVolume is connected 
                volumeBoxGrabbable.GetChild(1).parent = null;
            GameHelper.SetVisibility(volumeAnchor, false);
            GameHelper.SetVisibility(sliceAnchor, false);
            volumeBoxGrabbable.SetPositionAndRotation(volumeAnchor.position, volumeAnchor.rotation);
            sliceBoxGrabbable.SetPositionAndRotation(sliceAnchor.position, sliceAnchor.rotation);
        }
    }
}