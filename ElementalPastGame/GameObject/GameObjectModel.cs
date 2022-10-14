using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public partial class GameObjectModel : IGameObjectModel
    {
        public Location Location { get; set; }
        public GameObjectSize Size { get; set; }
        public String ImageID { get; set; }

        internal static long CurrentEntityID = 1;
        public long EntityID { get; set; }
        public Boolean IsCollidable { get; set; }
        public Image? Image { get; set; }

        public GameObjectModel(String ImageID, int X, int Y)
        {
            this.Location = new Location() { X = X, Y = Y };
            this.ImageID = ImageID;
            this.EntityID = GameObjectModel.CurrentEntityID;
            GameObjectModel.CurrentEntityID++;
        }

        public void LoadIfNeeded()
        {
            if (this.Image != null)
            {
                return;
            }

            this.Image = TextureMapping.Mapping[this.ImageID];
        }

        public void Unload()
        {
            this.Image = null;
        }
    }
}
