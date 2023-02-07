using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// This Class get the Scale of an object, and assign it to the current object
    /// </summary>
    public class GetScaleFrom : MonoBehaviour
    {
        // publics
        // the object, from which the scale is
        public Transform fromTransform;

        /// <summary>
        /// Unity-Update Method
        /// </summary>
        private void Update()
        {
            transform.localScale = new Vector3(GetHighestValue(fromTransform) * 1.5f, GetHighestValue(fromTransform) * 1.5f, GetHighestValue(fromTransform) * 1.5f);
        }

        /// <summary>
        /// Get the highest value from the x, y or z value.
        /// </summary>
        /// <param name="trans">The given transform</param>
        /// <returns>the highest value</returns>
        private float GetHighestValue(Transform trans)
        {
            float highestValue = trans.localScale.x;

            if (highestValue < trans.localScale.y)
            {
                highestValue = trans.localScale.y;
            }

            if (highestValue < trans.localScale.z)
            {
                highestValue = trans.localScale.z;
            }

            return highestValue;
        }
    }
}