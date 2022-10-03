using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is where all the 1 star pawns that are available to be 
/// used in the game will be stored
/// Be sure to always add any new 1 star pawns you create to this list!
/// </summary>

namespace AutoBattles
{
    public class PawnDatabase : Singleton<PawnDatabase>
    {
        [SerializeField]
        private List<PawnStats> _pawns;

        public List<PawnStats> Pawns { get => _pawns; protected set => _pawns = value; }
    }
}

