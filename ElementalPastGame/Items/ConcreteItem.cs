using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items
{
    public class ConcreteItem
    {
        public ItemType type { get; set; }
        public int itemID { get; set; }

        public String displayName { get; set; }
        public int basePrice { get; set; }

        public ConcreteItem(String displayName, ItemType type, int itemID, int basePrice)
        {
            this.type = type;
            this.itemID = itemID;
            this.displayName = displayName;
            this.basePrice = basePrice;
        }
    }
}
