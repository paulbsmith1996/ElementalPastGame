using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.Items.Consumables
{
    public class MinorHealthPotion : Item
    {
        public ItemType type { get { return ItemType.Consumable; } }
        public int itemID { get { return Item.MINOR_HEALTH_POTION_ID; } }
        public String displayName { get { return "Minor Health Potion"; } }
    }
}
