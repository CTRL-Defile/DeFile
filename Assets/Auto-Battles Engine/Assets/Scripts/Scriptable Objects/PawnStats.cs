using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// This is simply a data container for our base pawns stats
/// Other monobehaviours on the pawn prefabs will take this data
/// and do things with it
///     
/// </summary>

namespace AutoBattles
{
    [CreateAssetMenu(fileName = "New Pawn Stats", menuName = "Custom/PawnStats")]
    public class PawnStats : ScriptableObject
    {
        [Header("GENERAL INFO")]
        public new string name;
        public Sprite icon;
        public GameObject pawn;
        public StarRating starRating;
        public PawnQuality pawnQuality;
        [Tooltip("This is a reference to the PawnStats object that this PawnStats will upgrade into upon reaching 3 unique versions of itself. If not set, the pawn will not combine with others.")]
        public PawnStats upgradedPawn;

        [Header("ORIGINS")]
        public List<Origin> origins;

        [Header("CLASSES")]
        public List<Class> classes;

        [Header("ATK")]
        [Header("COMBAT STATS")]
        [Tooltip("Only set this if you want the pawn to spawn a projectile on attack (i.e. Ranged pawn)")]
        public GameObject projectilePrefab;
        [Tooltip("Minimum dmg a pawn can do on an auto attack.")]
        public int minAttackDamage;
        [Tooltip("Maximum dmg a pawn can do on an auto attack.")]
        public int maxAttackDamage;
        [Tooltip("The base attack time of the pawn. i.e. 1.7 = 1.7 seconds between each attack before any increases to attack speed are applied.")]
        public float baseAttackTime;
        [Tooltip("The amount of time between requesting an attack and launching the attack.")]
        public float attackPoint;
        [Tooltip("The range of the pawns auto attacks (in squares, i.e. 1 = attack 1 square away, 2 = attack 2 squares away")]
        public int attackRange;
        [Tooltip("The amount of mana a pawn will generate on each auto attack.")]
        public float manaPerAttack;
        [Tooltip("How fast the pawn can move around the chess board")]
        public int moveSpeed;

        [Header("DEF")]
        [Tooltip("The total amount of dmg a pawn can take before dying.")]
        public int health;
        [Tooltip("The total amount of mana a hero needs to accumulate before being able to cast their spell.")]
        public int mana;
        [Tooltip("Total armor used in calculating physical dmg reduction.")]
        public int armor;            

        public enum Origin
        {
            Orc,            
            Human            
        }

        public enum Class
        {
            Warrior,        
            Knight
        }

        public enum StarRating
        {
            One,
            Two,
            Three
        }

        public enum PawnQuality
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }
    }
}

