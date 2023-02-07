using System;
using System.Collections.Generic;
using UnityEngine;


namespace SonoGame
{
    /// <summary>
    /// UI colors
    /// </summary>
    public enum InterfaceColor { None, Text, SliceBackground, Selected, NotSelected, Success, Failure, ArcadeConfirmButton, ArcadeBackButton, ArcadeSkipButton, ArcadeResetViewButton, ArcadeInfoButton, ArcadeNumberButton, TimeAlert, TimeElapsed, TimeHighlight, Logout }

    /// <summary>
    /// UI alpha value modifier
    /// </summary>
    public enum InterfaceModifier { None, Hidden, ReducedContrast, Disabled }

    /// <summary>
    /// Config for app colors
    /// </summary>
    [CreateAssetMenu(fileName = "UiColorConfig", menuName = "ScriptableObjects/Sonogame UiColorConfig", order = 1)]
    public class UiColorConfig : ScriptableObject
    {
        /// <summary>
        /// Alpha channel value to hide UI element.
        /// </summary>
        public byte hiddenAlpha = 0;

        /// <summary>
        /// Alpha channel value for disabled UI element.
        /// </summary>
        public byte disabledAlpha = 32;

        /// <summary>
        /// Alpha channel value UI element with reduced contrast.
        /// </summary>
        public byte reducedContrastAlpha = 64;

        /// <summary>
        /// App color item
        /// </summary>
        [Serializable]
        public struct AppColor
        {
            public InterfaceColor interfaceColor;
            public Color color;

            public AppColor(InterfaceColor interfaceColor, Color color)
            {
                this.interfaceColor = interfaceColor;
                this.color = color;
            }
        }

        /// <summary>
        /// List of defined app colors.
        /// </summary>
        public List<AppColor> appColor;

        public Color[] choiceButtonColor;
        public Color choiceButtonDisabled = Color.grey;
        private Dictionary<InterfaceColor, Color> appColorDict;

        /// <summary>
        /// Returns white color white defined alpha state, e.g. <see cref="disabledAlpha"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="uiOffStyle">hidden, slightlyDarker, grayedOut</param>
        /// <returns></returns>
        public Color GetStatefulUiColor(bool state, InterfaceModifier uiOffStyle = InterfaceModifier.Hidden) => GetStatefulUiColor(InterfaceColor.Text, state, uiOffStyle);

        /// <summary>
        /// Stateful color (used in campaign menu)
        /// </summary>
        /// <param name="state"></param>
        /// <param name="uiDisabledState">hidden, slightlyDarker, grayedOut</param>
        /// <returns></returns>
        public Color GetStatefulUiColor(InterfaceColor interfaceColor, bool state, InterfaceModifier uiDisabledState = InterfaceModifier.Hidden)
        {
            byte offStateAlpha = hiddenAlpha;

            Color32 statefulColor = GetAppColorByName(interfaceColor);

            switch (uiDisabledState)
            {
                case InterfaceModifier.ReducedContrast:
                    offStateAlpha = reducedContrastAlpha;
                    break;
                case InterfaceModifier.Disabled:
                    offStateAlpha = disabledAlpha;
                    break;
            }

            statefulColor.a = state ? (byte)255 : offStateAlpha;

            return statefulColor;
        }

        /// <summary>
        /// Get application color by name.
        /// </summary>
        /// <param name="name">string from <see cref="appColor"/> list</param>
        /// <returns>Color</returns>
        public Color GetAppColorByName(InterfaceColor interfaceColor)
        {
            Color color = Color.white;

            if (appColorDict == null)
            {
                appColorDict = new Dictionary<InterfaceColor, Color>();

                if (appColor == null)
                {
                    Debug.LogError("UiColorConfig: appColor is null for Appconfig.current=" + (AppConfig.current != null ? AppConfig.current.name : "null"));
                }
                else
                {
                    foreach (var item in appColor)
                    {
                        appColorDict[item.interfaceColor] = item.color;
                    }
                }
            }

            if (appColorDict.ContainsKey(interfaceColor))
            {
                color = appColorDict[interfaceColor];
            }

            return color;
        }
    }
}
