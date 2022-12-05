using ElementalPastGame.Items.Equipment;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityImageData
    {

        /// <summary>
        /// This ImageID should be specific to the kind of GameObject that inherits from this
        /// interface. For example, there should be a "player image ID" in the CommonConstants
        /// class or a "goblin image ID".
        /// </summary>
        public String ImageID { get; set; }

        public Image? Image { get; set; }

        public EntityImageData(EntityType type) : this((String)EntityLookup.Mapping[type][EntityLookup.IMAGE_ID_KEY])
        {
        }

        public EntityImageData(String ImageID)
        {
            this.ImageID = ImageID;
            this.Load();
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
