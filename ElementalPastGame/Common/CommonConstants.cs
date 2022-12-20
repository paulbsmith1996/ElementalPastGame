using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Common
{
    public static class CommonConstants
    {
        // Rendering constants
        /// <summary>
        /// The size in pixels of each tile in the overworld.
        /// </summary>
        public static int TILE_DIMENSION = 75;

        // Map Constants
        /// <summary>
        /// THe distance in tiles that represents how far the player can see in every carindal direction.
        /// </summary>
        public static int TILE_VIEW_DISTANCE = 10;
        /// <summary>
        /// The location that the player begins at when the game starts up.
        /// </summary>
        public static Location GAME_START_LOCATION = new Location()
        {
            X = 815,
            Y = 880,
        };
        /// <summary>
        /// The max width and height that any tile can be placed at in the TileManager.
        /// </summary>
        public static int MAX_MAP_TILE_DIMENSION = 1000;
        public static int GAME_DIMENSION = TILE_DIMENSION * (2 * TILE_VIEW_DISTANCE + 1);

        public static int STANDARD_TEXTBOX_HEIGHT = 125;

        public static int BATTLE_PARTICIPANT_DIMENSION = 100;

        public static double KEY_DEBOUNCE_TIME_MS = 120;

        //public static char SELECT_KEY_BINDING_CHAR = 's';
        //public static Keys SELECT_KEY_BINDING = Keys.S;
        //public static char BACK_KEY_BINDING_CHAR = 'd';
        //public static Keys BACK_KEY_BINDING = Keys.D;
    }
}
