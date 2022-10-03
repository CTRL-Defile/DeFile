using UnityEngine;

namespace AutoBattles
{    
    public class Targeting : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private GameObject _target;
        private GameObject _previousTarget;
        private AutoAttack _targetsAttackScript;
        private Status _targetStatus;
        private HealthAndMana _targetHealthScript;
        private Movement _targetsMovementScript;
        private Pawn _targetsPawnScript;

        //References
        private Status _statusScript;
        private ArmyManager _armyManagerScript;
        private Movement _movementScript;
        private Pawn _pawnScript;
        private AutoAttack _autoAttackScript;
        #endregion

        #region Properties
        public GameObject Target { get => _target; protected set => _target = value; }

        protected GameObject PreviousTarget { get => _previousTarget; set => _previousTarget = value; }

        //These are all references to our targets scripts
        public AutoAttack TargetsAttackScript { get => _targetsAttackScript; protected set => _targetsAttackScript = value; }

        public Status TargetStatus { get => _targetStatus; protected set => _targetStatus = value; }

        public HealthAndMana TargetHealthScript { get => _targetHealthScript; set => _targetHealthScript = value; }

        public Movement TargetsMovementScript { get => _targetsMovementScript; set => _targetsMovementScript = value; }

        public Pawn TargetsPawnScript { get => _targetsPawnScript; set => _targetsPawnScript = value; }

        //References
        protected Status StatusScript { get => _statusScript; set => _statusScript = value; }
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        protected Movement MovementScript { get => _movementScript; set => _movementScript = value; }
        protected Pawn PawnScript { get => _pawnScript; set => _pawnScript = value; }
        protected AutoAttack AutoAttackScript { get => _autoAttackScript; set => _autoAttackScript = value; }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            StatusScript = GetComponent<Status>();
            if (!StatusScript)
            {
                Debug.LogError(gameObject.name + " pawn does not have a status script attached");
            }

            MovementScript = GetComponent<Movement>();
            if (!MovementScript)
            {
                Debug.LogError(gameObject.name + " has no Movement script. Please attached a Movement script to their prefab.");
            }

            PawnScript = GetComponent<Pawn>();
            if (!PawnScript)
            {
                Debug.LogError(gameObject.name + " has no Pawn script. Please attached a Pawn script to their prefab.");
            }

            AutoAttackScript = GetComponent<AutoAttack>();
            if (!AutoAttackScript)
            {
                Debug.LogError(gameObject.name + " has no AutoAttack script. Please attached a AutoAttack script to their prefab.");
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager script found, please add one to the game manager gameobject.");
            }
        }

        //called after current target is lost (death, invis, etc)
        // or at the start of combat, this will begin the combat sequence for pawns
        public virtual void SearchForNewTarget()
        {
            GameObject newTarget = null;

            if (StatusScript.IsPlayer)
            {
                newTarget = ArmyManagerScript.SearchForEnemyTarget(transform.position);
            }
            else
            {
                newTarget = ArmyManagerScript.SearchForPlayerTarget(transform.position);
            }

            //if we didnt find an active target, return
            if (newTarget == null)
            {
                return;
            }
            else
            {
                PreviousTarget = Target;

                Target = newTarget;

                //store this for later reference
                TargetsAttackScript = Target.GetComponent<AutoAttack>();
                TargetStatus = Target.GetComponent<Status>();
                TargetHealthScript = Target.GetComponent<HealthAndMana>();
                TargetsMovementScript = Target.GetComponent<Movement>();
                TargetsPawnScript = Target.GetComponent<Pawn>();

                //check our range and act accordingly
                RangeCheck();
            }
        }

        public virtual void DelayedSearchForNewTarget(float timer)
        {
            Invoke("SearchForNewTarget", timer);
        }

        public virtual void RangeCheck()
        {
            //if we discover while checking the range that our target is now dead,
            //find a new one!
            if (TargetStatus.IsDead)
            {
                SearchForNewTarget();
            }

            //otherwise, check if the target is in range
            //check range
            if (TargetIsInRange())
            {
                //let auto attacking script know this pawn is ready to attack
                AutoAttackScript.ReadyToAttack = true;
            }
            else
            {
                //begin movement
                MovementScript.MoveOneTileToTarget();
            }
        }

        //this uses this specific pawns attack range and checks if our current target is in range
        public virtual bool TargetIsInRange()
        {
            //if we somehow entered this function without having a target,
            //simply return false;
            if (Target == null)
                return false;

            //return false if the target is also dead
            if (TargetStatus.IsDead)
                return false;

            //set reference to our pawns attack range
            int range = PawnScript.AttackRange;

            Vector2 targetPos = Target.GetComponent<Movement>().GridPosition;
            Vector2 myPos = GetComponent<Movement>().GridPosition;

            Vector2 gridDistance = targetPos - myPos;

            //check the absolute value of both the x and y of grid distance, if either of them
            //exceed the passed range value, we are out of range
            if (Mathf.Abs(gridDistance.x) <= range && Mathf.Abs(gridDistance.y) <= range)
            {
                //we are in range
                return true;
            }
            else
            {
                //we are not in range
                return false;
            }
        }
        #endregion
    }
}

