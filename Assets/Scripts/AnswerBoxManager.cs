using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerBoxManager : MonoBehaviour
{
    public AnswerBoxColorHighlight colorHighlighter;
    private bool[] insideVolumes = new bool[4]; //Should be flexible size from Volume.Volumes.Count
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.name == "VolumeBoxGrabbable" && other.transform.parent.name.Contains(""+(i+1))) //If name-check isn't true it won't check for parent.
            {
                insideVolumes[i] = true;
                Debug.Log("Volume Trigger:"+i);
            }
        }
        colorHighlighter.ChangeOutline(insideVolumes);
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.transform.name.Contains(""+(i+1)))
            {
                insideVolumes[i] = false;
            }
        }
        colorHighlighter.ChangeOutline(insideVolumes);
    }
}
