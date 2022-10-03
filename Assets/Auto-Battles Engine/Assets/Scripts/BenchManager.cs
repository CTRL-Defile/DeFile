using System.Collections.Generic;
using UnityEngine;

namespace AutoBattles
{
    public class BenchManager : Singleton<BenchManager>
    {
        #region Variables
        [Header("Bench Info")]
        [SerializeField]
        private int _benchSlotsCount;
        [SerializeField]
        private Transform _benchStartLocation;
        [SerializeField]
        private List<BenchChessBoardTile> _benchSlotScripts = new List<BenchChessBoardTile>();

        [Header("Combining Info")]
        [SerializeField]
        [Tooltip("Number of like pawns needed in order to trigger a combo and upgrade that pawn to the next rank.")]
        private int _pawnsNeededForCombo;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _benchTilePrefab;

        //references
        private ArmyManager _armyManagerScript;
        private SynergyManager _synergyManagerScript;
        #endregion

        #region Properties
        //this is the # of bench slots the player will have,
        //will default to 8 at the start of runtime
        public int BenchSlotsCount { get => _benchSlotsCount; protected set => _benchSlotsCount = value; }

        //this is set in the inspector before runtime and will be the starting location for the first tile
        //further created titles will be made directly to the right of the first tile, will also hold
        //all the bench tiles in the inspector
        protected Transform BenchStartLocation { get => _benchStartLocation; set => _benchStartLocation = value; }

        //This will hold all our bench slot scripts and is added to as the bench is created
        public List<BenchChessBoardTile> BenchSlotScripts { get => _benchSlotScripts; set => _benchSlotScripts = value; }

        //this is the number of like pawns we need in order to get the next upgrade of that pawn,
        //will default to 3 if left at zero for runtime
        public int PawnsNeededForCombo { get => _pawnsNeededForCombo; protected set => _pawnsNeededForCombo = value; }

        //This is the prefab that will be instatiated when we create the players bench
        protected GameObject BenchTilePrefab { get => _benchTilePrefab; set => _benchTilePrefab = value; }

        //references
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        protected SynergyManager SynergyManagerScript { get => _synergyManagerScript; set => _synergyManagerScript = value; }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            //Check References
            if (!BenchTilePrefab)
            {
                Debug.LogError("No BenchTilePrefab set in the inspector in the BenchManager script located on the " + gameObject.name + " gameobject. Please add" +
                    "a BenchTilePrefab before entering playmode.");
                return;
            }

            if (!BenchStartLocation)
            {
                Debug.LogError("No BenchStartLocation set in the inspector in the BenchManager script located on the " + gameObject.name + " gameobject. Please add" +
                    "a BenchStartLocation before entering playmode.");
                return;
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. Please add an ArmyManager script to the Game Manager gameobject" +
                    "before entering playmode");
            }

            SynergyManagerScript = SynergyManager.Instance;
            if (!SynergyManagerScript)
            {
                Debug.LogError("No SynergyManager singleton instance found in the scene. Please add an ArmyManager script to the Game Manager gameobject" +
                    "before entering playmode");
            }

            //Initialization
            if (BenchSlotsCount == 0)
                BenchSlotsCount = 8;

            if (PawnsNeededForCombo == 0)
                PawnsNeededForCombo = 3;

