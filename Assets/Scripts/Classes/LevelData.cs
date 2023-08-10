using System;

namespace Classes
{
    [Serializable]
    public class LevelData
    {
        public bool answeredRight;
        public float time;
        public ObjectType compareObject;
        public ObjectType answerObject;

        public LevelData(ObjectType inputCompareObject, ObjectType inputAnswerObject,
            bool inputAnsweredRight, float inputTime)
        {
            compareObject = inputCompareObject;
            answerObject = inputAnswerObject;
            answeredRight = inputAnsweredRight;
            time = inputTime;
        }

        public string ToCsvString()
        {
            return $"{answeredRight};{time:F6};{compareObject};{answerObject};";
        }
    }
}