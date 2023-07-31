using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Classes;
using mKit;
using SonoGame;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Scripts")] public LevelInformationScript levelInformationScript;
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
    [Header("Scene")] public Transform compareAnchor;

    private List<Level> levelList;
    private int currentLevelID;
    private int winningAnswerId;
    [Header("Open to other scripts")] public bool activeRound = true;

    public GameObject afterglowPrefab;
    private IEnumerator afterglowCoroutine;

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
        var currentLevel = levelList[currentLevelID];
        SetWinningAnswerVolume();
        enabled = false; // will be re-enabled after generating artificials
        levelInformationScript.SetLevelInformation(currentLevel.levelType);
        yield return volGenMan.GenerateLevel(currentLevel, winningAnswerId, answerAnchors, compareAnchor);
        yield return null; //If yield return null it waits until generateLevel is fully finished //TODO: Rework
        if (currentLevel.levelType.answerOptions == ObjectType.HiddenVolumeAfterglow ||
            currentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterglow)
            StartCoroutine(HiddenVolumeAfterglow());
        enabled = true;
        //TODO Start Timer
        activeRound = true;
    }


    //Sets the layer of every child to either the default or an invisible layer

    #endregion

    #region Level handling

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
        StopCoroutine(afterglowCoroutine);
        //TODO: Stop Timer
        currentLevelID++;
        yield return new WaitForSeconds(1f); //TODO: Replace with some sort of check for the player
        yield return InitLevel();
    }
    #endregion
    
    #region LevelType Custom Functions

    private IEnumerator HiddenVolumeAfterglow()
    {
        while (true)
        {
            yield return CreateAfterglowStill();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CreateAfterglowStill()
    {
        //Copies current Texture
        List<Texture> sliceTextures = new List<Texture>();
        for (int i = 0; i < Volume.Volumes.Count; i++)
        {
            //TODO Change everything to be assigned here, copytexture overwrites instead of merging, so needs rework
            sliceTextures.Add(volGenMan.sliceViews[0].GetComponent<MeshRenderer>().material.GetTexture("texArray_"+i));
            if(i > 0) Graphics.CopyTexture(sliceTextures[i], sliceTextures[0]);
        }
        Texture2D texture = new Texture2D(sliceTextures[0].width, sliceTextures[0].height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(sliceTextures[0], texture);
        
        //Create afterglowPrefab with that texture
        GameObject movingScanArea = GameObject.Find("OsQuad");
        GameObject newInstance = Instantiate(afterglowPrefab, movingScanArea.transform.position,
            movingScanArea.transform.rotation);
        //Create new material to use
        Material mat = newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
        mat.SetTexture("_MainTex", texture);
        newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = new Material(mat);
        
        //newInstance.transform.SetParent(gameObject.transform);
        
        yield return null;
    }
    
    #endregion
}