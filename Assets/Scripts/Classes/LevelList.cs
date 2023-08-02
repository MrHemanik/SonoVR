using System.Collections.Generic;
using System.Linq;
using mKit;
using SonoGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Classes
{
    public class LevelList
    {
        public readonly List<Level> levelList;

        public static List<ShapeType> shapes = new List<ShapeType>()
        {
            ShapeType.TUBE_X, ShapeType.TUBE_Y, ShapeType.TUBE_Z, ShapeType.CUBOID, ShapeType.ELIPSOID
        };

        public List<Color> shapeColors = new List<Color>();

        public LevelList()
        {
            var materialConfig = VolumeManager.Instance.materialConfig;
            materialConfig.map.ForEach(map => shapeColors.Add(map.color));
            shapeColors.RemoveAt(0); //The first color is black that isnt used for volumeGeneration
            levelList = new List<Level>
            {
                LevelHelper.GenerateRandomizedLevel(LevelType.LevelTypes[2], GenerationType.DifferentShape, shapes,shapeColors,1,3),
                //Level 1
                new Level(LevelType.LevelTypes[0],
                    new List<ShapeConfig> // Shapes in every Volume
                        {LevelHelper.GenerateBasicCube(materialConfig.map[4].color)},
                    new List<List<ShapeConfig>> // Unique Shapes
                    {
                        // Volume 1
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[3].color,
                                usesSlices: true, rotation: Quaternion.identity)
                        },
                        // Volume 2
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[3].color,
                                size: new Vector3(100, 100, 100), usesSlices: true, edgeWidth: 10,
                                rotation: Quaternion.identity)
                        }
                    }
                ),
                //Level 2
                new Level(LevelType.LevelTypes[0],
                    new List<ShapeConfig> // Shapes in every Volume
                    {
                        LevelHelper.GenerateBasicCube(materialConfig.map[4].color),
                        LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[1].color,
                            usesSlices: true, rotation: Quaternion.identity),
                        LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[2].color,
                            usesSlices: true, rotation: Quaternion.identity),
                        new ShapeConfigVoxel(ShapeType.CUBOID,
                            color: materialConfig.map[2].color,
                            edgeWidth: 20,
                            size: new Vector3(50, 50, 50) + new Vector3(20, 20, 20) * Random.Range(-1f, 1f),
                            center: new Vector3(100 + Random.Range(-20f, 20.0f),
                                100 + Random.Range(-20f, 20.0f), 100 + Random.Range(-20f, 20.0f)),
                            rotation: Quaternion.identity),
                    },
                    new List<List<ShapeConfig>> // Unique Shapes
                    {
                        new List<ShapeConfig>
                        {
                            // Volume 1
                            new ShapeConfigVoxel(ShapeType.TUBE_Y,
                                color: materialConfig.map[3].color,
                                edgeWidth: 20,
                                size: new Vector3(80, 80, 80),
                                center: new Vector3(160, 100, 100),
                                rotation: Quaternion.Euler(0, 0, 90))
                        },
                        new List<ShapeConfig>
                        {
                            // Volume 2
                            new ShapeConfigVoxel(ShapeType.TUBE_Y,
                                color: materialConfig.map[3].color,
                                edgeWidth: 20,
                                size: new Vector3(60, 60, 60),
                                center: new Vector3(160, 80, 100),
                                rotation: Quaternion.Euler(0, 0, 90))
                        },
                    }),
                //Level 3
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(shapes, shapeColors, 2, true),
                    new List<List<ShapeConfig>>
                    {
                        new() // Volume 1
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[2].color,
                                usesSlices: true)
                        },
                        new() // Volume 2
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.CUBOID, materialConfig.map[2].color,
                                size: new Vector3(50, 50, 50), usesSlices: true, edgeWidth: 10)
                        }
                    }
                ),
                //Level 4
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(shapes, shapeColors, 3, true),
                    new List<List<ShapeConfig>>
                    {
                        new() // Volume 1
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_Y, materialConfig.map[1].color,
                                size: new Vector3(50, 50, 50), usesSlices: true)
                        },
                        new() // Volume 2
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_Y, materialConfig.map[1].color,
                                size: new Vector3(50, 50, 50), usesSlices: true, edgeWidth: 10)
                        }
                    }
                ),
                //Level 4
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(shapes, shapeColors, 4, true),
                    new List<List<ShapeConfig>>
                    {
                        new() // Volume 1
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_Y, materialConfig.map[1].color,
                                size: new Vector3(50, 50, 50), usesSlices: true)
                        },
                        new() // Volume 2
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_Y, materialConfig.map[1].color,
                                size: new Vector3(50, 50, 50), usesSlices: true, edgeWidth: 10)
                        }
                    }
                ),
                //Level 5
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(shapes, shapeColors, 2, true),
                    new List<List<ShapeConfig>>
                    {
                        new(), // Volume 1
                        new() // Volume 2
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.CUBOID, materialConfig.map[5].color,
                                size: new Vector3(50, 50, 50), usesSlices: true, edgeWidth: 10)
                        }
                    }
                ),
                LevelHelper.GenerateRandomizedLevel(LevelType.LevelTypes[0], GenerationType.DifferentShape, shapes,shapeColors,3,2)
            };
        }
    }
    /// <summary>
    /// Type of Volume differentiation 
    /// </summary>
    public enum GenerationType
    {
        SameShape, //Different size
        DifferentShape, //Different shape + different size
        ExtraShape // One volume will not be having an extra shape
    }

    public enum ObjectType
    {
        Slice, //Probable = never
        Volume, //Probable if answer = slice
        HiddenVolume, //Probable if answer = slice or volume
        HiddenVolumeAfterglow
    }

    public enum ProbeType
    {
        CompareObject, //Type of object the player gets to compare to the answerOptions, so always the right answer
        AnswerOptions
    }


    public static class LevelHelper
    {
        private static Vector3 baseCenter = new Vector3(100, 100, 100);
        private static Vector3 baseSize = new Vector3(50, 50, 50);


        public static Level GenerateRandomizedLevel(LevelType lt, GenerationType gt, List<ShapeType> shapes,
            List<Color> shapeColors, int extraShapes, int volumes)
        {
            var usesSlices = lt.answerOptions == ObjectType.Slice || lt.compareObject == ObjectType.Slice;
            var (commonShapes, remainingShapes, remainingColors) =
                GenerateRandomizedShapesOutputLists(shapes, shapeColors, extraShapes, usesSlices: usesSlices);
            var distinctShapeColor = remainingColors[Random.Range(0, remainingColors.Count - 1)];
            
            
            //TODO: rework this part to use GenerateRandomizedShapes or variation of it.
            //Case for when GenerationType is DifferentShape
            if (remainingShapes.Count < volumes)
            {
                var missingShapesAmount = volumes - remainingShapes.Count;
                //All shapes that are not in the remaining shapes
                var missingShapes = new List<ShapeType>(from shape in shapes where !remainingShapes.Contains(shape) select shape);
                //Add a shape that is missing until there
                for (int i = 0; i < missingShapesAmount; i++)
                {
                    var missingShape = missingShapes[Random.Range(0, missingShapes.Count - 1)];
                    remainingShapes.Add(missingShape);
                    missingShapes.Remove(missingShape);
                }
                remainingShapes = shapes;
            }
            var volList = new List<List<ShapeConfig>>();
            //Generates one unique shape per volume
            for (int i = 0; i < volumes; i++)
            {
                var shapeConfig = new List<ShapeConfig>();
                var shape = remainingShapes[Random.Range(0, remainingShapes.Count - 1)];
                shapeConfig.Add(GenerateRandomizedShape(shape,distinctShapeColor, usesSlices:usesSlices));
                shapeConfig.AddRange(commonShapes);
                remainingShapes.Remove(shape);
                volList.Add(shapeConfig);
            }
            
            return new Level(lt, volList);
        }

        #region Shape generation

        public static ShapeConfigVoxel GenerateBasicCube(Color color, Vector3? size = null, Vector3? center = null,
            Quaternion? rotation = null, int edgeWidth = 20)
        {
            return new ShapeConfigVoxel(ShapeType.CUBOID, edgeWidth, size ?? baseSize, color,
                center ?? baseCenter, rotation ?? Quaternion.identity);
        }

        /// <summary>
        /// Generates a list of randomized ShapeConfigs.
        /// </summary>
        /// <param name="shapes">List of shapes that are allowed </param>
        /// <param name="shapeColors">List of shapeColors that are allowed</param>
        /// <param name="amount">amount of shapes that should be generated</param>
        /// <param name="rotation">if shapes should have a random rotation</param>
        /// <param name="usesSlices">if slices will be generated from them</param>
        /// <returns></returns>
        public static List<ShapeConfig> GenerateRandomizedShapes(List<ShapeType> shapes, List<Color> shapeColors,
            int amount, bool rotation = false, bool usesSlices = false)
        {
            return GenerateRandomizedShapesOutputLists(shapes, shapeColors, amount, rotation, usesSlices).Item1;
        }

        /// <summary>
        /// Generates a list of randomized ShapeConfigs. Leftover shapes and colors will be returned as well
        /// </summary>
        /// <param name="shapes">List of shapes that are allowed </param>
        /// <param name="shapeColors">List of shapeColors that are allowed</param>
        /// <param name="amount">amount of shapes that should be generated</param>
        /// <param name="rotation">if shapes should have a random rotation</param>
        /// <param name="usesSlices">if slices will be generated from them</param>
        /// <returns></returns>
        private static (List<ShapeConfig>, List<ShapeType>, List<Color>) GenerateRandomizedShapesOutputLists(
            List<ShapeType> shapes, List<Color> shapeColors, int amount, bool rotation = false, bool usesSlices = false)
        {
            var list = new List<ShapeConfig>();
            List<ShapeType> shapesCopy = shapes.ToList();
            List<Color> shapeColorsCopy = shapeColors.ToList();
            //Generate unique Shape with unique color and add to List. If either cant be unique refill possibilities
            for (int i = 0; i < amount; i++)
            {
                ShapeType shape = shapesCopy[Random.Range(0, shapesCopy.Count - 1)];
                Color shapeColor = shapeColorsCopy[Random.Range(0, shapeColorsCopy.Count - 1)];
                //Remove generated from list, if list is empty afterwards, refill list.
                shapesCopy.Remove(shape);
                if (shapesCopy.Count == 0) shapesCopy.AddRange(shapes);
                shapeColorsCopy.Remove(shapeColor);
                if (shapeColorsCopy.Count == 0)
                    shapeColorsCopy.AddRange(shapeColors);
                list.Add(GenerateRandomizedShape(shape, shapeColor, rotation: rotation ? null : Quaternion.identity,
                    usesSlices: usesSlices));
            }

            return (list, shapes, shapeColors);
        }

        /// <summary>
        /// Generates a randomized shape of type ShapeConfigVoxel
        /// </summary>
        /// <param name="shape">ShapeType the shape will have</param>
        /// <param name="color">Color the shape will have</param>
        /// <param name="size">Size of the shape. Will be random if not declared</param>
        /// <param name="center">Position of the shape relative to the center. Will be random if not declared</param>
        /// <param name="rotation">Rotation of the shape. Will be random if not declared</param>
        /// <param name="edgeWidth">Thickness of the shape</param>
        /// <param name="usesSlices">If slices will be generated from the shape</param>
        /// <returns></returns>
        public static ShapeConfigVoxel GenerateRandomizedShape(ShapeType shape, Color color, Vector3? size = null,
            Vector3? center = null, Quaternion? rotation = null, int edgeWidth = 100, bool usesSlices = false)
        {
            return new ShapeConfigVoxel(shape, edgeWidth, size ?? RandomSize(), color,
                center ?? RandomPosition(usesSlices), rotation ?? RandomRotation());
        }

        #endregion

        #region Randomize base functions

        private static Vector3 RandomSize()
        {
            return baseSize + new Vector3(30, 30, 30) * Random.Range(-0.3f, 1f);
        }

        private static Vector3 RandomPosition(bool usesSlices)
        {
            Debug.Log(usesSlices);
            return baseCenter + new Vector3(Random.Range(-50f, 50.0f), Random.Range(-50f, 50.0f),
                usesSlices ? 0 : Random.Range(-50f, 50.0f));
        }

        private static Quaternion RandomRotation()
        {
            return Quaternion.Euler(new Vector3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f),
                Random.Range(0.0f, 360.0f)));
        }

        #endregion
    }
}