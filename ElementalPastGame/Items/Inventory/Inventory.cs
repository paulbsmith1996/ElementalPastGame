using ElementalPastGame.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.Items.Inventory
{
    public class Inventory
    {
        internal Dictionary<ItemType, List<InventoryItemEntry>> inventory = new();

        public void AddItem(Item item, int amount)
        {
            List<InventoryItemEntry>? typedItems = inventory.GetValueOrDefault(item.type);
            if (typedItems == null)
            {
                // This pocket does not exist yet
                InventoryItemEntry newEntry = new()
                {
                    item = item,
                    count = amount
                };
                inventory[item.type] = new List<InventoryItemEntry>() { newEntry };
                return;
            }

            InventoryItemEntry? existingEntry = GetExistingEntryForItem(item);

            if (existingEntry != null)
            {
                // Pocket exists and contains an existing entry for item
                InventoryItemEntry nonnullExistingEntry = (InventoryItemEntry)existingEntry;
                nonnullExistingEntry.count += amount;
                return;
            }

            // Pocket exists abut does not contain an existing entry for item
            InventoryItemEntry entryToAppend = new()
            {
                item = item,
                count = amount
            };
            inventory[item.type].Add(entryToAppend);
        }

        public bool RemoveItem(Item item, int amount)
        {
            InventoryItemEntry? existingEntry = GetExistingEntryForItem(item);
            if (existingEntry == null || ((InventoryItemEntry)existingEntry).count < amount)
            {
                return false;
            }

            InventoryItemEntry nonnullExistingEntry = (InventoryItemEntry)existingEntry;
            nonnullExistingEntry.count -= amount;
            return true;
        }

        public List<InventoryItemEntry> GetItemEntriesForType(ItemType type)
        {
            return inventory[type] ?? new();
        }

        internal InventoryItemEntry? GetExistingEntryForItem(Item item)
        {
            List<InventoryItemEntry> typedItems = inventory[item.type];
            if (typedItems == null)
            {
                return null;
            }

            foreach (InventoryItemEntry entry in typedItems)
            {
                if (entry.item.itemID == item.itemID)
                {
                    return entry;
                }
            }

            return null;
        }

        public static Inventory DebugInventory()
        {
            Inventory debugInventory = new();
            debugInventory.AddItem(new HealthPotion(), 3);
            debugInventory.AddItem(new MinorHealthPotion(), 2);
            return debugInventory;
        }
    }
}
