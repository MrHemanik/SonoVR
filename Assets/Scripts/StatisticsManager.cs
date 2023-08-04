using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private GameManager gm;
    private Timer overallTimer;
    private Timer levelTimer;
    void Start()
    {
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
        levelTimer.StartTimer();
    }

    private void EndLevel()
    {
        levelTimer.StopTimer();
    }

    private void EndGame()
    {
        overallTimer.StopTimer();
    }


}
