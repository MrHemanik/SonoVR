using System.Collections.Generic;
using mKit;

namespace Classes
{
    public class Level
    {
        public readonly LevelType levelType;
        public readonly List<List<ShapeConfig>> volumeList;

        public Level(LevelType lt, List<ShapeConfig> repeatingShapes, List<List<ShapeConfig>> volList)
        {
            levelType = lt;
            foreach (var volume in volList) //Adds every repeating shape to the volumeList
            {
                volume.AddRange(repeatingShapes);
            }
            volumeList = volList;
        }
        public Level(LevelType lt, List<List<ShapeConfig>> volList)
        {
            levelType = lt;
            volumeList = volList;
        }
    }
}