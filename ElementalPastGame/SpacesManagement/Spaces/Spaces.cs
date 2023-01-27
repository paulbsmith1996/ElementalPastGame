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
        public static String OVERWORLD = "Overworld";

        internal static Dictionary<String, ISpace> spaceMapping = new()
        {
            { HOUSE_1, new House1Space() },
            { HOUSE_2, new House2Space() },
            { OVERWORLD, new OverworldSpace() }
        };

        public static ISpace SpaceForIdentity(String identity)
        {
            return spaceMapping.GetValueOrDefault(identity);
        }


    }
}
