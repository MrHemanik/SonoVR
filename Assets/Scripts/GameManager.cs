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
            CreateAfterglowStill();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CreateAfterglowStill1()
    {
        //TODO: needs rework as it can't be parented like this
        //Create afterglowPrefab
        Transform movingScanArea = volGenMan.sliceCopyTransform;
        GameObject newInstance = Instantiate(afterglowPrefab, movingScanArea.position,
            movingScanArea.rotation);
        Material mat = newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
        mat.SetInt("texCount", Volume.Volumes.Count);
        //Copies current Texture(s)
        for (int i = 0; i < Volume.Volumes.Count; i++)
        {
            Texture sliceTexture = volGenMan.sliceViews[0].GetComponent<MeshRenderer>().material
                .GetTexture("texArray_" + i);
            Texture2D stillTexture =
                new Texture2D(sliceTexture.width, sliceTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(sliceTexture, stillTexture);
            mat.SetTexture("texArray_" + i, stillTexture);
            newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = new Material(mat);
        }
        //newInstance.transform.SetParent(gameObject.transform);

        yield return null;
    }

    private void CreateAfterglowStill()
    {
        Transform movingScanArea = volGenMan.sliceCopyTransform;
        LevelType levelType = levelList[currentLevelID].levelType;

        if (levelType.answerOptions == ObjectType.HiddenVolumeAfterglow)
        {
            for (int i = 0; i < Volume.Volumes.Count; i++)
            {
                GameObject newInstance = Instantiate(afterglowPrefab, movingScanArea.position,
                    movingScanArea.rotation,
                    answerAnchors[i].GetChild(0)
                        .GetChild(0)); //Sets VolumeBoxGrabbable of respective AnswerAnchor as parent
                CreateAndAssignAfterglowMaterial(newInstance.transform.GetChild(0),
                    newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial,
                    volGenMan.sliceViews[0].GetComponent<MeshRenderer>().material.GetTexture("texArray_" + i));
                volGenMan.temporaryObjects.Add(newInstance); //Adds it to "Delete with new level" List
            }
        }
        else
        {
            GameObject newInstance = Instantiate(afterglowPrefab, movingScanArea.position,
                movingScanArea.rotation,
                compareAnchor.GetChild(0).GetChild(0)); //Sets VolumeBoxGrabbable of compareObject as parent
            CreateAndAssignAfterglowMaterial(newInstance.transform.GetChild(0),
                newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial,
                volGenMan.sliceViews[0].GetComponent<MeshRenderer>().material
                    .GetTexture("texArray_" + winningAnswerId));
            volGenMan.temporaryObjects.Add(newInstance); //Adds it to "Delete with new level" List
        }

        static void CreateAndAssignAfterglowMaterial(Transform targetStillSlice, Material mat, Texture sliceTexture)
        {
            Texture2D stillTexture =
                new Texture2D(sliceTexture.width, sliceTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(sliceTexture, stillTexture);
            //Create new material to use (so it won't be sync't to the others)
            mat.SetTexture("_MainTex", stillTexture);
            targetStillSlice.GetComponent<Renderer>().sharedMaterial = new Material(mat);
        }
    }

    #endregion
}