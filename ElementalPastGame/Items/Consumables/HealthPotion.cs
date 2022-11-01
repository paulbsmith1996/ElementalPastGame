using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.Items.Consumables
{
    public class HealthPotion : Item
    {
        public ItemType type { get { return ItemType.Consumable; } }
        public int itemID { get { return Item.HEALTH_POTION_ID; } }

        public String displayName {  get { return "Health Potion"; } }
    }
}
