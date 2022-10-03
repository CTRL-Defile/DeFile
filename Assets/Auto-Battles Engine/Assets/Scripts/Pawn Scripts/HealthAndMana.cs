using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is responsible for handling our pawns current health
/// mana and health bar initialization
/// </summary>
/// 

namespace AutoBattles
{   
    public class HealthAndMana : MonoBehaviour
    {        
        #region Variables
        [Header("Health Variables")]
        [SerializeField]
        private float _currentHealth;

        [Header("Mana Variables")]
        private float _currentMana;

        [Header("Health Bar")]
        [SerializeField]
        private float _healthBarOffsetZ;
        private GameObject _healthBarGO;
        private Transform _healthBarTransform;
        private HealthBar _healthBarScript;
        private RectTransform _greenBarTransform;
        private RectTransform _blueBarTransform;        
        private float _greenBarStartWidth;
        private float _blueBarStartWidth;

        private bool healthBarInitialized = false;

        //references
        private Pawn _pawnScript;
        private Camera _mainCamera;
        private PrefabDatabase _prefabDatabaseScript;
        private UserInterfaceManager _UIManager;
        private Status _statusScript;
        private AutoAttack _attackScript;
        private Movement _movementScript;
        private ArmyManager _armyManagerScript;
        #endregion

        #region Properties
        //this will be our pawns current health, upon reaching 0 the pawn will die
        //there is no max health variable in this script because the pawn will use the health
        //variable from the 'Pawn' script
        public float CurrentHealth { get => _currentHealth; protected set => _currentHealth = value; }

        public float CurrentMana { get => _currentMana; protected set => _currentMana = value; }

        //this will store our newly created health bars transform as a reference
        //for moving the healthbar around the screen
        public Transform HealthBarTransform { get => _healthBarTransform; set => _healthBarTransform = value; }

        //this will store the reference to the healthbar script of the health bar prefab we created
        //so we can update the healthbar during gameplay
        public HealthBar HealthBarScript { get => _healthBarScript; set => _healthBarScript = value; }

        //this is the how off center the healthbar will be off of the pawn, set it in Awake()
        //will default to -1
        public float HealthBarOffsetZ { get => _healthBarOffsetZ; set => _healthBarOffsetZ = value; }

        //we store the start width of our healthbar & manabar so that we can use this as a reference when
        //calculating a percentage of currenthealth/maxhealth to set the correct size of our health bar
        protected float GreenBarStartWidth { get => _greenBarStartWidth; set => _greenBarStartWidth = value; }
        protected float BlueBarStartWidth { get => _blueBarStartWidth; set => _blueBarStartWidth = value; }

        //store these so we can alter them later
        protected RectTransform GreenBarTransform { get => _greenBarTransform; set => _greenBarTransform = value; }
        protected RectTransform BlueBarTransform { get => _blueBarTransform; set => _blueBarTransform = value; }

        //references
        protected Pawn PawnScript { get => _pawnScript; set => _pawnScript = value; }
        protected Camera MainCamera { get => _mainCamera; set => _mainCamera = value; }
        protected PrefabDatabase PrefabDatabaseScript { get => _prefabDatabaseScript; set => _prefabDatabaseScript = value; }
        protected UserInterfaceManager UIManager { get => _UIManager; set => _UIManager = value; }
        protected Status StatusScript { get => _statusScript; set => _statusScript = value; }
        protected AutoAttack AttackScript { get => _attackScript; set => _attackScript = value; }
        protected Movement MovementScript { get => _movementScript; set => _movementScript = value; }
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        #endregion

        #region Methods

        #region Awake, Start, Update

        protected virtual void Awake()
        {
            #region reference initialization
            PawnScript = GetComponent<Pawn>();
            if (!PawnScript)
            {
                Debug.LogError(gameObject.name + " does not have a Pawn script. Please add one to its prefab before entering playmode!");
            }

            PrefabDatabaseScript = PrefabDatabase.Instance;
            if (!PrefabDatabaseScript)
            {
                Debug.LogError("No PrefabDatabase singleton instance found in the scene. Please add a PrefabDatabase script to the Databases gameobject " +
                    "before entering playmode!");
            }

            UIManager = UserInterfaceManager.Instance;
            if (!UIManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add a UserInterfaceManager script to the Game Manager gameobject " +
                    "before entering playmode!");
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. PLease add a ArmyManager script to the Game Manager gameobject" +
                    "before entering playmode.");
            }

            MainCamera = Camera.main;
            if (!MainCamera)
            {
                Debug.LogError("Please put a main camera in the scene before entering playmode!");
            }

            StatusScript = GetComponent<Status>();
            if (!StatusScript)
            {
                Debug.LogError(gameObject.name + " has no Status script. Please attach one to their prefab before continuing playmode.");
            }

            AttackScript = GetComponent<AutoAttack>();
            if (!StatusScript)
            {
                Debug.LogError(gameObject.name + " has no AutoAttack script. Please attach one to their prefab before continuing playmode.");
            }

