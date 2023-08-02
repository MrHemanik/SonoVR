using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is used to put back the respective grabbable relating to the anchor this is put on
/// </summary>
public class AnchorGrabScript : MonoBehaviour
{
    public Transform grabbableBox;
    public XRSimpleInteractable xrsi;
    private XRInteractionManager xrim;

    void Start()
    {
        xrim = xrsi.interactionManager;
    }
    public void AnchorSelect(SelectEnterEventArgs args)
    {
        Debug.Log("Anchor Selected!");
        grabbableBox.transform.SetPositionAndRotation(transform.position,transform.rotation);
        xrim.SelectCancel(args.interactorObject,xrsi);
    }
}
