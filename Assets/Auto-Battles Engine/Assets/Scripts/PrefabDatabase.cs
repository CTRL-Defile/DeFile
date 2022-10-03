using UnityEngine;

/// <summary>
/// This script will hold global prefab that a lot of different prefabs
/// will use. We store it here as a way of avoiding having to update every
/// single prefab that uses one of these references if we happen to make a change
/// later down the road.
/// </summary>

namespace AutoBattles
{
    public class PrefabDatabase : Singleton<PrefabDatabase>
    {
        [Header("Pawn Prefabs")]
        public GameObject oneStarHealthBar;
        public GameObject twoStarHealthBar;
        public GameObject threeStarHealthBar;

        protected virtual void Awake()
        {
            if (!oneStarHealthBar)
            {
                Debug.LogError("No oneStarHealthBar prefab set in the prefab database! Please set a reference to the oneStarHealthBar prefab in the PrefabDatabase script located on the Databases gameobject" +
                    "before entering playmode!");
            }
            if (!twoStarHealthBar)
            {
                Debug.LogError("No twoStarHealthBar prefab set in the prefab database! Please set a reference to the twoStarHealthBar prefab in the PrefabDatabase script located on the Databases gameobject" +
                    "before entering playmode!");
            }
            if (!threeStarHealthBar)
            {
                Debug.LogError("No threeStarHealthBar prefab set in the prefab database! Please set a reference to the threeStarHealthBar prefab in the PrefabDatabase script located on the Databases gameobject" +
                    "before entering playmode!");
            }
        }
    }
}


