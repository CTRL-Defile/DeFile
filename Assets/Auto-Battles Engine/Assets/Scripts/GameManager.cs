using System.Collections;
using UnityEngine;

/// <summary>
/// This will be responsible for begining the round, ending the round, and
/// handling most of the games basic functions (rerolling the shop, leveling up,
/// etc).
/// </summary>

namespace AutoBattles
{
    public class GameManager : Singleton<GameManager>
    {
        #region Variables
        [Header("Global Variables")]
        [SerializeField]
        private bool _inCombat;
        [SerializeField]
        private int _currentRound;
        [SerializeField]
        [Tooltip("How much gold it will cost the player to reroll the pawn shop selection. Defaults to 2 if 0 at runtime.")]
        private int _rerollCost;
        [SerializeField]
        [Tooltip("How much gold the player will start the game with, will default to 5 if left at 0.")]
        private int _startingGold;

        [Header("Round Specific Variables")]
        [SerializeField]
        private bool _roundIsEnding;
        [SerializeField]
        [Tooltip("This is the amount of time the game will wait at the first EndRound call before deciding a winner. Set to 1 by default if 0 at runtime.")]
        private float _endRoundLeewayDuration;
        [SerializeField]
        private bool _playerArmyWon;
        [SerializeField]
        private bool _enemyArmyWon;

        //references
        private UserInterfaceManager _UIManager;
        private PawnDatabase _pawnDatabaseScript;
        private ArmyManager _armyManagerScript;
        private EnemyRosterManager _enemyRosterManagerScript;
        private PawnDragManager _pawnDragScript;
        private GoldManager _goldManagerScript;
        private ExperienceManager _expManager;
        private SynergyManager _synergyManagerScript;
        #endregion

        #region Properties
        //this is true while we are in the middle of a round of combat
        public bool InCombat { get => _inCombat; protected set => _inCombat = value; }

        //this will be the current round we are on and also correlate with which
        //enemy roster we will spawn at the start of a round, will default to 0 at start of runtime
        public int CurrentRound { get => _currentRound; set => _currentRound = value; }

        //how much gold it will cost the player to reroll the pawn shop selection
        protected int RerollCost { get => _rerollCost; set => _rerollCost = value; }

        //how much gold the player will start with
        public int StartingGold { get => _startingGold; set => _startingGold = value; }

        protected float _prepare_time;
        protected float _prepare_max_time;
        public float PrepareTime { get => _prepare_time; set => _prepare_time = value; }

        //if the round is currently in the process of ending
        protected bool RoundIsEnding { get => _roundIsEnding; set => _roundIsEnding = value; }

        //this is the amount of time the game will wait at the first EndRound call before deciding a winner
        protected float EndRoundLeewayDuration { get => _endRoundLeewayDuration; set => _endRoundLeewayDuration = value; }

        //this will be changed at the end of a round if the players army is victorious
        protected bool PlayerArmyWon { get => _playerArmyWon; set => _playerArmyWon = value; }

        //this will be changed at the end of a round if the enemies army is victorious
        protected bool EnemyArmyWon { get => _enemyArmyWon; set => _enemyArmyWon = value; }

        //references
        protected UserInterfaceManager UIManager { get => _UIManager; set => _UIManager = value; }
        protected PawnDatabase PawnDatabaseScript { get => _pawnDatabaseScript; set => _pawnDatabaseScript = value; }
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        protected EnemyRosterManager EnemyRosterManagerScript { get => _enemyRosterManagerScript; set => _enemyRosterManagerScript = value; }
        protected PawnDragManager PawnDragScript { get => _pawnDragScript; set => _pawnDragScript = value; }
        protected GoldManager GoldManagerScript { get => _goldManagerScript; set => _goldManagerScript = value; }
        protected ExperienceManager ExpManager { get => _expManager; set => _expManager = value; }
        protected SynergyManager SynergyManagerScript { get => _synergyManagerScript; set => _synergyManagerScript = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            //Initialize references
            UIManager = UserInterfaceManager.Instance;
            if (!UIManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add a UserInterfaceManager script to the Game Manager gameobject before" +
                    "entering playmode!");
            }

            SynergyManagerScript = SynergyManager.Instance;
            if (!SynergyManagerScript)
            {
                Debug.LogError("No SynergyManager singleton instance found in the scene. Please add a SynergyManager script to the Game Manager gameobject before" +
                    "entering playmode!");
            }

            PawnDatabaseScript = PawnDatabase.Instance;
            if (!PawnDatabaseScript)
            {
                Debug.LogError("No PawnDatabase singleton instance found in the scene. Please add a PawnDatabase script to the Database gameobject before" +
                      "entering playmode!");
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. Please add a ArmyManager script to the Game Manager gameobject before" +
                      "entering playmode!");
            }

