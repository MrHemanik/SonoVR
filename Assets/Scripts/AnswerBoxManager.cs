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

    public List<int> GetInsideVolumes()
    {
        List<int> inside = new List<int>();
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (insideVolumes[i]) inside.Add(i);
        }
        return inside;
    }
    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < insideVolumes.Length; i++)
        {
            if (other.name.Equals("VolumeBoxGrabbable")&& other.transform.GetChild(1).name.Contains(""+i)) //Namecheck own name as it gets detaached from parent on grab
            {
                insideVolumes[i] = true;
            }
        }
        colorHighlighter.ChangeOutline(GetInsideVolumes().Count);
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
        colorHighlighter.ChangeOutline(GetInsideVolumes().Count);
    }
}
