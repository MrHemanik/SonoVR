namespace SonoGame
{
    public enum AccountRole { None, User, Admin, Guest };
    public enum ResponseOverlayType { None, Right, Wrong, Skipped };

    public enum AppMessage { None, Submit, Button1, Button2, Button3, Button4, Skip, ResetView, KeysHelp, DepthChange, Joystick }

    public enum HideGeometry { None, Sphere, Cube }

    public enum RoundState { None, Playing, NextPass, Waiting, WaitForConfirm, Continue };

    public enum MiniGameStateTransition { None, MainMenu, Freemode, LastMenu, Logout, CampaignTutorialNode };

    internal enum TaskLaunchContext { None, Test, Tutorial, Exploration, Freemode, Campaign, Study };

    public enum Menu { None, MainMenu, Credits, Campaign, GameSelection };

    /// <summary>
    /// GameManager states
    /// </summary>
    public enum GameManagerState { None, Busy, Menu, UserPlaying, WaitShowSolution, WaitShowRating, Help, Info, WaitResume };

    /// <summary>
    /// Slice visuals
    /// </summary>
    public enum EVisualization { None = 0, Colored = 1, Gray = 2, Ultrasound = 3, Anatomic = 4, Mixed = 5 };

    /// <summary>
    /// Round outcomes
    /// </summary>
    public enum RoundOutcome { None, Success, Failed, Skipped };

    /// <summary>
    /// Sono medal enumeration
    /// </summary>
    public enum SonoMedal { None, Bronze, Silver, Gold };

    /// <summary>
    /// User rotation mode of 3D view
    /// </summary>
    public enum UserRotateMode
    {
        None, CameraY, CameraXY,

        /// <summary>
        /// Y-rotate solution cameras, except fixed main camera.
        /// </summary>
        CameraNoMainY, CameraNoMainIgnoreMaxY
    };

    /// <summary>
    /// Volume data-type.
    /// </summary>
    public enum VolumeMode { ArtificalVolumes = 0, CTVolumes = 1, CT4DVolumes = 2, VisibleHumanVolume = 4 }

    /// <summary>
    /// Sound effects
    /// </summary>
    public enum SonoSound
    {
        None = -1,
        Intro, MainMenu, Loading,
        GeneralButtonUnused,
        GeneralButtonHoverSelect, GeneralButtonPress,
        IngameShowKeys,
        IngameSubmitAnswer,
        IngameWrongAnswer,
        IngameRightAnswer,
        IngameRightAnswer2ndTry,
        IngameTickCrossAnimationSuccess,
        IngameTickCrossAnimationFail,
        IngameUnused,
        IngameSkip,
        CampaignStartChallenge,
        CampaignChangeChallenge,
        CampaignUnused,
        CampaignLockExplosion,
        CampaignOpenGate,
        CampaignGetMedalBronze,
        CampaignGetMedalSilver,
        CampaignGetMedalGold,
        IngameScoreScreenMedalBronze,
        IngameScoreScreenMedalSilver,
        IngameScoreScreenMedalGold,
        GeneralActionNotAvailable,
        GeneralButtonBack,
        WaitShowSolution,
        TimeReminder,
        CampaignDropMedal
    };
}
