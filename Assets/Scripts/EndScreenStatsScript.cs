using System;
using TMPro;
using UnityEngine;

public class EndScreenStatsScript : MonoBehaviour
{
    private StatisticsManager sm;
    public TextMeshPro time;
    public TextMeshPro rightText;
    public TextMeshPro wrongText;
    public TextMeshPro rightTextFirstHalf;
    public TextMeshPro wrongTextFirstHalf;
    public TextMeshPro rightTextSecondHalf;
    public TextMeshPro wrongTextSecondHalf;
    public void Start()
    {
        sm = FindObjectOfType<StatisticsManager>();
        time.text = $"Zeit: {(int) (sm.Data.overallTime / 60)} Minuten und {(int) (sm.Data.overallTime % 60)} Sekunden";
        var rightCount = 0;
        var wrongCount = 0;
        foreach (var levelData in sm.Data.levelData)
        {
            if (levelData.answeredRight) rightCount++;
            else wrongCount++;
        }

        var answerCount = sm.Data.levelData.Count;
        rightText.text = $"{rightCount} Richtige Antworten - {(float)rightCount/answerCount:P}";
        wrongText.text = $"{wrongCount} Falsche Antworten - {(float)wrongCount/answerCount:P}";

        var rightCountFirstHalf = 0;
        var wrongCountFirstHalf = 0;
        for (int i = 0; i < (sm.Data.levelData.Count/2); i++)
        {
            Debug.Log(sm.Data.levelData[i].answeredRight);
            if (sm.Data.levelData[i].answeredRight) rightCountFirstHalf++;
            else wrongCountFirstHalf++;
        }
        Debug.Log($"{rightCount}:{rightCountFirstHalf}+{rightCount-rightCountFirstHalf} - {wrongCount}: {wrongCountFirstHalf}+{wrongCount-wrongCountFirstHalf}");
        
        rightTextFirstHalf.text = $"{rightCountFirstHalf} Richtige Antworten - {(float)rightCountFirstHalf/answerCount*2:P}";
        wrongTextFirstHalf.text = $"{wrongCountFirstHalf} Falsche Antworten - {(float)wrongCountFirstHalf/answerCount*2:P}";
        rightTextSecondHalf.text = $"{rightCount-rightCountFirstHalf} Richtige Antworten - {(float)(rightCount-rightCountFirstHalf)/answerCount*2:P}";
        wrongTextSecondHalf.text = $"{wrongCount-wrongCountFirstHalf} Falsche Antworten - {(float)(wrongCount-wrongCountFirstHalf)/answerCount*2:P}";
        
    }
}