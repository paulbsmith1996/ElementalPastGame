using ElementalPastGame.Items.Equipment;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public static class EntityLookup
    {

        public static String NAME_KEY = "name";
        public static String DEFENSE_TYPE_KEY = "defense_type";

        public static String MAX_HEALTH_KEY = "max_health";
        public static String MAX_HEALTH_LEVEL_UP_INCREMENT_KEY = "max_health_lui";

        public static String AGILITY_KEY = "agility";
        public static String AGILITY_LEVEL_UP_INCREMENT_KEY = "agility_lui";

        public static String ALCHEMY_RESISTANCE_KEY = "alchemy_resistance";
        public static String ALCHEMY_RESISTANCE_LEVEL_UP_INCREMENT_KEY = "alchemy_resistance_lui";

        public static String ALCHEMY_POTENCY_KEY = "alchemy_potency";
        public static String ALCHEMY_POTENCY_LEVEL_UP_INCREMENT_KEY = "alchemy_potency_lui";

        public static String STRENGTH_KEY = "strength";
        public static String STRENGTH_LEVEL_UP_INCREMENT_KEY = "strength_lui";

        public static String PHSYCIAL_RESISTANCE_KEY = "physical_resistance";
        public static String PHSYCIAL_RESISTANCE_LEVEL_UP_INCREMENT_KEY = "physical_resistance_lui";

        public static String IMAGE_ID_KEY = "image_id";

        public static Dictionary<EntityType, Dictionary<String, Object>> Mapping = new()
        {
            { EntityType.Goblin, new Dictionary<string, object>() { { NAME_KEY, "Goblin" },
                                                              { DEFENSE_TYPE_KEY, EntityBattleModel.DefenseType.Light },
                                                              { MAX_HEALTH_KEY, 100 },
                                                              { MAX_HEALTH_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { AGILITY_KEY, 50 },
                                                              { AGILITY_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { ALCHEMY_RESISTANCE_KEY, 20 },
                                                              { ALCHEMY_RESISTANCE_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { ALCHEMY_POTENCY_KEY, 20 },
                                                              { ALCHEMY_POTENCY_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { STRENGTH_KEY, 75},
                                                              { STRENGTH_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { PHSYCIAL_RESISTANCE_KEY, 100 },
                                                              { PHSYCIAL_RESISTANCE_LEVEL_UP_INCREMENT_KEY, 10},
                                                              { IMAGE_ID_KEY, TextureMapping.Goblin }
                            }
            },

            { EntityType.Aendon, new Dictionary<string, object>() { { NAME_KEY, "Aendon" },
                                                              { DEFENSE_TYPE_KEY, EntityBattleModel.DefenseType.Light },
                                                              { MAX_HEALTH_KEY, 150 },
                                                              { MAX_HEALTH_LEVEL_UP_INCREMENT_KEY, 12},
                                                              { AGILITY_KEY, 75 },
                                                              { AGILITY_LEVEL_UP_INCREMENT_KEY, 12},
                                                              { ALCHEMY_RESISTANCE_KEY, 50 },
                                                              { ALCHEMY_RESISTANCE_LEVEL_UP_INCREMENT_KEY, 12 },
                                                              { ALCHEMY_POTENCY_KEY, 50 },
                                                              { ALCHEMY_POTENCY_LEVEL_UP_INCREMENT_KEY, 12 },
                                                              { STRENGTH_KEY, 100},
                                                              { STRENGTH_LEVEL_UP_INCREMENT_KEY, 12},
                                                              { PHSYCIAL_RESISTANCE_KEY, 120 },
                                                              { PHSYCIAL_RESISTANCE_LEVEL_UP_INCREMENT_KEY, 12},
                                                              { IMAGE_ID_KEY, TextureMapping.Player }
                            }
            },
            { EntityType.Villager1, new Dictionary<string, object>() { { NAME_KEY, "Villager" }, 
                                                                       { IMAGE_ID_KEY, TextureMapping.Villager1 }
                            }
            },
        };
    }
}
