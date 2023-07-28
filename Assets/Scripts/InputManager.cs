using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public VolumeGenerationManager gm;
    public PlayerInput playerInput;

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed the Left Trigger");
    }
    
}
