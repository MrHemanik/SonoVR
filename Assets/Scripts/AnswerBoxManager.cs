using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerBoxManager : MonoBehaviour
{
    public AnswerBoxColorHighlight colorHighlighter;
    public bool[] insideVolumes = new bool[4]; //Will get init with the right volume amount

    public void InitAnswerBox(int volumeCount)
    {
        insideVolumes = new bool[volumeCount];
        colorHighlighter.Reset();
    }
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            Debug.Log(other.name);
            if (other.name.Equals("VolumeBoxGrabbable")&& other.transform.GetChild(1).name.Contains(""+i)) //Namecheck own name as it gets detaached from parent on grab
            {
                insideVolumes[i] = true;
                Debug.Log("Volume Trigger in AnswerBox:"+i);
            }
        }
        colorHighlighter.ChangeOutline(insideVolumes);
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.name.Equals("VolumeBoxGrabbable")&& other.transform.GetChild(1).name.Contains(""+i))
            {
                insideVolumes[i] = false;
            }
        }
        colorHighlighter.ChangeOutline(insideVolumes);
    }
}
