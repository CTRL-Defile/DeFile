using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script is responsible for holding the ArmyRoster's for our enemy
/// waves
/// </summary>

namespace AutoBattles
{
    public class EnemyRosterManager : Singleton<EnemyRosterManager>
    {
        [SerializeField]
        private List<ArmyRoster> _rosters;

        //this will hold all waves of enemy pawns to play against
        //if no items in the list when the game is started, nothing will happen
        public List<ArmyRoster> Rosters { get => _rosters; protected set => _rosters = value; }

        protected virtual void Awake()
        {
            if (Rosters.Count < 1)
            {
                Debug.LogError("No ArmyRoster objects set in the waves list variable in the EnemyWaveManager script located on the " + gameObject.name + " gamobject. " +
                    "Please add some ArmyRoster objects to that script before entering playmode.");
            }
        }

        //returns the number of rosters we have
        public virtual int TotalRosterCount()
        {
            return Rosters.Count;
        }
    }
}

