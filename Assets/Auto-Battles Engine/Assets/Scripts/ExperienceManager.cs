using UnityEngine;

namespace AutoBattles
{
    public class ExperienceManager : Singleton<ExperienceManager>
    {
        #region Variables

        [Header("Levels")]
        [SerializeField]
        private int _currentLevel;
        [SerializeField]
        private int _maxLevel;

        [Header("Experience")]
        [SerializeField]
        private int _currentExperience;
        [SerializeField]
        private int _maxExperience;

        //references
        private ArmyManager _armyManagerScript;
        private UserInterfaceManager _userInterface;
        #endregion

        #region Properties
        //Players current level, 
        //must be at zero for start of runtime
        public int CurrentLevel { get => _currentLevel; protected set => _currentLevel = value; }

        //Players level cannot exceed this value, 
        //at runtime if this value is still zero it will be set to 10 (see Awake method)
        protected int MaxLevel { get => _maxLevel; set => _maxLevel = value; }

        //Players current experience, 
        //will default to zero at runtime
        public int CurrentExperience { get => _currentExperience; protected set => _currentExperience = value; }

        //Players max experience,
        //when CurrentExperience >= MaxExperience it will level the player
        //defaults to 1 at runtime
        public int MaxExperience { get => _maxExperience; protected set => _maxExperience = value; }

        //references
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        protected UserInterfaceManager UserInterface { get => _userInterface; set => _userInterface = value; }


        #endregion

        #region Methods

        protected virtual void Awake()
        {
            //find references
            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. Please add an ArmyManager script to the GameManager gamobject before entering playmode.");
            }

            UserInterface = UserInterfaceManager.Instance;
            if (!UserInterface)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add an UserInterfaceManager script to the GameManager gamobject before entering playmode.");
            }

            //if we did not set a max level in the inspector
            //then default the value to 10
            if (MaxLevel == 0)
                MaxLevel = 10;

            //Set current level to zero at start of game
            CurrentLevel = 0;

            //set current experience to zero at start of game
            CurrentExperience = 0;

            //set max experience to 1 at start of game
            MaxExperience = 1;
        }

        public virtual void GainExperience(int experience)
        {
            //if we are already max level, do nothing and return
            if (CurrentLevel == MaxLevel)
                return;

            //add our newly granted experience to our current experience
            CurrentExperience += experience;                       

            //check if we are over our max experience,
            //if true then level up
            if (CurrentExperience >= MaxExperience)
            {
                LevelUp();
            }
            else
            {
                //update UI
                UserInterface.UpdateCurrentExpText(CurrentExperience, MaxExperience);
            }
        }

        protected virtual void LevelUp()
        {
            //increment level
            CurrentLevel += 1;

            //Remove current max experience,
            //this will leave us with the remaining overage
            CurrentExperience -= MaxExperience;

            //Up our max experience
            IncreaseMaxExperience();

            //increase our max army size
            ArmyManagerScript.IncreaseMaxArmySize(1);

            //update UI
            UserInterface.UpdateCurrentLevelText(CurrentLevel);
        }

        //this function handles our max experience gain after leveling
        protected virtual void IncreaseMaxExperience()
        {
            if (CurrentLevel == 3)
            {
                MaxExperience += 1;
            }
            else if (CurrentLevel > 3)
            {
                MaxExperience += (CurrentLevel - 3) * 2;
            }

            //update UI
            UserInterface.UpdateCurrentExpText(CurrentExperience, MaxExperience);
        }
        #endregion
    }
}

