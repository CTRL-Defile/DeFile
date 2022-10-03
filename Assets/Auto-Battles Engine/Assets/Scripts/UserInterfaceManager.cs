using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoBattles;

namespace AutoBattles
{
    [System.Serializable]
    public class MenuObject
    {
        //when this menuobjects menu gameobject is shown
        //on the screen, this will be true
        public bool isActive;

        //this is the reference to the menu transform being displayed
        public Transform menuTransform;

        //this is the position the menu will take on the canvas
        //when it is active
        public Transform activePosition;

        //this is the position the menu will take on the canvas 
        //when is is inactive
        public Transform hiddenPosition;

        //used by all MenuObjects to toggle them on/off
        public void Toggle()
        {
            if (isActive)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            //set it to its 'active' position
            //menuTransform.position = activePosition.position;

            //set it to active
            isActive = true;
        }

        public void Close()
        {
            //set it to its 'hidden' position
            //menuTransform.position = hiddenPosition.position;

            //set it to inactive
            isActive = false;
        }
    }

    public class UserInterfaceManager : Singleton<UserInterfaceManager>
    {
        #region Variables
        [Header("Shop References")]
        [SerializeField]
        [Tooltip("This is the reference that will be instatiated at runtime to represent the buttons in the shop.")]
        private GameObject _shopSlotPrefab;
        [SerializeField]
        [Tooltip("This is the reference for the parent transform when we instantiate the shop slot prefabs.")]
        private Transform _shopSlotGrid;
        [SerializeField]
        [Tooltip("This is a custom class with a group of variables that makes up the shop window behaviour.")]
        private MenuObject _shopMenu;
        [SerializeField]
        [Tooltip("This is the number of shop slots that will be created at runtime. Defaults to 5 at runtime.")]
        private int _shopSlotCount;
        [SerializeField]
        [Tooltip("Contains the references to all ShopSlot scripts for our active shop slots at runtime.")]
        private List<ShopSlot> _shopSlots = new List<ShopSlot>();

        [Header("Timer")]
        public Text         txt_timer;
        public Image        img_gage;

        [Header("Experience Button")]
        [SerializeField]
        private Text _currentLevelText;
        [SerializeField]
        private Text _currentExpText;

        [Header("Gold")]
        [SerializeField]
        [Tooltip("The reference for where we will display the players current gold. Please drag a text element here before runtime!")]
        private Text _currentGoldText;

        [Header("Trash Panel")]
        [SerializeField]
        private MenuObject _trashPanel;

        [Header("Stats Panel")]
        [SerializeField]
        private MenuObject _statsPanel;

        [Header("Tooltip (Desktop)")]
        [SerializeField]
        private MenuObject _tooltipPanelDesktop;
        [SerializeField]
        private Text _tooltipName;
        [SerializeField]
        private Text _tooltipText;

        [Header("Synergy Panel")]
        [SerializeField]
        [Tooltip("Variable that holds the reference to the synergy UI panel.")]
        private MenuObject _synergyPanel;
        [SerializeField]
        [Tooltip("Variable that holds the prefab we instantiate for a new synergy. Displays the synergy bubble prefab and Icon")]
        private GameObject _synergyWidgetPrefab;
        [SerializeField]
        [Tooltip("Variable that holds the prefab for the bubbles we spawn in the synergyWidgetPrefab")]
        private GameObject _synergyBubblePrefab;
        

        [Header("Army Count Display")]
        [SerializeField]
        private MenuObject _armyCountDisplay;
        [SerializeField]
        private Text _armyCountText;
        [SerializeField]
        private Color normalColor;
        [SerializeField]
        private Color overArmySizeColor;
        [SerializeField]
        private Color underArmySizeColor;

        [Header("Winner Message")]
        [SerializeField]
        private MenuObject _winnerMessagePanel;
        [SerializeField]
        private Text _winnerMessageText;

        [Header("Round Display")]
        [SerializeField]
        private Text _currentRoundText;

        [Header("Pawn Healthbars")]
        [SerializeField]
        private Transform _pawnHealthbarCanvas;        
        #endregion

        #region Properties
        //This is the prefab that will hold the selections in the 
        //shop for the playerto pick their pawns
        protected GameObject ShopSlotPrefab { get => _shopSlotPrefab; set => _shopSlotPrefab = value; }

