using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Items.Inventory
{
    public struct InventoryItemEntry
    {
        public Item item { get; set; }
        public int count { get; set; }
    }
}
