using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is a data container that will hold information on synergies
/// Other Monobehaviours on the GameManager will do things with this information
/// </summary>

namespace AutoBattles
{
    [CreateAssetMenu(fileName = "New Synergy", menuName = "Custom/Synergy")]
    public class Synergy : ScriptableObject
    {
        [Header("GENERAL INFO")]
        public new string name;
        public Sprite icon;
        [Tooltip("Used for certain UI objects when displaying synergy info")]
        public Color color;

        /// <summary>
        /// This buff info will decide the breakdown of how/when your buffs will be applied to the units of a particular synergy.
        /// Example: 
        /// If you set totalBuffSize to 3 and totalSynergySize to 6, that means every 2 of a kind of this synergy will trigger a new buff because 6 / 3 = 2
        /// Our max number of buffs allowed for any synergy is currently 3
        /// **You do not need to use all 3 buffs but you will want to keep to particular sizes to avoid issues!**
        /// You will always want totalSynergySize / totalBuffSize to equal a whole number!
        /// *GOOD* totalSynergySize = 9 / totalBuffSize = 3 equals 3 (Whole number) *GOOD*
        /// *BAD* totalSynergySize = 9 / totalBuffSize = 2 equals 4.5 (Decimal number) *BAD*
        /// </summary>

        [Header("Synergy Buff Info")]
        [Tooltip("This is the total number of buffs this particular synergy can provide. Example: For buffs at 2 of a kind, 4 of a kind, 6 of a kind you would set this to 3.")]
        [Range(1, 3)]
        public int totalBuffSize = 1;

        [Tooltip("This is the total number of units required to max out this particular synergy. Example: totalBuffSize = 3 totalSynergySize = 6 means 3 total buffs or every 2 of a kind (6 / 3 = 2) is a new buff.")]
        [Range(1, 9)]
        public int totalSynergySize = 1;

        /// <summary>
        /// These unity events will hold the functions that are called once certain thresholds of pawns of the same synergy are put into play
        /// </summary>
        [Header("Buffs")]
        [Tooltip("Check this if the buffs this synergy provides are a negative affect meant for the enemy pawns.")]
        public bool enemyDebuff;

        [Tooltip("What to display to the player about the first buff from this synergy, i.e. +10 Armor")]
        [TextArea(3, 5)]
        public string buffOneTooltip;        

        [TextArea(3, 5)]
        public string buffTwoTooltip;
        
        [TextArea(3, 5)]
        public string buffThreeTooltip;

        //Set up our base delegate to handle dynamic functions for different synergy buffs        
        public delegate void Buff(List<GameObject> pawns);

        //create our delegates

        //our SynergyBuffsAndDebuffs script will handle assigning these at runtime
        public Buff buff1;

        public Buff buff2;

        public Buff buff3;        
    }
}

