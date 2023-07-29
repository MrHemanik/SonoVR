using System.Collections.Generic;
using mKit;

public class Level
{
    public LevelType levelType;
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