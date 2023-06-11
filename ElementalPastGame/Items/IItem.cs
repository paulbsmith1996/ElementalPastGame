using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Items
{
    public interface IItem
    {
        enum ItemType
        {
            Consumable,
            Equipment,
            KeyItem
        }
        public ItemType type { get; }
        public int itemID { get; }

        public String displayName { get; }

        public int basePrice { get; }

        //==================================================
        //                   Item IDs
        //==================================================

        // Weapon IDs
        public static int WOOD_SWORD_ID = 1;
        public static int BRONZE_SWORD_ID = 2;

        public static int BRONZE_DAGGER_ID = 3;

        // Consumable IDs
        public static int MINOR_HEALTH_POTION_ID = 1000;
        public static int HEALTH_POTION_ID = 1001;
        public static int MAJOR_HEALTH_POTION_ID = 1002;

        //==================================================
        //                   Item Prices
        //==================================================

        // Weapon IDs
        public static int WOOD_SWORD_PRICE = 200;
        public static int BRONZE_SWORD_PRICE = 500;

        public static int BRONZE_DAGGER_PRICE = 150;

        // Consumable IDs
        public static int MINOR_HEALTH_POTION_PRICE = 50;
        public static int HEALTH_POTION_PRICE = 100;
        public static int MAJOR_HEALTH_POTION_PRICE = 250;
    }
}
