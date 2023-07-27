using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerAreaOutlineHighlight : MonoBehaviour
{
    private bool[] insideVolumes = new bool[4]; //Should be flexible size from Volume.Volumes.Count

    public Outline outline;
    private Color noVolumes;
    private Color oneVolume = Color.green;
    private Color twoOrMoreVolumes = Color.red;

    void Start()
    {
        noVolumes = outline.OutlineColor;
    }
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.gameObject.name.Contains(""+(i+1)))
            {
                insideVolumes[i] = true;
                Debug.Log("Volume Trigger:"+i);
            }
        }
        ChangeOutline();
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.gameObject.name.Contains(""+(i+1)))
            {
                insideVolumes[i] = false;
            }
        }
        ChangeOutline();
    }

    private void ChangeOutline()
    {
        int inside = 0;
        foreach (var volCheck in insideVolumes)
        {
            if (volCheck) inside++;
        }
        if (inside < 1) outline.OutlineColor = noVolumes;
        else if (inside == 1) outline.OutlineColor = oneVolume;
        else outline.OutlineColor = twoOrMoreVolumes;
    }
}
