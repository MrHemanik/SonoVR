using SonoGame;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Application settings
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfig", menuName = "ScriptableObjects/Sonogame AppConfig", order = 1)]
    public class AppConfig : ScriptableObject
    {

        public static AppConfig current
        {
            get
            {
                if (_current == null)
                {
                    _current = SonogameSettings.Instance.appConfig;
                    Debug.Log("AppConfig.current: " + _current.name);
                }
                return _current;
            }
        }

        public static SonogameAssets assets
        {
            get
            {
                if (_assets == null)
                {
                    _assets = Resources.Load("SonogameAssets") as SonogameAssets;
                }

                return _assets;
            }
        }

        [Tooltip(@"Enable reading of %APPDATA%\Sonogame\LocalConfig.txt")]
        public bool readLocalConfig;

        [Header("Fightstick")]
        public string arcadeFightStickName = "PS3/PS4 Arcade Fightstick";

        [Header("Titles")]
        public string displayName;
        public bool showAppInfo = true;
        public bool introSplash = true;

        [Header("Campaign")]
        //public List<CampaignDescription> campaignDescriptions;
        public List<CampaignName> campaigns;
        public bool campaignScrolling;

        [Header("GameProfiles")]
        public GameProfile[] gameProfiles;

        [Header("MainMenu")]
        public MenuConfig mainMenuConfig;
        public bool hideMainMenuInNavigation = false;

        [Header("Content")]
        public Vector3 artificialVolumeSize = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3Int artificialVolumeDataSize = new Vector3Int(200, 200, 200);

        [Header("UI")]
        public UiColorConfig uiColorConfig;
        public IconConfig iconConfig;

        [Header("Wording")]
        public LanguageConfig languageConfig;

        [Header("TaskSets")]
        public int maxRandomRotateLiveViewDeg = 40;

        [Header("TaskInterface")]
        public bool visualizeTable = false;
        public float maxRotateSolutionDegX = 7;
        public float maxRotateSolutionDegY = 55;
        public float maxRotateVolumesDeg = 45;
        public CubeOutlineConfig cubeOutlineConfig;
        public bool invertCameraControl;
        public bool gameViewInfoOverlay;
        public float rotateDegreesPerSecond = 60;

        [Header("Flow")]
        public bool idleDetector;
        public bool tutorialButton;
        public bool logoutButton;
        public bool skippingAllowed;
        public bool manualGameSwitchAllowed;
        public bool showSolution = true;
        public bool MainMenuF2 = true;

        [Header("Animation")]
        /// <summary>
        /// For answer response without animation, visibility time.
        /// </summary>
        public float answerResponseVisibilityNoAnimation = 0.9f;

        /// <summary>
        /// On answer response, time before an symbol is animated.
        /// </summary>
        public float answerResponsePreAnimStayTime = 1.0f;

        /// <summary>
        /// On answer response, stay time after an symbol has been animated.
        /// </summary>
        public float delayAfterFlight = 0.2f;

        /// <summary>
        /// On answer response, time before an extra symbol appears.
        /// </summary>
        public float appearDelayTickCrossSymbol = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        public float flightTimeTickCrossSymbol = 2.0f;
        public float gateOpenSoundDelay = 1.8f;

        [Header("Audio")]
        public int defaultMaxPriority;

        [Header("Ultrasound")]
        public UltrasoundConfiguration ultrasoundConfiguration;

        [Header("LoginManager")]
        public LoginConfig loginConfig;

        [Header("Logging")]
        public bool sessionLogging = false;
        public bool sessionReplay = false;
        public bool exportReplayJson;
        public int targetFramerate = 60;

        [Header("Tracking")]
        public bool showActiveTrackingOnStart = true;
        public bool allowTrackingSystemChange = false;
        public bool allowInputOrderSwitch = false;
        public bool restoreTrackingTypeFromPlayerPrefs = true;

        [Header("Scoring")]
        public bool allowManualHighscoreReset;

        [Header("Help")]
        public bool showTrackingHint = true;
        public bool hideSphereCheat = false;

        [Header("Development")]
        public bool showFPS = false;
        public bool devSettingsUS = false;
        public bool debugMinigame_OIL = false;
        public bool shapeEditorOverlay;

        [Header("Episode Timing")]
        public float episodeTimeLimitMinutes = 30.0f;

        public bool episodeLogoutTime;
        public float episodeTimeLogoutMinutes = 45;

        public bool episodeTimeWarnLimit;
        public float episodeWarnLimitMinutes = 1.0f;

        public bool showTimeElapsed;

        [Header("Mouse Hiding")]
        public bool hideInactiveMousePointer = false;
        public float hideInactiveMouseTimeout = 3f;

        [Header("Help")]
        public bool helpMenu = false;
        private Dictionary<string, string> languageDict;
        private static AppConfig _current;
        private static SonogameAssets _assets;

        internal const long sessionLoggingWarningDiskspaceGB = 5;
        internal const long sessionLoggingMinimumDiskspaceGB = 1;

        public Dictionary<string, string> LanguageDict
        {
            get
            {
                if (languageDict == null)
                {
                    languageDict = languageConfig.GetLanguageDict();
                }

                return languageDict;
            }
        }

        public string GetTitle(string gameTypeName)
        {
            var result = "N/A";
            var gameProfile = GetGameProfileByTypeName(gameTypeName);

            if (gameProfile != null)
            {
                result = gameProfile.GetTitle();
            }

            return result;
        }

        public GameProfile GetGameProfileByTypeName(string miniGame)
        {
            GameProfile profile = null;
            string gameName = TypeRegistry.GetCanonicalTypeName(miniGame);

            foreach (var p in gameProfiles)
            {
                if (p.miniGame == gameName)
                {
                    profile = p;
                    break;
                }
            }

            if (profile != null)
            {
                Debug.Assert(profile.visualizations != null && profile.visualizations.Count != 0, "GameProfile.visualizations not configured (GameProfile:" + profile.miniGame + ")");
            }

            return profile;
        }

        //public void OnValidate()
        //{
        //    campaigns.Clear();

        //    foreach (var c in campaignDescriptions)
        //    {
        //        campaigns.Add(new CampaignName(c.name.Replace("CampaignDescription (", "").Replace(")", "")));
        //    }
        //}
    }

}