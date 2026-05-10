// File: Scripts/Core/GameEvents.cs
// Purpose: Central event bus for decoupled communication between systems.
// WHY: Instead of direct references between systems (tight coupling),
//      systems publish/subscribe to events. This means TimeSystem doesn't
//      need to know about FarmingSystem — it just fires OnDayChanged.

using System;
using UnityEngine;

namespace FarmSim.Core
{
    /// <summary>
    /// Static event hub. Systems subscribe here instead of holding cross-references.
    /// Pattern: Observer / Event Bus
    /// </summary>
    public static class GameEvents
    {
        // ─── Time Events ────────────────────────────────────────────────────
        public static event Action<int> OnDayChanged;          // int = new day number
        public static event Action<float> OnTimeChanged;        // float = normalized 0-1
        public static event Action OnNightBegan;
        public static event Action OnMorningBegan;

        // ─── Farming Events ──────────────────────────────────────────────────
        public static event Action<Vector3Int> OnSoilTilled;
        public static event Action<Vector3Int> OnSoilWatered;
        public static event Action<Vector3Int, string> OnSeedPlanted;   // pos, cropId
        public static event Action<Vector3Int> OnCropHarvested;
        public static event Action<Vector3Int> OnCropGrew;

        // ─── Inventory Events ────────────────────────────────────────────────
        public static event Action OnInventoryChanged;
        public static event Action<int> OnHotbarSlotChanged;    // int = slot index
        public static event Action<int> OnGoldChanged;          // int = new total

        // ─── Player Events ───────────────────────────────────────────────────
        public static event Action<float, float> OnEnergyChanged;  // current, max
        public static event Action OnPlayerDied;                    // energy depleted

        // ─── NPC Events ──────────────────────────────────────────────────────
        public static event Action<string> OnDialogueStarted;   // npcId
        public static event Action OnDialogueEnded;
        public static event Action<string> OnShopOpened;         // shopId

        // ─── Scene / Game State Events ───────────────────────────────────────
        public static event Action OnGameSaved;
        public static event Action OnGameLoaded;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;

        // ─── Raise helpers (keeps call sites clean) ─────────────────────────
        public static void RaiseDayChanged(int day)              => OnDayChanged?.Invoke(day);
        public static void RaiseTimeChanged(float t)             => OnTimeChanged?.Invoke(t);
        public static void RaiseNightBegan()                     => OnNightBegan?.Invoke();
        public static void RaiseMorningBegan()                   => OnMorningBegan?.Invoke();

        public static void RaiseSoilTilled(Vector3Int pos)       => OnSoilTilled?.Invoke(pos);
        public static void RaiseSoilWatered(Vector3Int pos)      => OnSoilWatered?.Invoke(pos);
        public static void RaiseSeedPlanted(Vector3Int p, string id) => OnSeedPlanted?.Invoke(p, id);
        public static void RaiseCropHarvested(Vector3Int pos)    => OnCropHarvested?.Invoke(pos);
        public static void RaiseCropGrew(Vector3Int pos)         => OnCropGrew?.Invoke(pos);

        public static void RaiseInventoryChanged()               => OnInventoryChanged?.Invoke();
        public static void RaiseHotbarSlotChanged(int s)         => OnHotbarSlotChanged?.Invoke(s);
        public static void RaiseGoldChanged(int g)               => OnGoldChanged?.Invoke(g);

        public static void RaiseEnergyChanged(float cur, float max) => OnEnergyChanged?.Invoke(cur, max);
        public static void RaisePlayerDied()                     => OnPlayerDied?.Invoke();

        public static void RaiseDialogueStarted(string id)       => OnDialogueStarted?.Invoke(id);
        public static void RaiseDialogueEnded()                  => OnDialogueEnded?.Invoke();
        public static void RaiseShopOpened(string id)            => OnShopOpened?.Invoke(id);

        public static void RaiseGameSaved()                      => OnGameSaved?.Invoke();
        public static void RaiseGameLoaded()                     => OnGameLoaded?.Invoke();
        public static void RaiseGamePaused()                     => OnGamePaused?.Invoke();
        public static void RaiseGameResumed()                    => OnGameResumed?.Invoke();
    }
}