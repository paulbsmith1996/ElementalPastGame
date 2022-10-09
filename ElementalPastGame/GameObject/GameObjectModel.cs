using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public partial class GameObjectModel
    {
        public Location Location { get; set; }
        public GameObjectSize Size { get; set; }
        public String? ImageID { get; set; }
        public String EntityID { get; set; }
        public Boolean IsCollidable { get; set; }

        internal IGameObjectManager gameObjectManager;

        public GameObjectModel(String EntityID, IGameObjectManager gameObjectManager)
        {
            this.gameObjectManager = gameObjectManager;
            this.EntityID = EntityID;
        }
    }
}
