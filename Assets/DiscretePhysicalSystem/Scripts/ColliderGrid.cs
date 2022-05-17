#nullable enable

using System.Collections.Generic;

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.DiscretePhysicalSystem
{
    public readonly struct CellCollider
    {
        public readonly DiscretPhysicalBody DiscretPhysicalBody { get; }
        public readonly ulong EndTime { get; }

        public CellCollider (DiscretPhysicalBody body, ulong endTime = ulong.MaxValue) => (DiscretPhysicalBody, EndTime) = (body, endTime);
    }

    public sealed class ColliderGrid
    {
        private readonly HashSet<DiscretPhysicalBody> _bodies;
        private readonly CellCollider[,,] _cells;
        public Vector3UInt Size { get; private set; }

        public CellCollider this[uint x, uint y, uint z]
        {
            get => _cells[x, y, z];
            private set => _cells[x, y, z] = value;
        }

        public CellCollider this[Vector3UInt position]
        {
            get => this[position.X, position.Y, position.Z];
            private set => this[position.X, position.Y, position.Z] = value;
        }

        public ColliderGrid (uint x, uint y, uint z) => (_cells, Size, _bodies) = (new CellCollider[x, y, z], new(x, y, z), new());

        public ColliderGrid (Vector3UInt size) : this(size.X, size.Y, size.Z)
        {
        }

        private void ResetCell (Vector3UInt position) => this[position] = new();

        private bool TrySetCell (DiscretPhysicalBody body)
        {
            var condition = CanSetCell(body, body.DiscreteTransform.Position);

            if (condition)
                this[body.DiscreteTransform.Position] = new(body);

            return condition;
        }

        public bool CanMove (DiscretPhysicalBody body, Vector3UInt to, uint ticks = default) =>
                                    PhysicalSystem.FixedUpdateFrameNumber + ticks > this[to].EndTime
                                    || this[to].DiscretPhysicalBody is null;

        public bool CanSetCell (DiscretPhysicalBody body, Vector3UInt to) =>
                    this[to].DiscretPhysicalBody is null
                    || this[to].DiscretPhysicalBody == body
                    || PhysicalSystem.FixedUpdateFrameNumber > this[to].EndTime;

        public bool CanSetCell (DiscretPhysicalBody body, Vector3UInt to, uint fixedUpdateFrameNumber) =>
                    this[to].DiscretPhysicalBody is null
                    || this[to].DiscretPhysicalBody == body
                    || fixedUpdateFrameNumber > this[to].EndTime;

        public bool CanSetCell (Vector3UInt to) =>
                    this[to].DiscretPhysicalBody is null
                    || PhysicalSystem.FixedUpdateFrameNumber > this[to].EndTime;

        public void RegisterBody (DiscretPhysicalBody body)
        {
            _bodies.Add(body);
            if (!TrySetCell(body))
                Debug.LogError($"{body} cannot be initialized in this position {body.DiscreteTransform.Position}");
        }

        public bool TryClearTempCell (DiscretPhysicalBody body, Vector3UInt position)
        {
            var cond = CanSetCell(body, position) || this[position].EndTime <= PhysicalSystem.FixedUpdateFrameNumber;
            if (cond)
                this[position] = new();
            return cond;
        }

        public bool TryMove (DiscretPhysicalBody body, Vector3UInt to, uint ticks = 1)
        {
            var condition = CanMove(body, to, ticks);

            if (condition)
            {
                this[to] = new(body);
                this[body.DiscreteTransform.Position] = new(body, PhysicalSystem.FixedUpdateFrameNumber + ticks);
            }

            return condition;
        }

        public void UnregisterBody (DiscretPhysicalBody body)
        {
            _bodies.Remove(body);

            if (body.TransaltingPosition.HasValue)
                ResetCell(body.TransaltingPosition.Value);
            ResetCell(body.DiscreteTransform.Position);
        }
    }
}