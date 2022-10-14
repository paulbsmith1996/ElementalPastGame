using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    /// <summary>
    /// This interface should be the base for all GameObjectModels. It is meant to be inherited
    /// from. It contains information about where the GameObject is located, and how large the
    /// GameObject is, as well as extra game logic information.
    /// </summary>
    public interface IGameObjectModel
    {
        /// <summary>
        ///  This location is not the actual location where the GameObject gets rendered. This is
        ///  the local Location on the current tile.
        /// </summary>
        public Location Location { get; set; }
        
        /// <summary>
        /// This size, like the Location, does not represent the size of the image of the
        /// GameObject, but rather how many tiles the GameObject should take up (e.g. Size(2,1)
        /// means the GameObject should be 2 tiles wide and 1 tile high.
        /// </summary>
        public GameObjectSize Size { get; set; }

        public Boolean IsCollidable { get; set; }

        /// <summary>
        /// This ImageID should be specific to the kind of GameObject that inherits from this
        /// interface. For example, there should be a "player image ID" in the CommonConstants
        /// class or a "goblin image ID".
        /// </summary>
        public String ImageID { get; set; }

        public Image? Image { get; set; }

        /// <summary>
        /// This EntityID must be unique for every GameObject. It is eventually used 
        /// to identify the IGameObjectModel when deciding if it should be removed from the
        /// overworld.
        /// </summary>
        public long EntityID { get; set; }

        public void LoadIfNeeded();
        public void Unload();
    }
}
