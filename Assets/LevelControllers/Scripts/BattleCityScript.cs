using System.Collections;

using SimpleVoxelTanks.CommonComponents;
using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.MapBuilders;
using SimpleVoxelTanks.PlayerInput;
using SimpleVoxelTanks.Tanks;
using SimpleVoxelTanks.TanksAI;

using UnityEngine;

namespace SimpleVoxelTanks.LevelControllers
{
    public class BattleCityScript : AbstractLevelScript
    {
        protected Camera _camera;
        protected CameraFolower _cameraFolower;

        protected GameObject _enemyTankPrefab;
        protected GameObject _playerTankPrefab;

        public const int TeamsCount = 2;
        public static readonly int[] BasesCount = { 1, 0 };
        public static readonly Vector3UInt Size = new(21, 2, 21);
        public uint EnemyTickets { get; protected set; } = 12;
        public uint LiveEnemy { get; protected set; } = 0;
        public uint MaxLiveEnemies { get; protected set; } = 4;
        public WASDTankController PlayerController { get; protected set; }

        private void OnPlayerDied ()
        {
            PlayerController.Init(SpawnPlayerTank()?.GetComponent<TankDiscreteModel>(), 0);
            if (PlayerController.TankDiscreteModel == null)
                RiseLoseEvent();
            else
                PlayerController.TankDiscreteModel.GetComponent<DamageableObject>().OnDied += OnPlayerDied;
        }

        protected void SetUpDamageableObject (DamageableObject damageableObject,
                                                       bool showHpIfGetDamage = true,
                                               bool showHealthBar = default)
        {
            damageableObject.ShowHpIfGetDamage = showHpIfGetDamage;
            damageableObject.ShowHealthBar = showHealthBar;
            damageableObject.SetHpBarCamera(_camera);
        }

        protected IEnumerator SpawnEnemys ()
        {
            var enemyBases = MapBuilder.Teams[0].Bases;
            var targetBase = MapBuilder[enemyBases[Random.Range(0, enemyBases.Count)]].GetComponent<DiscretPhysicalBody>();
            while (EnemyTickets > 0)
            {
                foreach (var _ in MapBuilder.Teams[1].SpawnPoints)
                {
                    if (LiveEnemy >= MaxLiveEnemies)
                        break;

                    var tank = SpawnEnemyTank(Random.Range(0, 2) is 0 || PlayerController.TankDiscreteModel == null
                        ? targetBase
                        : PlayerController.TankDiscreteModel);

                    if (tank != null)
                        EnemyTickets--;

                    if (EnemyTickets == 0)
                        break;
                }
                yield return new WaitForSeconds(20f);
            }
        }

        protected GameObject? SpawnEnemyTank (DiscretPhysicalBody discretPhysicalBody)
        {
            var tank = MapBuilder.TrySpawnObject(_enemyTankPrefab, 1, Direction.Down);
            if (tank != null)
            {
                var damageableObject = tank.GetComponent<DamageableObject>();
                if (damageableObject != null)
                    SetUpDamageableObject(damageableObject);

                ((AbstractTankAI) tank.AddComponent(AiTypes[0])).Init(tank.GetComponent<TankDiscreteModel>(),
                                                                      1,
                                                                      discretPhysicalBody);
                LiveEnemy++;
                damageableObject.OnDied += () =>
                {
                    LiveEnemy--;
                    if (LiveEnemy == 0 && EnemyTickets == 0)
                        RiseWinEvent();
                };
            }
            return tank;
        }

        protected GameObject? SpawnPlayerTank ()
        {
            var tank = MapBuilder.TrySpawnObject(_playerTankPrefab, 0);
            if (tank != null)
            {
                var damageableObject = tank.GetComponent<DamageableObject>();
                if (damageableObject != null)
                    SetUpDamageableObject(damageableObject);
                _cameraFolower.Init(new(0, 15, -8), tank.transform, Vector3.positiveInfinity, Vector3.negativeInfinity);
            }
            return tank;
        }

        protected WASDTankController SpwanController () => new GameObject("PlayerController").AddComponent<WASDTankController>();

        public override void Init (AbstractMapBuilder abstractMapBuilder)
        {
            base.Init(abstractMapBuilder);

            var cameraObject = new GameObject("Camera");
            _camera = cameraObject.AddComponent<Camera>();
            _cameraFolower = cameraObject.AddComponent<CameraFolower>();

            _aiTypes = new[] { typeof(AStarAI) };
            _enemyTankPrefab ??= Resources.Load<GameObject>("Prefabs/SimpleTank");
            _playerTankPrefab ??= Resources.Load<GameObject>("Prefabs/SpeedTank");
            PhysicalSystem.Init(Size);
            MapBuilder = abstractMapBuilder;
            MapBuilder.Init(Size, TeamsCount, BasesCount);
        }

        public override void StartScript ()
        {
            MapBuilder.BuildScene();
            PlayerController = SpwanController();
            PlayerController.Init(SpawnPlayerTank()?.GetComponent<TankDiscreteModel>(), 0);
            StartCoroutine(SpawnEnemys());

            var blocks = MapBuilder.GetBlocksArrayCopy();
            foreach (var block in blocks)
            {
                var damageableObject = block?.GetComponent<DamageableObject>();
                if (damageableObject != null)
                    SetUpDamageableObject(damageableObject, true, true);
            }

            MapBuilder[MapBuilder.Teams[0].Bases[0]].GetComponent<DamageableObject>().OnDied += RiseLoseEvent;
            PlayerController.TankDiscreteModel.GetComponent<DamageableObject>().OnDied += OnPlayerDied;
        }
    }
}