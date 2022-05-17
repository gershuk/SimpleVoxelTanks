#nullable enable

using System;

using UnityEngine;

namespace SimpleVoxelTanks.CommonComponents
{
    public class DamageableObject : MonoBehaviour
    {
        public event Action? OnDied;

        public event Action<int>? OnTakeDamage;

        [SerializeField]
        private int _healthPoints;

        [SerializeField]
        private int _maxHealthPoints;

        [SerializeField]
        private bool _showHpIfGetDamage;

        #region UnityObjectaComponents

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected GameObject _gameObject;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected Transform _transform;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

        #endregion UnityObjectaComponents

        [SerializeField]
        protected HpBarController? _hpBarController;

        protected int MaxHealthPoints { get => _maxHealthPoints; set => _maxHealthPoints = value; }

        public int HealthPoints
        {
            get => _healthPoints;
            protected set
            {
                _healthPoints = value <= MaxHealthPoints
                                ? value
                                : throw new ArgumentOutOfRangeException("_healthPoints > MaxHealthPoints");

                TryUpdateHpBar();

                if (_healthPoints <= 0)
                {
                    OnDied?.Invoke();
                    Destroy(_gameObject);
                }
            }
        }

        public bool ShowHealthBar
        {
            get => _hpBarController != null && _hpBarController.gameObject.activeInHierarchy;
            set
            {
                if (_hpBarController != null)
                    _hpBarController.gameObject.SetActive(value);
            }
        }

        public bool ShowHpIfGetDamage { get => _showHpIfGetDamage; set => _showHpIfGetDamage = value; }

        protected virtual void Awake ()
        {
            (_transform, _gameObject) = (transform, gameObject);
            TryUpdateHpBar();
        }

        protected void TryUpdateHpBar ()
        {
            if (_hpBarController != null && ShowHealthBar)
                _hpBarController.Health = HealthPoints * 1f / MaxHealthPoints;
        }

        public void Heal (int healthPoints) => HealthPoints = Math.Max(HealthPoints + healthPoints, MaxHealthPoints);

        public void SetHpBarCamera (Camera camera)
        {
            if (_hpBarController != null)
                _hpBarController.Camera = camera;
        }

        public void TakeDamege (int healthPoints)
        {
            if (ShowHpIfGetDamage)
                ShowHealthBar = true;
            OnTakeDamage?.Invoke(healthPoints);
            HealthPoints -= healthPoints;
        }
    }
}