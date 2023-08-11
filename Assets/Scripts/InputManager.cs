using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    private GameManager gm;
    private XRDirectInteractor leftController;
    public GameObject popupCanvas;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.resetComponentsEvent.AddListener(ActivateObjectPickUp);

        leftController = GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>();
    }

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (!context.started || gm.ActiveRound == false) return; //Only work when initial click & round active
        if (leftController.hasSelection)
        {
            int answerId = leftController.interactablesSelected[0] //TODO: check if it crashes when left trigger while tableheightchange
                .transform.GetComponent<InteractableInformation>().answerId;
            if (answerId == 0) return;
            leftController.allowSelect = false; //Will cancel the pickup, returning the answer to their anchor
            popupCanvas.SetActive(false);
            gm.CheckAnswer(answerId - 1);
        }
        else
        {
            //Only start coroutine if no other is running
            if (!popupCanvas.activeSelf) StartCoroutine(ShowPopupAndHidePopupAfterDelay(popupCanvas, 5.0f));
        }

        IEnumerator ShowPopupAndHidePopupAfterDelay(GameObject popupCanvas, float delay)
        {
            popupCanvas.SetActive(true);
            yield return new WaitForSeconds(delay);
            popupCanvas.SetActive(false);
        }
    }

    //EventListener
    private void ActivateObjectPickUp()
    {
        leftController.allowSelect = true;
    }
}