        //This is the reference to tell the UI where to display the
        //shop slot prefabs for the player to make their pawn selections
        protected Transform ShopSlotGrid { get => _shopSlotGrid; set => _shopSlotGrid = value; }

        //This is a custom class which holds the info for our shop window menu
        //Look above at the MenuObject class for the specifics of what each
        //variable in this class does
        protected MenuObject ShopMenu { get => _shopMenu; set => _shopMenu = value; }

        //the number of shop slots that will be created at the start of runtime,
        //will default to 5 at runtime
        public int ShopSlotCount { get => _shopSlotCount; set => _shopSlotCount = value; }

        //the references for all the shop slots scripts
        //will be added to as the shop slots are created
        protected List<ShopSlot> ShopSlots { get => _shopSlots; set => _shopSlots = value; }

        //this will hold the reference to the text component we alter to display the current level to the player
        protected Text CurrentLevelText { get => _currentLevelText; set => _currentLevelText = value; }

        //this will hold the reference to the text component we alter to display the current exp / max exp to the player
        protected Text CurrentExpText { get => _currentExpText; set => _currentExpText = value; }

        //this is the reference to the ui text element for our current gold,
        //set in the inspector before runtime
        protected Text CurrentGoldText { get => _currentGoldText; set => _currentGoldText = value; }

        //this is the menu object containing all the info for our trash panel
        public MenuObject TrashPanel { get => _trashPanel; set => _trashPanel = value; }

        //this is the menu object containing all the info for our stats panel
        public MenuObject StatsPanel { get => _statsPanel; protected set => _statsPanel = value; }

        //this is the menu object containing the data for the DESKTOP tooltip panel
        protected MenuObject TooltipPanelDesktop { get => _tooltipPanelDesktop; set => _tooltipPanelDesktop = value; }

        //reference to the tooltip name (header) text object
        public Text TooltipName { get => _tooltipName; set => _tooltipName = value; }

        //reference to the actual tooltip text, text object
        public Text TooltipText { get => _tooltipText; set => _tooltipText = value; }

        //stores the reference to the synergy panel that will display synergy info to the player
        public MenuObject SynergyPanel { get => _synergyPanel; protected set => _synergyPanel = value; }

        //stores the prefab we will spawn to display all info about a specific synergy
        public GameObject SynergyWidgetPrefab { get => _synergyWidgetPrefab; protected set => _synergyWidgetPrefab = value; }

        //stores the prefab we will spawn inside the synergywidget
        public GameObject SynergyBubblePrefab { get => _synergyBubblePrefab; protected set => _synergyBubblePrefab = value; }

        //the menu object containing info for our army count display
        public MenuObject ArmyCountDisplay { get => _armyCountDisplay; set => _armyCountDisplay = value; }

        //the text component we will modify to display the army count info to the player
        protected Text ArmyCountText { get => _armyCountText; set => _armyCountText = value; }

        public MenuObject WinnerMessagePanel { get => _winnerMessagePanel; set => _winnerMessagePanel = value; }

        //this holds the reference for the text component we will alter to display who won the round
        protected Text WinnerMessageText { get => _winnerMessageText; set => _winnerMessageText = value; }

        //this holds the reference for the text component we will update each round to let the player know the current round
        protected Text CurrentRoundText { get => _currentRoundText; set => _currentRoundText = value; }

        //this will hold the reference for where newly spawned healthbars 
        public Transform PawnHealthbarCanvas { get => _pawnHealthbarCanvas; protected set => _pawnHealthbarCanvas = value; }
        


        #endregion

