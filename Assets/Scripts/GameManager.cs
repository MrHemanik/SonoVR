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
        SetWinningAnswerVolume();
        enabled = false; // will be re-enabled after generating artificials
        levelInformationScript.SetLevelInformation(levelList[currentLevelID].levelType);
        yield return volGenMan.GenerateLevel(levelList[currentLevelID], winningAnswerId, answerAnchors, compareAnchor);
        enabled = true;
        
        

        activeRound = true;
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