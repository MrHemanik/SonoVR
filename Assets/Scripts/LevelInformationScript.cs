using System.Collections;
using System.Collections.Generic;
using Classes;
using TMPro;
using UnityEngine;

public class LevelInformationScript : MonoBehaviour
{
    public TextMeshPro generalDescription;
    public TextMeshPro compareType;
    public TextMeshPro answerType;

    public void SetLevelInformation(LevelType levelType)
    {
        generalDescription.text = levelType.description;
        switch (levelType.compareObject)
        {
            case (ObjectType.Slice):
                compareType.text = "Schnittbild";
                break;
            case (ObjectType.Volume):
                compareType.text = "Volumen";
                break;
            case (ObjectType.HiddenVolume):
                compareType.text = "Verstecktes Volumen";
                break;
            case (ObjectType.HiddenVolumeAfterglow):
                compareType.text = "Verstecktes Volumen mit Rekonstruktion";
                break;
        }

        switch (levelType.answerOptions)
        {
            case (ObjectType.Slice):
                answerType.text = "Schnittbilder";
                break;
            case (ObjectType.Volume):
                answerType.text = "Volumen";
                break;
            case (ObjectType.HiddenVolume):
                answerType.text = "Versteckte Volumen";
                break;
            case (ObjectType.HiddenVolumeAfterglow):
                answerType.text = "Versteckte Volumen mit Rekonstruktion";
                break;
        }

        if (levelType.toProbe == ProbeType.CompareObject)
            compareType.text = "Untersuchbares " + compareType.text;
        else answerType.text = "Untersuchbare " + answerType.text;
    }
    
}