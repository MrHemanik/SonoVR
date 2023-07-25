using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using mKit;
using SonoGame;
using UnityEngine;
using Random = System.Random;

public class LevelList
{
    public List<Level> levelList;

    public LevelList(MaterialConfig mc)
    {
        var materialConfig = mc;
        levelList = new List<Level>
        {
            new Level(LevelType.IdentifySliceFromObject,
                new List<ShapeConfig> //Shapes in every Volume
                {
                    LevelHelper.GenerateBasicCube(materialConfig.map[4].color),
                    new ShapeConfigVoxel(ShapeType.CUBOID,
                        color: materialConfig.map[2].color,
                        edgeWidth: 20,
                        size: new Vector3(50, 50, 50) + new Vector3(20, 20, 20) * UnityEngine.Random.Range(-1f, 1f),
                        center: new Vector3(100 + UnityEngine.Random.Range(-20f, 20.0f),
                            100 + UnityEngine.Random.Range(-20f, 20.0f), 100 + UnityEngine.Random.Range(-20f, 20.0f)),
                        rotation: Quaternion.identity),
                },
                new List<List<ShapeConfig>> //Unique Shapes
                {
                    new List<ShapeConfig>()
                    {
                        //Volume 1
                        new ShapeConfigVoxel(ShapeType.TUBE_Y,
                            color: materialConfig.map[3].color,
                            edgeWidth: 20,
                            size: new Vector3(80, 80, 80),
                            center: new Vector3(160, 100, 100),
                            rotation: Quaternion.Euler(0, 0, 90))
                    },
                    new List<ShapeConfig>()
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

    public static ShapeConfigVoxel GenerateBasicCube(Color color, Vector3? size = null, Vector3? center = null, Quaternion? rotation = null,
        int edgeWidth = 20)
    {
        return new ShapeConfigVoxel(ShapeType.CUBOID, edgeWidth,size ?? new Vector3(50, 50, 50), color,  center ?? new Vector3(100, 100, 100), rotation?? Quaternion.identity);
    }
}

public class Level
{
    public LevelType levelType;
    public List<ShapeConfig> repeatingShapes; //Shapes that are used in every volume
    public List<List<ShapeConfig>> volumeList;
    
    public Level(LevelType lt, List<ShapeConfig> repeatingShapes, List<List<ShapeConfig>> volList)
    {
        levelType = lt;
        foreach (var volume in volList) //Adds every repeating shape to the volumeList
        {
            volume.AddRange(repeatingShapes);
        }

        volumeList = volList;
    }
}