using System.Collections;

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.DiscretePhysicalSystem
{
    public class DiscretPhysicalBody : MonoBehaviour
    {
        private GameObject _gameObject;

        [SerializeField]
        private uint _movementTicks = 1;

        [SerializeField]
        private uint _rotationTicks = 1;

        protected Vector3UInt? _transaltingPosition;
        protected Transform _transform;
        public DiscreteTransform DiscreteTransform { get; protected set; }
        public bool IsRotating { get; private set; }
        public bool IsTranslating { get; private set; }
        public uint MovementTicks { get => _movementTicks; set => _movementTicks = value; }
        public uint RotationTicks { get => _rotationTicks; set => _rotationTicks = value; }
        public Vector3UInt? TransaltingPosition { get => _transaltingPosition; private set => _transaltingPosition = value; }

        protected virtual void Awake () => (_transform, _gameObject) = (transform, gameObject);

        protected void OnDestroy ()
        {
            PhysicalSystem.ColliderGrid.UnregisterBody(this);
            StopAllCoroutines();
        }

        protected IEnumerator Rotate (Direction newDirection)
        {
            IsRotating = true;
            var startFrameNumber = PhysicalSystem.FixedUpdateFrameNumber;
            while (startFrameNumber + RotationTicks > PhysicalSystem.FixedUpdateFrameNumber && IsRotating)
            {
                _transform.rotation = Quaternion.Slerp(Quaternion.Euler(DiscreteTransform.Direction.DirectionToEulerAngles()),
                    Quaternion.Euler(newDirection.DirectionToEulerAngles()),
                    (PhysicalSystem.FixedUpdateFrameNumber + 1 - startFrameNumber) * 1.0f / RotationTicks);
                yield return new WaitForFixedUpdate();
            }

            DiscreteTransform = new(DiscreteTransform.Position, newDirection);
            IsRotating = false;
        }

        protected IEnumerator Translate (Vector3UInt newPosition)
        {
            TransaltingPosition = newPosition;
            IsTranslating = true;
            var startFrameNumber = PhysicalSystem.FixedUpdateFrameNumber;
            while (startFrameNumber + MovementTicks > PhysicalSystem.FixedUpdateFrameNumber && IsTranslating)
            {
                _transform.position = Vector3.Lerp(DiscreteTransform.Position,
                                                   newPosition,
                                                   (PhysicalSystem.FixedUpdateFrameNumber + 1 - startFrameNumber) * 1.0f / MovementTicks);
                yield return new WaitForFixedUpdate();
            }

            PhysicalSystem.ColliderGrid.TryClearTempCell(this, DiscreteTransform.Position);
            DiscreteTransform = new(newPosition, DiscreteTransform.Direction);
            IsTranslating = false;
            TransaltingPosition = null;
        }

        protected bool TryRotate (Direction newDirection)
        {
            if (!IsRotating)
            {
                StartCoroutine(Rotate(newDirection));
                return true;
            }
            return false;
        }

        protected bool TryTranslate (Vector3UInt newPosition)
        {
            if (PhysicalSystem.ColliderGrid.TryMove(this, newPosition, MovementTicks) && !IsTranslating)
            {
                StartCoroutine(Translate(newPosition));
                return true;
            }
            return false;
        }

        public void Init (Vector3UInt position, Direction direction = default)
        {
            _transform.position = position;
            _transform.eulerAngles = direction.DirectionToEulerAngles();
            DiscreteTransform = new DiscreteTransform(position, direction);
            PhysicalSystem.ColliderGrid.RegisterBody(this);
        }
    }
}