#nullable enable

using System;

using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

using UnityEngine;

namespace SimpleVoxelTanks.TanksAI
{
    public class AbstractTankAI : AbstractTankController
    {
#pragma warning disable CS8618 // ����, �� ����������� �������� NULL, ������ ��������� ��������, �������� �� NULL, ��� ������ �� ������������. ��������, ����� �������� ���� ��� ����������� �������� NULL.
        public Func<GameObject, DiscretPhysicalBody?> GetNewTarget { get; protected set; }
#pragma warning restore CS8618 // ����, �� ����������� �������� NULL, ������ ��������� ��������, �������� �� NULL, ��� ������ �� ������������. ��������, ����� �������� ���� ��� ����������� �������� NULL.
        public DiscretPhysicalBody? Target { get; protected set; }

        public virtual void Init (TankDiscreteModel tankDiscreteModel,
                                  uint team,
                                  Func<GameObject, DiscretPhysicalBody?> getNewTarget,
                                  DiscretPhysicalBody? target = default)
        {
            base.Init(tankDiscreteModel, team);
            (GetNewTarget, Target) = (getNewTarget, target);
        }
    }
}