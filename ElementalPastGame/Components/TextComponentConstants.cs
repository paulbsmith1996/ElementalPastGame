using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public static class TextComponentConstants
    {
        public static int TEXT_INSET = 10;

        public static int FONT_SIZE = 12;

        public static int CORNER_RADIUS = 10;

        public static int INNER_RECT_OFFSET = 5;

        public static int MENU_ITEM_VERTICAL_SPACING = 20;
        public static int MENU_ITEM_HORIZONTAL_SPACING = 26;
        public static int MENU_SELECT_ICON_WIDTH = 10;
        public static int MENU_SELECT_ICON_HEIGHT = 10;
        public static int MENU_SELECT_ICON_TEXT_PADDING = 3;
        public static Color MENU_SELECT_ICON_COLOR = Color.White;

        public static double TEXTBOX_POINTER_ANIMATION_DURATION = 1000;
        public static int TEXTBOX_POINTER_ANIMATION_DISTANCE = 15;
        public static int TEXTBOX_POINTER_WIDTH = 20;
        public static int TEXTBOX_POINTER_HEIGHT = 15;

        public static String FONT_FAMILY = "Arial Narrow";
        public static Font FONT = new Font(FONT_FAMILY, FONT_SIZE);
        public static Color DEFAULT_BORDER_COLOR = Color.FromArgb(255, 70, 120, 150);
        public static Color DEFAULT_BACKGROUND_COLOR = Color.FromArgb(150, 30, 80, 150);

        public static double TEXT_COMPONENT_WAIT_TIME_MS = CommonConstants.KEY_DEBOUNCE_TIME_MS;
    }
}
