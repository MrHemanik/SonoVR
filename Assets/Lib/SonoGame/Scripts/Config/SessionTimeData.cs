using System;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Session time data provider (derived from ScriptableObject).
    /// Always use <see cref="Reset"/> upon app start to avoid undefined instance states.
    /// To initialise, use <see cref="StartSessionTime"/>.
    /// Returned time values change only once per frame.
    /// </summary>
    [CreateAssetMenu(fileName = "SessionTimeData", menuName = "ScriptableObjects/Sonogame SessionTimeData", order = 1)]
    public class SessionTimeData : ScriptableObject
    {
//#if UNITY_EDITOR
        [Tooltip("For editor debugging, enter test values for seconds of session time left.")]
        public float testTimeLeft = 0;
//#endif

        private float timeStartSeconds;
        private float timeMaxSeconds;

        public bool Running { get; protected set; }

        public float SessionStartTime { get => timeStartSeconds; }

        public float SessionElapsedTime { get { return Running ? CurrentFrameTime - timeStartSeconds : 0; } }

        public float SessionTimeLeft { get { return Running ? timeMaxSeconds - SessionElapsedTime : 0; } }

        [Serializable]
        public class TimeAccount
        {
            public string dateStr;
            public float elapsed;

            public TimeAccount(float elapsed)
            {
                this.elapsed = elapsed;
                dateStr = DateTime.Now.Date.ToString();
            }
        }

        /// <summary>
        /// Reset must be called on app start for defined instance state.
        /// </summary>
        public void Reset()
        {
            Running = false;
            timeStartSeconds = 0;
            timeMaxSeconds = 0;
        }

        internal void StartSessionTime(float timeMaxSeconds, float alreadyElapsed)
        {
            this.timeStartSeconds = CurrentFrameTime - alreadyElapsed;
            this.timeMaxSeconds = timeMaxSeconds;

            Running = true;
        }

        internal void StopSessionTime()
        {
            Running = false;
        }

        /// <summary>
        /// Returns the current frame time.
        /// </summary>
        private float CurrentFrameTime
        {
            get
            {
#if UNITY_EDITOR
                if (testTimeLeft != 0)
                {
                    timeStartSeconds = Time.time - timeMaxSeconds + testTimeLeft;
                    testTimeLeft = 0;
                }
#endif

                return Time.time;
            }

        }
    }
}