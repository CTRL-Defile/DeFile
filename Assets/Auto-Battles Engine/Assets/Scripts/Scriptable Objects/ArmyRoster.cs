using UnityEngine;

/// <summary>
/// This is a scriptable object that will hold the roster lineup
/// for enemy waves, the position in the array at roster[0] will 
/// correlate to tile 32 (position 0, 4) and extends right 
///  to (1, 4), (2, 4), (3, 4), etc...
///  ending at roster[31] at position (7, 7) in the top right corner
/// </summary>

namespace AutoBattles
{
    [CreateAssetMenu(fileName = "New Roster", menuName = "Custom/Roster")]
    public class ArmyRoster : ScriptableObject
    {
        public PawnStats[] roster = new PawnStats[32];
    }

}
