using System;
using System.IO;
using Classes;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private readonly string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private GameManager gm;
    private Timer overallTimer;
    private Timer levelTimer;
    private StatisticsData data;
    private string filePath;

    void Start()
    {
        filePath = $"{Application.persistentDataPath}/SonoVR_StatisticData.csv";
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
        data.levelData.Add(new LevelData(gm.CurrentLevel.levelType.compareObject,
            gm.CurrentLevel.levelType.answerOptions, gm.LevelWon != null && (bool) gm.LevelWon,
            levelTimer.StopTimer()));
    }

    private void EndGame()
    {
        data.overallTime = overallTimer.StopTimer();
        data.endDate = DateTime.Now.ToString(dateTimeFormat);
        //In case where the statisticsData file is accidently open, put the data in a different file 
        try
        {
            if (!File.Exists(filePath)) File.WriteAllText(filePath, data.ToCsvHeaderString());
            File.AppendAllText(filePath, data.ToCsvString());
        }
        catch (Exception e)
        {
            File.WriteAllText(
                filePath.Insert(filePath.Length - 4, DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                data.ToCsvString());
        }
    }
}