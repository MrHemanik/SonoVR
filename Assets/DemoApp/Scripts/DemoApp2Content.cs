using mKit;
using System.Collections.Generic;
using UnityEngine;


namespace SonoGame
{
    /// <summary>
    /// Generates a list of ShapeConfigList for multiple answer options.
    /// A single ShapeConfigList defines one volume.
    /// </summary>
    public class DemoContent
    {
        private readonly Color[] colors;
        private static int colorIndex = 0;

        public DemoContent()
        {
            colors = new Color[3] { Color.magenta, Color.cyan, Color.yellow };
        }

        /// <summary>
        /// Generate content for different game levels
        /// </summary>
        /// <param name="level">1 or 2</param>
        /// <returns>list of ShapeConfigList</returns>
        internal List<ShapeConfigList> DefineArtificialVolumeSet(int level)
        {
            Debug.Assert(level >= 1 && level <= 3, "Complexity range is 1-3, invalid complexity: " + level);

            List<ShapeConfigList> result = null;
            int volumeCount = level == 1 ? 2 : 3;

            switch (level)
            {
                case 1:
                    result = DistinctShape(volumeCount);
                    break;

                case 2:
                    result = DistinctHollowShape(volumeCount);
                    break;          
            }

            return result;
        }

        /// <summary>
        /// Create multiple instances of ShapeConfigList for multipel answer options.
        /// </summary>
        /// <param name="answerOptions">count of answeroptions</param>
        /// <returns>list of ShapeConfigList</returns>
        internal List<ShapeConfigList> DistinctShape(int answerOptions)
        {
            var color = GetNextColor();
            var shapeTypes = GetRandomShapeTypeArray(answerOptions, null);
            var size = new Vector3(0.5f, 0.5f, 0.5f);
            var center = size;
            var edgeWidth = 0.05f;

            var volumeList = new List<ShapeConfigList>();

            bool listHasSphere = System.Array.Exists(shapeTypes, st => st == ShapeType.ELIPSOID);

            foreach (var shapeType in shapeTypes)
            {
                var shapeRotation = shapeType == ShapeType.TUBE_Y && listHasSphere ? Quaternion.Euler(90, 0, 0) : Quaternion.identity;
                var shapeConfig = new ShapeConfig(type: shapeType, edgeWidth: edgeWidth, color: color, size: size, center: center, rotation: shapeRotation);

                var volumeShapes = new ShapeConfigList(new List<ShapeConfig>() { shapeConfig });
                volumeList.Add(volumeShapes);
            }

            return volumeList;
        }

        /// <summary>
        /// Create multiple instances of ShapeConfigList for multipel answer options.
        /// </summary>
        /// <param name="answerOptions">count of answeroptions</param>
        /// <returns>list of ShapeConfigList</returns>
        internal List<ShapeConfigList> DistinctHollowShape(int answerOptions)
        {
            var color = GetNextColor();
            var shapeType = GetRandomShapeTypeArray(1, null)[0];
            var shapeTypeInner = GetRandomShapeTypeArray(answerOptions, null);

            var size = new Vector3(0.5f, 0.5f, 0.5f);
            var center = new Vector3(0.5f, 0.5f, 0.5f);

            var innerSize = new Vector3(0.2f, 0.2f, 0.6f);

            var volumeList = new List<ShapeConfigList>();
            for (int i = 0; i < answerOptions; i++)
            {
                var edgeWidth1 = size.x;
                var shapeConfig = new ShapeConfig(type: shapeType, edgeWidth: edgeWidth1, color: color, size: size, center: center, rotation: Quaternion.identity);

                var edgeWidth2 = innerSize.x;
                var hollowShapeConfig = new ShapeConfig(type: shapeTypeInner[i], edgeWidth: edgeWidth2, color: Color.clear, size: innerSize, center: center, rotation: Quaternion.identity);

                var volumeShapes = new ShapeConfigList(new List<ShapeConfig>() { shapeConfig, hollowShapeConfig });
                volumeList.Add(volumeShapes);
            }

            return volumeList;
        }

        /// <summary>
        /// Return random <see cref="ShapeType"/>
        /// </summary>
        /// <param name="allowed">allowed shape types</param>
        /// <returns>single shapetype</returns>
        public ShapeType GetRandomShapeType(List<ShapeType> allowed)
        {
            if (allowed == null || allowed.Count == 0)
            {
                allowed = new List<ShapeType>() { ShapeType.CUBOID, ShapeType.ELIPSOID, ShapeType.TUBE_Y };
            }

            int max = allowed.Count;
            int candidate = Random.Range(0, max); 

            return allowed[candidate];
        }

        /// <summary>
        /// Return array of random <see cref="ShapeType"/> items.
        /// </summary>
        /// <param name="allowed">allowed shape types</param>
        /// <returns>array of shapetypes</returns>
        public ShapeType[] GetRandomShapeTypeArray(int count, List<ShapeType> allowed)
        {
            ShapeType[] result = new ShapeType[count];

            if (allowed == null || allowed.Count == 0)
            {
                allowed = new List<ShapeType>() { ShapeType.CUBOID, ShapeType.ELIPSOID, ShapeType.TUBE_Y };
            }

            if (count > allowed.Count)
            {
                Debug.LogError("GetRandomShapeTypes: count > allowed.Count (" + count + " > " + allowed.Count + ")");
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = GetRandomShapeType(allowed);
                    allowed.Remove(result[i]);
                }
            }

            return result;
        }

        private Color GetNextColor()
        {
            var result = colors[colorIndex++];

            if (colorIndex >= colors.Length)
            {
                colorIndex = 0;
            }

            return result;
        }

    }
}
