using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this script we will define and assign our buffs to their appropriate synergies
/// 
/// Each Synergy should have its own section containing:
/// 
/// -A reference to the synergy
/// -Initialization of the synergy which includes assigning the buff functions 
///     to the appropriate delegates inside the synergy
/// -The actual buff functions themselves that take in a List<GameObject> of pawns to apply the
///     buffs/debuffs to
/// </summary>

namespace AutoBattles
{
    public class SynergyBuffsAndDebuffs : Singleton<SynergyBuffsAndDebuffs>
    {
        #region Variables       

        #endregion

        #region Properties

        #endregion

        #region Methods
        protected virtual void Awake()
        {           

            //Initialize our synergies
            InitializeWarriorSynergy();
            InitializeOrcSynergy();
            InitializeKnightSynergy();
            InitializeHumanSynergy();
        }

        //we will assign this function to any remaining buff delegates in our synergies that will
        //not be used. This is to avoid compiler errors in the event we accidently call a buff
        //that is null, it will instead trigger this empty function and do nothing and let us know
        protected virtual void Empty(List<GameObject> pawns)
        {
            Debug.LogWarning("Called 'Empty' function in SynergyBuffsAndDebuffs script. Make sure your synergies are properly" +
                "initialized and setup.");
        }

        #region WARRIOR SYNERGY

        [Header("WARRIOR SYNERGY")]
        [SerializeField]
        private Synergy _warriorSynergy;

        protected Synergy WarriorSynergy { get => _warriorSynergy; set => _warriorSynergy = value; }

        protected virtual void InitializeWarriorSynergy()
        {
            if (!WarriorSynergy)
            {
                Debug.LogError("No 'WarriorSynergy' reference set on the SynergyBuffsAndDebuffs script on the GameManager gameobject. Please set a reference to this synergy before entering playmode!");
                return;
            }

            //set our delegates up
            WarriorSynergy.buff1 = WARRIOR_BUFF_1;
            WarriorSynergy.buff2 = WARRIOR_BUFF_2;
            WarriorSynergy.buff3 = Empty;            
        }

        /// <summary>
        /// This section contains all the warrior synergy buffs
        /// </summary>

