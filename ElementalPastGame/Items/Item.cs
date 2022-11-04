﻿using System;
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

        public String displayName { get; }

        //==================================================
        //                   Item IDs
        //==================================================

        // Weapon IDs
        public static int WOOD_SWORD_ID = 1;
        public static int BRONZE_SWORD_ID = 2;

        // Consumable IDs
        public static int MINOR_HEALTH_POTION_ID = 1000;
        public static int HEALTH_POTION_ID = 1001;
        public static int MAJOR_HEALTH_POTION_ID = 1002;
    }
}