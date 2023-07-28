using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerBoxColorHighlight : MonoBehaviour
{
    public Material material;
    private Color noVolumes = new Color(1f, 1f, 1f, 0.2f);
    private Color oneVolume = new Color(0f, 1f, 0f, 0.2f);
    private Color twoOrMoreVolumes = new Color(1f, 0f, 0f, 0.2f);

    void Start()
    {
        noVolumes = material.color;
    }

    public void ChangeOutline(bool[] insideVolumes)
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
