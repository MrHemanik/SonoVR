using System;

namespace SonoGame
{
    /// <summary>
    /// Mapping of material to impedance. 
    /// Impedance is set as the coeffecient of specific acustic impedance (formerly Rayls)
    /// Impedance unit: c * 10^6 [kg / m^2*s]
    /// </summary>
    [Serializable]
    public class ImpedanceMapping
    {
        /// <summary>
        /// Material index 0 is the medium 
        /// </summary>
        public int materialIndex;

        /// <summary>
        /// The material voxels are randomly assigned one of two impedance values.
        /// </summary>
        public float impedance1;

        /// <summary>
        /// The material is homogenous, if impedance1 == impedance2.
        /// </summary>
        public float impedance2;

        public ImpedanceMapping(int materialIndex, float impedance1, float impedance2)
        {
            this.materialIndex = materialIndex;
            this.impedance1 = impedance1;
            this.impedance2 = impedance2;
        }
    }

}
