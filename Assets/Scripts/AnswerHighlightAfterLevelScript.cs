using System;
using System.Collections.Generic;
using System.Linq;
using Classes;
using UnityEngine;

public class AnswerHighlightAfterLevelScript : MonoBehaviour
{
    private readonly Color rightAnswer = Color.green;
    private readonly Color wrongAnswer = Color.red;
    private GameManager gm;
    private List<Outline> currentlyHighlighted = new List<Outline>();

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.resetComponentsEvent.AddListener(RemoveHighlights);
        gm.endLevelEvent.AddListener(AfterLevelHighlight);
    }

    private void AfterLevelHighlight()
    {
        var ansIsSlice = gm.CurrentLevel.levelType.answerOptions == ObjectType.Slice;
        for (int i = 0; i < gm.CurrentLevel.volumeList.Count; i++)
        {
            //Adds either SliceBox or Volumebox OutlineManager
            var outlineManagers = ansIsSlice
                ? gm.AnswerSliceBoxGrabbables[i].GetComponentsInChildren<OutlineManager>().ToList()
                : gm.AnswerVolumeBoxGrabbables[i].GetComponentsInChildren<OutlineManager>().ToList();
            //Adds either VolumeAnchor or SliceAnchors OutlineManager
            outlineManagers.Add(ansIsSlice
                ? gm.answerAnchors[i].GetChild(1).GetChild(0).GetComponent<OutlineManager>()
                : gm.answerAnchors[i].GetChild(0).GetChild(0).GetComponent<OutlineManager>());
            foreach (var outlineManager in outlineManagers)
            {
                outlineManager.outline.enabled = true;
                if (gm.WinningAnswerId == i) outlineManager.outline.OutlineColor = rightAnswer;
                else outlineManager.outline.OutlineColor = wrongAnswer;
                currentlyHighlighted.Add(outlineManager.outline);
            }
        }
    }

    private void RemoveHighlights()
    {
        foreach (var outline in currentlyHighlighted)
        {
            outline.enabled = false;
        }

        currentlyHighlighted.Clear();
    }
}