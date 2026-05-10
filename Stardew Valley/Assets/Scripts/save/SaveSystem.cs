using UnityEngine;
using FarmSim.Core;
using FarmSim.Systems;
using FarmSim.Inventory;
using FarmSim.Farming;

namespace FarmSim.Save
{
    /// <summary>
    /// Central save/load coordinator.
    /// Builds and restores full game snapshot.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private GameSaveData _current;

        public void Init()
        {
            Debug.Log("[SaveSystem] Init");
        }

        // ─────────────────────────────────────────────
        // SAVE
        // ─────────────────────────────────────────────

        public void Save()
        {
            _current = new GameSaveData();

            if (ServiceLocator.TryGet(out CurrencySystem currency))
                _current.gold = currency.GetSaveData();

            if (ServiceLocator.TryGet(out EnergySystem energy))
                _current.energy = energy.GetSaveData();

            if (ServiceLocator.TryGet(out TimeSystem time))
                _current.time = time.GetSaveData();

            if (ServiceLocator.TryGet(out InventorySystem inventory))
                _current.inventory = inventory.GetSaveData();

            if (ServiceLocator.TryGet(out FarmingSystem farming))
                _current.farming = farming.GetSaveData();

            GameEvents.RaiseGameSaved();
        }

        // ─────────────────────────────────────────────
        // LOAD (FIXED: actually applies data)
        // ─────────────────────────────────────────────

        public void Load()
        {
            if (_current == null)
            {
                Debug.LogWarning("[SaveSystem] No save data found");
                return;
            }

            Apply(_current);
            GameEvents.RaiseGameLoaded();
        }

        // ─────────────────────────────────────────────
        // APPLY SNAPSHOT TO GAME
        // ─────────────────────────────────────────────

        private void Apply(GameSaveData data)
        {
            if (ServiceLocator.TryGet(out CurrencySystem currency))
                currency.LoadFromData(data.gold);

            if (ServiceLocator.TryGet(out EnergySystem energy))
                energy.LoadFromData(data.energy);

            if (ServiceLocator.TryGet(out TimeSystem time))
                time.LoadFromData(data.time);

            if (ServiceLocator.TryGet(out InventorySystem inventory))
                inventory.LoadFromData(data.inventory);

            if (ServiceLocator.TryGet(out FarmingSystem farming))
                farming.LoadFromData(data.farming);
        }

        public GameSaveData GetCurrentData() => _current;
    }

    // ─────────────────────────────────────────────
    // DATA MODEL
    // ─────────────────────────────────────────────

    [System.Serializable]
    public class GameSaveData
    {
        public int gold;
        public float energy;

        public TimeData time;
        public InventoryData inventory;
        public FarmingData farming;
    }
}