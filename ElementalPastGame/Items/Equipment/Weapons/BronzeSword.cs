using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Equipment.Weapons.Weapon;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items.Equipment.Weapons
{
    public class BronzeSword : Weapon
    {
        public BronzeSword() : base("Bronze Sword", BRONZE_SWORD_ID, WeaponType.Sword, BRONZE_SWORD_BASE_DAMAGE, IItem.BRONZE_SWORD_PRICE)
        {
        }
    }
}
