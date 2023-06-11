using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items.Equipment.Weapons
{
    public class WoodSword : Weapon
    {
        public WoodSword() : base("Wood Sword", WOOD_SWORD_ID, WeaponType.Sword, WOOD_SWORD_BASE_DAMAGE, IItem.WOOD_SWORD_PRICE)
        { }
    }
}
