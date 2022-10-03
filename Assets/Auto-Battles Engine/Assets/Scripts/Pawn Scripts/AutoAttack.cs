using System.Collections;
using UnityEngine;

namespace AutoBattles
{   
    public class AutoAttack : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        private bool usingAnimations = false;
        [SerializeField]
        private string _attackTriggerString = "Attack 1";

        #region Variables
        [Header("Variables")]
        [SerializeField]
        private bool _attacking;
        [SerializeField]
        private bool _readyToAttack;

        [Header("Projectiles")]
        [SerializeField]
        [Tooltip("If this is set, this is the transform we will use to set the projectiles spawn position. Otherwise, will spawn from the pawns position.")]
        private Transform _projectileSpawnTransform;

        //References
        private Targeting _targetingScript;
        private Pawn _pawnScript;
        private Movement _movementScript;
        private Animator _anim;
        private Status _statusScript;
        private HealthAndMana _healthAndManaScript;
        #endregion

        #region Properties
        protected bool UsingAnimations { get => usingAnimations; set => usingAnimations = value; }

        //this will be the string with the same name as your attack animation trigger in your animator componenent. 
        protected string AttackTriggerString { get => _attackTriggerString; set => _attackTriggerString = value; }

        //this will be true while the pawn is in the middle of an auto attack
        protected bool Attacking { get => _attacking; set => _attacking = value; }

        //will be true when the pawn is ready to begin auto attacking
        //pawns wont attack each other until both are ready to attack
        public bool ReadyToAttack { get => _readyToAttack; set => _readyToAttack = value; }

        //If this is set, this is the transform we will use to set the projectiles spawn position. Otherwise, will spawn from the pawns position.
        protected Transform ProjectileSpawnTransform { get => _projectileSpawnTransform; set => _projectileSpawnTransform = value; }

        //references
        protected Targeting TargetingScript { get => _targetingScript; set => _targetingScript = value; }
        protected Pawn PawnScript { get => _pawnScript; set => _pawnScript = value; }
        protected Movement MovementScript { get => _movementScript; set => _movementScript = value; }
        protected Animator Anim { get => _anim; set => _anim = value; }
        protected Status StatusScript { get => _statusScript; set => _statusScript = value; }
        protected HealthAndMana HealthAndManaScript { get => _healthAndManaScript; set => _healthAndManaScript = value; }
        #endregion

        #region Methods

        #region Awake, Start, Update
        protected virtual void Awake()
        {
            //cache references for later use
            TargetingScript = GetComponent<Targeting>();
            PawnScript = GetComponent<Pawn>();
            MovementScript = GetComponent<Movement>();
            StatusScript = GetComponent<Status>();
            HealthAndManaScript = GetComponent<HealthAndMana>();

            //Only set reference to animator if we are using animations
            if (usingAnimations)
            {
                Anim = GetComponent<Animator>();
                if (!Anim)
                {
                    Debug.LogError("AutoAttack script has 'useAnimations' set to true but does not have an Animator component.");
                }

                if (AttackTriggerString == "")
                {
                    Debug.LogWarning("No 'AttackTriggerString' set in the AutoAttack script for the " + gameObject.name + " pawn. Please set the name of the trigger " +
                        "in the corresponding animator componenet or disable animations on the prefab");
                }
            }
        }

        protected virtual void Update()
        {
            //first check if we are alive, if not, return
            if (StatusScript.IsDead)
                return;

            //this will force us to wait until both pawns are ready to begin
            //auto attacking before either beings
            if (ReadyToAttack && !Attacking)
            {
                //check if our target has an attack range less than ours
                //if they do, we will begin attacking as soon as we can
                if (TargetingScript.TargetsPawnScript.AttackRange < PawnScript.AttackRange)
                {
                    BeginAutoAttack();
                }
                //if they have an attack range = or > then we will wait until both pawns are ready to attack
                //to prevent unfair advantages with movement
                else
                {
                    if (TargetingScript.TargetsAttackScript.ReadyToAttack)
                    {
                        BeginAutoAttack();
                    }
                }
            }
        }
        #endregion

        #region Auto attack 
        //begin the coroutine loop for auto attacking
        public virtual void BeginAutoAttack()
        {
            //print("BeginAutoAttack " + gameObject.name);
            Attacking = true;

            StartCoroutine(AutoAttackTimer());
        }

        public virtual void StopAutoAttack()
        {
            //print("StopAutoAttack " + gameObject.name);

            Attacking = false;

            ReadyToAttack = false;

            //stop all swing timers
            StopAllCoroutines();
        }

        //once this coroutine is called it will start the timer 
        //for the actual attack launch and then once the full
        //attack time has completed it will call itself until cancelled
        protected virtual IEnumerator AutoAttackTimer()
        {
            //print("AutoAttackTimer " + gameObject.name);
            //check if our target is still in range & not currently dead
            if (TargetingScript.TargetIsInRange())
            {
                //start attack point timer
                StartCoroutine(LaunchAttackTimer());

                MovementScript.RotatePawn(TargetingScript.Target.transform);

                //start attack animation if we are using animations
                if (usingAnimations)
                    Anim.SetTrigger("Attack 1");

                yield return new WaitForSeconds(PawnScript.AttackTime);

                //restart auto attack timer
                StartCoroutine(AutoAttackTimer());
            }
            else
            {
                StopAutoAttack();

                //we lost range on our target so run the search again
                //a new target may be closer now
                TargetingScript.SearchForNewTarget();
            }
        }

        protected virtual IEnumerator LaunchAttackTimer()
        {
            //wait for duration of pawns attack point then launch attack
            yield return new WaitForSeconds(PawnScript.AttackPoint);
            LaunchAttack();
        }

        //this will be called in order to actually deal damage to the target pawn
        protected virtual void LaunchAttack()
        {
            GameObject projectilePrefab = PawnScript.Stats.projectilePrefab;

            //this is a pawn with a projectile attack, spawn the projectile
            if (projectilePrefab != null)
            {
                GameObject projectile;

                //does this pawn have a custom projectile spawn position set?
                if (ProjectileSpawnTransform != null)
                {
                    projectile = Instantiate(projectilePrefab, ProjectileSpawnTransform.position, Quaternion.identity);
                }
                //if not, spawn at the pawns transform position
                else
                {
                    projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                }

                //get a reference to the projectile script on the projectile we just instantiated
                Projectile projectileScript = projectile.GetComponent<Projectile>();

                //make sure we setup the projectile we just instantiated
                projectileScript.Setup(TargetingScript.Target.transform, TargetingScript.TargetHealthScript, CalculateDamage());
            }
            //this is a melee pawn, deal the damage immediately     
            else
            {
                TargetingScript.TargetHealthScript.TakeDamage(CalculateDamage());
            }

            //gain mana from our attack
            //TODO possibly move this to our enemies TakeDamage script to be sure
            //our attack actually landed (dodge change) before granting mana
            HealthAndManaScript.GainMana(PawnScript.ManaPerAttack);
        }
        #endregion

        //called to calculate this pawns damage based on the stats in the 
        //PawnStats scriptable object on the Pawn script
        protected virtual float CalculateDamage()
        {
            float damage = Random.Range(PawnScript.MinAttackDmg, PawnScript.MaxAttackDmg + 1);

            //add bonus damage
            damage += PawnScript.BonusDamage;

            return damage;
        }

        #endregion
    }
}

