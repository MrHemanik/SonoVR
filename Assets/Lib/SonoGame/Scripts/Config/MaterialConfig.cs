using mKit;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    [CreateAssetMenu(fileName = "MaterialConfig", menuName = "ScriptableObjects/Sonogame MaterialConfig", order = 1)]
    public class MaterialConfig : ScriptableObject
    {
        public NoiseType noiseType = NoiseType.RANGE;

        [NonReorderable]
        public List<MaterialMapping> map;

        /// <summary>
        /// Get the shape config list for rendering. Color alpha and noise parameters are changed according to MaterialConfig.
        /// </summary>
        /// <param name="shapeConfigList"></param>
        /// <returns></returns>
        public List<ShapeConfig> GetRenderShapeConfigList(List<ShapeConfig> shapeConfigList)
        {
            UpdateMaterialIndexForNoiseType();

            var result = new List<ShapeConfig>();

            if (shapeConfigList != null)
            {
                foreach (var item in shapeConfigList)
                {
                    var renderItem = (ShapeConfig)item.Clone();

                    switch (noiseType)
                    {
                        case NoiseType.NONE:
                            UpdateShapeConfigForSingleColor(renderItem);
                            break;
                        case NoiseType.RANGE:
                            UpdateShapeConfigForNoisedRange(renderItem);
                            break;
                        case NoiseType.ALTERNATING:
                            UpdateShapeConfigForAlternatingColors(renderItem);
                            break;
                        default:
                            Debug.LogError("GetRenderShapeConfigList: noiseType not supported (" + noiseType + ")");
                            break;
                    }

                    result.Add(renderItem);
                }
            }

            return result;
        }

        /// <summary>
        /// Distribute material indices (used for transfer function) depending on noise type.
        /// </summary>
        /// <param name="noiseType"></param>
        public void UpdateMaterialIndexForNoiseType()
        {
            byte currentMaterialIndex = 0;

            for (int i = 0; i < map.Count; i++)
            {
                map[i].materialIndex = currentMaterialIndex;

                switch (noiseType)
                {
                    case NoiseType.NONE:
                        currentMaterialIndex++;
                        break;
                    case NoiseType.RANGE:
                        currentMaterialIndex += (byte)(1 + Mathf.RoundToInt(map[i].noiseRange * 255.0f)); // noise range defines necessary increase of index
                        break;
                    case NoiseType.GAUSS:
                        Debug.LogError("GAUSS NOISE index distribution not available");
                        break;
                    case NoiseType.ALTERNATING:
                        currentMaterialIndex += 2; // add 2 for alternating colors
                        break;
                }
            }

        }

        /// <summary>
        /// Updates the shape color with an alpha value used for material mapping and uses <see cref="MaterialMapping.color1"/>.
        /// </summary>
        /// <param name="shapeConfig"></param>
        private bool UpdateShapeConfigForSingleColor(ShapeConfig shapeConfig)
        {
            bool updated = false;

            for (int i = 0; i < map.Count; i++)
            {
                var m = map[i];

                if (m.color.r == shapeConfig.color.r && m.color.g == shapeConfig.color.g && m.color.b == shapeConfig.color.b)
                {
                    shapeConfig.color = m.color1;
                    //shapeConfig.color2 = m.color2;

                    shapeConfig.color.a = m.materialIndex / 255.0f; // alpha channel used for mapping
                    //shapeConfig.color2.a = (m.materialIndex + 1) / 255.0f; //alpha channel used for mapping
                    shapeConfig.noise = NoiseType.NONE;

                    updated = true;
                }
            }

            if (!updated)
            {
                Debug.LogError("MaterialConfig.UpdateShapeConfigForAlternatingColors: no entry for " + ((Color32)shapeConfig.color) + " in " + name);
            }

            return updated;
        }

        /// <summary>
        /// Updates the shape color with an alpha value used for material mapping and applies the noise range from the <see cref="MaterialConfig"/>
        /// </summary>
        /// <param name="shapeConfig"></param>
        private bool UpdateShapeConfigForAlternatingColors(ShapeConfig shapeConfig)
        {
            bool updated = false;

            for (int i = 0; i < map.Count; i++)
            {
                var m = map[i];

                if (m.color.r == shapeConfig.color.r && m.color.g == shapeConfig.color.g && m.color.b == shapeConfig.color.b)
                {
                    shapeConfig.color = m.color1;
                    shapeConfig.color2 = m.color2;

                    shapeConfig.color.a = m.materialIndex / 255.0f; // alpha channel used for mapping
                    shapeConfig.color2.a = (m.materialIndex + 1) / 255.0f; //alpha channel used for mapping
                    shapeConfig.noise = NoiseType.ALTERNATING;

                    updated = true;
                }
            }

            if (!updated)
            {
                Debug.LogError("MaterialConfig.UpdateShapeConfigForAlternatingColors: no entry for " + ((Color32)shapeConfig.color) + " in " + name);
            }

            return updated;
        }

        /// <summary>
        /// Updates the shape color with an alpha value used for material mapping and applies the noise range from the <see cref="MaterialConfig"/>
        /// </summary>
        /// <param name="shapeConfig"></param>
        private bool UpdateShapeConfigForNoisedRange(ShapeConfig shapeConfig)
        {
            bool updated = false;

            foreach (var m in map)
            {
                if (m.color.r == shapeConfig.color.r && m.color.g == shapeConfig.color.g && m.color.b == shapeConfig.color.b)
                {
                    shapeConfig.color.a = m.materialIndex / 255.0f; // alpha channel used for mapping
                    shapeConfig.noise = NoiseType.RANGE;
                    shapeConfig.noiseRange = m.noiseRange;

                    updated = true;
                }
            }

            if (!updated)
            {
                Debug.LogError("MaterialConfig.UpdateShapeConfigForNoisedRange: no entry for " + ((Color32)shapeConfig.color) + " in " + name);
            }

            return updated;
        }

        public Color GetColor(Color color)
        {
            foreach (var m in map)
            {
                if (m.color.r == color.r && m.color.g == color.g && m.color.b == color.b)
                {
                    return m.color;
                }
            }

            Debug.LogError("MaterialConfig.GetColor: no entry for " + color + ", using Color.white");
            return Color.white;
        }

        public void CreateTransferFunction(EVisualization sliceVis, Color32[] texel32)
        {
            foreach (var mapping in map)
            {
                switch (noiseType)
                {
                    case NoiseType.NONE:
                        texel32[mapping.materialIndex] = sliceVis == EVisualization.Colored ? mapping.color1 : mapping.grayscale1;
                        break;

                    case NoiseType.ALTERNATING:
                        texel32[mapping.materialIndex] = sliceVis == EVisualization.Colored ? mapping.color1 : mapping.grayscale1;
                        texel32[mapping.materialIndex + 1] = sliceVis == EVisualization.Colored ? mapping.color2 : mapping.grayscale2;
                        break;

                    case NoiseType.RANGE:
                        float step = 1.0f / 255.0f;
                        float range = mapping.noiseRange + step;

                        for (float i = 0; i < range; i += step)
                        {
                            int matDelta = Mathf.RoundToInt(i * 255);

                            Color color = sliceVis == EVisualization.Colored ? mapping.color : mapping.grayscale;

                            color.r = Mathf.Clamp01(color.r - i);
                            color.g = Mathf.Clamp01(color.g - i);
                            color.b = Mathf.Clamp01(color.b - i);

                            int texIndex = mapping.materialIndex + matDelta;
                            //Debug.Log("texIndex:" + texIndex+" i="+i);

                            if (texIndex >= 0 && texIndex < texel32.Length)
                            {
                                texel32[mapping.materialIndex + matDelta] = color;
                            }
                            else
                            {
                                Debug.LogError("Out of bounds (" + texIndex + ") for materialIndex=" + mapping.materialIndex + " and matDelta=" + matDelta);
                            }
                        }

                        break;

                    default:
                        Debug.LogError("GetRenderShapeConfigList: noiseType not supported (" + noiseType + ")");
                        break;

                }
            }
        }
    }
}
