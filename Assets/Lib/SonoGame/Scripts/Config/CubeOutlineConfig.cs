using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeOutlineConfig", menuName = "ScriptableObjects/CubeOutline Config", order = 1)]
[Serializable]
public class CubeOutlineConfig : ScriptableObject
{

    public bool[] line;

    public Color[] lineColor;

    public bool quadBack;
    public bool quadBottom;

    public Color floorColor;

    public bool hint;
    public float hintSize = 0.05f;
    public bool hintLeft = true;
    public bool hintTop = true;
    public Color hintColor;

    [Range(0.0f, 0.5f)]
    public float normStart = 0.5f;
    [Range(0.0f, 0.5f)]
    public float normEnd = 0f;

    public CubeOutlineConfig() : base()
    {
        int items = 12;

        line = new bool[items];
        lineColor = new Color[items];

        for (int i = 0; i < lineColor.Length; i++)
        {
            line[i] = true;
            lineColor[i] = Color.white;
        }
    }
}
