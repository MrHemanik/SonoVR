using System.Collections.Generic;
using mKit;
using SonoGame;
using UnityEngine;
using Volume = System.Collections.Generic.List<mKit.ShapeConfig>;

public class LevelList
{
    public List<Level> levelList;

    public LevelList(MaterialConfig mc)
    {
        var materialConfig = mc;
        List<ShapeType> shapes = new List<ShapeType>()
        {
            ShapeType.TUBE_X, ShapeType.TUBE_Y, ShapeType.TUBE_Z, ShapeType.CUBOID, ShapeType.SPLINE, ShapeType.ELIPSOID
        };
        List<Color> shapeColors = new List<Color>();
        mc.map.ForEach(map => shapeColors.Add(map.color));
        levelList = new List<Level>
        {
            //Level 1
            new Level(LevelType.IdentifySliceFromObject,
                new List<ShapeConfig> //Shapes in every Volume
                    {LevelHelper.GenerateBasicCube(materialConfig.map[4].color)},
                new List<List<ShapeConfig>> //Unique Shapes
                {
                    //Volume 1
                    new()
                    {
                        LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[3].color,
                            usesSlices: true)
                    },
                    //Volume 2
                    new()
                    {
                        LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[3].color,
                            size: new Vector3(100, 100, 100), usesSlices: true, edgeWidth: 10)
                    }
                }
            ),
            //Level 2
            new Level(LevelType.IdentifySliceFromObject,
                new List<ShapeConfig> //Shapes in every Volume
                {
                    LevelHelper.GenerateBasicCube(materialConfig.map[4].color),
                    LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[1].color,
                        usesSlices: true),
                    LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[2].color,
                        usesSlices: true),
                    new ShapeConfigVoxel(ShapeType.CUBOID,
                        color: materialConfig.map[2].color,
                        edgeWidth: 20,
                        size: new Vector3(50, 50, 50) + new Vector3(20, 20, 20) * Random.Range(-1f, 1f),
                        center: new Vector3(100 + Random.Range(-20f, 20.0f),
                            100 + Random.Range(-20f, 20.0f), 100 + Random.Range(-20f, 20.0f)),
                        rotation: Quaternion.identity),
                },
                new List<List<ShapeConfig>> //Unique Shapes
                {
                    new List<ShapeConfig>
                    {
                        //Volume 1
                        new ShapeConfigVoxel(ShapeType.TUBE_Y,
                            color: materialConfig.map[3].color,
                            edgeWidth: 20,
                            size: new Vector3(80, 80, 80),
                            center: new Vector3(160, 100, 100),
                            rotation: Quaternion.Euler(0, 0, 90))
                    },
                    new List<ShapeConfig>
                    {
                        //Volume 2
                        new ShapeConfigVoxel(ShapeType.TUBE_Y,
                            color: materialConfig.map[3].color,
                            edgeWidth: 20,
                            size: new Vector3(60, 60, 60),
                            center: new Vector3(160, 80, 100),
                            rotation: Quaternion.Euler(0, 0, 90))
                    },
                })
        };
    }
}

public enum LevelType
{
    IdentifyObjectFromObjectWithSlice,
    IdentifySliceFromObject,
    IdentifyObjectFromHiddenObject,
    IdentifySliceFromHiddenObject,
    IdentifyObjectFromHiddenObjectAfterglow,
    IdentifySliceFromHiddenObjectAfterglow,
}

public static class LevelHelper
{
    public static string TypeDescription(LevelType lt)
    {
        switch (lt)
        {
            case LevelType.IdentifyObjectFromObjectWithSlice:
                return "Untersuche die Volumen und finde heraus, welches der Volumen das Schnittbild darstellt!";
            case LevelType.IdentifySliceFromObject:
                return "Untersuche das Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!";
            case LevelType.IdentifyObjectFromHiddenObject:
            case LevelType.IdentifyObjectFromHiddenObjectAfterglow:
                return
                    "Untersuche das unsichtbare Volumen und finde heraus, welches der sichtbaren Volumen identisch ist!";
            case LevelType.IdentifySliceFromHiddenObject:
            case LevelType.IdentifySliceFromHiddenObjectAfterglow:
                return
                    "Untersuche das unsichtbare Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!";
        }

        return "Keine Beschreibung für dieses Level";
    }

    public static ShapeConfigVoxel GenerateBasicCube(Color color, Vector3? size = null, Vector3? center = null,
        Quaternion? rotation = null,
        int edgeWidth = 20)
    {
        return new ShapeConfigVoxel(ShapeType.CUBOID, edgeWidth, size ?? new Vector3(50, 50, 50), color,
            center ?? new Vector3(100, 100, 100), rotation ?? Quaternion.identity);
    }

    public static ShapeConfigVoxel GenerateRandomizedShape(ShapeType shape, Color color, Vector3? size = null,
        Vector3? center = null,
        Quaternion? rotation = null,
        int edgeWidth = 100, bool usesSlices = false)

    {
        //usesSlices is relevant when Slice is generated automatically to make sure that every shape is visible
        var zPositionOffset = usesSlices ? 0 : Random.Range(-40f, 40.0f);

        return new ShapeConfigVoxel(shape, edgeWidth,
            (size ?? new Vector3(50, 50, 50)) + new Vector3(20, 20, 20) * Random.Range(-0.5f, 1f),
            color,
            center ?? new Vector3(100 + Random.Range(-40f, 40.0f),
                100 + Random.Range(-40f, 40.0f), 100 + zPositionOffset),
            rotation ?? Quaternion.identity);
    }

    public static List<ShapeConfig> GenerateRandomizedShapes(List<ShapeType> shapes, List<Color> shapeColors,
        int amount)
    {
        var list = new List<ShapeConfig>();

        //Add Shapes to List
        for (int i = 0; i < amount; i++)
        {
            ShapeType shape = shapes[Random.Range(0, shapes.Count)];
            Color shapeColor = shapeColors[Random.Range(0, shapeColors.Count)];
            shapes.Remove(shape);
            shapeColors.Remove(shapeColor);
            list.Add(GenerateRandomizedShape(shapes[Random.Range(0, shapes.Count)],
                shapeColors[Random.Range(0, shapeColors.Count)]));
        }

        return list;
    }
}