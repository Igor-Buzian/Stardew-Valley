// File: Scripts/Systems/EnergySystem.cs
// Purpose: Manages player stamina/energy used by farming actions.
// WHY: Energy is shared state that both Player and UI read.
//      Centralizing it here prevents the Player script from bloating.

using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Systems
{
    public class EnergySystem : MonoBehaviour
    {
        [Header("Energy Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float restoreOnSleep = 100f; // restore to full

        private float _currentEnergy;

        public float CurrentEnergy => _currentEnergy;
        public float MaxEnergy     => maxEnergy;
        public bool  HasEnergy     => _currentEnergy > 0f;

        public void Init()
        {
            _currentEnergy = maxEnergy;
            GameEvents.OnMorningBegan += OnMorningBegan;
            NotifyChange();
        }

        private void OnDestroy()
        {
            GameEvents.OnMorningBegan -= OnMorningBegan;
        }

        /// <summary>Attempt to spend energy. Returns false if insufficient.</summary>
        public bool TrySpend(float amount)
        {
            if (_currentEnergy < amount) return false;

            _currentEnergy = Mathf.Max(0f, _currentEnergy - amount);
            NotifyChange();

            if (_currentEnergy <= 0f)
                GameEvents.RaisePlayerDied();

            return true;
        }

        public void Restore(float amount)
        {
            _currentEnergy = Mathf.Min(maxEnergy, _currentEnergy + amount);
            NotifyChange();
        }

        private void OnMorningBegan() => RestoreForNewDay();

        public void RestoreForNewDay()
        {
            _currentEnergy = Mathf.Min(maxEnergy, _currentEnergy + restoreOnSleep);
            NotifyChange();
        }

        private void NotifyChange() =>
            GameEvents.RaiseEnergyChanged(_currentEnergy, maxEnergy);

        // Save/Load
        public float GetSaveData() => _currentEnergy;
        public void LoadFromData(float energy) { _currentEnergy = energy; NotifyChange(); }
    }
}