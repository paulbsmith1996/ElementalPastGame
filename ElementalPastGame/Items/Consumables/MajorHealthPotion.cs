using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items.Consumables
{
    public class MajorHealthPotion : IItem
    {
        public ItemType type { get { return ItemType.Consumable; } }
        public int itemID { get { return IItem.MAJOR_HEALTH_POTION_ID; } }
        public String displayName { get { return "Major Health Potion"; } }
    }
}
