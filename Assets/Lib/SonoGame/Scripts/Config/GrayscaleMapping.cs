using System;

namespace SonoGame
{
    /// <summary>
    /// Mapping of material to grayscale. 
    /// </summary>
    [Serializable]
    public class GrayscaleMapping
    {
        /// <summary>
        /// Material index 0 is the medium 
        /// </summary>
        public int materialIndex;

        /// <summary>
        /// Grayscale, normalized [0..1]
        /// </summary>
        public float grayscale;

        /// <summary>
        /// Noise range as normalized delta [0..1]
        /// </summary>
        public float noiseRange;

        public GrayscaleMapping(int materialIndex, float grayscale, float noiseRange)
        {
            this.materialIndex = materialIndex;
            this.grayscale = grayscale;
            this.noiseRange = noiseRange;
        }
    }

}
