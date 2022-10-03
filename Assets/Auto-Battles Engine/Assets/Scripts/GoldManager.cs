using UnityEngine;

namespace AutoBattles
{
    public class GoldManager : Singleton<GoldManager>
    {
        #region Variables
        [SerializeField]
        private int _currentGold;

        //references
        private UserInterfaceManager _uIManager;
        #endregion

        #region Properties
        //Used as currency to purchase pawns / gain experience
        //this amount will default to 0 at runtime
        public int CurrentGold { get => _currentGold; protected set => _currentGold = value; }

        //references
        public UserInterfaceManager UIManager { get => _uIManager; set => _uIManager = value; }

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            UIManager = UserInterfaceManager.Instance;
            if (!UIManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. PLease add a UserInterfaceManager script to the Game Manager gameobject.");
            }

            CurrentGold = 0;
        }

        //If we have enough gold for the spend request,
        //return true and remove the gold spent else return false
        public virtual bool SpendGold(int amount)
        {
            if (amount <= CurrentGold)
            {
                CurrentGold -= amount;

                UIManager.UpdateCurrentGoldText(CurrentGold);

                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void GainGold(int amount)
        {
            CurrentGold += amount;

            UIManager.UpdateCurrentGoldText(CurrentGold);
        }        
        #endregion
    }

}
