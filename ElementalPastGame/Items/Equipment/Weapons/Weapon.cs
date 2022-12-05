using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items.Equipment.Weapons
{
    public class Weapon : ConcreteItem, IItem
    {
        public enum WeaponType
        {
            Sword,
            Dagger,
            Bow,
            Axe,
            Mace
        };
        public new ItemType type => ItemType.Equipment;

        public WeaponType weaponType { get; }

        public int baseDamage { get; }

        public Weapon(String displayName, int itemID, WeaponType weaponType, int baseDamage) : base(displayName, ItemType.Equipment, itemID)
        {
            this.weaponType = weaponType;
            this.baseDamage = baseDamage;
        }

        // Weapon Constants

        public static int BRONZE_SWORD_BASE_DAMAGE = 100;
        public static int WOOD_SWORD_BASE_DAMAGE = 75;
    }
}
