using Classes;
using TMPro;
using UnityEngine;

public class LevelInformationScript : MonoBehaviour
{
    public TextMeshPro generalDescription;
    public TextMeshPro compareType;
    public TextMeshPro answerType;
    public Renderer probeExamineInformation;
    public Light probeExamineInformationLight;
    public Material compareMaterial;
    public Material answerMaterial;
    private GameManager gm;
    private static readonly Color CompareColor = new Color(1f,0.83f,0.04f);
    private static readonly Color AnswerColor = new Color(0,0.525f,0.819f);
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.initLevelEvent.AddListener(SetLevelInformation);
    }
    void SetLevelInformation()
    {
        var levelType = gm.CurrentLevel.levelType;
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
            probeExamineInformationLight.color = CompareColor;
        }
        else
        {
            answerType.text = "Untersuchbare " + answerType.text;
            probeExamineInformation.material = answerMaterial;
            probeExamineInformationLight.color = AnswerColor;

        }
    }
    
}