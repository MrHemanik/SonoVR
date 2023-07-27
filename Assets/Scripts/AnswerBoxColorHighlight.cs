using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerBoxColorHighlight : MonoBehaviour
{
    private bool[] insideVolumes = new bool[4]; //Should be flexible size from Volume.Volumes.Count
    
    public Material material;
    private Color noVolumes;
    private Color oneVolume = new Color(0f, 1f, 0f, 0.2f);
    private Color twoOrMoreVolumes = new Color(1f, 0f, 0f, 0.2f);

    void Start()
    {
        noVolumes = material.color;
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
        if (inside < 1) material.color = noVolumes;
        else if (inside == 1) material.color = oneVolume;
        else material.color = twoOrMoreVolumes;
    }
}
