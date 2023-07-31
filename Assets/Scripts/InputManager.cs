using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    public GameManager gm;

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (!context.started || gm.activeRound==false) return; //Only work when initial click & round active
        int answerId = GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>().interactablesSelected[0]
            .transform.GetComponent<InteractableInformation>().answerId;
        if (answerId == 0) return;
        gm.CheckAnswer(answerId-1);
    }
}