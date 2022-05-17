using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

namespace SimpleVoxelTanks.TanksAI
{
    public class AbstractTankAI : AbstractTankController
    {
        private DiscretPhysicalBody _target;
        public DiscretPhysicalBody Target { get => _target; private set => _target = value; }

        public virtual void Init (TankDiscreteModel tankDiscreteModel, uint team, DiscretPhysicalBody target)
        {
            base.Init(tankDiscreteModel, team);
            Target = target;
        }
    }
}