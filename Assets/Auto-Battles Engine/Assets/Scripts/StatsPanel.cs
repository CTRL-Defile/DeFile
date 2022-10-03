using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script is attached to the Stats Panel in the UI Canvas and is
/// responsible for updating that panel with the currently selected pawns stats
/// </summary>

namespace AutoBattles
{
    public class StatsPanel : Singleton<StatsPanel>
    {
        #region Variables
        [SerializeField]
        private GameObject _pawn;
        private Pawn _pawnScript;
        private PawnStats _pawnStats;
        private HealthAndMana _healthAndManaScript;

        [Header("Stat UI References")]
        [SerializeField]
        protected Text pawnName;
        [SerializeField]
        protected GameObject oneStarParent;
        [SerializeField]
        protected GameObject twoStarParent;
        [SerializeField]
        protected GameObject threeStarParent;
        [SerializeField]
        protected Image pawnIcon;
        [SerializeField]
        protected Text pawnHealth;
        [SerializeField]
        protected Text pawnArmor;
        [SerializeField]
        protected Text pawnDamage;
        [SerializeField]
        protected Text pawnAttackSpeed;
        [SerializeField]
        protected Text pawnDps;
        [SerializeField]
        protected Text pawnMoveSpeed;

        //references
        private PawnDragManager _pawnDragScript;
       
        #endregion

        #region Properties
        //this property will hold the pawn who's stats we want to display
        protected GameObject Pawn { get => _pawn; set => _pawn = value; }

        protected PawnStats PawnStats { get => _pawnStats; set => _pawnStats = value; }
        protected HealthAndMana HealthAndManaScript { get => _healthAndManaScript; set => _healthAndManaScript = value; }
        protected Pawn PawnScript { get => _pawnScript; set => _pawnScript = value; }

        //references
        protected PawnDragManager PawnDragScript { get => _pawnDragScript; set => _pawnDragScript = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            //initialize references
            PawnDragScript = PawnDragManager.Instance;
            if (!PawnDragScript)
            {
                Debug.LogError("No 'PawnDragManager' singleton instance found in the scene. Please add one before entering playmode.");
            }                      
        }        

        //this is a healtier alternative to just putting our code in 'Update'
        IEnumerator StatTick()
        {
            yield return new WaitForSeconds(0.1f);

            RefreshStats();

            StartCoroutine("StatTick");
        }

        //called when we want to display the stats of a new pawn
        public virtual void DisplayNewPawnStats(GameObject pawn)
        {
            //set reference to the new pawn who's stats we
            //want to display
            Pawn = pawn;

            //set reference to the actual pawnstats component
            PawnScript = pawn.GetComponent<Pawn>();
            PawnStats = PawnScript.Stats;
            HealthAndManaScript = Pawn.GetComponent<HealthAndMana>();

            RefreshStats();

            //make sure we start our coroutine
            StartCoroutine("StatTick");
        }

        //called to stop our corountine from updating the stats
        public virtual void StopStatTick()
        {
            StopCoroutine("StatTick");
        }

        //called everytime 'RefreshStats' ticks
        protected virtual void RefreshStats()
        {
            if (Pawn != null)
            {
                //refresh the stats

                //name
                pawnName.text = PawnStats.name;

                //Star Quality
                if (PawnStats.starRating == PawnStats.StarRating.One)
                {
                    oneStarParent.SetActive(true);
                    twoStarParent.SetActive(false);
                    threeStarParent.SetActive(false);
                }
                else if (PawnStats.starRating == PawnStats.StarRating.Two)
                {
                    oneStarParent.SetActive(false);
                    twoStarParent.SetActive(true);
                    threeStarParent.SetActive(false);
                }
                else if (PawnStats.starRating == PawnStats.StarRating.Three)
                {
                    oneStarParent.SetActive(false);
                    twoStarParent.SetActive(false);
                    threeStarParent.SetActive(true);
                }

                //icon
                pawnIcon.sprite = PawnStats.icon;

                //health
                pawnHealth.text = HealthAndManaScript.CurrentHealth.ToString("F0") + " / " + PawnScript.Health.ToString();

                //armor
                string bonusArmorString = BonusStringFormatting(PawnScript.BonusArmor, false);                

                pawnArmor.text = PawnScript.Armor.ToString() + bonusArmorString;

                //damage
                string bonusDamageString = BonusStringFormatting(PawnScript.BonusDamage, false);               

                pawnDamage.text = PawnScript.MinAttackDmg.ToString() + "-" + PawnScript.MaxAttackDmg.ToString() + bonusDamageString;

                //attack speed
                string bonusAttackSpeed = BonusStringFormatting((int)PawnScript.IncreasedAttackSpeed, true);                

                pawnAttackSpeed.text = PawnScript.AttacksPerSecond.ToString("F2") + bonusAttackSpeed;

                //dps
                pawnDps.text = PawnScript.Dps.ToString("F1");

                //move speed
                pawnMoveSpeed.text = PawnScript.MoveSpeed.ToString();
            }
            else
            {
                PawnDragScript.CloseStatsWindow();
            }
        }

        protected string BonusStringFormatting(int value, bool isPercent)
        {
            string message = "";

            if (isPercent)
            {
                if (value > 0)
                {
                    message = "<color=#00ff00> (+" + value.ToString() + "%)</color>";
                }
                else if (value < 0)
                {
                    message = "<color=#ff0000> (-" + value + "%)</color>";
                }
                else
                {
                    //this means the bonus stat we provided is zero and we dont need to do anything
                }
            }
            else
            {
                if (value > 0)
                {
                    message = "<color=#00ff00> (+" + value + ")</color>";
                }
                else if (value < 0)
                {
                    message = "<color=#ff0000> (-" + value + ")</color>";
                }
                else
                {
                    //this means the bonus stat we provided is zero and we dont need to do anything
                }
            }
            

            return message;
        }

        #endregion


    }

}
