using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OutlineManager : MonoBehaviour
{
    public Outline outline;
    public bool startWithVisibleOutline = true;
    public Color hover = new Color(1f, 0.52f, 0.56f);
    public Color grab = new Color(1f, 0.03f, 0f);

    private void Start()
    {
        outline.enabled = startWithVisibleOutline;
    }

    public void StartHover(HoverEnterEventArgs args)
    {
        outline.OutlineColor = hover;
        outline.enabled = true;
    }

    public void StopHover(HoverExitEventArgs args)
    {
        outline.enabled = false;
    }

    public void StartGrab(SelectEnterEventArgs args)
    {
        outline.OutlineColor = grab;
        outline.enabled = true;
    }
    public void StopGrab(SelectExitEventArgs args)
    {
        outline.OutlineColor = hover; //sets color to interactable due to it still being in range of hover
        outline.enabled = true;
    }
    
}
