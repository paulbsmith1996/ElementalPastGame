using ElementalPastGame.SpacesManagement.Spaces.Houses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.SpacesManagement.Spaces
{
    public static class Spaces
    {

        public static String HOUSE_1 = "House1";
        public static String HOUSE_2 = "House2";
        public static String HOUSE_3 = "House3";
        public static String HOUSE_4 = "House4";
        public static String WIZARD_TOWER = "WizardTower";
        public static String OVERWORLD = "Overworld";

        internal static Dictionary<String, ISpace> spaceMapping = new()
        {
            { HOUSE_1, new House1Space() },
            { HOUSE_2, new House2Space() },
            { HOUSE_3, new House3Space() },
            { HOUSE_4, new House4Space() },
            { WIZARD_TOWER, new WizardTowerSpace() },
            { OVERWORLD, new OverworldSpace() }
        };

        public static ISpace SpaceForIdentity(String identity)
        {
            return spaceMapping.GetValueOrDefault(identity);
        }


    }
}
