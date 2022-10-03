using UnityEngine;

namespace AutoBattles
{
    public class HealthBar : MonoBehaviour
    {
        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}

