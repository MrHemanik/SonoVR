using UnityEngine;

namespace SonoGame
{
    [CreateAssetMenu(fileName = "IconConfig", menuName = "ScriptableObjects/Sonogame IconConfig", order = 1)]
    public class IconConfig : ScriptableObject
    {
        public Sprite transparentSprite;
        public Sprite bronzeMedalSprite;
        public Sprite silverMedalSprite;
        public Sprite goldMedalSprite;

        /// <summary>
        /// Map <see cref="SonoMedal"/> to sprite.
        /// </summary>
        /// <param name="sonoMedal"></param>
        /// <returns>transparent sprite for <see cref="SonoMedal.None"/></returns>
        public Sprite GetMedalIcon(SonoMedal sonoMedal)
        {
            Sprite sprite = null;

            switch (sonoMedal)
            {
                case SonoMedal.None:
                    sprite = transparentSprite;
                    break;
                case SonoMedal.Bronze:
                    sprite = bronzeMedalSprite;
                    break;
                case SonoMedal.Silver:
                    sprite = silverMedalSprite;
                    break;
                case SonoMedal.Gold:
                    sprite = goldMedalSprite;
                    break;
            }

            return sprite;
        }

        /// <summary>
        /// Get medal sprite by index
        /// </summary>
        /// <param name="index">0,1,2 or -1 for transparent sprite</param>
        /// <returns>Sprite</returns>
        public Sprite GetMedalSpriteByIndex(int index)
        {
            Sprite sprite = null;

            switch (index)
            {
                case -1:
                    sprite = GetMedalIcon(SonoMedal.None);
                    break;
                case 0:
                    sprite = GetMedalIcon(SonoMedal.Bronze);
                    break;
                case 1:
                    sprite = GetMedalIcon(SonoMedal.Silver);
                    break;
                case 2:
                    sprite = GetMedalIcon(SonoMedal.Gold);
                    break;
            }

            return sprite;
        }
    }
}
