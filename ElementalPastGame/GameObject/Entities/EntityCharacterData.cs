using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityCharacterData
    {
        public String name { get; set; }
        public int level { get; set; }

        public EntityCharacterData(EntityType type, int level) : this((String)EntityLookup.Mapping[type][EntityLookup.NAME_KEY], level)
        {
        }

        public EntityCharacterData(string name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }
}
