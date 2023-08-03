using System.Collections.Generic;
using System.Linq;
using mKit;
using SonoGame;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;

namespace Classes
{
    /// <summary>
    /// Class that contains all levels
    /// </summary>
    public class LevelList
    {
        /// <summary>
        /// List of all levels used
        /// </summary>
        public readonly List<Level> levelList;

        /// <summary>
        /// List of all allowed shapes that can generate
        /// </summary>
        private static readonly List<ShapeType> Shapes = new List<ShapeType>()
        {
            ShapeType.TUBE_X, ShapeType.TUBE_Y, ShapeType.TUBE_Z, ShapeType.CUBOID, ShapeType.ELIPSOID
        };

        /// <summary>
        /// List of all allowed shapeColors for the shapes that can generate
        /// </summary>
        public List<Color> shapeColors = new List<Color>();

        /// <summary>
        /// Base Constructor filling levelList and shapeColors
        /// </summary>
        public LevelList()
        {
            var materialConfig = VolumeManager.Instance.materialConfig;
            materialConfig.map.ForEach(map => shapeColors.Add(map.color));
            shapeColors.RemoveAt(0); //The first color is black that isnt used for volumeGeneration
            levelList = new List<Level>
            {
                LevelHelper.GenerateRandomizedLevel(LevelType.LevelTypes[3], GenerationType.SameShape, Shapes,
                    shapeColors, 1, 3),
                LevelHelper.GenerateRandomizedLevel(LevelType.LevelTypes[0], GenerationType.DifferentShape, Shapes,
                    shapeColors, 1, 3, true),
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
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(Shapes, shapeColors, 2, true),
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
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(Shapes, shapeColors, 3, true),
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
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(Shapes, shapeColors, 4, true),
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
                new Level(LevelType.LevelTypes[0], LevelHelper.GenerateRandomizedShapes(Shapes, shapeColors, 2, true),
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
                LevelHelper.GenerateRandomizedLevel(LevelType.LevelTypes[0], GenerationType.DifferentShape, Shapes,
                    shapeColors, 3, 2)
            };
        }
    }

    /// <summary>
    /// Type of Volume generation differentiation. They always have different Position, different Rotation, different Size
    /// </summary>
    public enum GenerationType
    {
        /// <summary>
        /// Same Shape, different Color
        /// </summary>
        SameShape,

        /// <summary>
        /// Different Shape, same Color
        /// </summary>
        DifferentShape,
    }

    /// <summary>
    /// Type of object used for answers or compare object
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// A Slice is a two-dimensional image from a volume.
        /// It is never examinable with a probe
        /// </summary>
        Slice,

        /// <summary>
        /// A Volume is a three-dimensional object with visible content
        /// It is only examinable with a probe if the other ObjectType is slice
        /// </summary>
        Volume,

        /// <summary>
        /// A HiddenVolume is a three-dimensional object with invisible content
        /// It it always examinable with a probe, as it is the only way to see the content
        /// </summary>
        HiddenVolume,

        /// <summary>
        /// A HiddenVolumeAfterimage is a three-dimensional object with invisible content that reconstructs with examining
        /// It it always examinable with a probe
        /// </summary>
        HiddenVolumeAfterimage
    }

    /// <summary>
    /// Defines what object group is examinable with the probe
    /// </summary>
    public enum ProbeType
    {
        /// <summary>
        /// Type of object the player gets to compare to the answerOptions, so always the right answer
        /// </summary>
        CompareObject,

        /// <summary>
        /// Objects that can be chosen as an answer
        /// </summary>
        AnswerOptions
    }

    /// <summary>
    /// Class with all Level generation functions
    /// </summary>
    public static class LevelHelper
    {
        private static Vector3 baseCenter = new Vector3(100, 100, 100);
        private static Vector3 baseSize = new Vector3(50, 50, 50);

        /// <summary>
        /// Generates a Level based on parameter inputs
        /// </summary>
        /// <param name="lt">LevelType the Level will have</param>
        /// <param name="gt">GenerationType the unique Shapes in each volume will have</param>
        /// <param name="shapes">List of shapes that can generate</param>
        /// <param name="shapeColors">List of colors the shapes can have</param>
        /// <param name="extraShapes">Amount of same shapes each volume should have</param>
        /// <param name="volumes">Amount of volumes the level should have</param>
        /// <param name="randomRotation">If Shapes should be randomly rotated</param>
        /// <returns>Level based on input parameter</returns>
        public static Level GenerateRandomizedLevel(LevelType lt, GenerationType gt, List<ShapeType> shapes,
            List<Color> shapeColors, int extraShapes, int volumes, bool randomRotation = false)
        {
            var usesSlices = lt.answerOptions == ObjectType.Slice || lt.compareObject == ObjectType.Slice;
            if (randomRotation)
            {
                //If randomRotation is active, the amount of tubes will be lessend to only 1, as *_X,*_Y,*_Z are the same then disregarding rotation
                shapes = shapes
                    .ToList(); //If not done it will overwrite the STATIC READONLY shapes. I don't know why it does that in the first place though
                shapes.Remove(ShapeType.TUBE_Y);
                shapes.Remove(ShapeType.TUBE_Z);
            }

            var (commonShapes, remainingShapes, remainingColors) =
                GenerateRandomizedShapesOutputLists(shapes, shapeColors, extraShapes, randomRotation, usesSlices);

            var volList = new List<List<ShapeConfig>>();
            if (gt == GenerationType.DifferentShape)
            {
                //Generate a different distinct shape with the same color for every volume (Position/Rotation is also different)
                var distinctShapeColor = new List<Color>() {GetRandomShapeColor(shapeColors, ref remainingColors)};
                for (int i = 0; i < volumes; i++)
                {
                    List<ShapeConfig> volume;
                    (volume, remainingShapes, remainingColors) = GenerateRandomizedShapesOutputLists(
                        shapes, distinctShapeColor, 1, randomRotation, usesSlices, remainingShapes);
                    volume.AddRange(commonShapes);
                    volList.Add(volume);
                }
            }
            else if (gt == GenerationType.SameShape)
            {
                //Generate the same shape with a different distinct color for every volume (Position/Rotation is also different)
                var distinctShape = new List<ShapeType>() {GetRandomShape(shapes, ref remainingShapes)};
                for (int i = 0; i < volumes; i++)
                {
                    List<ShapeConfig> volume;
                    (volume, remainingShapes, remainingColors) = GenerateRandomizedShapesOutputLists(distinctShape,
                        shapeColors, 1, randomRotation, usesSlices, remainingShapeColors: remainingColors);
                    volume.AddRange(commonShapes);
                    volList.Add(volume);
                }
            }

            return new Level(lt, volList);
        }

