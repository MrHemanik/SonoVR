using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Outline outline;
    public Color hover = new Color(1f, 0.52f, 0.56f);
    public Color grab = new Color(1f, 0.03f, 0f);

    public void StartHover()
    {
        outline.OutlineColor = hover;
        outline.enabled = true;
    }

    public void StopHover()
    {
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
