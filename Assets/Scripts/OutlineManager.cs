using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OutlineManager : MonoBehaviour
{
    public Outline outline;
    public bool startWithVisibleOutline = true;
    public Color hover = new Color(1f, 0.52f, 0.56f);
    public Color grab = new Color(1f, 0.03f, 0f);
    private bool isGrabbed;
    private GameManager gm;
    private void Start()
    {
        outline.enabled = startWithVisibleOutline;
        gm = FindObjectOfType<GameManager>();
    }
    public void StartHover(HoverEnterEventArgs args)
    {
        if (!gm.ActiveRound) return;
        outline.OutlineColor = hover;
        outline.enabled = true;
    }

    public void StopHover(HoverExitEventArgs args)
    {
        if (!gm.ActiveRound) return;
        if(!isGrabbed) outline.enabled = false;
        else outline.OutlineColor = grab;
    }

    public void StartGrab(SelectEnterEventArgs args)
    {
        if (!gm.ActiveRound) return;
        isGrabbed = true;
        outline.OutlineColor = grab;
        outline.enabled = true;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
    }
    public void StopGrab(SelectExitEventArgs args)
    {
        if (!gm.ActiveRound) return;
        isGrabbed = false;
        outline.OutlineColor = hover; //sets color to interactable due to it still being in range of hover
        outline.OutlineMode = Outline.Mode.OutlineAll;
    }
}
