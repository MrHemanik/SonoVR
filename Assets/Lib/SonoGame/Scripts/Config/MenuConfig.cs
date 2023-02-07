using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    [CreateAssetMenu(fileName = "MenuConfig", menuName = "ScriptableObjects/Sonogame MenuConfig", order = 1)]
    public class MenuConfig : ScriptableObject
    {
        public ChallengeParameters exploreTask;
        public ChallengeParameters tutorialTask;
        public ChallengeParameters studyTask;
        public MiniGameStateTransition onStudyEnd = MiniGameStateTransition.Logout;

        public MainMenuOption defaultOption = MainMenuOption.Guided;

        public List<MainMenuItem> menuItems;
    }
}
