using UnityEngine;

/// <summary>
/// This script will be on any projectile prefab that is spawned
/// by our ranged pawns.
/// </summary>

namespace AutoBattles
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        private bool _isReady;
        private Transform _targetTransform;
        private HealthAndMana _targetHealthScript;
        private float _damage;
        [SerializeField]
        [Tooltip("How fast the projectile will travel to its target. If 0 at runtime it will default to 25.")]
        private float _projectileSpeed;
        #endregion

        #region Properties
        protected bool IsReady { get => _isReady; set => _isReady = value; }
        protected Transform TargetTransform { get => _targetTransform; set => _targetTransform = value; }
        protected HealthAndMana TargetHealthScript { get => _targetHealthScript; set => _targetHealthScript = value; }
        protected float Damage { get => _damage; set => _damage = value; }

        protected float ProjectileSpeed { get => _projectileSpeed; set => _projectileSpeed = value; }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            if (ProjectileSpeed == 0)
                ProjectileSpeed = 25f;
        }

        //this is called by the pawn who is instantiating us
        public virtual void Setup(Transform targetTransform, HealthAndMana targetHealthScript, float damage)
        {
            TargetTransform = targetTransform;
            TargetHealthScript = targetHealthScript;
            Damage = damage;

            IsReady = true;
        }

        protected virtual void Update()
        {
            //if our target is deleted before this reaches them
            //delete this gameobject and return
            if (TargetTransform == null)
            {
                Destroy(gameObject);
                return;
            }

            if (IsReady)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetTransform.position, ProjectileSpeed * Time.deltaTime);

                float distance = Vector3.Distance(transform.position, TargetTransform.position);

                if (distance <= 0.02f)
                {
                    //we hit the target
                    DealDamageAndDestruct();
                }
            }
        }

        protected virtual void DealDamageAndDestruct()
        {
            TargetHealthScript.TakeDamage(Damage);

            Destroy(gameObject);
        }
        #endregion
    }
}