        /// <summary>
        /// Outputs a random shape from remainingShapes. Generated shape will be removed from the reference of remainingShapes. If no shapes are left, all possible shapes will be put back in remainingShapes
        /// </summary>
        /// <param name="allShapes">List of shapes that are allowed in general</param>
        /// <param name="remainingShapes">Reference of list of shapes that can be currently generated. Will get modified to remove generated shape from list</param>
        /// <returns>Random Shape</returns>
        private static ShapeType GetRandomShape(List<ShapeType> allShapes, ref List<ShapeType> remainingShapes)
        {
            var shape = remainingShapes[Random.Range(0, remainingShapes.Count - 1)];
            remainingShapes.Remove(shape);
            if (remainingShapes.Count == 0) remainingShapes.AddRange(allShapes);
            return shape;
        }

        /// <summary>
        /// Outputs a random shapeColor from remainingShapeColors. Generated shapeColor will be removed from the reference of remainingShapeColors. If no shapeColors are left, all possible shapeColors will be put back in remainingShapeColors
        /// </summary>
        /// <param name="allShapes">List of shapeColors that are allowed in general</param>
        /// <param name="remainingShapes">Reference of list of shapeColors that can be currently generated. Will get modified to remove generated shapeColor from list</param>
        /// <returns>Random Color</returns>
        private static Color GetRandomShapeColor(List<Color> allShapeColors, ref List<Color> remainingShapeColors)
        {
            var shapeColor = remainingShapeColors[Random.Range(0, remainingShapeColors.Count - 1)];
            remainingShapeColors.Remove(shapeColor);
            if (remainingShapeColors.Count == 0) remainingShapeColors.AddRange(allShapeColors);
            return shapeColor;
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
        /// <returns>List of randomized shapes</returns>
        public static List<ShapeConfig> GenerateRandomizedShapes(List<ShapeType> shapes, List<Color> shapeColors,
            int amount, bool rotation = false, bool usesSlices = false)
        {
            return GenerateRandomizedShapesOutputLists(shapes, shapeColors, amount, rotation, usesSlices).Item1;
        }

        /// <summary>
        /// Generates a list of randomized ShapeConfigs. Leftover shapes and colors will be returned as well
        /// </summary>
        /// <param name="allShapes">List of shapes that are allowed </param>
        /// <param name="allShapeColors">List of shapeColors that are allowed</param>
        /// <param name="amount">amount of shapes that should be generated</param>
        /// <param name="rotation">if shapes should have a random rotation</param>
        /// <param name="usesSlices">if slices will be generated from them</param>
        /// <param name="remainingShapes">Optional: List of shapes that are allowed, modified to not all shapes</param>
        /// <param name="remainingShapeColors">Optional: List of shapeColors that are allowed, modified to not all shapeColors</param>
        /// <returns>Tuple with List of random shapeConfigs, shapes that were not used/remain and shapeColors that were not used/remain</returns>
        private static (List<ShapeConfig>, List<ShapeType>, List<Color>) GenerateRandomizedShapesOutputLists(
            List<ShapeType> allShapes, List<Color> allShapeColors, int amount, bool rotation = false,
            bool usesSlices = false, List<ShapeType> remainingShapes = null, List<Color> remainingShapeColors = null)
        {
            var list = new List<ShapeConfig>();
            if (remainingShapes == null) remainingShapes = allShapes.ToList();
            if (remainingShapeColors == null) remainingShapeColors = allShapeColors.ToList();
            //Generate unique Shape with unique color and add to List. If either cant be unique refill possibilities
            for (int i = 0; i < amount; i++)
            {
                ShapeType shape = GetRandomShape(allShapes, ref remainingShapes);
                Color shapeColor = GetRandomShapeColor(allShapeColors, ref remainingShapeColors);
                list.Add(GenerateRandomizedShape(shape, shapeColor, rotation: rotation ? null : Quaternion.identity,
                    usesSlices: usesSlices));
            }

            return (list, remainingShapes, remainingShapeColors);
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
        /// <returns>ShapeConfig of a random shape</returns>
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