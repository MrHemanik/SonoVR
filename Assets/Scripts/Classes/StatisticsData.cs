using System;
using System.Collections.Generic;

namespace Classes
{
    [Serializable]
    public class StatisticsData
    {
        public string startDate;
        public string endDate;
        public float overallTime;
        public List<LevelData> levelData = new List<LevelData>();

        public StatisticsData(string dateTimeFormat)
        {
            startDate = DateTime.Now.ToString(dateTimeFormat);
        }
    }
}