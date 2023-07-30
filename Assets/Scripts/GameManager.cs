using System.Collections;
using System.Collections.Generic;
using Classes;
using mKit;
using SonoGame;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Scripts")]
    public LevelInformationScript levelInformationScript;
    public VolumeGenerationManager volGenMan;
    
    /// <summary>
    /// MaterialConfig for different visualizations
    /// </summary>
    [Header("Fixed")] public MaterialConfig materialConfig;
    /// <summary>
    /// Position anchors for answer placement ObjectType
    /// </summary>
    [Header("Scene")] public Transform[] answerAnchors;
    /// <summary>
    /// Position anchor for compare placement ObjectType
    /// </summary>
    public Transform compareAnchor;
    
    private List<Level> levelList;
    private int currentLevelID;
    private int winningAnswerId;
    [Header("Open to other scripts")] public bool activeRound = true;
    

    #endregion

    #region Initiation

    private void Awake()
    {
        levelList = new LevelList(materialConfig).levelList;
    }

    private IEnumerator Start()
    {
        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        yield return InitLevel();
    }

    private IEnumerator InitLevel()
    {
        enabled = false; // will be re-enabled after generating artificials
        ResetComponents();
        levelInformationScript.SetLevelInformation(levelList[currentLevelID].levelType);
        yield return volGenMan.GenerateVolumesWithVolumeManager(levelList[currentLevelID]);
        volGenMan.SetupVolumes(answerAnchors);
        enabled = true;
        SetWinningAnswerVolume();
        if (levelList[currentLevelID].levelType.compareObject == ObjectType.Slice)
        {
            yield return volGenMan.GetStillDefaultSlice(winningAnswerId, answerAnchors[winningAnswerId],
                compareAnchor.GetComponentInChildren<RawImage>());
            volGenMan.SetVisibility(compareAnchor, true);
        }

        activeRound = true;
    }

    private void ResetComponents()
    {
        //Resets all grabbable boxes to their respective anchor
        Transform[] anchors = {answerAnchors[0],answerAnchors[1],answerAnchors[2],answerAnchors[3],compareAnchor};
        foreach (var anchor in anchors)
        {
            //TODO: Will fail if object is currently grabbed, need to fix!
            Transform volumeAnchor = anchor.GetChild(0);
            Transform sliceAnchor = anchor.GetChild(1);
            Transform volumeBoxGrabbable = volumeAnchor.GetChild(0);
            Transform sliceBoxGrabbable = sliceAnchor.GetChild(0);
            volGenMan.SetVisibility(anchor, false);
            volumeBoxGrabbable.position = volumeAnchor.position;
            volumeBoxGrabbable.rotation = volumeAnchor.rotation;
            sliceBoxGrabbable.position = sliceAnchor.position;
            sliceBoxGrabbable.rotation = sliceAnchor.rotation;
        }

        Debug.Log("Loading level " + currentLevelID + " with " + levelList[currentLevelID].volumeList.Count + "Volumes");
    }

    //Sets the layer of every child to either the default or an invisible layer

    #endregion

    #region levelhandling

    private void SetWinningAnswerVolume()
    {
        winningAnswerId = Random.Range(0, Volume.Volumes.Count);
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
        currentLevelID++;
        yield return new WaitForSeconds(1f);
        yield return InitLevel();
    }

    #endregion
}