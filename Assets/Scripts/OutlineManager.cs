using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Outline outline;
    private Color defaultOutlineColor;
    private Color hover = new Color(1f, 0.52f, 0.56f);
    private Color grab = new Color(1f, 0.03f, 0f);
    void Start()
    {
        defaultOutlineColor = outline.OutlineColor;
    }

    public void StartHover()
    {
        outline.OutlineColor = hover;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.OutlineColor = defaultOutlineColor;
        outline.enabled = false;
    }

    public void StartGrab()
    {
        outline.OutlineColor = grab;
        outline.enabled = true;
    }
    public void StopGrab()
    {
        outline.OutlineColor = hover; //sets color to interactable due to it still being in range of hover
        outline.enabled = true;
    }
    
}
