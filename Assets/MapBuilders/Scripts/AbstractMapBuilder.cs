using System;
using System.Collections.Generic;

using SimpleVoxelTanks.CommonComponents;
using SimpleVoxelTanks.DiscretePhysicalSystem;

using UnityEngine;

namespace SimpleVoxelTanks.MapBuilders
{
    public abstract class AbstractMapBuilder : MonoBehaviour
    {
        protected GameObject[,,] _blocks;
        protected Team[] _teams;
        protected Transform _transform;

        public Vector3UInt Size { get; private set; }
        public IReadOnlyList<Team> Teams => _teams;

        public GameObject this[uint x, uint y, uint z]
        {
            get => _blocks[x, y, z];
            protected set => _blocks[x, y, z] = value;
        }

        public GameObject this[Vector3UInt position]
        {
            get => this[position.X, position.Y, position.Z];
            protected set => this[position.X, position.Y, position.Z] = value;
        }

        protected virtual GameObject? TrySpawnBlock (GameObject prefab, Vector3UInt position, Direction startDirection = default)
        {
            this[position] = TrySpawnObject(prefab, position, startDirection);
            if (this[position] is null)
                Debug.LogError($"Can't spawn block at {position}");
            return this[position];
        }

        protected virtual GameObject? TrySpawnObject (GameObject prefab, Vector3UInt position, Direction startDirection = default)
        {
            var gameObject = PhysicalSystem.ColliderGrid.CanSetCell(position)
                        ? Instantiate(prefab, position, Quaternion.Euler(startDirection.DirectionToEulerAngles()), _transform)
                        : null;
            gameObject?.GetComponent<DiscretPhysicalBody>()?.Init(position, startDirection);
            return gameObject;
        }

        public abstract void BuildScene ();

        public void DestroyScene ()
        {
            foreach (var gameObject in _blocks)
                GameObject.Destroy(gameObject);
        }

        public GameObject[,,] GetBlocksArrayCopy () => (GameObject[,,]) _blocks.Clone();

        public virtual void Init (uint x, uint y, uint z, int teamsCount, int[] teamsBaseCount)
        {
            if (teamsCount != teamsBaseCount.Length)
                throw new ArgumentException(nameof(teamsBaseCount), "teamsCount !=  teamsBaseCount.Count");

            _transform = transform;
            _blocks = new GameObject[x, y, z];
            Size = new(x, y, z);
            _teams = new Team[teamsCount];
        }

        public void Init (Vector3UInt size, int teamsCount, int[] teamsBaseCount) =>
            Init(size.X, size.Y, size.Z, teamsCount, teamsBaseCount);

        public GameObject? TrySpawnObject (GameObject prefab, int teamIndex, Direction startDirection = default)
        {
            foreach (var point in _teams[teamIndex].SpawnPoints)
            {
                if (PhysicalSystem.ColliderGrid.CanSetCell(point))
                    return TrySpawnObject(prefab, point, startDirection);
            }

            return null;
        }
    }

    public class BadMapSizeException : Exception
    {
        public BadMapSizeException (Vector3UInt settedSize, Vector3UInt requiredSize)
            : base($"Minimal map size is {requiredSize}, but setted {settedSize}")
        {
        }
    }

    public class Team
    {
        private readonly Vector3UInt[] _bases;
        private readonly Vector3UInt[] _spawnPoints;

        public IReadOnlyList<Vector3UInt> Bases => _bases;

        public HashSet<GameObject> Members { get; protected set; }
        public IReadOnlyList<Vector3UInt> SpawnPoints => _spawnPoints;

        public Team (Vector3UInt[] bases, Vector3UInt[] spawnPoints)
        {
            _bases = bases ?? throw new ArgumentNullException(nameof(bases));
            _spawnPoints = spawnPoints ?? throw new ArgumentNullException(nameof(spawnPoints));
            Members = new();
        }
    }
}