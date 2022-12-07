using ElementalPastGame.GameObject.Entities;
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
        public enum WeaponAction
        {
            Punch,
            Slash,
            Stab,
            Chop,
            Bash,
            Shoot
        };

        public new ItemType type => ItemType.Equipment;

        public WeaponType weaponType { get; }

        public int baseDamage { get; }

        public Dictionary<WeaponAction, double> multipliersForWeaponActions { get; set; }

        public Weapon(String displayName, int itemID, WeaponType weaponType, int baseDamage) : base(displayName, ItemType.Equipment, itemID)
        {
            this.weaponType = weaponType;
            this.baseDamage = baseDamage;
            this.multipliersForWeaponActions = Weapon.GetMovesForWeaponType(weaponType);
        }

        // Weapon Constants

        public static int BRONZE_SWORD_BASE_DAMAGE = 50;
        public static int WOOD_SWORD_BASE_DAMAGE = 30;

        public static int BRONZE_DAGGER_BASE_DAMAGE = 45;

        public static Dictionary<WeaponAction, double> GetMovesForWeaponType(WeaponType weaponType)
        {
            Dictionary<WeaponAction, double> moves;
            switch (weaponType)
            {
                case WeaponType.Sword:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Slash, 1.0 }, { WeaponAction.Stab, 1.0 }, { WeaponAction.Chop, 1.0 }, { WeaponAction.Bash, 1.0 } };
                    break;
                case WeaponType.Dagger:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Stab, 2.0 }, { WeaponAction.Shoot, 2.0 } };
                    break;
                case WeaponType.Bow:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Shoot, 3.0 } };
                    break;
                case WeaponType.Axe:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Chop, 2.0 }, { WeaponAction.Bash, 2.0 } };
                    break;
                case WeaponType.Mace:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Bash, 3.0 } };
                    break;
                default:
                    moves = new Dictionary<WeaponAction, double>() { { WeaponAction.Punch, 1.0 } };
                    break;
            }
            return moves;
        }

        // TODO: this is very dumb, and not updateable at all, stop being lazy and come up with a better solution here
        // (Edit: this is slightly less dumb, but does merit some attention)
        public static WeaponAction WeaponActionForString(String weaponActionString)
        {
            foreach (int enumIndex in Enum.GetValues(typeof(WeaponAction)))
            {
                String? actionName = Enum.GetName(typeof(WeaponAction), enumIndex);
                if (actionName != null && actionName.Equals(weaponActionString)) {
                    return (WeaponAction)enumIndex;
                }
            }

            return WeaponAction.Punch;
        }
    }
}