        #region Methods
        protected virtual void Awake()
        {            
            //Initialization
            ShopSlotCount = 5;

            if (!ShopSlotPrefab)
            {
                Debug.LogError("Please set a reference to the ShopSlotPrefab in the UserInterfaceManager script located on the" + gameObject.name + " gameobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!ShopSlotGrid)
            {
                Debug.LogError("Please set a reference to the ShopSlotGrid in the UserInterfaceManager script located on the" + gameObject.name + " gameobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            // if (!ShopMenu.menuTransform)
            // {
            //     Debug.LogError("No menu transform reference set for the ShopMenu menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            // if (!ShopMenu.hiddenPosition)
            // {
            //     Debug.LogError("No hidden position reference set for the ShopMenu menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            // if (!ShopMenu.activePosition)
            // {
            //     Debug.LogError("No active position reference set for the ShopMenu menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            if (!CurrentLevelText)
            {
                Debug.LogError("No reference set for 'CurrentLevelText' text component. Please set that in the UserInterfaceManager script on the Game Manager gameobject before entering playmode!");
            }

            if (!CurrentExpText)
            {
                Debug.LogError("No reference set for 'CurrentExpText' text component. Please set that in the UserInterfaceManager script on the Game Manager gameobject before entering playmode!");
            }

            if (!CurrentGoldText)
            {
                Debug.LogError("Please insert a reference to the text element that will display the players current gold on the UserInterfaceManager script on the Game Manager gameobject.");
            }

            if (!TrashPanel.menuTransform)
            {
                Debug.LogError("No menu transform reference set for the TrashPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            // if (!TrashPanel.hiddenPosition)
            // {
            //     Debug.LogError("No hidden position reference set for the TrashPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            // if (!TrashPanel.activePosition)
            // {
            //     Debug.LogError("No active position reference set for the TrashPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            if (!StatsPanel.menuTransform)
            {
                Debug.LogError("No menu transform reference set for the StatsPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!StatsPanel.hiddenPosition)
            {
                Debug.LogError("No hidden position reference set for the StatsPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!StatsPanel.activePosition)
            {
                Debug.LogError("No active position reference set for the StatsPanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!TooltipPanelDesktop.menuTransform)
            {
                Debug.LogError("No menu transform reference set for the TooltipPanelDesktop menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!TooltipPanelDesktop.hiddenPosition)
            {
                Debug.LogError("No hidden position reference set for the TooltipPanelDesktop menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!TooltipPanelDesktop.activePosition)
            {
                Debug.LogError("No active position reference set for the TooltipPanelDesktop menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!ArmyCountDisplay.menuTransform)
            {
                Debug.LogError("No menu transform reference set for the ArmyCountDisplay menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            // if (!ArmyCountDisplay.hiddenPosition)
            // {
            //     Debug.LogError("No hidden position reference set for the ArmyCountDisplay menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            // if (!ArmyCountDisplay.activePosition)
            // {
            //     Debug.LogError("No active position reference set for the ArmyCountDisplay menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
            //         "Please set this reference in the inspector before entering playmode.");
            // }

            if (!ArmyCountText)
            {
                Debug.LogError("Please set a reference to the text component 'ArmyCountText' in the UserInterfaceManager script located on the Game Manager gameobject before entering playmode!");
            }

            if (!WinnerMessagePanel.menuTransform)
            {
                Debug.LogError("No menu transform reference set for the WinnerMessagePanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!WinnerMessagePanel.hiddenPosition)
            {
                Debug.LogError("No hidden position reference set for the WinnerMessagePanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!WinnerMessagePanel.activePosition)
            {
                Debug.LogError("No active position reference set for the WinnerMessagePanel menuobject in the UserInterfaceManager script located on the" + gameObject.name + " gamobject." +
                    "Please set this reference in the inspector before entering playmode.");
            }

            if (!WinnerMessageText)
            {
                Debug.LogError("Please set a reference to the text component 'WinnerMessageText' in the UserInterfaceManager script located on the Game Manager gameobject before entering playmode!");

            }

            if (!CurrentRoundText)
            {
                Debug.LogError("Please set a reference to the text component 'CurrentRoundText' in the UserInterfaceManager script located on the Game Manager gameobject before entering playmode!");
            }

            if (!PawnHealthbarCanvas)
            {
                Debug.LogError("No PawnHealthBarCanvas reference set. Please set a reference to a canvas which will hold all the pawn health bars before entering playmode! This is found on the UserInterfaceManager script" +
                    "on the " + gameObject.name + "gameobject.");
            }
        }

        #region Current Gold

        public virtual void UpdateCurrentGoldText(int amount)
        {
            CurrentGoldText.text = amount.ToString();
        }

        #endregion

        #region Shop Methods
        //Called by the game manager at the start of runtime to create the
        //shop slots for later use
        public virtual void CreateShopSlots()
        {
            for (int i = 0; i < ShopSlotCount; i++)
            {
                //create the shop slot
                GameObject shopSlotGameobject = Instantiate(ShopSlotPrefab, ShopSlotGrid);

                //name the slot in the inspector
                shopSlotGameobject.name = "Shop Slot " + i.ToString();

                //reference the shopSlotScript
                ShopSlot shopSlotScript = shopSlotGameobject.GetComponent<ShopSlot>();

                //add the reference to the list for later use
                ShopSlots.Add(shopSlotScript);
            }
        }

        //this should receive an array of pawnstats and use that data
        //to display a new set of shop options for the player
        //limited to the size of the shopslotcount as to avoid an outofargument exception
        public virtual void DisplayNewShopLineUp(PawnStats[] newPawns)
        {
            for (int i = 0; i < ShopSlotCount; i++)
            {
                ShopSlots[i].Setup(newPawns[i]);
            }
        }

        //we make these 3 functions below specifically because we have UI buttons
        //that need public functions not specifically tied to the MenuObject object

        //called by the 'Shop' button in game
        public virtual void ToggleShopMenu()
        {
            ShopMenu.Toggle();
        }

        //opens the shop menu
        public virtual void OpenShopMenu()
        {
            ShopMenu.Open();
        }

        //closes the shop menu
        public virtual void CloseShopMenu()
        {
            ShopMenu.Close();
        }
        #endregion

        #region Army Count Display

        //this will be called anytime we make a change to our active roster
        public virtual void UpdateArmyCountDisplay(int currentArmySize, int maxArmySize)
        {
            //update the text displayed to the player
            ArmyCountText.text = currentArmySize.ToString() + " / " + maxArmySize.ToString() + " Active Units";

            //update the color based on wether we are over, under, or at our current max army size limit
            if (currentArmySize > maxArmySize)
                ArmyCountText.color = overArmySizeColor;
            else if (currentArmySize < maxArmySize)
                ArmyCountText.color = underArmySizeColor;
            else
                ArmyCountText.color = normalColor;
        }

        #endregion

        #region Experience Button

        public virtual void UpdateCurrentLevelText(int currentLevel)
        {
            CurrentLevelText.text = "Level " + currentLevel.ToString();
        }

        public virtual void UpdateCurrentExpText(int currentExp, int maxExp)
        {
            CurrentExpText.text = currentExp.ToString() + " / " + maxExp.ToString() + " XP";
        }

        #endregion

        #region Winner Message

        public virtual void UpdateWinnerMessageText(string message)
        {
            WinnerMessageText.text = message;
        }

        #endregion

        #region Current Round

        public virtual void UpdateCurrentRoundText(int round)
        {
            CurrentRoundText.text = "Round " + round.ToString();
        }

        #endregion

        #region Tooltip Window DESKTOP

        //opens the desktop version of the tooltip and generates the
        //propery tooltip
        public virtual void OpenDesktopTooltip(Synergy synergy)
        {
            TooltipName.text = synergy.name;

            //grab for later reference
            int sizeForBuff = synergy.totalSynergySize / synergy.totalBuffSize;

            string tooltipDesc = "";

            if (synergy.buffOneTooltip != "")
            {
                tooltipDesc += sizeForBuff.ToString() + "/" + synergy.totalSynergySize.ToString() + ": ";

                tooltipDesc += synergy.buffOneTooltip + "\n";
            }

            if (synergy.buffTwoTooltip != "")
            {
                int secondBuffSize = sizeForBuff * 2;

                tooltipDesc += "\n" + secondBuffSize.ToString() + "/" + synergy.totalSynergySize.ToString() + ": ";

                tooltipDesc += synergy.buffTwoTooltip + "\n";
            }

            if (synergy.buffThreeTooltip != "")
            {
                int thirdBuffSize = sizeForBuff * 3;

                tooltipDesc += "\n" + thirdBuffSize.ToString() + "/" + synergy.totalSynergySize.ToString() + ": ";

                tooltipDesc += synergy.buffThreeTooltip + "\n";
            }

            TooltipText.text = tooltipDesc;

            TooltipPanelDesktop.Open();
        }
        
        public virtual void CloseDesktopTooltip()
        {
            TooltipPanelDesktop.Close();
        }
        #endregion

        #endregion
    }
}

