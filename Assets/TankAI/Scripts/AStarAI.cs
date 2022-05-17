#nullable enable

using System;
using System.Collections.Generic;

using SimpleVoxelTanks.Bullets;
using SimpleVoxelTanks.CommonComponents;
using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

using UnityEngine;

namespace SimpleVoxelTanks.TanksAI
{
    public sealed class AStarAI : AbstractTankAI
    {
        [SerializeField]
        private uint _lastPathTickNumber = 0;

        private List<Vector3UInt>? _path;

        [SerializeField]
        private uint _tickPathFindCooldown = 1;

        private int _wayPointindex = 1;

        public static Vector3Int[] Movement { get; private set; } = new Vector3Int[]
                                {
            new (-1,0,0),
            new (0,0,-1),
            new (1,0,0),
            new (0,0,1)
        };

        private void FixedUpdate ()
        {
            if (TankDiscreteModel == null)
                return;

            if (!(TankDiscreteModel.IsRotating || TankDiscreteModel.IsRotating))
            {
                if (PhysicalSystem.FixedUpdateFrameNumber - _tickPathFindCooldown > _lastPathTickNumber)
                {
                    if (Target == null)
                        Target = GetNewTarget(gameObject);
                    if (Target != null)
                    {
                        _path = PathFinder.AStar(TankDiscreteModel,
                                                      new() { Target.DiscreteTransform.Position },
                                                      Movement,
                                                      true,
                                                      TankDiscreteModel.ShootCooldown,
                                                      TankDiscreteModel.BulletPrefab.GetComponent<Bullet>().DamagePoints);
                        _lastPathTickNumber = PhysicalSystem.FixedUpdateFrameNumber;
                        _wayPointindex = 1;
                    }
                }

                if (_path?.Count > 1)
                {
                    if (_wayPointindex < _path.Count - 1 && _path[^(1 + _wayPointindex)] == TankDiscreteModel.DiscreteTransform.Position)
                        _wayPointindex++;
                    var delta = (Vector3Int) _path[^(1 + _wayPointindex)] - TankDiscreteModel.DiscreteTransform.Position;
                    TankDiscreteModel.Move(new Vector2Int(delta.x, delta.z).Vector2IntToDirection());
                }
            }

            if (Physics.Raycast(_transform.position, TankDiscreteModel.BulletSpawnPoint.forward, out var info)
                && info.transform != null && info.transform.GetComponent<DamageableObject>() != null)
            {
                var bullet = info.transform.GetComponent<Bullet>();
                if (bullet != null)
                {
                    if (info.transform.forward * -1 == TankDiscreteModel.DiscreteTransform.Direction.DirectionToDeltaPosition())
                        TankDiscreteModel.Shoot();
                    else
                        return;
                }

                var abstractContoller = info.transform.GetComponent<AbstractTankController>();
                if (abstractContoller == null || abstractContoller.Team != Team)
                    TankDiscreteModel.Shoot();
            };
        }

        public override void Init (TankDiscreteModel tankDiscreteModel,
                                   uint team,
                                   Func<GameObject, DiscretPhysicalBody?> getNewTarget,
                                   DiscretPhysicalBody? target = default)
        {
            base.Init(tankDiscreteModel, team, getNewTarget, target);
            _tickPathFindCooldown = Math.Max(TankDiscreteModel.MovementTicks, TankDiscreteModel.RotationTicks);
        }
    }
}