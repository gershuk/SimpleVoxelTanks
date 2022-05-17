#nullable enable

using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

namespace SimpleVoxelTanks.TanksAI
{
    public class AbstractTankAI : AbstractTankController
    {
        public DiscretPhysicalBody? Target { get; private set; }

        public virtual void Init (TankDiscreteModel tankDiscreteModel, uint team, DiscretPhysicalBody target)
        {
            base.Init(tankDiscreteModel, team);
            Target = target;
        }
    }
}