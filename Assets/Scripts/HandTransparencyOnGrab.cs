using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandTransparencyOnGrab : MonoBehaviour
{
    public SkinnedMeshRenderer handModel;
    public Material grabMaterial;
    private Material defaultMaterial;

    public void Start()
    {
        defaultMaterial = handModel.material;
    }

    public void StartGrab(SelectEnterEventArgs args)
    {
        handModel.material = grabMaterial;
    }
    public void EndGrab(SelectExitEventArgs args)
    {
        handModel.material = defaultMaterial;
    }
}