        public virtual void WARRIOR_BUFF_1(List<GameObject> pawns)
        {
            //the amount of armor we want to increase for each affect pawn
            int armorBuff = 10;           

            foreach (GameObject pawn in pawns)
            {                
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both armorBuff and Synergy_BonusArmor so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusArmor += armorBuff;
                pawnScript.Synergy_BonusArmor += armorBuff;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }        

        public virtual void WARRIOR_BUFF_2(List<GameObject> pawns)
        {            
            //the amount of armor we want to increase for each affect pawn
            //since this is the second buff, if we wanted a total of 20 armor from the
            //warrior synergy buff at this stage, we would only buff by another 10 here, since the first buff
            //was 10
            int armorBuff = 10;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both armorBuff and Synergy_BonusArmor so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusArmor += armorBuff;
                pawnScript.Synergy_BonusArmor += armorBuff;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        #endregion

        #region ORC SYNERGY
        [Header("ORC SYNERGY")]
        [SerializeField]
        private Synergy _orcSynergy;

        protected Synergy OrcSynergy { get => _orcSynergy; set => _orcSynergy = value; }

        protected virtual void InitializeOrcSynergy()
        {
            if (!OrcSynergy)
            {
                Debug.LogError("No 'OrcSynergy' reference set on the SynergyBuffsAndDebuffs script on the GameManager gameobject. Please set a reference to this synergy before entering playmode!");
                return;
            }

            OrcSynergy.buff1 = ORC_BUFF_1;
            OrcSynergy.buff2 = Empty;
            OrcSynergy.buff3 = Empty;
        }

        protected virtual void ORC_BUFF_1(List<GameObject> pawns)
        {
            //the amount of increased attack speed we want to provide each
            //affected pawn of this synergy
            float attackSpeedBuff = 50f;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both increasedAttackSpeed and Synergy_AttackSpeed so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.IncreasedAttackSpeed += attackSpeedBuff;
                pawnScript.Synergy_IncreasedAttackSpeed += attackSpeedBuff;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        #endregion

        #region KNIGHT SYNERGY
        [Header("KNIGHT SYNERGY")]
        [SerializeField]
        private Synergy _knightSynergy;

        protected Synergy KnightSynergy { get => _knightSynergy; set => _knightSynergy = value; }

        protected virtual void InitializeKnightSynergy()
        {
            if (!KnightSynergy)
            {
                Debug.LogError("No 'KnightSynergy' reference set on the SynergyBuffsAndDebuffs script on the GameManager gameobject. Please set a reference to this synergy before entering playmode!");
                return;
            }

            KnightSynergy.buff1 = KNIGHT_BUFF_1;
            KnightSynergy.buff2 = KNIGHT_BUFF_2;
            KnightSynergy.buff3 = KNIGHT_BUFF_3;
        }

        protected virtual void KNIGHT_BUFF_1(List<GameObject> pawns)
        {
            //the amount of bonus damage we want to provide each of the pawns
            //of this synergy
            int damageBonus = 25;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusDamage and Synergy_BonusDamage so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusDamage += damageBonus;
                pawnScript.Synergy_BonusDamage += damageBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        protected virtual void KNIGHT_BUFF_2(List<GameObject> pawns)
        {
            //the amount of bonus damage we want to provide each of the pawns
            //of this synergy
            int damageBonus = 25;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusDamage and Synergy_BonusDamage so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusDamage += damageBonus;
                pawnScript.Synergy_BonusDamage += damageBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        protected virtual void KNIGHT_BUFF_3(List<GameObject> pawns)
        {
            //the amount of bonus damage we want to provide each of the pawns
            //of this synergy
            int damageBonus = 50;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusDamage and Synergy_BonusDamage so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusDamage += damageBonus;
                pawnScript.Synergy_BonusDamage += damageBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        #endregion

        #region HUMAN SYNERGY
        [Header("HUMAN SYNERGY")]
        [SerializeField]
        private Synergy _humanSynergy;

        protected Synergy HumanSynergy { get => _humanSynergy; set => _humanSynergy = value; }

        protected virtual void InitializeHumanSynergy()
        {
            if (!HumanSynergy)
            {
                Debug.LogError("No 'HumanSynergy' reference set on the SynergyBuffsAndDebuffs script on the GameManager gameobject. Please set a reference to this synergy before entering playmode!");
                return;
            }

            HumanSynergy.buff1 = HUMAN_BUFF_1;
            HumanSynergy.buff2 = HUMAN_BUFF_2;
            HumanSynergy.buff3 = HUMAN_BUFF_3;
        }

        protected virtual void HUMAN_BUFF_1(List<GameObject> pawns)
        {
            //the amount of bonus health we want to provide each of the pawns
            //of this synergy
            int healthBonus = 200;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusHealth and Synergy_BonusHealth so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusHealth += healthBonus;
                pawnScript.Synergy_BonusHealth += healthBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        protected virtual void HUMAN_BUFF_2(List<GameObject> pawns)
        {
            //the amount of bonus health we want to provide each of the pawns
            //of this synergy
            int healthBonus = 200;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusHealth and Synergy_BonusHealth so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusHealth += healthBonus;
                pawnScript.Synergy_BonusHealth += healthBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        protected virtual void HUMAN_BUFF_3(List<GameObject> pawns)
        {
            //the amount of bonus health we want to provide each of the pawns
            //of this synergy
            int healthBonus = 200;

            foreach (GameObject pawn in pawns)
            {
                Pawn pawnScript = pawn.GetComponent<Pawn>();

                //we buff both bonusHealth and Synergy_BonusHealth so that our 
                //ClearSynergies function knows exactly how much to remove from synergy buffs later
                pawnScript.BonusHealth += healthBonus;
                pawnScript.Synergy_BonusHealth += healthBonus;

                //once we have finished buffing/debuffing, make sure we ask the pawnscript to recalcuate their stats
                pawnScript.CalculateAllStats();
            }
        }

        #endregion

        #endregion
    }
}

