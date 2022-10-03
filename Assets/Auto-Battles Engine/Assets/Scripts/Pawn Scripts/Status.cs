using UnityEngine;

/// <summary>
/// this script is resposible for all of the possible status's a pawn can
/// have (dead, player, combat) and is also what allows the pawn to begin combat
/// </summary>

namespace AutoBattles
{    
    public class Status : MonoBehaviour
    {
        #region Variables
        [Header("General Variables")]
        [SerializeField]
        private bool _isDead;
        [SerializeField]
        private bool _isPlayer;
        [SerializeField]
        private bool _inCombat;

        [Header("Destruction Variables")]
        [SerializeField]
        private int _goldWorth;

        //references
        private Targeting _targetingScript;
        private Movement _movementScript;
        private HomeBase _homeBaseScript;
        private HealthAndMana _healthAndManaScript;
        private ArmyManager _armyManagerScript;
        #endregion

        #region Properties
        //defaults to false, obviously...
        public bool IsDead { get => _isDead; set => _isDead = value; }

        //defaults to false because we will explicitly set when a pawn
        //is owned by the player, otherwise it will always be an enemy pawn
        public bool IsPlayer { get => _isPlayer; set => _isPlayer = value; }

        //used to tell if this particular pawn is current in combat
        public bool InCombat { get => _inCombat; protected set => _inCombat = value; }

        //we store a reference to what the player paid for this particular pawn
        //even though each pawn has a worth based on quality, we still want to store what the 
        //player actually paid because we may later introduce some sort of store gold reduction mechanic
        //meaning the same pawn could be bought for different amounts
        public int GoldWorth { get => _goldWorth; set => _goldWorth = value; }

        //references
        protected Targeting TargetingScript { get => _targetingScript; set => _targetingScript = value; }
        protected Movement MovementScript { get => _movementScript; set => _movementScript = value; }
        protected HomeBase HomeBaseScript { get => _homeBaseScript; set => _homeBaseScript = value; }
        protected HealthAndMana HealthAndManaScript { get => _healthAndManaScript; set => _healthAndManaScript = value; }
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in scene. Please add one before entering playmode!");
            }            

            TargetingScript = GetComponent<Targeting>();
            if (!TargetingScript)
            {
                Debug.LogError("No Targetings script on pawn prefab: " + gameObject.name + ". Please add a Targeting script to this pawns prefab before entering playmode.");
            }

            MovementScript = GetComponent<Movement>();
            if (!MovementScript)
            {
                Debug.LogError(gameObject.name + " has no MOvement script. please add one to its prefab before entering playmode.");
            }

            HomeBaseScript = GetComponent<HomeBase>();
            if (!HomeBaseScript)
            {
                Debug.LogError(gameObject.name + " has no HomeBase script. please add one to its prefab before entering playmode.");
            }

            HealthAndManaScript = GetComponent<HealthAndMana>();
            if (!HomeBaseScript)
            {
                Debug.LogError(gameObject.name + " has no HealthAndMana script. please add one to its prefab before entering playmode.");
            }

            //initialization
            IsDead = false;

            IsPlayer = false;

            InCombat = false;
        }

        //TODO start targeting
        //this will let this particular pawn know to begin combat
        public virtual void BeginCombat()
        {
            InCombat = true;

            //reset our previous movement tiles from last round of combat
            MovementScript.ResetPreviousTiles();

            //find a new combat
            TargetingScript.SearchForNewTarget();
        }

        //lets the pawn know it is no longer in combat
        public virtual void EndCombat()
        {
            //take the pawn out of combat
            InCombat = false;
        }

        //this will reset all important combat variables from the previous round
        //and send the pawn back to its tile it occupied before combat started
        public virtual void ResetPawnAfterCombat()
        {
            IsDead = false;

            //put this pawn back in the active player roster
            ArmyManagerScript.AddActivePawnToPlayerRoster(gameObject);

            //send our pawn back to the tile that it occupied before the start of combat
            HomeBaseScript.SendToHomeBase();

            //change our health and mana to be reset back to pre-combat status
            HealthAndManaScript.ResetHealthAndMana();

            //set to active again in case it was inactive from death
            gameObject.SetActive(true);
        }

        //do anything specific for this pawn that needs to be done when
        //this pawn is destroyed (i.e. destroy health bar associated with the pawn)
        public virtual void SelfDestruct()
        {
            //destroys healthbar if we have one
            if (HealthAndManaScript.HealthBarScript)
                Destroy(HealthAndManaScript.HealthBarScript.gameObject);

            //let the tile we were occupying know that we are now gone
            MovementScript.CurrentTile.ClearActivePawn();

            Destroy(gameObject);
        }
        #endregion
    }

}
