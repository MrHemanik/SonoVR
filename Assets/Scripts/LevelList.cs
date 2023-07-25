using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using mKit;
using SonoGame;
using UnityEngine;

public class LevelList
{
        public List<Level> levelList;

        public LevelList(MaterialConfig mc)
        {
                var materialConfig = mc;
                levelList = new List<Level>
                {
                        new Level(LevelType.IdentifySliceFromObject,new List<List<ShapeConfig>>
                        {
                                new List<ShapeConfig>(){ //Volume 1
                                        new ShapeConfigVoxel(ShapeType.ELIPSOID,
                                                color: materialConfig.map[1].color,
                                                edgeWidth: 20,
                                                size: new Vector3(80, 80, 80),
                                                center: new Vector3(40, 100, 100),
                                                rotation: Quaternion.identity),
                                        new ShapeConfigVoxel(ShapeType.CUBOID,
                                                color: materialConfig.map[2].color,
                                                edgeWidth: 20,
                                                size: new Vector3(40, 40, 40),
                                                center: new Vector3(100, 100, 100),
                                                rotation: Quaternion.identity),
                                        new ShapeConfigVoxel(ShapeType.TUBE_Y,
                                                color: materialConfig.map[3].color,
                                                edgeWidth: 20,
                                                size: new Vector3(80, 80, 80),
                                                center: new Vector3(160, 100, 100),
                                                rotation: Quaternion.Euler(0, 0, 90))
                                },new List<ShapeConfig>(){ //Volume 2
                                        new ShapeConfigVoxel(ShapeType.ELIPSOID,
                                                color: materialConfig.map[1].color,
                                                edgeWidth: 20,
                                                size: new Vector3(80, 80, 80),
                                                center: new Vector3(40, 100, 100),
                                                rotation: Quaternion.identity),
                                        new ShapeConfigVoxel(ShapeType.CUBOID,
                                                color: materialConfig.map[3].color,
                                                edgeWidth: 20,
                                                size: new Vector3(40, 40, 40),
                                                center: new Vector3(80, 100, 120),
                                                rotation: Quaternion.identity),
                                        new ShapeConfigVoxel(ShapeType.TUBE_Y,
                                                color: materialConfig.map[2].color,
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
                                return "Untersuche das unsichtbare Volumen und finde heraus, welches der sichtbaren Volumen identisch ist!";
                        case LevelType.IdentifySliceFromHiddenObject:
                        case LevelType.IdentifySliceFromHiddenObjectAfterglow:
                                return "Untersuche das unsichtbare Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!";
                }

                return "Keine Beschreibung für dieses Level";
        }
}
public class Level
{
        public LevelType levelType;
        public List<List<ShapeConfig>> volumes;

        public Level(LevelType lt,List<List<ShapeConfig>> v)
        {
                levelType = lt;
                volumes = v;
        }
        
}