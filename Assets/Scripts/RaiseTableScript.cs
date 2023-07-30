using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseTableScript : MonoBehaviour
{
    void Start() //Nessesary for configurable joint to work both up and down
    {
        transform.position+= (new Vector3(0, 0.5f, 0));
    }
}
