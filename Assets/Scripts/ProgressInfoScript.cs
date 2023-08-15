using TMPro;
using UnityEngine;

public class ProgressInfoScript : MonoBehaviour
{
    private GameManager gm;
    public GameObject progressBar;
    public TextMeshPro levelText;
    public TextMeshPro rightText;
    public TextMeshPro wrongText;
    private float progressBarMaxLength;
    private float progressBarCenter;
    private int levelCount;
    private int currentLevelNumber;
    private int rightCount;
    private int wrongCount;

    public void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.initLevelEvent.AddListener(UpdateVisualisation);
        gm.endLevelEvent.AddListener(UpdateRightWrongText);
        progressBarCenter = progressBar.transform.localPosition.x;
        progressBarMaxLength = progressBar.transform.localScale.x;
    }

    private void UpdateVisualisation()
    {
        levelCount = gm.LevelCount;
        currentLevelNumber = gm.CurrentLevelID;
        UpdateCurrentLevelText();
        UpdateProgressBar();
    }

    private void UpdateCurrentLevelText()
    {
        levelText.text = $"Level {currentLevelNumber} von {levelCount}";
    }

    private void UpdateProgressBar()
    {
        var newBarLength = progressBarMaxLength * currentLevelNumber / levelCount;
        var localScale = progressBar.transform.localScale;
        localScale = new Vector3(newBarLength, localScale.y, localScale.z);
        progressBar.transform.localScale = localScale;

        var localPosition = progressBar.transform.localPosition;
        localPosition = new Vector3(progressBarCenter - (progressBarMaxLength / 2) + (newBarLength / 2),
            localPosition.y, localPosition.z);
        progressBar.transform.localPosition = localPosition;
    }

    private void UpdateRightWrongText()
    {
        if (gm.LevelWon != null)
        {
            if ((bool) gm.LevelWon)
            {
                rightCount++;
                UpdateRightText();
            }
            else
            {
                wrongCount++;
                UpdateWrongText();
            }
        }
    }
    private void UpdateRightText()
    {
        rightText.text = $"{rightCount} Richtig";
    }

    private void UpdateWrongText()
    {
        wrongText.text = $"{wrongCount} Falsch";
    }
    
}