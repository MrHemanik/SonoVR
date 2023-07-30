using System.Collections;
using System.Collections.Generic;
using mKit;
using SonoGame;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    
    /// <summary>
    /// Position anchor for volume placement
    /// </summary>
    [Header("Scene")] public Transform[] volumeAnchors;

    /// <summary>
    /// MaterialConfig for different visualizations
    /// </summary>
    public MaterialConfig materialConfig;
    
    /// <summary>
    /// still 2D slice view (on Canvas with RawImage)
    /// </summary>
    public GameObject answerSliceView;


    private List<Level> levelList;
    private int currentLevel = 0;

    private int winningAnswerId;
    public bool activeRound = true;
    public LevelInformationScript levelInformationScript;
    public VolumeGenerationManager volGenMan;

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
        levelInformationScript.SetLevelInformation(levelList[currentLevel].levelType);
        yield return volGenMan.GenerateVolumesWithVolumeManager(levelList[currentLevel]);
        volGenMan.SetupVolumes();
        enabled = true;
        SetWinningAnswerVolume();
        yield return volGenMan.GetStillDefaultSlice(winningAnswerId, answerSliceView.GetComponent<RawImage>());
        activeRound = true;
    }

    private void ResetComponents()
    {
        //Resets volumeAnchor positions so new Volumes can be generated
        foreach (var volumeAnchor in volumeAnchors)
        {
            volGenMan.SetVisibility(volumeAnchor, false);
            Transform
                volumeBox = volumeAnchor.GetChild(0); //TODO: Will fail if object is currently grabbed, need to fix!
            volumeBox.position = volumeAnchor.position;
            volumeBox.rotation = volumeAnchor.rotation;
        }

        //Resets answerSliceBox to AnswerAnchor
        Transform answerSliceBox = answerSliceView.transform.parent.parent;
        answerSliceBox.position = answerSliceBox.parent.position;
        answerSliceBox.rotation = answerSliceBox.parent.rotation;

        Debug.Log("Loading level " + currentLevel + " with " + levelList[currentLevel].volumeList.Count + "Volumes");
    }

    //Sets the layer of every child to either the default or an invisible layer

    #endregion

    #region levelhandling

    internal void SetWinningAnswerVolume()
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