            //create our bench
            CreateBenchTiles();
        }


        //This is called at the start of playtime to setup the players bench tiles for 
        //storing their inactive pawns they buy from the shop
        protected virtual void CreateBenchTiles()
        {
            for (int i = 0; i < BenchSlotsCount; i++)
            {
                //create the tile and set it to the benchstartlocation transform
                GameObject benchTile = Instantiate(BenchTilePrefab, BenchStartLocation);

                //rename the tile in the inspector
                benchTile.name = "Bench Tile " + i.ToString();

                //set the local location of the tile
                benchTile.transform.localPosition = new Vector3(i * 5, 0, 0);

                //Grab the BenchChessBoardTile script to add to our list and to do basic setup
                BenchChessBoardTile tileScript = benchTile.GetComponent<BenchChessBoardTile>();

                //give ID equal to our current iteration
                tileScript.Setup(i, new Vector2(i, 0), ChessBoardTile.TileCategory.Bench);

                //add to our list for later reference
                BenchSlotScripts.Add(tileScript);
            }
        }

        //simple function which returns true if we have room on our bench
        //for a new pawn
        public virtual bool BenchHasSpace()
        {
            bool hasSpace = false;

            for (int i = 0; i < BenchSlotScripts.Count; i++)
            {
                if (!BenchSlotScripts[i].HasActivePawn())
                {
                    hasSpace = true;

                    break;
                }
            }

            return hasSpace;
        }

        //this method first checks if we have available room on our
        //bench and if we do it will add the passed pawnstats 'pawn' to our bench
        //otherwise returns false
        public virtual bool AddNewPawnToBench(PawnStats pawnStats, int goldCost)
        {
            for (int i = 0; i < BenchSlotScripts.Count; i++)
            {
                //check if slot is empty
                if (!BenchSlotScripts[i].HasActivePawn())
                {
                    //we found an empty slot, fill it
                    BenchSlotScripts[i].CreatePlayerPawn(pawnStats, goldCost);

                    //update our synergies
                    SynergyManagerScript.PawnAcquired(pawnStats);

                    //check if we just made a combination
                    CheckForCombination(pawnStats);

                    //let the calling script know we were successful
                    return true;
                }
            }

            //retuns true if we were able to add the pawn to our bench
            //returns false if we were not able
            return false;
        }

        //this will send an active pawn to the bench if we have the space
        //otherwise will return false
        public virtual bool SendActivePawnToBench(GameObject pawn)
        {
            for (int i = 0; i < BenchSlotScripts.Count; i++)
            {
                //check if slot is empty
                if (!BenchSlotScripts[i].HasActivePawn())
                {
                    //we found an empty slot, fill it
                    BenchSlotScripts[i].ChangePawnOutOfCombat(pawn);

                    //grab for quick reference since we need it twice in a row
                    PawnStats stats = pawn.GetComponent<Pawn>().Stats;

                    //update our synergy manager
                    SynergyManagerScript.PawnOutOfPlay(stats);

                    //check if we just made a combination
                    CheckForCombination(stats);

                    //let the calling script know we were successful
                    return true;
                }
            }

            //retuns true if we were able to add the pawn to our bench
            //returns false if we were not able
            return false;
        }

        //this function takes in an PawnStats and checks the bench for 3 of that particular PawnStats, 
        //if it finds 3, it will combine them, creating a pawn 1 star higher than the other 3 and deleting them
        public virtual void CheckForCombination(PawnStats pawnStats)
        {
            //first make sure the PawnStats we were passed actually has an upgradePawn set
            PawnStats upgradePawn = pawnStats.upgradedPawn;

            if (upgradePawn == null)
            {
                return;
            }

            //if we made it this far, we do have a upgradePawn reference to use

            //create a new list to store the similiar pawns we find during our loop
            List<GameObject> similiarPawns = new List<GameObject>();
            bool combine = false;

            //search all bench slots for combinations
            for (int i = 0; i < BenchSlotScripts.Count; i++)
            {
                //check if this bench slot has an active pawn to compare
                if (BenchSlotScripts[i].HasActivePawn())
                {
                    //get the pawn on this bench slots PawnStats and compare it to the PawnStats we were passed
                    if (BenchSlotScripts[i].ActivePawn.GetComponent<Pawn>().Stats == pawnStats)
                    {
                        //add this pawn to our similiarPawns list if it was the same PawnStats
                        similiarPawns.Add(BenchSlotScripts[i].ActivePawn);
                    }
                }

                //once we have checked this particular tile,
                //see if we have 3 of the same so we can combine and exit the function
                if (similiarPawns.Count == PawnsNeededForCombo)
                {
                    combine = true;
                    break;
                }
            }

            //we have exited the for loop (either through the break or naturally) and if
            //combine is true, we will take the pawns from our list and combine them
            if (combine)
            {
                int totalGoldCost = 0;

                //first, delete all 3 of the similiar pawns
                foreach (GameObject pawn in similiarPawns)
                {
                    //remove the pawn from the players total roster
                    ArmyManagerScript.RemovePawnFromTotalPlayerRoster(pawn);

                    //grab this reference, we are about to use it more than once
                    Status status = pawn.GetComponent<Status>();

                    //add this pawns gold cost to the total gold cost to add to  
                    //the new pawn we are going to create in a minute
                    totalGoldCost += status.GoldWorth;

                    //tell the pawn to destroy itself
                    status.SelfDestruct();
                }

                //update our synergy manager that we just lost pawns
                SynergyManagerScript.AdjustmentFromUpgrade(pawnStats);

                //add the upgraded pawn to our bench
                if (AddNewPawnToBench(upgradePawn, totalGoldCost))
                {
                    //this should be the outcome every time,
                    //something weird with bench space might be happening if this if
                    //statement is returning false           
                }
                else
                {
                    Debug.LogError("Pawns attempted to combine but something went wrong. More than likely there is an issue there being not enough room on your bench for the new pawn.");
                }
            }
        }
        #endregion
    }
}

    