using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Config for UI label wording. Define key-value pairs in <see cref="items"/>.
    /// The application should use <see cref="GetLanguageDict"/> to get a key-value dictionary.
    /// </summary>
    [CreateAssetMenu(fileName = "LanguageConfig", menuName = "ScriptableObjects/Sonogame LanguageConfig", order = 1)]
    public class LanguageConfig : ScriptableObject
    {
        [Serializable]
        public struct LanguageItem
        {
            public string key;
            public string value;
        }

        [Header("Language of this file")]
        public string languageCode;

        [Header("Wording items")]
        public List<LanguageItem> items;


        /// <summary>
        /// Get a dictionary for key/value pairs from language <see cref="items"/>.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetLanguageDict()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in items)
            {
                if (result.ContainsKey(item.key))
                {
                    Debug.LogError("LanguageConfig.GetLanguageDict: duplicate entry for " + item.key);
                }
                else
                {
                    result[item.key] = item.value;
                }
            }

            return result;
        }


        internal string GetPlural(string s)
        {
            return s + "s";
        }

        /// <summary>
        /// The implemenation should offer solutions for obvious cases like "Visualization"
        /// </summary>
        /// <param name="input">word</param>
        /// <returns>hyphenated word</returns>
        public string Hyphenate(string input)
        {
            string result = input;

            switch (input)
            {
                case "Visualization":
                    result = "Visuali- zation";
                    break;

                default:
                    Debug.LogWarning("LanguageConfig.Hyphenate: unchanged " + input);
                    break;
            }

            return result;
        }

        public string VisualizationToDisplayString(EVisualization visualization)
        {
            string result;

            switch (visualization)
            {
                case EVisualization.Colored:
                    result = "COLOR";
                    break;
                case EVisualization.Gray:
                    result = "GRAY";
                    break;
                case EVisualization.Ultrasound:
                    result = "SONO";
                    break;
                case EVisualization.Anatomic:
                    result = "ANATOMIC";
                    break;
                case EVisualization.Mixed:
                    result = "MIXED";
                    break;
                case EVisualization.None:
                default:
                    result = "UNDEF";
                    break;
            }

            return result;
        }
    }
}
