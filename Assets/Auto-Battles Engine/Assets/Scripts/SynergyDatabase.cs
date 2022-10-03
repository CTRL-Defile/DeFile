using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will be our data container for all the available synergies
/// that our pawns can have. The SynergyManager script will compile a new list
/// and handle all things relating to synergies based on the items provided here
/// If you create a new synergy, make sure to drag it into this list in the editor!
/// </summary>

namespace AutoBattles
{
    public class SynergyDatabase : Singleton<SynergyDatabase>
    {
        [SerializeField]
        private List<Synergy> _synergies;

        public List<Synergy> Synergies { get => _synergies; protected set => _synergies = value; }
    }
}

