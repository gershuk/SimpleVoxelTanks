#nullable enable

using SimpleVoxelTanks.DiscretePhysicalSystem;
using SimpleVoxelTanks.Tanks;

using UnityEngine;

namespace SimpleVoxelTanks.PlayerInput
{
    public class WASDTankController : AbstractTankController
    {
        [SerializeField]
        private TankDiscreteModel _body;

#pragma warning disable CS8618 // ����, �� ����������� �������� NULL, ������ ��������� ��������, �������� �� NULL, ��� ������ �� ������������. ��������, ����� �������� ���� ��� ����������� �������� NULL.
        private WASDInput _inputActions;
#pragma warning restore CS8618 // ����, �� ����������� �������� NULL, ������ ��������� ��������, �������� �� NULL, ��� ������ �� ������������. ��������, ����� �������� ���� ��� ����������� �������� NULL.

        protected override void Awake ()
        {
            base.Awake();
            _inputActions = new();
        }

        protected void OnDisable () => _inputActions.Disable();

        protected void OnEnable () => _inputActions.Enable();

        protected void Update ()
        {
            if (TankDiscreteModel == null)
                return;
            TankDiscreteModel.Move(_inputActions.Player.Move.ReadValue<Vector2>().Vector2ToDirection());
            if (_inputActions.Player.Shoote.IsPressed())
                TankDiscreteModel.Shoot();
        }
    }
}