using System;
using UnityEngine;

namespace SonoGame
{
    public enum MainMenuOption { None = -2, MainMenu = -1, Tutorial, Guided, Free, Explore, Settings, Credits, Quit, Calibration, Study, LogUI }

    [Serializable]
    public class MainMenuItem : IEquatable<MainMenuItem>, IComparable<MainMenuItem>
    {
        /// <summary>
        /// Active in menu list
        /// </summary>
        public bool active;

        /// <summary>
        /// Sorting hint
        /// </summary>
        public int sortIndex;

        /// <summary>
        /// Only active for admin user
        /// </summary>
        public bool adminOnly;

        public MainMenuOption option;
        public string displayTitle;

        /// <summary>
        /// Button gameObject
        /// </summary>
        [NonSerialized]
        internal GameObject go;

        /// <summary>
        /// Icon
        /// </summary>
        public Sprite menuIcon;

        public int CompareTo(MainMenuItem other)
        {
            return sortIndex.CompareTo(other.sortIndex);
        }

        public bool Equals(MainMenuItem other)
        {
            return other.sortIndex == sortIndex;
        }

        public void SetGameObject(GameObject go)
        {
            this.go = go;
        }
    }
}
