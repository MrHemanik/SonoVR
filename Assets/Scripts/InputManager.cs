using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    private GameManager gm;
    private XRDirectInteractor leftController;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.initLevelEvent.AddListener(ActivateObjectPickUp);
        gm.endLevelEvent.AddListener(DeactivateObjectPickUp);
        
        leftController = GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>();
        
    }

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (!context.started || gm.ActiveRound==false) return; //Only work when initial click & round active
        int answerId = leftController.interactablesSelected[0]
            .transform.GetComponent<InteractableInformation>().answerId;
        if (answerId == 0) return;
        gm.CheckAnswer(answerId-1);
    }

    private void ActivateObjectPickUp()
    {
        leftController.allowSelect = true;
    }private void DeactivateObjectPickUp()
    {
        leftController.allowSelect = false; //If there is an object currently grabbed it will cancel it.
    }
}