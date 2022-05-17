#nullable enable

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        protected int _damagePoints;

        [SerializeField]
        protected ParticleSystem _particleSystem;

        [SerializeField]
        protected float _speed;

        [SerializeField]
        protected float _timeToDestroy;

        protected Transform _transform;

        public int DamagePoints { get => _damagePoints; set => _damagePoints = value; }

        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                UpdateParticleLifeTime();
            }
        }

        private void UpdateParticleLifeTime ()
        {
            var mainModul = _particleSystem.main;
            mainModul.startLifetimeMultiplier = 2 / Speed;
        }

        protected void Awake ()
        {
            _transform = transform;
            UpdateParticleLifeTime();
            Destroy(gameObject, _timeToDestroy);
        }

        protected void FixedUpdate () => _transform.Translate(Speed * Time.fixedDeltaTime * Vector3.forward);

        protected void OnTriggerEnter (Collider other)
        {
            var damageableObject = other.GetComponent<DamageableObject>();
            if (damageableObject != null)
                damageableObject.TakeDamege(DamagePoints);
            Destroy(gameObject);
        }
    }
}