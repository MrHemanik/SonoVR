using System;
using System.Collections.Generic;
using System.Text;

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


        public string ToCsvString()
        {
            var sb = new StringBuilder();
            sb.Append($"{startDate:F6};{endDate:F6};{overallTime:F6};");
            foreach (var data in levelData)
            {
                sb.Append(data.ToCsvString());
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();
            return sb.ToString();
        }

        public string ToCsvHeaderString()
        {
            var sb = new StringBuilder();
            sb.Append("StartDate;EndDate;OverallTime;");
            for (var i = 0; i < levelData.Count; i++)
            {
                sb.Append($"{i}_AnsweredRight;{i}_Time;{i}_CompareObject;{i}_AnswerObject;");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}