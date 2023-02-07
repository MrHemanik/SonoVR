using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// MiniGame profile
    /// </summary>
    [CreateAssetMenu(fileName = "GameProfile", menuName = "ScriptableObjects/Sonogame GameProfile", order = 3)]
    public class GameProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Assets")]
        [Tooltip("If unset, attempts to load a scene named like the minigame")]
        public UnityEngine.Object scene;

        [Tooltip("MiniGame class name. A same-named scene must be added to the \"Scenes in build\" list in the build settings")]
        public string miniGame;

        [Header("UI")]
        [Tooltip("(Optional) display title. Can be used to insert whitespace.")]
        public string displayTitle;

        [Tooltip("Icon (144*144 pixel)")]
        public Sprite icon;

        [Tooltip("Icon (104*104 pixel)")]
        public Sprite campaignIcon;

        //public string subType;

        [Header("Settings")]
        [Tooltip("List of available complexity levels")]
        public int complexityLevels = 1;

        [Tooltip("List of available visualization modes")]
        public List<EVisualization> visualizations;

        [Header("Listing")]
        [Tooltip("If true, the minigame is not listed in the game selection")]
        public bool doNotList;

        [Tooltip("If true, the minigame is not counted as challenge in the campaign")]
        public bool doNotCountAsChallenge;


        public string GetTitle()
        {
            return (displayTitle != "") ? displayTitle : miniGame;
        }

        public bool IsLast(EVisualization current)
        {
            return visualizations.Count > 0 && visualizations[visualizations.Count - 1] == current;
        }

        public EVisualization GetNextVisulization(EVisualization current)
        {
            EVisualization result = EVisualization.None;

            bool found = false;

            for (int i = 0; i < visualizations.Count; i++)
            {
                if (visualizations[i] == current)
                {
                    found = true;

                    if ((i + 1) < visualizations.Count)
                    {
                        result = visualizations[i + 1]; // switching to next DiagMode
                    }
                    else
                    {
                        result = EVisualization.None;
                    }
                }
            }

            if (!found)
            {
                Debug.LogError("GetNextDiagMode: current not found (" + current + ")");
            }

            return result;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }
    }
}
