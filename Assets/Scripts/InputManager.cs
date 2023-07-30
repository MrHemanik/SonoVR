using System;
using System.Collections;
using System.Collections.Generic;
using mKit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    public GameManager gm;
    
    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (!context.started) return; //Only work when initial click

        foreach (var xrSelectInteractable in GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>().interactablesSelected)
        {
            Debug.Log("In Hand: "+xrSelectInteractable.transform.name);
            for (int i = 0; i < Volume.Volumes.Count; i++)
            {
                if(xrSelectInteractable.transform.GetChild(1).name.Contains(""+i))
                {
                    gm.CheckAnswer(i);
                    break;
                }
            }
        }

    }
}
