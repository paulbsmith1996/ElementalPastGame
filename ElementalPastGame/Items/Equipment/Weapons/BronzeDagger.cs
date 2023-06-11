using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Equipment.Weapons.Weapon;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.Items.Equipment.Weapons
{
    public class BronzeDagger : Weapon
    {

        public BronzeDagger() : base("Bronze Dagger", BRONZE_DAGGER_ID, WeaponType.Dagger, BRONZE_DAGGER_BASE_DAMAGE, IItem.BRONZE_DAGGER_PRICE)
        {

        }
    }
}
