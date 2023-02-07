using System;
using UnityEngine;

namespace SonoGame
{
    [Serializable]
    public class ChallengeParameters
    {
        public string miniGame;
        public EVisualization visualization = EVisualization.Colored;
        [Range(1, 9)] public int level = 1;

        public ChallengeParameters(string miniGame, EVisualization visualization, int level)
        {
            this.miniGame = miniGame;
            this.visualization = visualization;
            this.level = level;
        }

        public override int GetHashCode()
        {
            return (miniGame, level, visualization).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = (ChallengeParameters)obj;
            return miniGame == other.miniGame && visualization == other.visualization && level == other.level;
        }
    }
}