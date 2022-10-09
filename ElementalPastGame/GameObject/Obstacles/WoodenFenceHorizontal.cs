using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Obstacles
{
    public class WoodenFenceHorizontal : GameObjectModel, IGameObjectModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Location">The tile location of the object</param>
        /// <param name="EntityID">The entity ID must be unique for all objects in an active tile</param>
        /// <param name="gameObjectManager">Manages object ineractions</param>
        public WoodenFenceHorizontal(Location Location, String EntityID, IGameObjectManager gameObjectManager) : base(EntityID, gameObjectManager)
        {
            this.IsCollidable = true;
            this.Size = new GameObjectSize()
            {
                Width = 1,
                Height = 1
            };
            this.Location = Location;
            this.ImageID = CommonConstants.WOODEN_FENCE_HORIZONTAL_IMAGE_ID;
        }
    }
}