            EnemyRosterManagerScript = EnemyRosterManager.Instance;
            if (!EnemyRosterManagerScript)
            {
                Debug.LogError("No EnemyRosterManager singleton instance found in the scene. Please add a EnemyRosterManager script to the Game Manager gameobject before" +
                      "entering playmode!");
            }

            PawnDragScript = PawnDragManager.Instance;
            if (!PawnDragScript)
            {
                Debug.LogError("No PawnDragManager singleton instance found in the scene. Pleas add a PawnDragManager script to the Game Manager gameobject before" +
                    "entering playmode.");
            }

            GoldManagerScript = GoldManager.Instance;
            if (!GoldManagerScript)
            {
                Debug.LogError("No GoldManager singletone instance found! Please add a GoldManager script to the scene.");
            }

            ExpManager = ExperienceManager.Instance;
            if (!ExpManager)
            {
                Debug.LogError("No ExperienceManager singletone instance found! Please add a ExperienceManager script to the scene.");
            }

            //if we didnt set a leeyway duration in the inspector, set it to 1
            if (EndRoundLeewayDuration == 0)
            {
                EndRoundLeewayDuration = 1f;
            }

            //set to 2 by default if 0 at runtime
            if (RerollCost == 0)
            {
                RerollCost = 2;
            }

            //make sure we dont accidently start the player with no money
            if (StartingGold == 0)
            {
                StartingGold = 5;
            }

            //Setup Scene
            CurrentRound = 0;

