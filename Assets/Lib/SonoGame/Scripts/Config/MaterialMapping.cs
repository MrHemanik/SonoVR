using System;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Mapping of material to color/grayscale/impedance. 
    /// </summary>
    [Serializable]
    public class MaterialMapping
    {
        /// <summary>
        /// Material index 0 is the medium 
        /// </summary>
        internal byte materialIndex;

        /// <summary>
        /// The material voxels are randomly assigned one of two impedance values.
        /// </summary>
        public Color color;

        public Color color1;
        public Color color2;

        /// <summary>
        /// Grayscale
        /// </summary>
        public Color grayscale;

        public Color grayscale1;
        public Color grayscale2;

        /// <summary>
        /// Noise range as normalized delta [0..1]
        /// </summary>
        public float noiseRange;

        /// <summary>
        /// The material voxels are randomly assigned one of two impedance values.
        /// </summary>
        [Header("Ultrasound simulation")]
        public float impedance1;

        /// <summary>
        /// The material is homogenous, if impedance1 == impedance2.
        /// </summary>
        public float impedance2;

        public MaterialMapping(int materialIndex, Color color, Color grayscale, float noiseRange, float impedance1, float impedance2)
        {
            this.materialIndex = (byte)materialIndex;
            this.color = color;
            this.noiseRange = noiseRange;
            this.grayscale = grayscale;
            this.impedance1 = impedance1;
            this.impedance2 = impedance2;
        }
    }

}
