using System;
using System.IO;
using Classes;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private readonly string dateTimeFormat = "yyyyMMdd_HHmmss";
    private GameManager gm;
    private Timer overallTimer;
    private Timer levelTimer;
    private StatisticsData data;
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath;
        data = new StatisticsData(dateTimeFormat);
        overallTimer = gameObject.AddComponent<Timer>();
        overallTimer.StartTimer();
        levelTimer = gameObject.AddComponent<Timer>();
        gm = FindObjectOfType<GameManager>();
        gm.initLevelEvent.AddListener(StartLevel);
        gm.endLevelEvent.AddListener(EndLevel);
        gm.endGameEvent.AddListener(EndGame);
    }

    private void StartLevel()
    {
        Debug.Log("Timer started for level");
        levelTimer.StartTimer();
    }

    private void EndLevel()
    {
        data.levelData.Add(new LevelData(gm.CurrentLevelID, gm.CurrentLevel.levelType.compareObject,
            gm.CurrentLevel.levelType.answerOptions, (bool) gm.LevelWon, levelTimer.StopTimer()));
    }

    private void EndGame()
    {
        data.overallTime = overallTimer.StopTimer();
        data.endDate = DateTime.Now.ToString(dateTimeFormat);
        File.WriteAllText($"{filePath}/SonoVR_StatisticData_{DateTime.Now.ToString(dateTimeFormat)}.json",
            JsonUtility.ToJson(data));
    }
}