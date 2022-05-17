#nullable enable

using UnityEngine;

namespace SimpleVoxelTanks.Tanks
{
    public abstract class AbstractTankController : MonoBehaviour
    {
        private uint _team;
        protected Transform _transform;
        public TankDiscreteModel TankDiscreteModel { get; protected set; }

        public uint Team { get => _team; protected set => _team = value; }

        protected virtual void Awake ()
        {
            _transform = transform;
        }

        public virtual void Init (TankDiscreteModel tankDiscreteModel, uint team) => (TankDiscreteModel, Team) = (tankDiscreteModel, team);
    }
}