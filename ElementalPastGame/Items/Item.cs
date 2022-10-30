using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Items
{
    public interface Item
    {
        enum ItemType
        {
            Consumable,
            Equipment,
            KeyItem
        }
        public ItemType type { get; }
        public int itemID { get; }
    }
}
