// File: Scripts/Systems/CurrencySystem.cs
// Purpose: Single source of truth for player gold/currency.
// WHY: Having gold scattered across Player or InventorySystem causes
//      sync bugs. One system, one responsibility.

using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Systems
{
    public class CurrencySystem : MonoBehaviour
    {
        [Header("Starting Gold")]
        [SerializeField] private int startingGold = 500;

        private int _gold;

        public int Gold => _gold;

        public void Init()
        {
            _gold = startingGold;
            GameEvents.RaiseGoldChanged(_gold);
        }

        public bool TrySpend(int amount)
        {
            if (_gold < amount) return false;
            _gold -= amount;
            GameEvents.RaiseGoldChanged(_gold);
            return true;
        }

        public void Add(int amount)
        {
            _gold += amount;
            GameEvents.RaiseGoldChanged(_gold);
        }

        // Save/Load
        public int GetSaveData() => _gold;
        public void LoadFromData(int gold) { _gold = gold; GameEvents.RaiseGoldChanged(_gold); }
    }
}