using mKit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    [Serializable]
    public class UltrasoundConfiguration
    {
        public float defaultFrequency;
        public float defaultGain;

        [Header("Display")]
        /// <summary>
        /// sector opening angle [degrees] 
        /// </summary>
        public float sectorAngleDeg = 40f;

        /// <summary>
        /// Horizontal stretch factor [applied to slice X pos * sin(w)]
        /// </summary>
        public float sectorHorizontalStretch = 0.8f;

        /// <summary>
        /// Sector vertical stretch [applied to slice Y pos * cos(w)]
        /// </summary>
        public float sectorVerticalStretch = 1.03f;

        /// <summary>
        /// Sector translation offset [from row 0]
        /// </summary>
        public float sectorVerticalOffset = 68.43f;

        [SerializeField]
        public List<ImpedanceMapping> impedanceMapping;

        public UltrasoundConfiguration()
        {
            impedanceMapping = new List<ImpedanceMapping>();
        }

        public void UpdateImpedanceMapping(int mappingIndex, float value1, float value2)
        {
            if (impedanceMapping != null && mappingIndex < impedanceMapping.Count)
            {
                impedanceMapping[mappingIndex].impedance1 = value1;
                impedanceMapping[mappingIndex].impedance2 = value2;
            }
            else
            {
                Debug.LogError("UpdateImpedanceMapping: mappingIndex out of range (" + mappingIndex + ")");
            }
        }

        public void Apply()
        {
            UltrasoundSimulation sim = UltrasoundSimulation.Instance;

            sim.Frequency = defaultFrequency;
            sim.Gain = defaultGain;

            sim.Impedance0 = impedanceMapping[0].impedance1; // medium
            sim.Impedance1 = impedanceMapping[0].impedance2;

            sim.Impedance6 = impedanceMapping[1].impedance1; // pink
            sim.Impedance7 = impedanceMapping[1].impedance2;

            sim.Impedance9 = impedanceMapping[2].impedance1;  // turkis
            sim.Impedance10 = impedanceMapping[2].impedance2;

            sim.Impedance12 = impedanceMapping[3].impedance1;  // yellow
            sim.Impedance13 = impedanceMapping[3].impedance2;

        }

        /// <summary>
        /// Apply impedances from <see cref="MaterialConfig"/>.
        /// The configured impedances values are alternated over the range of indices covered by the <see cref="MaterialMapping.noiseRange"/>.
        /// </summary>
        /// <param name="materialConfig"></param>
        public void Apply(MaterialConfig materialConfig)
        {
            UltrasoundSimulation sim = UltrasoundSimulation.Instance;

            foreach (var mapping in materialConfig.map)
            {
                float step = 1.0f / 255.0f;
                float range = mapping.noiseRange + step;

                bool alternate = false;

                for (float i = 0; i < range; i += step)
                {
                    int matDelta = Mathf.RoundToInt(i * 255);

                    int impedanceIndex = mapping.materialIndex + matDelta;
                    //Debug.Log("impedanceIndex:" + impedanceIndex + " i=" + i);

                    if (impedanceIndex >= 0 && impedanceIndex < sim.ImpedanceArray.Length)
                    {
                        sim.ImpedanceArray[mapping.materialIndex + matDelta] = alternate ? mapping.impedance2 : mapping.impedance1;
                    }
                    else
                    {
                        Debug.LogError("Out of bounds (" + impedanceIndex + ") for materialIndex=" + mapping.materialIndex + " and matDelta=" + matDelta);
                    }

                    alternate = !alternate;
                }
            }
        }

        /// <summary> 
        /// Apply display parameters
        /// </summary>
        /// <param name="volume"></param>
        public void ApplySectorParameters(Volume volume)
        {
            volume.UltrasoundRenderer.SetSectorParameters(sectorAngleDeg, sectorVerticalOffset, sectorHorizontalStretch, sectorVerticalStretch);
        }
    }

}