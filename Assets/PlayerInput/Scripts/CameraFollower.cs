#nullable enable

using UnityEngine;

namespace SimpleVoxelTanks.PlayerInput
{
    public sealed class CameraFolower : MonoBehaviour
    {
        private Vector3 _maxPos;

        private Vector3 _minPos;

        [SerializeField]
        private Vector3 _offset;

        [SerializeField]
        private Transform _target;

        private Transform _transform;

        private void Awake () => _transform = transform;

        private void FixedUpdate ()
        {
            if (_target == null)
                return;

            var newPos = _target.position + _offset;
            newPos = Vector3.Max(_minPos, newPos);
            newPos = Vector3.Min(_maxPos, newPos);
            _transform.position = Vector3.Lerp(_transform.position, newPos, Time.fixedDeltaTime);
        }

        private void UpdatePosition () => _transform.position = _target.position + _offset;

        private void UpdateRotation () => _transform.LookAt(_target);

        public void Init (Vector3 offset, Transform target, Vector3 maxPos, Vector3 minPos)
        {
            _maxPos = maxPos;
            _minPos = minPos;
            _offset = offset;
            _target = target;

            UpdatePosition();
            UpdateRotation();
        }
    }
}