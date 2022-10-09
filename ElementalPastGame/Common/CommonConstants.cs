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
        public static int TILE_DIMENSION = 50;

        // Map Constants
        public static int TILE_VIEW_DISTANCE = 10;
        public static Location GAME_START_LOCATION = new Location()
        {
            X = 200,
            Y = 200,
        };
        public static int MAX_MAP_TILE_DIMENSION = 1000;

        // Image IDs
        public static String PLAYER_IMAGE_ID = "PlayerImageID";
        public static String WOODEN_FENCE_HORIZONTAL_IMAGE_ID = "WoodenFenceHorizontalImageID";
    }
}
