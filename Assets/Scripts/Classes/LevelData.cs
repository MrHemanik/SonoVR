using System;

namespace Classes
{
    [Serializable]
    public class LevelData
    {
        public int levelId;
        public bool answeredRight;
        public float time;
        public ObjectType compareObject;
        public ObjectType answerObject;

        public LevelData(int inputLevelId, ObjectType inputCompareObject, ObjectType inputAnswerObject, bool inputAnsweredRight, float inputTime)
        {
            levelId = inputLevelId;
            compareObject = inputCompareObject;
            answerObject = inputAnswerObject;
            answeredRight = inputAnsweredRight;
            time = inputTime;
        }
    }
}