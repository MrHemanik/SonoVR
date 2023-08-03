using Classes;
using TMPro;
using UnityEngine;

public class LevelInformationScript : MonoBehaviour
{
    public TextMeshPro generalDescription;
    public TextMeshPro compareType;
    public TextMeshPro answerType;
    public Renderer probeExamineInformation;
    public Material compareMaterial;
    public Material answerMaterial;
    private GameManager gm;
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.initLevelEvent.AddListener(SetLevelInformation);
    }
    void SetLevelInformation()
    {
        var levelType = gm.currentLevel.levelType;
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
            case (ObjectType.HiddenVolumeAfterimage):
                compareType.text = "Verstecktes Volumen mit Nachbild";
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
            case (ObjectType.HiddenVolumeAfterimage):
                answerType.text = "Versteckte Volumen mit Nachbild";
                break;
        }

        if (levelType.toProbe == ProbeType.CompareObject)
        {
            compareType.text = "Untersuchbares " + compareType.text;
            probeExamineInformation.material = compareMaterial;
        }
        else
        {
            answerType.text = "Untersuchbare " + answerType.text;
            probeExamineInformation.material = answerMaterial;

        }
    }
    
}