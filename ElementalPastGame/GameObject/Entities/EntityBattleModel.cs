using ElementalPastGame.Items.Equipment;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityBattleModel
    {
        public enum DefenseType
        {
            Heavy,
            Light,
            Skirmisher
        };
        public DefenseType type { get; set; }
        public int health { get; set; }
        public int maxHealth { get; set; }
        public int agility { get; set; }
        public int alchemyResistance { get; set; }
        public int alchemyPotency { get; set; }
        public int strength { get; set; }
        public int physicalResistance { get; set; }
        public bool isDead { get; set; }
        public ActiveEquipment? activeEquipment { get; set; }
        public EntityImageData imageData { get; set; }
        public EntityCharacterData characterData { get; set; }

        public EntityBattleModel(EntityType type, int level)
        {
            Dictionary<String, Object> entityInfo = EntityLookup.Mapping[type];
            this.characterData = new(type, level);
            this.imageData = new(type);
            this.maxHealth = (int)entityInfo[EntityLookup.MAX_HEALTH_KEY] + level * (int)entityInfo[EntityLookup.MAX_HEALTH_LEVEL_UP_INCREMENT_KEY];
            this.health = this.maxHealth;
            this.agility = (int)entityInfo[EntityLookup.AGILITY_KEY] + level * (int)entityInfo[EntityLookup.AGILITY_KEY];
            this.alchemyResistance = (int)entityInfo[EntityLookup.ALCHEMY_RESISTANCE_KEY] + level * (int)entityInfo[EntityLookup.ALCHEMY_RESISTANCE_LEVEL_UP_INCREMENT_KEY]; 
            this.alchemyPotency = (int)entityInfo[EntityLookup.ALCHEMY_POTENCY_KEY] + level * (int)entityInfo[EntityLookup.ALCHEMY_POTENCY_LEVEL_UP_INCREMENT_KEY];
            this.strength = (int)entityInfo[EntityLookup.STRENGTH_KEY] + level * (int)entityInfo[EntityLookup.STRENGTH_LEVEL_UP_INCREMENT_KEY];
            this.physicalResistance = (int)entityInfo[EntityLookup.PHSYCIAL_RESISTANCE_KEY] + level * (int)entityInfo[EntityLookup.PHSYCIAL_RESISTANCE_LEVEL_UP_INCREMENT_KEY];
        }

        public void Damage(int damage)
        {
            if (this.isDead)
            {
                return;
            }

            this.health -= damage;
            if (this.health <= 0)
            {
                this.isDead = true;
                this.imageData.Image = TextureMapping.Mapping[TextureMapping.Dead_Goblin];
            }
        }
    }
}
