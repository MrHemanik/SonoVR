using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelInformationScript : MonoBehaviour
{
    public TextMeshPro generalDescription;
    
    public void SetLevelInformation(LevelType levelType)
    {
        generalDescription.text = levelType.description;
        
    }
}
