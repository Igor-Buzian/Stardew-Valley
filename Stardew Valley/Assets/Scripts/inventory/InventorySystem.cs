// File: Scripts/Inventory/InventorySystem.cs

using System.Collections.Generic;
using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Inventory
{
    /// <summary>
    /// Slot-based inventory system with stacking support.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private int size = 24;
        [SerializeField] private int hotbarSize = 8;
        [SerializeField] private int maxStack = 99;

        private List<ItemStack> _items;
        private int _selectedSlot;

        public void Init()
        {
            _items = new List<ItemStack>(size);

            // Pre-fill empty slots
            for (int i = 0; i < size; i++)
                _items.Add(null);

            _selectedSlot = 0;

            GameEvents.RaiseInventoryChanged();
        }

        // ─────────────────────────────────────────────
        // ITEM LOGIC
        // ─────────────────────────────────────────────

        public bool AddItem(string itemId, int amount = 1)
        {
            // 1. Try stacking into existing slots
            for (int i = 0; i < _items.Count; i++)
            {
                var slot = _items[i];

                if (slot != null && slot.itemId == itemId && slot.amount < maxStack)
                {
                    int space = maxStack - slot.amount;
                    int toAdd = Mathf.Min(space, amount);

                    slot.amount += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        GameEvents.RaiseInventoryChanged();
                        return true;
                    }
                }
            }

            // 2. Fill empty slots
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null)
                {
                    int toAdd = Mathf.Min(maxStack, amount);

                    _items[i] = new ItemStack
                    {
                        itemId = itemId,
                        amount = toAdd
                    };

                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        GameEvents.RaiseInventoryChanged();
                        return true;
                    }
                }
            }

            // Inventory full
            GameEvents.RaiseInventoryChanged();
            return false;
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var slot = _items[i];
                if (slot == null || slot.itemId != itemId)
                    continue;

                if (slot.amount > amount)
                {
                    slot.amount -= amount;
                    GameEvents.RaiseInventoryChanged();
                    return true;
                }

                amount -= slot.amount;
                _items[i] = null;

                if (amount <= 0)
                {
                    GameEvents.RaiseInventoryChanged();
                    return true;
                }
            }

            GameEvents.RaiseInventoryChanged();
            return false;
        }

        public bool HasItem(string itemId, int amount = 1)
        {
            int total = 0;

            foreach (var slot in _items)
            {
                if (slot != null && slot.itemId == itemId)
                    total += slot.amount;

                if (total >= amount)
                    return true;
            }

            return false;
        }

        // ─────────────────────────────────────────────
        // HOTBAR
        // ─────────────────────────────────────────────

        public void SetHotbarSlot(int index)
        {
            _selectedSlot = Mathf.Clamp(index, 0, hotbarSize - 1);

            GameEvents.RaiseHotbarSlotChanged(_selectedSlot);
        }

        public ItemStack GetSelectedItem()
        {
            if (_selectedSlot >= _items.Count)
                return null;

            return _items[_selectedSlot];
        }

        // ─────────────────────────────────────────────
        // SAVE / LOAD
        // ─────────────────────────────────────────────

        public InventoryData GetSaveData()
        {
            var data = new InventoryData
            {
                selectedSlot = _selectedSlot,
                items = new List<ItemStackData>()
            };

            foreach (var slot in _items)
            {
                data.items.Add(slot == null
                    ? null
                    : new ItemStackData
                    {
                        itemId = slot.itemId,
                        amount = slot.amount
                    });
            }

            return data;
        }

        public void LoadFromData(InventoryData data)
        {
            _items = new List<ItemStack>(size);

            foreach (var s in data.items)
            {
                if (s == null)
                {
                    _items.Add(null);
                }
                else
                {
                    _items.Add(new ItemStack
                    {
                        itemId = s.itemId,
                        amount = s.amount
                    });
                }
            }

            _selectedSlot = data.selectedSlot;

            GameEvents.RaiseInventoryChanged();
        }
    }

    // ─────────────────────────────────────────────
    // DATA STRUCTURES
    // ─────────────────────────────────────────────

    [System.Serializable]
    public class ItemStack
    {
        public string itemId;
        public int amount;
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<ItemStackData> items;
        public int selectedSlot;
    }

    [System.Serializable]
    public class ItemStackData
    {
        public string itemId;
        public int amount;
    }
}