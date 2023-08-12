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
    private AudioManager am;
    private ObjectPool afterimagePool;
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

    public Transform[] MKitVolumes { get; private set; } = new Transform[4];
    [HideInInspector] public List<Level> LevelList { private get; set; }

    [HideInInspector] public int WinningAnswerId { get; private set; }
    [HideInInspector] public Level CurrentLevel { get; private set; }


    /// <summary>
    /// Variable that is null when currently playing and either true or false after level completion
    /// </summary>
    public bool? LevelWon { get; private set; } = null;

    [HideInInspector] public int CurrentLevelID { get; private set; } = 0;
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

    #endregion

    #region Initiation

    private IEnumerator Start()
    {
        VolumeManager.Instance.SetMaterialConfig(materialConfig);
        LevelList = new LevelList().levelList;
        LevelList.AddRange(new LevelList().levelList); //Adds another randomly generated instance of all levels to levelList to compare to.
        afterimagePool = GetComponent<ObjectPool>();
        am = GetComponent<AudioManager>();
        compareVolumeBoxGrabbable = compareAnchor.GetChild(0).GetChild(1);
        compareSliceBoxGrabbable = compareAnchor.GetChild(1).GetChild(1);
        AnswerVolumeBoxGrabbables = new Transform[answerAnchors.Length];
        AnswerSliceBoxGrabbables = new Transform[answerAnchors.Length];
        for (var i = 0; i < answerAnchors.Length; i++)
        {
            AnswerVolumeBoxGrabbables[i] = answerAnchors[i].GetChild(0).GetChild(1);
            AnswerSliceBoxGrabbables[i] = answerAnchors[i].GetChild(1).GetChild(1);
        }

        //Instantiate all mKitVolumes so they can be found easier later
        for (int i = 0; i < 4; i++)
        {
            var mKitVolume = GameObject.Find($"mKitVolume #{i} (ArtificialVolume.vm2)");
            if (mKitVolume == null)
            {
                yield return VolumeManager.Instance.GenerateArtificialVolume(new ShapeConfig(), i);
                mKitVolume = GameObject.Find($"mKitVolume #{i} (ArtificialVolume.vm2)");
                DontDestroyOnLoad(GameObject.Find($"mKitToolgroup #{i} (ArtificialVolume.vm2)"));
                DontDestroyOnLoad(mKitVolume);
            }
            MKitVolumes[i] = mKitVolume.transform;
            
        }

        yield return InitLevel();
    }

    private IEnumerator InitLevel()
    {
        resetComponentsEvent.Invoke();
        CurrentLevel = LevelList[CurrentLevelID];
        Debug.Log(
            $"Loading Level {CurrentLevelID} of with compare:{CurrentLevel.levelType.compareObject} and answer:{CurrentLevel.levelType.answerOptions}");
        LevelWon = null;
        SetWinningAnswerVolume();
        enabled = false; // will be re-enabled after generating artificials
        yield return volGenMan.GenerateLevel(CurrentLevel, WinningAnswerId, answerAnchors, compareAnchor, MKitVolumes,
            AnswerVolumeBoxGrabbables);
        yield return null; //If yield return null it waits until generateLevel is fully finished
        if (CurrentLevel.levelType.answerOptions == ObjectType.HiddenVolumeAfterimage ||
            CurrentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterimage)
            afterimageCoroutine = StartCoroutine(HiddenVolumeAfterimage());
        enabled = true;
        ActiveRound = true;
        initLevelEvent.Invoke();
    }

    #endregion

    #region Level handling

    private void SetWinningAnswerVolume()
    {
        WinningAnswerId = Random.Range(0, CurrentLevel.volumeList.Count);
        Debug.Log($"The winning volume is: {WinningAnswerId}");
    }

    public void CheckAnswer(int answerID)
    {
        ActiveRound = false;
        StartCoroutine(EndLevel(WinningAnswerId == answerID));
    }

    private IEnumerator EndLevel(bool winning)
    {
        LevelWon = winning;
        am.Play(winning ? "RightAnswer" : "WrongAnswer");
        Debug.Log(winning ? "Richtige Antwort abgegeben!" : "Falsche Antwort abgegeben!");
        if (CurrentLevel.levelType.answerOptions == ObjectType.HiddenVolumeAfterimage ||
            CurrentLevel.levelType.compareObject == ObjectType.HiddenVolumeAfterimage)
            StopCoroutine(afterimageCoroutine);
        endLevelEvent.Invoke();

        //Part of loading new level
        CurrentLevelID++;
        yield return new WaitForSeconds(1f); //TODO: Replace with some sort of check for the player
        if (CurrentLevelID >= LevelList.Count) EndGame();
        else yield return InitLevel();
    }

    private void EndGame()
    {
        endGameEvent.Invoke();
        am.Play("RightAnswer");
        am.Play("WrongAnswer");
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
            for (int i = 0; i < CurrentLevel.volumeList.Count; i++)
            {
                CreateAfterimageStillBody(AnswerVolumeBoxGrabbables[i].GetChild(0), i);
            }
        }
        else
        {
            CreateAfterimageStillBody(compareVolumeBoxGrabbable.GetChild(0), WinningAnswerId);
        }

        void CreateAfterimageStillBody(Transform parent, int id)
        {
            GameObject newInstance = afterimagePool.GetObjectFromPool();
            newInstance.transform.SetPositionAndRotation(movingScanArea.position, movingScanArea.rotation);
            newInstance.transform.parent =
                parent; //Sets VolumeBoxGrabbable "visualComponents" of respective AnswerAnchor or CompareAnchor as parent
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