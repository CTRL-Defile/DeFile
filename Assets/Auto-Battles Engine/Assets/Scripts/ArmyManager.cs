using System.Collections.Generic;
using UnityEngine;

namespace AutoBattles
{
    public class ArmyManager : Singleton<ArmyManager>
    {
        #region Variables
        [Header("Player Army")]
        [SerializeField]
        private int _maxArmySize;
        [SerializeField]
        private List<GameObject> _activePlayerPawns = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _playerRoundStartRoster = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _playerTotalRoster = new List<GameObject>();

        [Header("Enemy Army")]
        [SerializeField]
        private List<GameObject> _activeEnemyPawns = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _enemyRoundStartRoster = new List<GameObject>();

        //references
        private ChessBoardManager _boardManager;
        private GameManager _gameManagerScript;
        private UserInterfaceManager _userInterface;
        private BenchManager _benchManagerScript;
        #endregion

        #region Properties  

        //this will limit how many pawns we can have on the board at the start of a round
        //will default to 0 at the start of runtime
        public int MaxArmySize { get => _maxArmySize; protected set => _maxArmySize = value; }

        //contains all active player pawns at the start of a round
        //if the count of this list exceeds the max army size then
        //it will be dealt with at the start of combat
        public List<GameObject> ActivePlayerPawns { get => _activePlayerPawns; protected set => _activePlayerPawns = value; }

        //this will hold the player roster at the start of combat so we can reset after combat
        public List<GameObject> PlayerRoundStartRoster { get => _playerRoundStartRoster; protected set => _playerRoundStartRoster = value; }

        //this will hold a refernce to all the pawns the player currently has both on the bench and board
        //used mainly for upgrading pawns to 2 & 3 stars
        public List<GameObject> PlayerTotalRoster { get => _playerTotalRoster; set => _playerTotalRoster = value; }

        //contains all enemy player pawns at the start of a round
        //this list will decrease in size as the pawns are killed
        public List<GameObject> ActiveEnemyPawns { get => _activeEnemyPawns; protected set => _activeEnemyPawns = value; }

        //this will contain all enemy pawns for the current round and
        //will NOT change as the pawns are killed, used to destroy them all after a round
        public List<GameObject> EnemyRoundStartRoster { get => _enemyRoundStartRoster; set => _enemyRoundStartRoster = value; }

        //references
        protected ChessBoardManager BoardManager { get => _boardManager; set => _boardManager = value; }
        protected GameManager GameManagerScript { get => _gameManagerScript; set => _gameManagerScript = value; }
        protected UserInterfaceManager UserInterface { get => _userInterface; set => _userInterface = value; }
        protected BenchManager BenchManagerScript { get => _benchManagerScript; set => _benchManagerScript = value; }


        #endregion

        #region Methods

        protected virtual void Awake()
        {
            BoardManager = ChessBoardManager.Instance;
            if (!BoardManager)
            {
                Debug.LogError("No ChessBoardManager singleton instance found in the scene. Please add a ChessBoardManager script to the Game Mananger gameobject before" +
                    "entering playmode!");
            }

            GameManagerScript = GameManager.Instance;
            if (!GameManagerScript)
            {
                Debug.LogError("No GameManager singleton instance found in the scene.Please add a GameManager script to the Game Mananger gameobject before" +
                    "entering playmode!");
            }

            BenchManagerScript = BenchManager.Instance;
            if (!BenchManagerScript)
            {
                Debug.LogError("No BenchManager singleton instance found in the scene.Please add a BenchManager script to the Game Mananger gameobject before" +
                    "entering playmode!");
            }

            UserInterface = UserInterfaceManager.Instance;
            if (!UserInterface)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene.Please add a UserInterfaceManager script to the Game Mananger gameobject before" +
                    "entering playmode!");
            }

