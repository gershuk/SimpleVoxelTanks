using System;

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.MapBuilders
{
    public sealed class BoxBuilder : AbstractMapBuilder
    {
        #region Prefabs

        private readonly GameObject[] _basesBlocks = new GameObject[2];
        private GameObject _destructibleWall;
        private GameObject _ground;
        private GameObject _nonDestructibleWall;

        #endregion Prefabs

        private int[] _teamsBaseCount;

        private void BuildGround ()
        {
            for (var w = 0u; w < Size.X; ++w)
            {
                for (var z = 0u; z < Size.Z; ++z)
                {
                    TrySpawnBlock(_ground, new(w, 0, z));
                }
            }
        }

        private void BuildObstacleWalls ()
        {
            for (var w = 1u; w < Size.X - 1; ++w)
            {
                for (var h = 3u; h < Size.Z - 3; ++h)
                {
                    TrySpawnBlock(_destructibleWall, new(w, 1, h));
                }
            }
        }

        private void BuildWorldBorderWalls ()
        {
            for (var w = 0u; w < Size.X; ++w)
            {
                TrySpawnBlock(_nonDestructibleWall, new(w, 1, 0));
                TrySpawnBlock(_nonDestructibleWall, new(w, 1, Size.Z - 1));
            }

            for (var z = 1u; z < Size.Z - 1; ++z)
            {
                TrySpawnBlock(_nonDestructibleWall, new(0, 1, z));
                TrySpawnBlock(_nonDestructibleWall, new(Size.X - 1, 1, z));
            }
        }

        private void InitTeams ()
        {
            uint[] z = { 1, Size.Z - 2 };
            var baseWallOffset = new[]
            {
                new Vector3Int[] { new(-1, 0, 0), new(-1, 0, 1), new(0, 0, 1), new(1,0,1), new(1,0,0) },
                new Vector3Int[] { new(-1, 0, 0), new(-1, 0, -1), new(0, 0, -1), new(1,0,-1), new(1,0,0) },
            };
            for (var i = 0; i < 2; ++i)
            {
                var spawnPoints = new Vector3UInt[]
                {
                    new(1, 1, z[i]),
                    new(Size.X-2, 1, z[i]),
                    new(Size.X / 2 - 2, 1, z[i]),
                    new(Size.X / 2 + 2, 1, z[i]),
                };

                var bases = new Vector3UInt[_teamsBaseCount[i]];
                if (bases.Length > 0)
                {
                    Vector3UInt position = new(Size.X / 2, 1, z[i]);
                    TrySpawnBlock(_basesBlocks[i], position);
                    bases[i] = position;

                    foreach (var offset in baseWallOffset[i])
                    {
                        TrySpawnBlock(_destructibleWall, (Vector3UInt) (position + offset));
                    }
                }

                _teams[i] = new(bases, spawnPoints);
            }
        }

        public override void BuildScene ()
        {
            BuildGround();

            BuildWorldBorderWalls();

            BuildObstacleWalls();

            InitTeams();
        }

        public override void Init (uint x, uint y, uint z, int teamsCount, int[] teamsBaseCount)
        {
            _basesBlocks[0] ??= Resources.Load<GameObject>("Prefabs/RedFlag");
            _basesBlocks[1] ??= Resources.Load<GameObject>("Prefabs/BlueFlag");
            _nonDestructibleWall ??= Resources.Load<GameObject>("Prefabs/StoneBlock");
            _destructibleWall ??= Resources.Load<GameObject>("Prefabs/BrickBlock");
            _ground ??= Resources.Load<GameObject>("Prefabs/GrassBlock");

            if (x < 7 || y < 2 || z < 7)
                throw new BadMapSizeException(new(x, y, z), new(10, 2, 10));

            if (teamsCount != 2)
                throw new ArgumentOutOfRangeException(nameof(teamsCount), "Teams count should be == 2");

            if (teamsBaseCount[0] > 1)
                throw new ArgumentOutOfRangeException($"First team has {teamsBaseCount[0]}. Teams shoud have one|none base");

            if (teamsBaseCount[1] > 1)
                throw new ArgumentOutOfRangeException($"First team has {teamsBaseCount[1]}. Teams shoud have one|none base");

            base.Init(x, y, z, teamsCount, teamsBaseCount);

            _teamsBaseCount = teamsBaseCount;
        }
    }
}