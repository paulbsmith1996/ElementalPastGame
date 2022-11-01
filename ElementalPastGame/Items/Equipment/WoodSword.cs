using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.Items.Equipment
{
    public class WoodSword : Item
    {
        public ItemType type { get { return ItemType.Equipment; } }

        public int itemID { get { return WOOD_SWORD_ID; } }

        public String displayName { get { return "Wood Sword"; } }
    }
}
