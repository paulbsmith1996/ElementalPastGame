using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityDataModel
    {
        public int level { get; set; }
        public int health { get; set; }
        public int maxHealth { get; set; }
        public int agility { get; set; }
        public int alchemyResistance { get; set; }
        public int alchemyPotency { get; set; }
        public int strength { get; set; }
        public int physicalResistance { get; set; }

        /// <summary>
        /// This ImageID should be specific to the kind of GameObject that inherits from this
        /// interface. For example, there should be a "player image ID" in the CommonConstants
        /// class or a "goblin image ID".
        /// </summary>
        public String ImageID { get; set; }

        public Image? Image { get; set; }

        public EntityDataModel(EntityType type, int level)
        {
            switch (type)
            {
                case EntityType.Goblin:
                    this.Goblin(level);
                    break;
                case EntityType.Aendon:
                    this.Aendon(level);
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.Load();
        }

        public void Goblin(int level)
        {
            this.level = level;
            this.health = 100 + level * 10;
            this.maxHealth = 100 + level * 10;
            this.agility = 100 + level * 10;
            this.alchemyResistance = 100 + level * 10;
            this.alchemyPotency = 100 + level * 10;
            this.strength = 100 + level * 10;
            this.physicalResistance = 100 + level * 10;
            this.ImageID = TextureMapping.Goblin;
        }

        public void Aendon(int level)
        {
            this.level = level;
            this.health = 200 + level * 10;
            this.maxHealth = 200 + level * 10;
            this.agility = 200 + level * 10;
            this.alchemyResistance = 200 + level * 10;
            this.alchemyPotency = 200 + level * 10;
            this.strength = 200 + level * 10;
            this.physicalResistance = 200 + level * 10;
            this.ImageID = TextureMapping.Goblin;
        }

        public void Load()
        {
            if (this.Image != null)
            {
                return;
            }

            this.Image = TextureMapping.Mapping[this.ImageID];
        }
    }
}