            MaxArmySize = 0;
        }

        public virtual void IncreaseMaxArmySize(int amount)
        {
            MaxArmySize += amount;

            //update our army count display to the player
            UserInterface.UpdateArmyCountDisplay(ActivePlayerPawns.Count, MaxArmySize);
        }

        #region Syncing Armies

        public virtual void AddPawnToTotalPlayerRoster(GameObject pawn)
        {
            PlayerTotalRoster.Add(pawn);
        }

        public virtual void RemovePawnFromTotalPlayerRoster(GameObject pawn)
        {
            PlayerTotalRoster.Remove(pawn);
        }

        //this will be responsible for removing active  player pawns from the 
        //activePlayerPawn list, it will also check if the players team is at zero
        //meaning the enemy has won the round
        public virtual void RemoveActivePawnFromPlayerRoster(GameObject pawn)
        {
            ActivePlayerPawns.Remove(pawn);

            UserInterface.UpdateArmyCountDisplay(ActivePlayerPawns.Count, MaxArmySize);

            //if this gets called while we are in combat, that means a pawn
            //has been killed, check if we should end the round
            if (GameManagerScript.InCombat)
            {
                //check if the entire army has been killed
                CheckIfEnemyWonRound();
            }
        }

        public virtual void CheckIfEnemyWonRound()
        {
            if (ActivePlayerPawns.Count <= 0)
            {
                //enemy has won :(
                GameManagerScript.EnemyWonRound();
            }
        }

        //this will be responsible for removing active enemy pawns from the 
        //activeEnemyPawn list, it will also check if the enemies team is at zero
        //meaning the player has won the round
        public virtual void RemoveActivePawnFromEnemyRoster(GameObject pawn)
        {
            ActiveEnemyPawns.Remove(pawn);

            //if this gets called while we are in combat, that means a pawn
            //has been killed, check if we should end the round
            if (GameManagerScript.InCombat)
            {
                //check if the entire army has been killed
                CheckIfPlayerWonRound();
            }
        }

        public virtual void CheckIfPlayerWonRound()
        {
            if (ActiveEnemyPawns.Count <= 0)
            {
                //player has won!
                GameManagerScript.PlayerWonRound();
            }
        }

        public virtual void AddActivePawnToPlayerRoster(GameObject pawn)
        {
            ActivePlayerPawns.Add(pawn);

            //update our army count display to the player
            UserInterface.UpdateArmyCountDisplay(ActivePlayerPawns.Count, MaxArmySize);
        }

        //this is called at the start of a round of combat and will assess if the 
        //player is over their max active pawn limit and then take action if necessary
        public virtual void CheckIfPlayerIsOverMaxArmySize()
        {
            //if this is true, we are currently over our max army size
            if (ActivePlayerPawns.Count > MaxArmySize)
            {
                //take action

                //this is how many pawns over we are from our max allowed army size
                int overageAmount = ActivePlayerPawns.Count - MaxArmySize;

                //store this because our activeplayerpawns.count will be changing as we alter the list
                int iteration = ActivePlayerPawns.Count - 1;

                //this will iterate over our ActivePlayerPawns list starting from the last gameobject in
                //the list down until we have reached our correct # of active player pawns
                for (int i = iteration; i > (iteration - overageAmount); i--)
                {
                    //check if we can send pawn to the bench
                    if (BenchManagerScript.SendActivePawnToBench(ActivePlayerPawns[i]))
                    {
                        //we successfully sent the pawn to the bench
                        //remove it from the active player roster
                        RemoveActivePawnFromPlayerRoster(ActivePlayerPawns[i]);
                    }
                    else
                    {
                        //sell the pawn if we didnt have enough room on the bench
                        GameManagerScript.SellActivePawn(ActivePlayerPawns[i]);
                    }
                }
            }
        }

        //this will be called at the start of combat but after we check
        //and correct any overages in the players army/roster
        public virtual void SetPlayerRoster()
        {
            PlayerRoundStartRoster.Clear();

            foreach (GameObject pawn in ActivePlayerPawns)
            {
                PlayerRoundStartRoster.Add(pawn);
            }
        }

        //this will only be called at the start of combat when the enemy board 
        //is being changed completely and generated
        public virtual void ChangeActiveEnemyRoster(List<GameObject> pawns)
        {
            //clear the old active pawns
            ActiveEnemyPawns.Clear();

            //set new pawns to be active
            ActiveEnemyPawns = pawns;

            //make sure this is empty first (should already be)
            EnemyRoundStartRoster.Clear();

            //set our enemy roster
            foreach (GameObject pawn in ActiveEnemyPawns)
            {
                EnemyRoundStartRoster.Add(pawn);
            }
        }

        public virtual void CreateEnemyArmy(ArmyRoster armyRoster)
        {
            //we will pass this reference to the ChangeActiveEnemyRoster function
            //at the end of this function
            List<GameObject> army = new List<GameObject>();

            //iterate through our passed ArmyRoster's roster variable
            for (int i = 0; i < armyRoster.roster.Length; i++)
            {
                PawnStats currentPawnStats = armyRoster.roster[i];

                if (currentPawnStats != null)
                {
                    //create the enemy pawn
                    GameObject enemyPawn = Instantiate(currentPawnStats.pawn);

                    //add to our army
                    army.Add(enemyPawn);

                    //add 32 to our current iteration to get the position in the list
                    //for the corresponding ChessBoardTile to set this newly created pawn to
                    int enemyTileId = i + 32;

                    //set the tile scripts active pawn to the enemy pawn we just made
                    BoardManager.ChessBoardTiles[enemyTileId].ChangePawnOutOfCombat(enemyPawn);
                }
            }

            ChangeActiveEnemyRoster(army);
        }

        //ends combat for both player and enemy active pawns
        public virtual void EndCombatForAllActivePawns()
        {
            foreach (GameObject pawn in EnemyRoundStartRoster)
            {
                pawn.GetComponent<Status>().EndCombat();
            }

            foreach (GameObject pawn in PlayerRoundStartRoster)
            {
                pawn.GetComponent<Status>().EndCombat();

                //remove all synergies from player pawns
                //we dont need to do this for enemy pawns since they
                //are deleted and re-instantiated each round
                pawn.GetComponent<Pawn>().ClearSynergyBonuses();

                //pawn.GetComponent<HealthAndMana>().StartOfCombatHealthRefresh();
            }
        }

        //this should only be called after a round of combat and after a winner is decided
        public virtual void DestroyAndResetEnemyRoster()
        {
            ActiveEnemyPawns.Clear();

            //this will destroy all enemy pawns
            for (int i = EnemyRoundStartRoster.Count - 1; i >= 0; i--)
            {
                EnemyRoundStartRoster[i].GetComponent<Status>().SelfDestruct();
            }

            EnemyRoundStartRoster.Clear();
        }

        //this is called at the end of every round to get the board 
        //ready for the next round of combat
        public virtual void ResetActivePlayerPawns()
        {
            ActivePlayerPawns.Clear();

            foreach (GameObject pawn in PlayerRoundStartRoster)
            {
                pawn.GetComponent<Status>().ResetPawnAfterCombat();
            }

            PlayerRoundStartRoster.Clear();
        }

        #endregion

        #region Targeting

        public virtual GameObject SearchForEnemyTarget(Vector3 myPosition)
        {
            //set this to null so if we cycle through all
            //active enemies and find nothing we will return null
            GameObject target = null;

            //set this to a ridiculous distance to start, then
            //as we filter through all active pawns we will compare the 
            //distance from requesting pawn to determine the closest pawn
            float nearestTargetDistance = 999;

            //cycle through each active enemy pawn
            foreach (GameObject enemyPawn in ActiveEnemyPawns)
            {
                //first check if the pawn is dead
                //TODO Add this check

                //TODO REMOVE THIS OLD CODE
                //get the length of the vector between the requesting pawns current position
                //and the current enemy pawn gameobject we are checking
                //Vector2 length = enemyPawn.transform.position - myPosition;

                //get the sqr magnitude of this new length vector to compare to our
                //current nearestTargetDistance
                //int distance = (int)(Vector3.SqrMagnitude(length));
                float distance = Vector3.Distance(myPosition, enemyPawn.transform.position);

                //compare the nearestTargetDistance & the distance we just calculated
                if (distance < nearestTargetDistance)
                {
                    //if the distance was less, make this enemy pawn our new target to return
                    target = enemyPawn;

                    //change this so we can check future pawns against our 
                    //current closest pawn
                    nearestTargetDistance = distance;
                }
            }

            //if we searched through all enemy pawns and found nothing
            //this will remain null
            return target;
        }

        public virtual GameObject SearchForPlayerTarget(Vector3 myPosition)
        {
            //set this to null so if we cycle through all
            //active enemies and find nothing we will return null
            GameObject target = null;

            //set this to a ridiculous distance to start, then
            //as we filter through all active pawns we will compare the 
            //distance from requesting pawn to determine the closest pawn
            float nearestTargetDistance = 999;

            //cycle through each active enemy pawn
            foreach (GameObject playerPawn in ActivePlayerPawns)
            {
                //first check if the pawn is dead
                //TODO Add this check

                //TODO REMOVE THIS OLD CODE!
                //get the length of the vector between the requesting pawns current position
                //and the current enemy pawn gameobject we are checking
                //Vector2 length = playerPawn.transform.position - myPosition;

                //get the sqr magnitude of this new length vector to compare to our
                //current nearestTargetDistance
                //int distance = (int)(Vector2.SqrMagnitude(length));
                float distance = Vector3.Distance(myPosition, playerPawn.transform.position);

                //compare the nearestTargetDistance & the distance we just calculated
                if (distance < nearestTargetDistance)
                {
                    //if the distance was less, make this enemy pawn our new target to return
                    target = playerPawn;

                    //change this so we can check future pawns against our 
                    //current closest pawn
                    nearestTargetDistance = distance;
                }
            }

            //if we searched through all enemy pawns and found nothing
            //this will remain null
            return target;
        }
        #endregion


        #endregion
    }
}