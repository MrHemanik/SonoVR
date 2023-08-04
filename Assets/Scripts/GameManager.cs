using System.Collections;
using System.Collections.Generic;
using Classes;
using mKit;
using SonoGame;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Scripts")] public VolumeGenerationManager volGenMan;

    private Coroutine afterimageCoroutine;

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

    private Transform compareVolumeBoxGrabbable;
    private Transform compareSliceBoxGrabbable;
    public Transform[] AnswerVolumeBoxGrabbables { get; private set; }
    public Transform[] AnswerSliceBoxGrabbables { get; private set; }


    [HideInInspector] private List<Level> levelList;

    [HideInInspector] public int WinningAnswerId { get; private set; }
    [HideInInspector] public Level CurrentLevel { get; private set; }


    /// <summary>
    /// Variable that is null when currently playing and either true or false after level completion
    /// </summary>
    public bool? LevelWon { get; private set; } = null;

    [HideInInspector] public int CurrentLevelID { get; private set; }
    [HideInInspector] public bool ActiveRound { get; private set; } = true;
    /// <summary>
    /// Everything that should be done before the level gets loaded, and after the player wants to load the next level
    /// </summary>
    [HideInInspector] public UnityEvent resetComponentsEvent = new UnityEvent();
    /// <summary>
    /// Everything that should be done after the volumes and slices are generated and placed
    /// </summary>
    [HideInInspector] public UnityEvent initLevelEvent = new UnityEvent();
    /// <summary>
    /// Everything that should be done after an answer was selected
    /// </summary>
    [HideInInspector] public UnityEvent endLevelEvent = new UnityEvent();
    /// <summary>
    /// Everything that should be done after every level has been played
    /// </summary>
    [HideInInspector] public UnityEvent endGameEvent = new UnityEvent();

    private ObjectPool afterimagePool;

    #endregion

    #region Initiation

    private void Awake()
    {
        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        levelList = new LevelList().levelList;
    }

    private IEnumerator Start()
    {
        afterimagePool = gameObject.GetComponent<ObjectPool>();
        compareVolumeBoxGrabbable = compareAnchor.GetChild(0).GetChild(1);
        compareSliceBoxGrabbable = compareAnchor.GetChild(1).GetChild(1);
        AnswerVolumeBoxGrabbables = new Transform[answerAnchors.Length];
        AnswerSliceBoxGrabbables = new Transform[answerAnchors.Length];
        for (var i = 0; i < answerAnchors.Length; i++)
        {
            AnswerVolumeBoxGrabbables[i] = answerAnchors[i].GetChild(0).GetChild(1);
            AnswerSliceBoxGrabbables[i] = answerAnchors[i].GetChild(1).GetChild(1);
        }

        yield return InitLevel();
    }

    private IEnumerator InitLevel()
    {
        resetComponentsEvent.Invoke();
        Debug.Log(CurrentLevelID);
        CurrentLevel = levelList[CurrentLevelID];
        LevelWon = null;
        SetWinningAnswerVolume();
        enabled = false; // will be re-enabled after generating artificials
        yield return volGenMan.GenerateLevel(CurrentLevel, WinningAnswerId, answerAnchors, compareAnchor);
        yield return null; //If yield return null it waits until generateLevel is fully finished //TODO: Rework
        if (CurrentLevel.levelType.answerOptions == ObjectType.HiddenVolumeAfterimage ||
            CurrentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterimage)
            afterimageCoroutine = StartCoroutine(HiddenVolumeAfterimage());
        enabled = true;
        ActiveRound = true;
        initLevelEvent.Invoke();
    }


    //Sets the layer of every child to either the default or an invisible layer

    #endregion

    #region Level handling

    private void SetWinningAnswerVolume()
    {
        WinningAnswerId = Random.Range(0, Volume.Volumes.Count);
        Debug.Log(WinningAnswerId);
    }

    public void CheckAnswer(int answerID)
    {
        ActiveRound = false;
        StartCoroutine(EndLevel(WinningAnswerId == answerID));
    }

    private IEnumerator EndLevel(bool winning)
    {
        LevelWon = winning;
        Debug.Log(winning ? "Richtige Antwort abgegeben!" : "Falsche Antwort abgegeben!");
        if (CurrentLevel.levelType.answerOptions == ObjectType.HiddenVolumeAfterimage ||
            CurrentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterimage)
            StopCoroutine(afterimageCoroutine);
        endLevelEvent.Invoke();
        System.GC.Collect(); //Manual Garbage Collect, as this runs in 1 scene which can lead to some garbage

        //Part of loading new level
        CurrentLevelID++;
        yield return new WaitForSeconds(1f); //TODO: Replace with some sort of check for the player
        if (CurrentLevelID >= levelList.Count) EndGame();
        else yield return InitLevel();
    }

    private void EndGame()
    {
        endGameEvent.Invoke();
    }

    #endregion

    #region LevelType Custom Functions

    private IEnumerator HiddenVolumeAfterimage()
    {
        Transform movingScanArea = volGenMan.sliceCopyTransform;
        LevelType levelType = CurrentLevel.levelType;
        while (true)
        {
            CreateAfterimageStill(movingScanArea, levelType);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CreateAfterimageStill(Transform movingScanArea, LevelType levelType)
    {
        if (levelType.answerOptions == ObjectType.HiddenVolumeAfterimage)
        {
            for (int i = 0; i < Volume.Volumes.Count; i++)
            {
                CreateAfterimageStillBody(AnswerVolumeBoxGrabbables[i], i);
            }
        }
        else
        {
            CreateAfterimageStillBody(compareVolumeBoxGrabbable, WinningAnswerId);
        }

        void CreateAfterimageStillBody(Transform parent, int id)
        {
            GameObject newInstance = afterimagePool.GetObjectFromPool();
            newInstance.transform.SetPositionAndRotation(movingScanArea.position, movingScanArea.rotation);
            newInstance.transform.parent =
                parent; //Sets VolumeBoxGrabbable of respective AnswerAnchor or CompareAnchor as parent
            newInstance.transform.localScale = Vector3.one;
            CreateAndAssignAfterimageMaterial(newInstance.transform.GetChild(0),
                newInstance.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial,
                volGenMan.sliceViews[0].GetComponent<MeshRenderer>().material.GetTexture($"texArray_{id}"));
            //volGenMan.temporaryObjects.Add(newInstance); //Adds it to "Delete with new level" List
            StartCoroutine(afterimagePool.ReturnObjectToPoolAfterTime(newInstance, 0.5f));
        }

        static void CreateAndAssignAfterimageMaterial(Transform targetStillSlice, Material mat, Texture sliceTexture)
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