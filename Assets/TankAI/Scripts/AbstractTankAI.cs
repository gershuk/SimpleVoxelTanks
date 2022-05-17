#nullable enable

using System;

using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

using UnityEngine;

namespace SimpleVoxelTanks.TanksAI
{
    public class AbstractTankAI : AbstractTankController
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public Func<GameObject, DiscretPhysicalBody?> GetNewTarget { get; protected set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
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