            MovementScript = GetComponent<Movement>();
            if (!StatusScript)
            {
                Debug.LogError(gameObject.name + " has no Movement script. Please attach one to their prefab before continuing playmode.");
            }
            #endregion           
        }

        protected virtual void Start()
        {
            //set our current health to our max health at initialization
            CurrentHealth = PawnScript.Health;

            //set our mana
            CurrentMana = 0;

            HealthBarOffsetZ = -1f;

            InitializeHealthBar();

            SetManaBarSize();
        }

        protected virtual void Update()
        {
            //if we have an active healthbar transform
            //move it
            SetHealthBarPositionToPawn();           
        }
        #endregion

        #region Initialization

        //this gets called when the pawn is created and makes the healthbar
        //based on the star rating of this particular pawn
        private void InitializeHealthBar()
        {
            if (healthBarInitialized)
                return;

            //create a gameobject to store our health bar we are about to instantiate
            GameObject healthBarPrefab = null;
            
            //set the prefab to the correct one based on the star rating
            if (PawnScript.Stats.starRating == PawnStats.StarRating.One)
            {
                healthBarPrefab = PrefabDatabaseScript.oneStarHealthBar;
            }
            else if (PawnScript.Stats.starRating == PawnStats.StarRating.Two)
            {
                healthBarPrefab = PrefabDatabaseScript.twoStarHealthBar;
            }
            else if (PawnScript.Stats.starRating == PawnStats.StarRating.Three)
            {
                healthBarPrefab = PrefabDatabaseScript.threeStarHealthBar;
            }
            else
            {
                Debug.LogError("No compatible star rating set in the PawnStats object for " + gameObject.name);
            }

            //make sure we actually found an acceptable prefab
            if (healthBarPrefab != null)
            {
                //create the healthbar and make its parent the pawn healthbar cavnas
                GameObject healthBar = Instantiate(healthBarPrefab, UIManager.PawnHealthbarCanvas);

                //set a reference to the transform of the new healthbar
                HealthBarTransform = healthBar.transform;

                //snap its position
                SetHealthBarPositionToPawn();

                //set reference to the health bars 'HealthBar' script
                HealthBarScript = healthBar.GetComponent<HealthBar>();

                //make sure the green bar is always the first child of the healthbar prefab and the
                //blue bar is the second child
                GreenBarTransform = healthBar.transform.GetChild(0).GetComponent<RectTransform>();
                BlueBarTransform = healthBar.transform.GetChild(1).GetComponent<RectTransform>();

                GreenBarStartWidth = GreenBarTransform.rect.width;
                BlueBarStartWidth = BlueBarTransform.rect.width;

                //if this is a player pawn, change the color of the health bar to green
                if (StatusScript.IsPlayer)
                {                   
                    GreenBarTransform.GetComponent<Image>().color = Color.green;
                }
            }

            healthBarInitialized = true;
        }
        #endregion

        #region Health & Mana Bar Functions
        protected virtual void SetHealthBarPositionToPawn()
        {
            if (HealthBarTransform)
            {
                Vector3 healthBarPos = MainCamera.WorldToScreenPoint(transform.position + new Vector3(0, 0, HealthBarOffsetZ));
                HealthBarTransform.position = healthBarPos;
            }
        }

        //this is called after changing the current health to
        //update the current size of our healthbar to match our current health
        protected virtual void SetHealthBarSize()
        {
            if (!healthBarInitialized)
                InitializeHealthBar();

            float percent = CurrentHealth / PawnScript.Health;

            float newSize = percent * GreenBarStartWidth;

            GreenBarTransform.sizeDelta = new Vector2(newSize, GreenBarTransform.rect.height);
        }

        //this is called after changing the current mana to
        //update the current size of our mana bar to match our current mana
        protected virtual void SetManaBarSize()
        {
            if (!healthBarInitialized)
                InitializeHealthBar();

            float percent = CurrentMana / PawnScript.Mana;

            float newSize = percent * BlueBarStartWidth;

            BlueBarTransform.sizeDelta = new Vector2(newSize, BlueBarTransform.rect.height);
        } 
        #endregion

        public virtual void StartOfCombatHealthRefresh()
        {
            CurrentHealth = PawnScript.Health;
        
            SetHealthBarSize();
        }

        public virtual void TakeDamage(float damage)
        {            
            //if we are already dead, simply return from the function
            if (StatusScript.IsDead)
                return;

            float mitigatedDamage = damage * (PawnScript.PhysicalDmgReduction / 100);

            float actualDamage = damage - mitigatedDamage;

            //apply the dmg
            CurrentHealth -= actualDamage;            

            //check if this dmg killed us
            if (CurrentHealth <= 0)
            {
                //make sure we dont go below 0 health for the healthbar size function
                CurrentHealth = 0;

                //we are dead
                Death();
            }

            //update health bar size
            SetHealthBarSize();
        }

        //this is resposible for taking in mana gains
        //will also check each time if we have reached max mana
        //and if so will make an ability function call
        public virtual void GainMana(float amount)
        {
            CurrentMana += amount;

            if (CurrentMana >= PawnScript.Mana)
            {
                //we have reached max mana
                //possibly cast a spell?

                CurrentMana = PawnScript.Mana;
            }

            SetManaBarSize();
        }

        protected virtual void Death()
        {
            //change our status
            StatusScript.IsDead = true;

            //stop any auto attacks
            AttackScript.StopAutoAttack();

            //turn off our health bar
            HealthBarScript.Disable();

            //let the tile we were occupying know we aren't any longer
            MovementScript.CurrentTile.ClearActivePawn();

            //remove ourselves from the active pawn registry
            if (StatusScript.IsPlayer)
            {
                ArmyManagerScript.RemoveActivePawnFromPlayerRoster(gameObject);
            }
            else
            {
                ArmyManagerScript.RemoveActivePawnFromEnemyRoster(gameObject);
            }

            //TODO REMOVE THIS!
            gameObject.SetActive(false);
        }

        public virtual void ResetHealthAndMana()
        {
            //set current health to max
            CurrentHealth = PawnScript.Health;

            //reset current mana
            CurrentMana = 0;

            //snap the positon before turning it back on to avoid any weird
            //graphical issues
            SetHealthBarPositionToPawn();            

            //update the healthbar
            SetHealthBarSize();

            //update mana bar
            SetManaBarSize();

            //turn the healthbar back on
            HealthBarScript.Enable();
        }

        #endregion

    }

}

