using UnityEngine;

namespace AutoBattles
{
    [RequireComponent(typeof(Targeting), typeof(AutoAttack), typeof(Movement))]
    [RequireComponent(typeof(Status), typeof(HomeBase), typeof(HealthAndMana))]
    public class Pawn : MonoBehaviour
    {
        #region Variables

        [Header("BASE STATS")]
        [SerializeField]
        [Tooltip("Requires a PawnStats scriptable object.")]
        private PawnStats _stats;

        [Header("ATK")]
        [Header("DO NOT ALTER IN EDITOR!")]
        [Header("Read Only - Calculated Stats")]
        [SerializeField]
        [Tooltip("Minimum dmg a pawn can do on an auto attack.")]
        private int _minAttackDmg;
        [SerializeField]
        [Tooltip("Maximum dmg a pawn can do on an auto attack.")]
        private int _maxAttackDmg;
        [SerializeField]
        [Tooltip("The calculated amount of damage a pawn can do per second.")]
        private float _dps;
        [SerializeField]
        [Tooltip("The amount of time between requesting an attack and launching the attack.")]
        private float _attackPoint;
        [SerializeField]
        private float _attackBackswing;
        [SerializeField]
        [Tooltip("Self explanatory :)")]
        private float _attacksPerSecond;
        [SerializeField]
        [Tooltip("The amount of time between attacks landing.")]
        private float _attackTime;
        [SerializeField]
        [Tooltip("The range of the pawns auto attacks (in squares, i.e. 1 = attack 1 square away, 2 = attack 2 squares away")]
        private int _attackRange;
        [SerializeField]
        [Tooltip("The amount of mana a pawn will generate on each auto attack.")]
        private float _manaPerAttack;
        [SerializeField]
        [Tooltip("How fast the pawn can move around the chess board")]
        private int _moveSpeed;

        [Header("DEF")]
        [Header("DO NOT ALTER IN EDITOR!")]
        [Header("Read Only - Calculated Stats")]
        [SerializeField]
        [Tooltip("The total amount of dmg a pawn can take before dying.")]
        private int _health;
        [SerializeField]
        [Tooltip("The total amount of health a pawn has at the start of combat, factors in bonus health")]
        private int _maxHealth;
        [SerializeField]
        [Tooltip("The total amount of mana a hero needs to accumulate before being able to cast their spell.")]
        private int _mana;
        [SerializeField]
        [Tooltip("Total strike armor used in calculating physical dmg reduction.")]
        private int _armor;             
        [SerializeField]
        [Tooltip("The percent reduction of physical dmg before applying to health.")]
        private float _physicalDmgReduction;

        /// <summary>
        /// These stats are gained from Synergy bonuses and items added to the values above respectively
        /// </summary>

        [Header("BONUS STATS")]        
        [Header("DO NOT ALTER IN EDITOR!")]
        [Header("Read Only - Calculated Stats")]
        [SerializeField]
        [Tooltip("The increased attack speed, 1 = 1%")]
        private float _increasedAttackSpeed;
        [SerializeField]
        [Tooltip("The bonus armor received, can also go negative")]
        private int _bonusArmor;
        [SerializeField]
        [Tooltip("The bonus damage received, can also go negative")]
        private int _bonusDamage;
        [SerializeField]
        [Tooltip("The bonus health received, can also go negative")]
        private int _bonusHealth;

        /// <summary>
        /// These stats are increased (or decreased) as we do so respectively to the stats above
        /// This is so we know exactly how much to remove after combat
        /// We want to avoid just simply resetting to zero because other things may be affecting bonus
        /// stats that shouldnt be reset every round (items, spells, etc)
        /// </summary>

        [Header("EXACT SYNERGY BONUS AMOUNTS")]
        [Header("DO NOT ALTER IN EDITOR!")]
        [Header("Read Only - Calculated Stats")]
        [SerializeField]
        [Tooltip("The increased attack speed, 1 = 1%")]
        private float _synergy_IncreasedAttackSpeed;
        [SerializeField]
        [Tooltip("The bonus armor received, can also go negative")]
        private int _synergy_BonusArmor;
        [SerializeField]
        [Tooltip("The bonus damage received, can also go negative")]
        private int _synergy_BonusDamage;
        [SerializeField]
        [Tooltip("The bonus health received, can also go negative")]
        private int _synergy_BonusHealth;

        #endregion

        #region Properties
        //This will hold a scriptable object 
        //containing all data for the base stats of the pawn
        public PawnStats Stats { get => _stats; protected set => _stats = value; }

