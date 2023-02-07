using System;
using UnityEngine;
namespace SonoGame
{
    [Serializable]
    public struct GameLogEvent
    {
        public string userInput;
        public string gameEvent;

        public Vector3 slicePosition;
        public Vector3 probePosition;
        public Quaternion sliceRotation;

        public GameLogEvent(string userInput, string gameEvent, Transform slice, Transform probe)
        {
            this.userInput = userInput;
            this.gameEvent = gameEvent;
            this.slicePosition = slice.position;
            this.probePosition = probe.position;
            this.sliceRotation = slice.rotation;
        }

    }

}