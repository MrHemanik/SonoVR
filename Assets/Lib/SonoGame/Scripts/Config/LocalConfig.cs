using System;

namespace SonoGame
{
    [Serializable]
    public class LocalConfig
    {
        public bool episodeTimeLimit;

        internal void InitFrom(AppConfig appConfig)
        {
            episodeTimeLimit = appConfig.episodeLogoutTime;
        }

        internal void Apply(AppConfig appConfig)
        {
            appConfig.episodeLogoutTime = episodeTimeLimit;
        }
    }
}
