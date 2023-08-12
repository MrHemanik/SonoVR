using System;
using UnityEngine;

namespace Tutorial
{
    public class CheckForFirstProbeScript : MonoBehaviour
    {
        private TutorialManager tm;

        private void Start()
        {
            tm = FindObjectOfType<TutorialManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (tm.CurrentTutorialTextId == 4 && other.name == "EmptySliceCopy")
                tm.TriggerWithVolume();
        }
    }
}