            //UI setup
            UIManager.CreateShopSlots();           
        }

        protected virtual void Start()
        {
            //begin game
            BeginGame();
        }

        protected virtual void IncreaseRoundCounter()
        {
            CurrentRound++;

            UIManager.UpdateCurrentRoundText(CurrentRound);
        }

        //this is the special function we call once at the start of the game to start us on our journey
        //this is very similiar to an 'end of round' function but we keep them seperate
        public virtual void BeginGame()
        {
            //name is a bit misleading, but we start the game with a free roll as if it were
            //the end of a round
            RerollShopAtEndofRound();

            //again, sort of misleading, but we need to increase the round by 1 at the start so we 'start' at round 1 instead of 0
            IncreaseRoundCounter();

            //Grand the player gold to start the game
            GoldManagerScript.GainGold(StartingGold);

            //give 1 exp to level the player to level 1
            GrantEndOfRoundExp();
        }      

        //this will get called at the start of every round
        public virtual void BeginRound()
        {
            //avoid calling this more than once
            if (InCombat)
                return;

            //close the army count display
            UIManager.ArmyCountDisplay.Close();

            //close the shop in case its open
            UIManager.CloseShopMenu();

            //reset clicked on pawn to avoid issues
            if (PawnDragScript.IsClickedOnPawn())
            {
                PawnDragScript.ClearClickedPawn();
            }

            //if we are currently dragging a pawn, send it back to where we got it
            if (PawnDragScript.IsDraggingPawn())
            {
                PawnDragScript.SendPawnBack();
            }            

            //CREATE ENEMY ROSTER

            //check if we have any enemy rosters set up
            if (EnemyRosterManagerScript.TotalRosterCount() < 1)
            {
                Debug.LogError("No rosters set up. Exiting start of combat. Set some rosters in the EnemyRosterManager before starting combat.");
                return;
            }

            //if our # of current rounds exceeds the number of waves we have set up in the EnemyRosterManager
            //then use the last available wave going forward otherwise use the currentRound as the index
            if (CurrentRound - 1 >= EnemyRosterManagerScript.TotalRosterCount())
            {
                //this will spawn the last available roster
                ArmyManagerScript.CreateEnemyArmy(EnemyRosterManagerScript.Rosters[EnemyRosterManagerScript.TotalRosterCount() - 1]);
            }
            else
            {
                ArmyManagerScript.CreateEnemyArmy(EnemyRosterManagerScript.Rosters[CurrentRound - 1]);
            }

            //SOLIDIFY PLAYER ROSTER

            //first make sure we arent currently over our max allowed active pawns
            ArmyManagerScript.CheckIfPlayerIsOverMaxArmySize();

            //set the player roster for reseting after combat
            ArmyManagerScript.SetPlayerRoster();

            //CHECK FOR SYNERGY BUFFS ON EITHER TEAM

            //first check with the players army being the 'friendly' army
            SynergyManagerScript.ApplySynergyEffects(ArmyManagerScript.ActivePlayerPawns, ArmyManagerScript.ActiveEnemyPawns);

            //then check with the enemies army being the 'friendly' army
            SynergyManagerScript.ApplySynergyEffects(ArmyManagerScript.ActiveEnemyPawns, ArmyManagerScript.ActivePlayerPawns);

            //tell our HealthAndMana script to update the current health of pawns in case they gained/loss health from synergies
            foreach(GameObject pawn in ArmyManagerScript.ActivePlayerPawns)
            {
                pawn.GetComponent<HealthAndMana>().StartOfCombatHealthRefresh();
            }

            foreach (GameObject pawn in ArmyManagerScript.ActiveEnemyPawns)
            {
                pawn.GetComponent<HealthAndMana>().StartOfCombatHealthRefresh();
            }

            //Let all active pawns on enemy and player side know they are in combat            
            foreach (GameObject pawn in ArmyManagerScript.ActivePlayerPawns)
            {
                pawn.GetComponent<Status>().BeginCombat();
            }

            foreach (GameObject pawn in ArmyManagerScript.ActiveEnemyPawns)
            {
                pawn.GetComponent<Status>().BeginCombat();
            }

            InCombat = true;

            //check if either army is at 0 at the start of the round
            ArmyManagerScript.CheckIfEnemyWonRound();
            ArmyManagerScript.CheckIfPlayerWonRound();
        }

        #region End of round methods

        public virtual void PlayerWonRound()
        {
            PlayerArmyWon = true;

            if (!RoundIsEnding)
            {
                RoundIsEnding = true;

                EndRound();
            }
        }

        public virtual void EnemyWonRound()
        {
            EnemyArmyWon = true;

            if (!RoundIsEnding)
            {
                RoundIsEnding = true;

                EndRound();
            }
        }

        //this calls everything resposible for bringing the current round to an end
        protected virtual void EndRound()
        {
            //let all pawns from that round know that combat has finished
            //this will also reset synergy bonuses for player pawns
            _prepare_time = 15f;
            _prepare_max_time = 15f;
            UserInterfaceManager.Instance.img_gage.fillAmount = 1f;

            ArmyManagerScript.EndCombatForAllActivePawns();                

            //start the timer to decide a winner
            StartCoroutine(CountdownToDecideWinner());

            //start timer for rewards and board reset
            StartCoroutine(ResetBoardAndGiveRewards());
        }

        public void Update()
        {
            if (!InCombat)
            {
                _prepare_time -= Time.deltaTime;
                UserInterfaceManager.Instance.txt_timer.text = _prepare_time.ToString("N0");
                UserInterfaceManager.Instance.img_gage.fillAmount = (float)_prepare_time / (float)_prepare_max_time;
            }

            if (_prepare_time <= 0f)
            {
                _prepare_time = 15f;
                UserInterfaceManager.Instance.txt_timer.text = _prepare_time.ToString("N0");
                UserInterfaceManager.Instance.img_gage.fillAmount = (float)_prepare_time / (float)_prepare_max_time;
                BeginRound();
            }
        }

        //this will give a little leeway at the end of the round in case both armies killed
        //each other and it is a draw
        protected virtual IEnumerator CountdownToDecideWinner()
        {
            yield return new WaitForSeconds(EndRoundLeewayDuration);

            DecideWinnerOfRound();
        }

        //decides the winner of the round and then resets the chess board
        protected virtual void DecideWinnerOfRound()
        {
            if (PlayerArmyWon && EnemyArmyWon)
            {
                //both armies won, draw
                UIManager.UpdateWinnerMessageText("It's a Draw!");
            }
            else if (PlayerArmyWon)
            {
                //player army won
                UIManager.UpdateWinnerMessageText("You Won the Round!");

                //add one extra gold to player purse
                GoldManagerScript.GainGold(1);
            }
            else if (EnemyArmyWon)
            {
                //enemy army won
                UIManager.UpdateWinnerMessageText("Enemy Won the Round!");
            }

            //tell the UI to display the winner message
            UIManager.WinnerMessagePanel.Open();

            //change this back now that the round has totally finished
            RoundIsEnding = false;

            //reset these
            EnemyArmyWon = false;
            PlayerArmyWon = false;
        }

        //called once the round has offically ended
        //responsible for reseting the players board back to before combat status
        //and giving the player gold/exp
        protected virtual IEnumerator ResetBoardAndGiveRewards()
        {
            yield return new WaitForSeconds(4);

            ResetChessBoard();

            //close the winner message
            UIManager.WinnerMessagePanel.Close();

            //increase current round
            IncreaseRoundCounter();

            GrantEndOfRoundExp();

            GrantEndOfRoundGold();            

            //reroll the shop for free
            RerollShopAtEndofRound();
        }

        //we end combat in this function because otherwise we would be able to move
        //pawns around on the board pre-maturely and mess things up
        protected virtual void ResetChessBoard()
        {
            //destroy all enemy pawns and clear the respective lists
            ArmyManagerScript.DestroyAndResetEnemyRoster();

            ArmyManagerScript.ResetActivePlayerPawns();

            //open the army count display again
            UIManager.ArmyCountDisplay.Open();

            InCombat = false;
        }

        protected virtual void GrantEndOfRoundGold()
        {
            int goldGain = CurrentRound;

            if (goldGain >= 10)
                goldGain = 100;

            //dont give more than 5 base gold per round
            //if (goldGain > 5)
            //    goldGain = 5;

            GoldManagerScript.GainGold(goldGain);
        }

        protected virtual void GrantEndOfRoundExp()
        {
            ExpManager.GainExperience(1);
        }

        #endregion

        #region shop methods
        //This will be called when the player presses the reroll shop button, it will create an array of random pawns and then 
        //send that array to the UserInterfaceManager script to be displayed to the player if the player has enough gold
        public virtual void RerollShopSlots()
        {
            //check if we have the gold to reroll
            //if we do, reroll it
            if (GoldManagerScript.SpendGold(RerollCost))
            {
                //create an array of pawns with the size of shop slots we set
                //in the user interface manager
                PawnStats[] newPawns = new PawnStats[UIManager.ShopSlotCount];

                //for loop that will iterate the number of times based on 
                //how many shop slots we specified in the user interface manager
                for (int i = 0; i < UIManager.ShopSlotCount; i++)
                {
                    //this will give us a random index based on the size of our pawn list in the 
                    //pawn database (all possible pawns that can be used in playmode)
                    int randomIndex = Random.Range(0, PawnDatabaseScript.Pawns.Count);

                    //set our current iteration in newPawns to our corresponding pawn generated
                    //by our randomIndex
                    newPawns[i] = PawnDatabaseScript.Pawns[randomIndex];
                }

                //Once the for loop is completed and our newPawns array is filled
                //pass the array to the UserInterfaceManager to be displayed to the player
                UIManager.DisplayNewShopLineUp(newPawns);

                //Add this here in case we ever add a reroll hotkey usable when shop is closed
                //won't matter if the shop is already open
                UIManager.OpenShopMenu();
            }
            //otherwise, let the player know he needs more gold
            else
            {
                //TODO add this to the interface
                print("Need more gold to reroll.");
            }
        }

        //this is called at the end of each round after rewards are handed out
        //rerolls the shop for free for the player
        protected virtual void RerollShopAtEndofRound()
        {
            //create an array of pawns with the size of shop slots we set
            //in the user interface manager
            PawnStats[] newPawns = new PawnStats[UIManager.ShopSlotCount];

            //for loop that will iterate the number of times based on 
            //how many shop slots we specified in the user interface manager
            for (int i = 0; i < UIManager.ShopSlotCount; i++)
            {
                //this will give us a random index based on the size of our pawn list in the 
                //pawn database (all possible pawns that can be used in playmode)
                int randomIndex = Random.Range(0, PawnDatabaseScript.Pawns.Count);

                //set our current iteration in newPawns to our corresponding pawn generated
                //by our randomIndex
                newPawns[i] = PawnDatabaseScript.Pawns[randomIndex];
            }

            //Once the for loop is completed and our newPawns array is filled
            //pass the array to the UserInterfaceManager to be displayed to the player
            UIManager.DisplayNewShopLineUp(newPawns);

            //bring the shop menu up to display to the player
            UIManager.OpenShopMenu();
        }

        //called when we press the Level button in the shop
        public virtual void PurchaseExperience()
        {
            if (GoldManagerScript.SpendGold(5))
            {
                ExpManager.GainExperience(4);
            }
            else
            {
                print("Not enough gold!");
            }
        }

        //this is only called when the player starts the round with more active pawns
        //than allowed and has a full bench, will sell the overage of active pawns
        public virtual void SellActivePawn(GameObject pawn)
        {
            //remove from active player roster
            ArmyManagerScript.RemoveActivePawnFromPlayerRoster(pawn);

            //remove from total player roster
            ArmyManagerScript.RemovePawnFromTotalPlayerRoster(pawn);

            //grab reference
            Status status = pawn.GetComponent<Status>();

            //give the player the gold worth equivalent of the pawn we are selling
            GoldManagerScript.GainGold(status.GoldWorth);

            //actually destroy the pawn
            status.SelfDestruct();
        }
        #endregion

        #endregion
    }
}

