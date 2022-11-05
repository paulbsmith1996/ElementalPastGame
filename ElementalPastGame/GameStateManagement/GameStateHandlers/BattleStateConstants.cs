using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers
{
    public static class BattleStateConstants
    {
        public static int GAME_OBJECT_HORIZONTAL_PADDING = 150;
        public static int BACKGROUND_WIDTH = CommonConstants.GAME_DIMENSION + 120;
        public static int BACKGROUND_HEIGHT = 4 * CommonConstants.GAME_DIMENSION / 10;
        public static int BACKGROUND_X = (CommonConstants.GAME_DIMENSION - BACKGROUND_WIDTH) / 2;
        public static int BACKGROUND_Y = (CommonConstants.GAME_DIMENSION - BACKGROUND_HEIGHT) - 200;
        public static int DEPTH = 500;
        public static Point PERSPECTIVE = new Point(BACKGROUND_WIDTH / 2, DEPTH);

        public static int ENEMY_SELECTOR_PADDING = 5;
    }
}
