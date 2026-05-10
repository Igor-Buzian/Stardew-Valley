// File: Scripts/Core/Bootstrap.cs
// Purpose: Controls initialization ORDER across all systems.
// WHY: Unity's Script Execution Order settings in ProjectSettings are fragile
//      and hard to reason about across scenes. A single Bootstrap MonoBehaviour
//      with explicit ordering is far more maintainable.
//      Pattern: Composition Root

using UnityEngine;
using FarmSim.Systems;
using FarmSim.Farming;
using FarmSim.Inventory;
using FarmSim.Save;

namespace FarmSim.Core
{
    /// <summary>
    /// Attach to a "Bootstrap" GameObject in every gameplay scene.
    /// Drag system references in Inspector; Bootstrap calls Init() in the right order.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        [Header("Core Systems (assign in Inspector)")]
        [SerializeField] private TimeSystem timeSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private FarmingSystem farmingSystem;
        [SerializeField] private EnergySystem energySystem;
        [SerializeField] private CurrencySystem currencySystem;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private AudioManager audioManager;

        private void Awake()
        {
            // 1. Register services BEFORE any Start() calls
            ServiceLocator.Register(timeSystem);
            ServiceLocator.Register(inventorySystem);
            ServiceLocator.Register(farmingSystem);
            ServiceLocator.Register(energySystem);
            ServiceLocator.Register(currencySystem);
            ServiceLocator.Register(saveSystem);
            ServiceLocator.Register(audioManager);
        }

        private void Start()
        {
            // 2. Initialize systems in dependency order
            audioManager.Init();
            currencySystem.Init();
            inventorySystem.Init();
            energySystem.Init();
            farmingSystem.Init();
            timeSystem.Init();   // TimeSystem last — it drives events that others listen to
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}