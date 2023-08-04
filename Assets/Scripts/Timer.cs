using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float Time { get; private set; }

    private void Awake()
    {
        enabled = false;
    }
    
    private void Update()
    {
        Time += UnityEngine.Time.deltaTime;
    }

    public void StartTimer()
    {
        Time = 0;
        enabled = true;
    }
    

    public float StopTimer()
    {
        enabled = false;
        return Time;
    }
}
