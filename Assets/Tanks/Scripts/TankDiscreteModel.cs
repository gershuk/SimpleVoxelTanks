#nullable enable

using SimpleVoxelTanks.DiscretePhysicalSystem;

using UnityEngine;

namespace SimpleVoxelTanks.Tanks
{
    public sealed class TankDiscreteModel : DiscretPhysicalBody
    {
        #region MovementFX

        private const float _movementFXIdleTime = 0.1f;
        private bool _isMovementFXPlaying;
        private Vector3 _lastEulerAngles;
        private float _lastMoveTime = 0;
        private Vector3 _lastPosition;

        #endregion MovementFX

        [SerializeField]
        private GameObject _bulletPrefab;

        [SerializeField]
        private Transform _bulletSpawnPoint;

        private uint _lastShootFrame = 0;

        [SerializeField]
        private ParticleSystem[] _movementFX;

        [SerializeField]
        private uint _shootCooldown;

        public GameObject BulletPrefab { get => _bulletPrefab; private set => _bulletPrefab = value; }
        public Transform BulletSpawnPoint { get => _bulletSpawnPoint; private set => _bulletSpawnPoint = value; }
        public uint ShootCooldown { get => _shootCooldown; private set => _shootCooldown = value; }

        [ContextMenu("MoveDown")]
        private void CallMoveDown () => Move(Direction.Down);

        [ContextMenu("MoveLeft")]
        private void CallMoveLeft () => Move(Direction.Left);

        [ContextMenu("MoveRight")]
        private void CallMoveRight () => Move(Direction.Right);

        [ContextMenu("MoveUp")]
        private void CallMoveUp () => Move(Direction.Up);

        private void MovementFXStart ()
        {
            foreach (var fx in _movementFX)
                fx.Play();
            _isMovementFXPlaying = true;
        }

        private void MovementFXStop ()
        {
            foreach (var fx in _movementFX)
                fx.Stop();
            _isMovementFXPlaying = false;
        }

        private void Update ()
        {
            if (_transform.position != _lastPosition || _transform.eulerAngles != _lastEulerAngles)
            {
                _lastMoveTime = Time.time;
                _lastPosition = _transform.position;
                _lastEulerAngles = _transform.eulerAngles;
                if (!_isMovementFXPlaying)
                    MovementFXStart();
            }

            if (Time.time - _lastMoveTime > _movementFXIdleTime)
                MovementFXStop();
        }

        protected override void Awake ()
        {
            base.Awake();
            MovementFXStop();
            _lastPosition = transform.position;
            _lastEulerAngles = transform.eulerAngles;
        }

        public void Move (Direction? direction)
        {
            if (!direction.HasValue || IsTranslating || IsRotating)
                return;

            var newTransform = DiscreteTransform.AddDirection(direction.Value);

            if (newTransform.Direction != DiscreteTransform.Direction)
                TryRotate(newTransform.Direction);

            if (newTransform.Position != DiscreteTransform.Position)
                TryTranslate(newTransform.Position);
        }

        public void Shoot ()
        {
            if (_lastShootFrame + ShootCooldown < PhysicalSystem.FixedUpdateFrameNumber && !IsRotating)
            {
                Instantiate(BulletPrefab, BulletSpawnPoint.position, BulletSpawnPoint.rotation);
                _lastShootFrame = PhysicalSystem.FixedUpdateFrameNumber;
            }
        }
    }
}