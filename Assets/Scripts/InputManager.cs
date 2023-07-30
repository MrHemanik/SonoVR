using System;
using System.Collections;
using System.Collections.Generic;
using mKit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class InputManager : MonoBehaviour
{
    public GameManager gm;

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (!context.started) return; //Only work when initial click
        int answerId = GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>().interactablesSelected[0]
            .transform.GetComponent<InteractableInformation>().answerId;
        if (answerId == 0) return;
        gm.CheckAnswer(answerId-1);
    }
}