        public int MinAttackDmg { get => _minAttackDmg; protected set => _minAttackDmg = value; }
        public int MaxAttackDmg { get => _maxAttackDmg; protected set => _maxAttackDmg = value; }
        public float Dps { get => _dps; protected set => _dps = value; }
        public float AttackPoint { get => _attackPoint; protected set => _attackPoint = value; }
        public float AttackBackswing { get => _attackBackswing; protected set => _attackBackswing = value; }
        public float AttacksPerSecond { get => _attacksPerSecond; protected set => _attacksPerSecond = value; }
        public float AttackTime { get => _attackTime; protected set => _attackTime = value; }
        public int AttackRange { get => _attackRange; protected set => _attackRange = value; }

        public int Health { get => _health; protected set => _health = value; }
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int Mana { get => _mana; protected set => _mana = value; }
        public int Armor { get => _armor; protected set => _armor = value; }        
        public float PhysicalDmgReduction { get => _physicalDmgReduction; protected set => _physicalDmgReduction = value; }       
        public float ManaPerAttack { get => _manaPerAttack; protected set => _manaPerAttack = value; }
        public int MoveSpeed { get => _moveSpeed; protected set => _moveSpeed = value; }

        //BONUS stats
        public float IncreasedAttackSpeed { get => _increasedAttackSpeed; set => _increasedAttackSpeed = value; }
        public int BonusArmor { get => _bonusArmor; set => _bonusArmor = value; }
        public int BonusDamage { get => _bonusDamage; set => _bonusDamage = value; }
        public int BonusHealth { get => _bonusHealth; set => _bonusHealth = value; }

        //Exact Synergy Increases
        public float Synergy_IncreasedAttackSpeed { get => _synergy_IncreasedAttackSpeed; set => _synergy_IncreasedAttackSpeed = value; }
        public int Synergy_BonusArmor { get => _synergy_BonusArmor; set => _synergy_BonusArmor = value; }
        public int Synergy_BonusDamage { get => _synergy_BonusDamage; set => _synergy_BonusDamage = value; }
        public int Synergy_BonusHealth { get => _synergy_BonusHealth; set => _synergy_BonusHealth = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            CalculateAllStats();
        }
       

        /// <summary>
        /// Removes the bonus stats we received from synergies 
        /// If you ever add new bonuses, make sure you add the counterpart here
        /// otherwise it will stack each round!
        /// </summary>
        public virtual void ClearSynergyBonuses()
        {
            IncreasedAttackSpeed -= Synergy_IncreasedAttackSpeed;
            BonusArmor -= Synergy_BonusArmor;
            BonusDamage -= Synergy_BonusDamage;
            BonusHealth -= Synergy_BonusHealth;

            //reset the bonuses we received so we dont
            //take away more than intended next time this is called
            Synergy_IncreasedAttackSpeed = 0;
            Synergy_BonusArmor = 0;
            Synergy_BonusDamage = 0;
            Synergy_BonusHealth = 0;

            CalculateAllStats();
        }

        public virtual void CalculateAllStats()
        {
            CalculateDPS();
            CalculateAttackRange();
            CalculateHealth();
            CalculateMana();
            CalculatePhysicalReduction();            
            CalculateManaPerAttack();
            CalculateMoveSpeed();
        }

        //This calculation will also calculate 
        //attack speed & attack min & max dmg 
        //because it is necessary for the base calculation
        protected virtual void CalculateDPS()
        {
            CalculateAttackSpeed();
            CalculateDamage();

            Dps = (MaxAttackDmg - ((MaxAttackDmg - MinAttackDmg) / 2)) * AttacksPerSecond;
        }

        protected virtual void CalculateAttackSpeed()
        {            
            AttacksPerSecond = ((100 + IncreasedAttackSpeed) * 0.01f) / Stats.baseAttackTime;

            AttackTime = 1 / AttacksPerSecond;

            AttackPoint = Stats.attackPoint / (1 + IncreasedAttackSpeed);           
        }

        protected virtual void CalculateDamage()
        {
            MinAttackDmg = Stats.minAttackDamage;

            MaxAttackDmg = Stats.maxAttackDamage;
        }

        protected virtual void CalculateAttackRange()
        {
            AttackRange = Stats.attackRange;
        }

        protected virtual void CalculateManaPerAttack()
        {
            ManaPerAttack = Stats.manaPerAttack;
        }

        protected virtual void CalculateMoveSpeed()
        {
            MoveSpeed = Stats.moveSpeed;
        }

        protected virtual void CalculateHealth()
        {
            Health = Stats.health + BonusHealth;         
        }

        protected virtual void CalculateMana()
        {
            Mana = Stats.mana;
        }

        //Calcuates total strike armor and the resulting
        //physical damage reduction armount
        protected virtual void CalculatePhysicalReduction()
        {
            Armor = Stats.armor;

            PhysicalDmgReduction = (Armor + BonusArmor) * 1.7f;
        }        
        #endregion

    }

}
